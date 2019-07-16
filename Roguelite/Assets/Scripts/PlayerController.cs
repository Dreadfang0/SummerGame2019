using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 *Original Author: Saku Petlin
 * 8.6.2019
 */ 

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CapsuleCollider capsuleCollider;
    //private CharacterController characterController;
    //private Vector3 moveDir;
    private float speed;
    public float currentSpeed = 5f;
    //public float sprintMult = 2f;
    public float crouchMult = 0.5f;
    //public float gravity = 20f;
    //public float jumpForce = 10f;
    public Vector3 jumpHeight;
    private float verticalVelocity;
    private Transform look;
    private float height;
    private float crouchHeight;
    private bool isCrouching = false;
    private bool isSprinting = false;
    private bool isGrounded = false;
    private float lerpSpeed = 20;

    private Rigidbody rb;
    private Vector3 movement;

    private float moveX, moveY;

    [SerializeField]
    private GunScript gunScript;

    //Dash stuff
    //private int aPressed, dPressed, wPressed, sPressed;
    //public float tapDelay = 0.4f;
    //public float dashSpeed = 5;

    //[System.Serializable]
    //public class DashTargets
    //{
    //    public Transform dashTargetL;
    //    public Transform dashTargetR;
    //    public Transform dashTargetF;
    //    public Transform dashTargetB;
    //}

    //public DashTargets dashTargets;
    //private bool dashReset = false;
    //private bool canDash = false;

    //public float stamina;
    //private float staminaMin = 0;
    //public float staminaMax = 100;
    //[SerializeField]
    //private float dashCost = 25;
    //private float staminaTimer;
    //[SerializeField]
    //private float staminaRegenFrequency = 0.2f;
    //[SerializeField]
    //private float staminaRegen = 1f;
    //private bool staminaLoss = false;
    //private bool staminaLimit = false;
    //[SerializeField]
    //private float sprintCost;
    //private float sprintMin = 0;

    public int health;
    private int healthMin = 0;
    public int healthMax = 100;
    public int healthLvlUp = 10;
    public int armor;
    [HideInInspector]
    public int armorMin = 0;
    public int armorMax = 100;
    public double armorAbsorption = 0.8;
    [HideInInspector]
    public double armorSpill;
    private bool hasDied = false;
    //public float experience = 0;
    //[HideInInspector]
    //public float experienceMax;
    //private int level = 1;
    public int baseDamage;
    [HideInInspector]
    public int currentDamage;
    public float attackFrequency;
    private float attackTimer;
    public int ammo;
    public int ammoMax = 10;
    private bool outOfAmmo = false;
    [HideInInspector]
    public bool reloading = false;
    public float reloadSpeed = 3;
    public float critChance = 10;
    public double critMultiplier = 2;
    public bool isCrit = false;
    public float projectileSpeed = 10;
    public float explosiveMultiplier = 0.25f;
    public float burstDelay = 0.1f;
    public int berserkDamage;
    public double berserkThreshold = 0.1;
    public double berserkStackAmount;
    public float slowMultiplier;
    public PerkSystem perkSystem;
    public Animator animator;
  

    private void Awake()
    {
        //characterController = GetComponent<CharacterController>();
        look = transform.GetChild(0);
        //height = look.transform.localPosition.y;
        height = capsuleCollider.height;
        crouchHeight = height * 0.75f;
        speed = currentSpeed;
        rb = GetComponent<Rigidbody>();
        ammo = ammoMax;
        //experienceMax = level * 100f;
        health = healthMax;
        //currentDamage = baseDamage;
        armorSpill = 1 - armorAbsorption;
    }

    private void Start()
    {
        currentDamage = baseDamage;
    }

    private void Update()
    {
        if (GameManager.instance.pausedGame == false)
        {
            //Movement();
            //Sprint();
            Crouch();
            CrouchLerp();
            Jump();
            //Stamina();
            //StaminaRegeneration();
            //CanSprint();
            //DashInput();
            Health();
            Armor();
            attackTimer += Time.deltaTime;
            berserkStackAmount = 1 - ((double)health / (double)healthMax);
            Shoot();
            Reload();
        }
    }

    void FixedUpdate()
    {
        //Dash();
        //moveX = Input.GetAxisRaw("Horizontal");
        //moveY = Input.GetAxisRaw("Vertical");
        MovementInput();
        Move(moveX, moveY);

        //if (isSprinting == true && moveX != 0 || isSprinting == true && moveY != 0)
        //{
        //    stamina -= sprintCost;
        //}
    }

    void MovementInput()
    {
        if (Input.GetKey(KeyCode.W) && moveY != -1)
        {
            moveY = 1;
            animator.SetBool("isMoving", true);
        }

        else if (Input.GetKey(KeyCode.S) && moveY != 1)
        {
            moveY = -1;
        }

        else
        {
            moveY = 0;
            animator.SetBool("isMoving", false);
        }

        if (Input.GetKey(KeyCode.D) && moveX != -1)
        {
            moveX = 1;
            animator.SetBool("isMoving", true);
        }

        else if (Input.GetKey(KeyCode.A) && moveX != 1)
        {
            moveX = -1;
            animator.SetBool("isMoving", true);
        }

        else
        {
            moveX = 0;
        }

        if(moveX == 0 && moveY == 0)
        {
            animator.SetBool("isMoving", false);
        }
    }

    //void CanSprint()
    //{
    //    if (stamina <= 1)
    //    {
    //        sprintMin = 10f;
    //    }

    //    else if (stamina >= 10f)
    //    {
    //        sprintMin = 0f;
    //    }
    //}

    void Move(float moveX, float moveY)
    {
        movement.Set(moveX, 0, moveY);
        movement = movement.normalized * currentSpeed * Time.deltaTime;
        movement = transform.TransformDirection(movement);
        rb.MovePosition(transform.position + movement);
    }

    //void Movement()
    //{
    //    moveDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    //    moveDir = transform.TransformDirection(moveDir);
    //    moveDir *= currentSpeed * Time.deltaTime;
    //    Gravity();
    //    characterController.Move(moveDir);
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "WoodStuff")
        {
            isGrounded = true;
            animator.SetBool("inAir", false);
        }

        //if (other.tag == "Ground" && Input.GetKeyDown(KeyCode.Space))
        //{
        //    rb.AddForce(jumpHeight, ForceMode.VelocityChange);
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "WoodStuff")
        {
            isGrounded = false;
            animator.SetBool("inAir", true);
        }
    }

    void Jump()
    {
        if(isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpHeight, ForceMode.VelocityChange);
        }
    }

    /*void Gravity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity -= gravity;// * Time.deltaTime;
            Jump();
        }

        else
        {
            verticalVelocity -= gravity;// * Time.deltaTime;
        }

        moveDir.y = verticalVelocity;// * Time.deltaTime;
    }

    void Jump()
    {
        if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = jumpForce;
        }
    }*/

    /*void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrouching && stamina >= sprintMin)
        {
            currentSpeed *= sprintMult;
            isSprinting = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && !isCrouching || stamina <= sprintMin)
        {
            currentSpeed = speed;
            isSprinting = false;
        }
    }*/

    /*void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            aPressed += 1;
        }

        if (Input.GetKeyDown(KeyCode.A) && dashReset == false)
        {
            Invoke("SetDashPressedToZero", tapDelay);
            dashReset = true;
        }

        if (aPressed > 1)
        {
            canDash = true;
        }

        //----------------------------------------------------------------//

        if (Input.GetKeyDown(KeyCode.D))
        {
            dPressed += 1;
        }

        if (Input.GetKeyDown(KeyCode.D) && dashReset == false)
        {
            Invoke("SetDashPressedToZero", tapDelay);
            dashReset = true;
        }

        if (dPressed > 1)
        {
            canDash = true;
        }

        //----------------------------------------------------------------//

        if (Input.GetKeyDown(KeyCode.W))
        {
            wPressed += 1;
        }

        if (Input.GetKeyDown(KeyCode.W) && dashReset == false)
        {
            Invoke("SetDashPressedToZero", tapDelay);
            dashReset = true;
        }

        if (wPressed > 1)
        {
            canDash = true;
        }

        //----------------------------------------------------------------//

        if (Input.GetKeyDown(KeyCode.S))
        {
            sPressed += 1;
        }

        if (Input.GetKeyDown(KeyCode.S) && dashReset == false)
        {
            Invoke("SetDashPressedToZero", tapDelay);
            dashReset = true;
        }

        if (sPressed > 1)
        {
            canDash = true;
        }
    }*/

    /*void Dash()
    {
        //if (aPressed > 1 && isGrounded == true && stamina >= dashCost && canDash == true)
        if(Input.GetKeyDown(KeyCode.LeftShift) && moveX == -1 && isGrounded == true && stamina >= dashCost)
        {
            //attackTimer = 0;
            //dPressed = 0;
            //wPressed = 0;
            //sPressed = 0;
            //transform.position = Vector3.MoveTowards(transform.position, dashTargets.dashTargetL.position , dashSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, dashTargets.dashTargetL.position, dashSpeed * Time.deltaTime);


            if (staminaLimit == false)
            {
                staminaLimit = true;
                staminaLoss = true;
            }

            Invoke("RecentlyLostStamina", tapDelay);
        }

        //----------------------------------------------------------------//


        //if (dPressed > 1 && isGrounded == true && stamina >= dashCost && canDash == true)
        if (Input.GetKeyDown(KeyCode.LeftShift) && moveX == 1 && isGrounded == true && stamina >= dashCost)
        {
                //aPressed = 0;
                //wPressed = 0;
                //sPressed = 0;
                transform.position = Vector3.MoveTowards(transform.position, dashTargets.dashTargetR.position, dashSpeed * Time.deltaTime);

            if (staminaLimit == false)
            {
                staminaLimit = true;
                staminaLoss = true;
            }

            Invoke("RecentlyLostStamina", tapDelay);
        }

        //----------------------------------------------------------------//



        //if (wPressed > 1 && isGrounded == true && stamina >= dashCost && canDash == true)
        if (Input.GetKeyDown(KeyCode.LeftShift) && moveY == 1 && isGrounded == true && stamina >= dashCost)
        {
            //dPressed = 0;
            //aPressed = 0;
            //sPressed = 0;
            transform.position = Vector3.MoveTowards(transform.position, dashTargets.dashTargetF.position, dashSpeed * Time.deltaTime);

            if (staminaLimit == false)
            {
                staminaLimit = true;
                staminaLoss = true;
            }

            Invoke("RecentlyLostStamina", tapDelay);
        }

        //----------------------------------------------------------------//

        //if (sPressed > 1 && isGrounded == true && stamina >= dashCost && canDash == true)
        if (Input.GetKeyDown(KeyCode.LeftShift) && moveY == -1 && isGrounded == true && stamina >= dashCost)
        {
            //dPressed = 0;
            //wPressed = 0;
            //aPressed = 0;
            transform.position = Vector3.MoveTowards(transform.position, dashTargets.dashTargetB.position, dashSpeed * Time.deltaTime);

            if (staminaLimit == false)
            {
                staminaLimit = true;
                staminaLoss = true;
            }

            Invoke("RecentlyLostStamina", tapDelay);
        }

        canDash = false;
    }*/

    /*void Stamina()
    {
        staminaTimer += Time.deltaTime;
        stamina = Mathf.Clamp(stamina, staminaMin, staminaMax);
    }*/

    /*void SetDashPressedToZero()
    {
        aPressed = 0;
        dPressed = 0;
        wPressed = 0;
        sPressed = 0;
        dashReset = false;
        staminaLimit = false;
    }*/

    /*void RecentlyLostStamina()
    {
        if (staminaLoss == true)
        {
            staminaLoss = false;
            staminaLimit = false;
            stamina -= dashCost;
        }
    }*/

    /*void StaminaRegeneration()
    {
        if (stamina < 100 && staminaTimer > staminaRegenFrequency)
        {
            stamina += staminaRegen;
            staminaTimer = 0;
        }
    }*/

    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isSprinting)
        {
            currentSpeed *= crouchMult;
            isCrouching = true;
            capsuleCollider.height = crouchHeight;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && !isSprinting)
        {
            currentSpeed = speed;
            isCrouching = false;
            capsuleCollider.height = height;
        }
    }

    void CrouchLerp()
    {
        if (isCrouching == true)
        {
            look.localPosition = Vector3.Lerp(look.localPosition, new Vector3(0f, crouchHeight), Time.deltaTime * lerpSpeed);
        }
        else
        {
            look.localPosition = Vector3.Lerp(look.localPosition, new Vector3(0f, height), Time.deltaTime * lerpSpeed);
        }
    }

    void Health()
    {
        health = Mathf.Clamp(health, healthMin, healthMax);

        if (health == healthMin)
        {
            Death();
        }
    }

    void Armor()
    {
        armor = Mathf.Clamp(armor, armorMin, armorMax);
    }

    void Death()
    {
        if(hasDied == false)
        {
            hasDied = true;
            Debug.Log("You got BONED");
        }
    }

    /*public void Experience()
    {
        if(experience >= experienceMax)
        {
            healthMax += 5;
            level += 1;
            experience -= experienceMax;
            experienceMax = level * 100f; //Needs better levelling equation
            GameManager.instance.LevelUp();
        }
    }*/

    /*public void ExpGain(int gainedExperience)
    {
        experience += gainedExperience;
    }*/

    void Shoot()
    {
        if(Input.GetKey(KeyCode.Mouse0) && attackTimer > attackFrequency && ammo > 0 && reloading == false)
        {

            animator.SetBool("isShooting", true);
            attackTimer = 0;
            BerserkDamage();
            StartCoroutine(BurstDelay());
            //Debug.Log(currentDamage);
            ammo -= 1;
            currentDamage = baseDamage;
        }

        if (Input.GetKey(KeyCode.Mouse0) && ammo == 0 && reloading == false)
        {
            outOfAmmo = true;
            Reload();
        }
    }

    void BerserkDamage()
    {
        if(perkSystem.Berserk == true)
        {
            if(health != healthMax) //this removes the extra damage stack
            currentDamage -= berserkDamage;

            for (double d = 0; d < berserkStackAmount; d += berserkThreshold)
            {
                //Debug.Log("Tapahtuu");
                currentDamage += berserkDamage;
                //Debug.Log(currentDamage);

            }
        }
    }

    public int GetDamage()
    {
        if (Random.Range(0, 100) < critChance)
        {
            currentDamage = (currentDamage * (int)(critMultiplier * 10)) / 10;
            isCrit = true;
        }

        else
        {
            isCrit = false;
        }
        return currentDamage;
    }

    IEnumerator BurstDelay()
    {
        for (int i = 0; i <= perkSystem.burstUpgrade; i++)
        {
            gunScript.Shoot();
            yield return new WaitForSeconds(burstDelay);
            animator.SetBool("isShooting", false);
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && ammo != ammoMax && reloading == false || outOfAmmo == true)
        {
            Invoke("SetAmmoFull", reloadSpeed);
            outOfAmmo = false;
            reloading = true;
            animator.SetFloat("ReloadSpeed", 1 / reloadSpeed);
            animator.SetBool("isReloading", true);
            GameManager.instance.reloading.fillAmount = 0;
        }
    }

    void SetAmmoFull()
    {
        ammo = ammoMax;
        reloading = false;
        animator.SetBool("isReloading", false);
        GameManager.instance.reload.SetActive(false);
    }
}
