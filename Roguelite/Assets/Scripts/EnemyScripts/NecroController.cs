using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
/*
Original Author: Janne Karhu
V:1.0 | 25.6.2019 Janne Karhu
Made this as a copy from EnemyRangedController and adjusted for boss
V:1.1 | 26.6.2019 Janne Karhu
Made it throw bottles that explode
     */

public class NecroController : MonoBehaviour
{
    // Enemy states
    [HideInInspector]
    public enum EnemyState {Idle, Special, Attacking }

    private EnemyState State;

    [Header("Special State")]
    public float SpecialTime; // How long enemy flees from something
    private bool SpecialActive = false; // Prevents timer from being run more than once
    public GameObject[] BottleTossPoints;
    public Transform ExplosionPoint;
    public GameObject WineBottle;
    public float bottleLaunchMax;
    public float bottleLaunchMin;
    bool timerStarted = false;
    public float timeToBottleMin;
    public float timeToBottleMax;

    [Header("Attack State")]
    public float attackDistance; // Distance before enemy switches to attack state
    public float attackingTime; // Time before stops attacking
    public float attackCooldown;
    [SerializeField]
    ParticleSystem Casting;
    private bool attackOnCooldown = false;
    private bool attackTimerStarted = false; //Checks if attacktimer has been started to avoid spamming it.

    [Header("Miscellaneous")]
    [SerializeField]
    GameObject ProjectileLauncher;
    [SerializeField]
    GameObject Projectile;
    public int damage = 1;
    public bool isDead;
    [SerializeField]
    public int health;

    [Serializable]
    public class StateChanges // A class used for collapsable inspector properties ---
    {
        [Header("Special State")]
        public EnemyState stateAfterSpecial;

        [Header("Attack State")]
        public EnemyState stateAfterAttackTimer;

    }
    [Space]
    public StateChanges stateChanges; // Used for collapsable inspector properties ---

    private float Speed; // Holds agent's original speed
    private float timer;
    Transform target;

    private AudioSource audioSource;

    // Audio files
    public AudioClip idleAudio;
    public AudioClip specialAudio;
    public AudioClip damagedAudio;
    public AudioClip attackAudio;
    RaycastHit hit;

    Collider[] playerDetected; // Finds any players
    public Animator animator;
    //public SpriteRenderer sr;

    private Transform playerTransform;
    private Rigidbody playerRigidbody;

    private Rigidbody selfbody;

    public PerkSystem perkSystem;

    public void perkele()
    {
        //Thanks kimi!
        Debug.Log("Perkele");
    }
    void Start()
    {
        perkSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<PerkSystem>();
        audioSource = GetComponent<AudioSource>();
        selfbody = GetComponent<Rigidbody>();
        State = EnemyState.Attacking; // Initial state
        if (GameObject.FindWithTag("Player"))
        {
            playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
            playerRigidbody = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        }
        else
        {
            Debug.Log("Can't find player gameobject, is player tagged or placed in scene?");
        }

        GameManager.instance.BossHealthBarSetup(health, true);
    }

    void Update()
    {
        /*if (audioSource.isPlaying == false)
        {
            audioSource.clip = idleAudio;
            audioSource.Play();
        }*/
        if (isDead == false)
        {
            GameManager.instance.BossHealthUpdater(health,0);
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
            if (State == EnemyState.Special) // --FLEEING--------------------------------------------------------
            {
                animator.SetInteger("AnimState", 2);
                if (SpecialActive == false)
                {
                    StartCoroutine(SpecialAbility());
                }
            }
            else if (State == EnemyState.Attacking) // --ATTACKING--------------------------------------------------
            {
                animator.SetInteger("AnimState", 1);
                if (attackTimerStarted == false) // Start attacktimer, when it ends switch back to idle
                {
                    StartCoroutine("Attack");
                }
            }
            else if (State == EnemyState.Idle)
            {
                animator.SetInteger("AnimState", 0);
            }
            if (timerStarted == false)
            {
                timer = Random.Range(timeToBottleMin, timeToBottleMax);
                timerStarted = true;
            }
            else
            {
                timer -= 0.1f;
            }
            if (timer <= 0 && SpecialActive == false && attackTimerStarted == false)
            {
                State = EnemyState.Special;
            }
            if (health <= 0)
            {
                if (perkSystem.LifeSteal == true)
                {
                    perkSystem.LifeStealHeal();
                }
                isDead = true;
            }
            if (attackOnCooldown == false && SpecialActive == false)
            {
                State = EnemyState.Attacking;
            }
        }
        else
        {
            //disable stuff here
            
            animator.SetInteger("AnimState", 3);
            gameObject.tag = "Untagged";
            AddTagRecursively(this.transform,"Untagged");
        }
        
    }
    void AddTagRecursively(Transform trans, string tag)
    {
        trans.gameObject.tag = tag;
        if (trans.childCount > 0)
            foreach (Transform t in trans)
                AddTagRecursively(t, tag);
    }
    IEnumerator SpecialAbility() // How long enemy flees from player
    {   
        SpecialActive = true;
        
        yield return new WaitForSeconds(1.1f);
        for (int i = 0; i < BottleTossPoints.Length; i++)
        {
            GameObject Botul = (GameObject)Instantiate(WineBottle, BottleTossPoints[i].transform.position, transform.rotation);
            Botul.GetComponent<Bottle>().SetDamage(damage);
            Botul.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(bottleLaunchMin, bottleLaunchMax), ExplosionPoint.transform.position, 10, 0.4f, ForceMode.Impulse);
            State = EnemyState.Idle;
        }
        audioSource.PlayOneShot(specialAudio);
        yield return new WaitForSeconds(SpecialTime);
        SpecialActive = false;
        timerStarted = false;
    }

    IEnumerator AttackCooldown() // How often enemy attacks
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        attackOnCooldown = false;
    }

    IEnumerator Attack() // How long enemy stays in attacking state, return to roaming afterwards and start cooldown to prevent immediatelly chasing again
    {
        attackTimerStarted = true;
        yield return new WaitForSeconds(attackingTime);
        Casting.Play();
        GameObject.Instantiate(Projectile, ProjectileLauncher.transform.position, ProjectileLauncher.transform.rotation).gameObject.GetComponent<EnemyProjectile>().SetDamage(damage);
        audioSource.PlayOneShot(attackAudio);
        StartCoroutine("AttackCooldown");
        State = EnemyState.Idle;
        attackTimerStarted = false;
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
