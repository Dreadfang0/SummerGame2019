using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
/*
Original Author: Janne Karhu
V:1.0 | 24.6.2019 Janne Karhu
Made this as a copy from EnemyBaseController and adjusted for boss
V:1.1 | 25.5.2019 Janne Karhu
Made the charge work
     */
public class BGMController : MonoBehaviour
{
    // Enemy states
    [HideInInspector]
    public enum EnemyState {Chasing, Special, Attacking }

    private EnemyState State;

    public bool movementBasedDetection = false;

    

    [Header("Chase State")]
    public float maxChaseRange; // Distance before enemy gives up chasing
    public float chaseCoolDown; // Time before enemy can chase again.

    private bool chaseOnCoolDown = false;

    [Header("Special State")]
    public float SpecialTime; // How long enemy does its special ability
    bool chargeOnCooldown = false;
    private bool isCharging = false; // Prevents timer from being run more than once
    public float chargeCooldown;
    [Header("Attack State")]
    public float attackDistance; // Distance before enemy switches to attack state
    public float attackingTime; // Time before stops attacking
    public float knockbackToPlayer;
    public float knockbackToSelf;
    public float dashCooldown;

    private bool isDashing = false;
    private bool attackTimerStarted = false; //Checks if attacktimer has been started to avoid spamming it.
    public bool attacking;
    [Header("Speed Multipliers")]
    public float attackSpeedMultiplier; // A multiplier for speed so the Enemy can approach the player faster when attacking.
    public float chaseSpeedMultiplier; // -||-
    public float chargeSpeedMultiplier; // -||-
    //public float alertSpeedMultiplier; // -||-

    [Header("Miscellaneous")]
    public float alertDetectionMultiplier;
    public float alertTime;
    public float idleLimit; // A speed limit for the idle animation to kick in
    public int damage = 1;
    private float baseDetectionRadius; // Holds base value for detection
    float baseAcceleration;
    public bool isDead;
    [SerializeField]
    public int health;

    [Serializable]
    public class StateChanges // A class used for collapsable inspector properties ---
    {
        [Header("Roaming State")]
        public EnemyState stateAfterDetectTarget;

        [Header("Chase State")]
        public EnemyState stateAfterCloseToTarget;

        [Header("Special State")]
        public EnemyState stateAfterSpecial;

        [Header("Attack State")]
        public EnemyState stateAfterAttackTimer;


    }
    [Space]
    public StateChanges stateChanges; // Used for collapsable inspector properties ---

    private float baseRoamingDetectionRadius;
    private float playerMagnitude; // Used for detection of movement speed

    private float Speed; // Holds agent's original speed
    private float timer;
    Transform target;
    NavMeshAgent agent;

    private AudioSource audioSource;

    // Audio files
    public AudioClip idleAudio;
    public AudioClip chaseAudio;
    public AudioClip damagedAudio;
    public AudioClip attackAudio;

    public GameObject ExplosionPoint;

    RaycastHit hit;

    Collider[] playerDetected; // Finds any players
    public Animator animator;
    //public SpriteRenderer sr;

    private Transform playerTransform;
    private Rigidbody playerRigidbody;

    private Rigidbody selfbody;

    public PerkSystem perkSystem;

    [SerializeField]
    GameObject DeathParticle;
    [SerializeField]
    int id;

    void Start()
    {
        
        perkSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<PerkSystem>();
        audioSource = GetComponent<AudioSource>();
        selfbody = GetComponent<Rigidbody>();
        State = EnemyState.Chasing; // Initial state
        if (GameObject.FindWithTag("Player"))
        {
            playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
            playerRigidbody = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        }
        else
            Debug.Log("Can't find player gameobject, is player tagged or placed in scene?");

        agent = GetComponent<NavMeshAgent>();
        Speed = agent.speed; // Gets the speed value of the navmesh agent
        baseAcceleration = agent.acceleration;
        GameManager.instance.BossHealthBarSetup(health);
        StartCoroutine(ChargeCooldownTimer());
    }

    void FixedUpdate()
    {
        /*if (audioSource.isPlaying == false)
        {
            audioSource.clip = idleAudio;
            audioSource.Play();
        }*/
        if (isDead == false)
        {
            GameManager.instance.BossHealthUpdater(health,id);
            if (State == EnemyState.Chasing) // --CHASING--------------------------------------------------
            {
                animator.SetInteger("AnimState", 1);
                agent.acceleration = baseAcceleration;
                //animator.SetInteger("AnimPos", 2); // Can be used to set aggressive animation or something.

                agent.speed = Speed * chaseSpeedMultiplier;

                //playerDetected = Physics.OverlapSphere(transform.position, roamingDetectionRadius, (1 << LayerMask.NameToLayer("Player"))); // Locates players using layers.

                //transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                float distance = Vector3.Distance(playerTransform.transform.position, transform.position);
                if (chaseOnCoolDown == false)
                {
                    if (distance >= attackDistance) // if outside of attack distance, move at player
                    {
                        MoveAtPlayer(1);
                    }
                    else if (distance < attackDistance) // If inside attack distance, change state (most likely to attack state)
                    {
                        State = stateChanges.stateAfterCloseToTarget;
                    }
                }
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxChaseRange))
                {
                    if (hit.collider.tag == "Player" && attackTimerStarted == false && chargeOnCooldown == false)
                    {
                        State = stateChanges.stateAfterDetectTarget;
                    }
                }
            }
            else if (State == EnemyState.Special) // --SPECIAL--------------------------------------------------------
            {
                if (isCharging == false)
                {
                    /*if (audioSource.isPlaying == false)
                        audioSource.PlayOneShot(damagedAudio, 1);*/
                    StartCoroutine("SpecialTimer");
                    audioSource.clip = null;
                }
                else
                {
                    if (audioSource.isPlaying == false)
                    {
                        audioSource.clip = chaseAudio;
                        audioSource.Play();
                    }
                    
                    animator.SetInteger("AnimState", 2);
                    float dist = agent.remainingDistance;
                    if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
                    {
                        State = stateChanges.stateAfterSpecial;
                        StartCoroutine(ChargeCooldownTimer());
                        isCharging = false;
                    }
                }

            }
            else if (State == EnemyState.Attacking) // --ATTACKING--------------------------------------------------
            {
                transform.LookAt(playerTransform);
                if (attackTimerStarted == false) // Start attacktimer, when it ends switch back to roaming
                {
                    StartCoroutine("Attack");
                }
                

            }
            if (health <= 0)
            {
                GameObject Death = (GameObject)Instantiate(DeathParticle, new Vector3(this.transform.position.x, this.transform.position.y + 1.14f, this.transform.position.z), transform.rotation);
                Destroy(Death, 1);
                isDead = true;
            }
        }
        else
        {
            //disable stuff here
            MoveAtPlayer(0);
            if (perkSystem.LifeSteal == true)
            {
                perkSystem.LifeStealHeal();
            }
            Destroy(gameObject);
        }
    }

    public void MoveAtPlayer(int directionMultiplier) // 1 = move towards player, -1 = move away from player.
    {
        if (audioSource.isPlaying == false)
        {
            audioSource.clip = idleAudio;
            audioSource.Play();
        }
        Vector3 direction = (playerTransform.position - transform.position); // Finds the direction where the player is.
        Vector3 runTo = transform.position + (direction * directionMultiplier);
        NavMeshHit navHitEngage;
        NavMesh.SamplePosition(runTo, out navHitEngage, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
        agent.SetDestination(navHitEngage.position);
    }



    

    IEnumerator SpecialTimer() // How long enemy uses special
    {
        isCharging = true;
        MoveAtPlayer(2);
        StartCoroutine(ChargeCooldownTimer());
        agent.speed = Speed * chargeSpeedMultiplier;
        agent.acceleration = 50;
        agent.velocity = agent.velocity * 2f;
        yield return new WaitForSeconds(0.2f);
        agent.acceleration = baseAcceleration;
        yield return new WaitForSeconds(SpecialTime);
        State = stateChanges.stateAfterSpecial;
        isCharging = false;
    }

    IEnumerator ChargeCooldownTimer() // How often enemy dashes
    {
        chargeOnCooldown = true;
        yield return new WaitForSeconds(chargeCooldown);
        chargeOnCooldown = false;
    }

    IEnumerator Attack() // How long enemy stays in attacking state, return to roaming afterwards and start cooldown to prevent immediatelly chasing again
    {
        attackTimerStarted = true;
        attacking = true;
        animator.SetInteger("AnimState", 4);
        MoveAtPlayer(0);
        yield return new WaitForSeconds(attackingTime /2);
        audioSource.PlayOneShot(attackAudio);
        yield return new WaitForSeconds(attackingTime / 2);
        //Attack animation that swings the weapon or shoots projectile
        State = stateChanges.stateAfterAttackTimer;
        attackTimerStarted = false;
    }

    IEnumerator ChaseCoolDown() // Cooldown to prevent infinite chasing after attacking
    {
        chaseOnCoolDown = true;
        yield return new WaitForSeconds(chaseCoolDown);
        chaseOnCoolDown = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            isCharging = false;
        }
    }
    public void Increase(float difficulty)
    {
        damage = (int)(damage * difficulty);
        health = (int)(health * difficulty);
    }
    public void damageEnemy(int dmg)
    {
        health -= dmg;
    }
}
