using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/*
Original Author: Janne Karhu
V:1.0 | 17.6.2019 Janne Karhu
Taken from another project and going through adjustments to fit into this one. (Why should i reinvent the wheel?)
V:1.1 | 18.6.2019 Janne Karhu
Mostly adjusted and seems to work nicely. Using this version as base for ranged enemy
*/


public class EnemyBaseController : MonoBehaviour
{
    // Enemy states
    [HideInInspector]
    public enum EnemyState {Positioning, Chasing, Special, Attacking}

    private EnemyState State;

    public bool movementBasedDetection = false;

    [Header("Positioning State")]
    public float roamingAreaRadius; // How far enemy can move from its current position
    public float roamingDetectionRadius;// How far player is detected from
    public float roamingMaxIdleTime; // 0.5 is always minimum (hardcoded) wait time before enemy moves again when roaming
    bool positioning = false;
    [SerializeField]
    float positioningDistance;

    [Header("Chase State")]
    public float maxChaseRange; // Distance before enemy gives up chasing
    public float chaseCoolDown; // Time before enemy can chase again.

    private bool chaseOnCoolDown = false;

    [Header("Fleeing State")]
    public float fleeingTime; // How long enemy flees from something

    private bool isFleeing = false; // Prevents timer from being run more than once

    [Header("Attack State")]
    public float attackDistance; // Distance before enemy switches to attack state
    public float attackingTime; // Time before stops attacking
    public float knockbackToPlayer;
    public float knockbackToSelf;
    public float dashCooldown;

    private bool isDashing = false;
    private bool attackTimerStarted = false; //Checks if attacktimer has been started to avoid spamming it.

    [Header("Speed Multipliers")]
    public float attackSpeedMultiplier; // A multiplier for speed so the Enemy can approach the player faster when attacking.
    public float chaseSpeedMultiplier; // -||-
    public float fleeingSpeedMultiplier; // -||-
    //public float alertSpeedMultiplier; // -||-

    [Header("Miscellaneous")]
    public float alertDetectionMultiplier;
    public float alertTime;
    public float idleLimit; // A speed limit for the idle animation to kick in
    public int damage = 1;
    private float baseDetectionRadius; // Holds base value for detection
    public bool isDead;
    [SerializeField]
    public int health;
    public int expWorth;
    public GameObject player;

    [Serializable]
    public class StateChanges // A class used for collapsable inspector properties ---
    {
        [Header("Roaming State")]
        public EnemyState stateAfterDetectTarget;

        [Header("Chase State")]
        public EnemyState stateAfterCloseToTarget;

        [Header("Fleeing State")]
        public EnemyState stateAfterFleeing;

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

    [SerializeField]
    bool isBoomington;
    [SerializeField]
    GameObject BoomingtonBarrel;

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
    GameObject healOrb;

    void Start()
    {
        player = GameObject.Find("RigidPlayer");
        perkSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<PerkSystem>();
        audioSource = GetComponent<AudioSource>();
        baseRoamingDetectionRadius = roamingDetectionRadius;
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
    }
	
	void Update ()
    {
        if (health <= 0)
        {
            if (isBoomington == false)
            {
                GameObject Death = (GameObject)Instantiate(DeathParticle, new Vector3(this.transform.position.x, this.transform.position.y + 1.14f, this.transform.position.z), transform.rotation);
                Destroy(Death, 1);
            }
            else
            {
                BoomingtonBarrel.GetComponent<WoodStuff>().setDamage(damage);
                BoomingtonBarrel.GetComponent<WoodStuff>().BreakingWood();
            }
            float healthDropChance = Random.Range(0, 5);
            if (healthDropChance == 4)
            {
                GameObject.Instantiate(healOrb, this.transform.position, transform.rotation);
            }
            //player.GetComponent<PlayerController>().ExpGain(expWorth);
            isDead = true;
        }
        /*if (audioSource.isPlaying == false)
        {
            audioSource.clip = idleAudio;
            audioSource.Play();
        }*/
        if (isDead == false)
        {
            if (State == EnemyState.Positioning) // --ROAMING--------------------------------------------------
            {
                float distance = Vector3.Distance(playerTransform.transform.position, transform.position);
                MoveAtPlayer(-1);

                if (distance >= positioningDistance)
                {
                    State = stateChanges.stateAfterDetectTarget;
                }

            }
            else if (State == EnemyState.Chasing) // --CHASING--------------------------------------------------
            {

                //animator.SetInteger("AnimPos", 2); // Can be used to set aggressive animation or something.


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
                if (chaseOnCoolDown == true && distance >= positioningDistance && agent.remainingDistance == 0)
                {
                        animator.SetInteger("AnimState", 0);   
                }
            }
            else if (State == EnemyState.Special) // --FLEEING--------------------------------------------------------
            {
                if (isFleeing == false)
                {
                    if (audioSource.isPlaying == false)
                        audioSource.PlayOneShot(damagedAudio, 1);
                    agent.speed = Speed * fleeingSpeedMultiplier;
                    StartCoroutine("FleeingTimer");
                }
                else if (isFleeing)
                    MoveAtPlayer(-1);
            }
            else if (State == EnemyState.Attacking) // --ATTACKING--------------------------------------------------
            {
                transform.LookAt(new Vector3(playerTransform.position.x,transform.position.y,playerTransform.position.z));
                if (attackTimerStarted == false) // Start attacktimer, when it ends switch back to roaming
                {
                    StartCoroutine("Attack");
                }
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

    // Used for the wandering to find a random position in range on the navmesh and path to that destination
    Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    public void MoveAtPlayer(int directionMultiplier) // 1 = move towards player, -1 = move away from player.
    {
        if (directionMultiplier == 1)
        {
            animator.SetInteger("AnimState", 1);
            if (audioSource.isPlaying == false && attackTimerStarted == false)
            {
                audioSource.clip = idleAudio;
                audioSource.Play();
            }
        }
        if (directionMultiplier == -1)
        {
            animator.SetInteger("AnimState", 1);
            if (audioSource.isPlaying == false && attackTimerStarted == false)
            {
                audioSource.clip = idleAudio;
                audioSource.Play();
            }
        }
        
        Vector3 direction = (playerTransform.position - transform.position); // Finds the direction where the player is.
        Vector3 runTo = transform.position + (direction * directionMultiplier);
        NavMeshHit navHitEngage;
        NavMesh.SamplePosition(runTo, out navHitEngage, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
        agent.SetDestination(navHitEngage.position);
    }

    

    IEnumerator PositioningTimer() // How long enemy stays alert (not a state)
    {
        positioning = true;
        roamingDetectionRadius = baseRoamingDetectionRadius * alertDetectionMultiplier;
        yield return new WaitForSeconds(alertTime);
        roamingDetectionRadius = baseRoamingDetectionRadius;
        positioning = false;
    }

    IEnumerator Attack() // How long enemy stays in attacking state, return to roaming afterwards and start cooldown to prevent immediatelly chasing again
    {
        attackTimerStarted = true;
        animator.SetInteger("AnimState", 4);
        MoveAtPlayer(0);
        yield return new WaitForSeconds(attackingTime/2);
        audioSource.PlayOneShot(attackAudio);
        yield return new WaitForSeconds(attackingTime/2);
        StartCoroutine("ChaseCoolDown");
        State = stateChanges.stateAfterAttackTimer;
        attackTimerStarted = false;
    }

    IEnumerator ChaseCoolDown () // Cooldown to prevent infinite chasing after attacking
    {
        chaseOnCoolDown = true;
        yield return new WaitForSeconds(chaseCoolDown);
        chaseOnCoolDown = false;
    }
    public void damageEnemy(int dmg)
    {
        health -= dmg;
        audioSource.PlayOneShot(damagedAudio);
    }

    public void Increase(float difficulty)
    {
        damage = (int)(damage * difficulty);
        health = (int)(health * difficulty);
    }


}
