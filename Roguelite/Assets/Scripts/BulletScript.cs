using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Original Author: Saku Petlin
V:1.0 | 8.6.2019 Saku Petlin
made this script
V:1.1 | 20.6.2019 Janne Karhu
Adjusted to hit and damage enemy.
     */
public class BulletScript : MonoBehaviour
{
    //public float speed = 10f;
    public Rigidbody rb;
    //public int damage;
    public PlayerController playerController;
    public PerkSystem perkSystem;
    public GameObject explosive;
    private int damage;
    private bool isCrit;

    private void OnEnable()
    {
        gameObject.transform.GetChild(2).GetComponent<ExplosiveBolt>();
        perkSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<PerkSystem>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (perkSystem.FireProjectiles == true)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }

        if(perkSystem.ExplosiveProjectiles == true)
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }

        damage = playerController.GetDamage();

        if(damage > playerController.baseDamage)
        //if() needs work
        {
            isCrit = true;
        }

        else
        {
            isCrit = false;
        }

        if(isCrit == true)
        {
            gameObject.transform.GetChild(3).gameObject.SetActive(true);
        }

        else
        {
            gameObject.transform.GetChild(3).gameObject.SetActive(false);
        }

 
    }

    void FixedUpdate()
    {
        rb.velocity = transform.TransformDirection(Vector3.forward * playerController.projectileSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(damage);
        if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<EnemyHealth>().damageEnemy(damage);

            if(perkSystem.PiercingProjectiles == false)
            {
                gameObject.SetActive(false);
            }

            if (perkSystem.ExplosiveProjectiles == true)
            {
                GameObject Boom = (GameObject)Instantiate(explosive, this.transform.position, transform.rotation);
            }

            if (perkSystem.FireProjectiles == true)
            {
                other.GetComponent<EnemyHealth>().burnEnemy(playerController.currentDamage / 5);
            }
        }

        else if (other.gameObject.tag == "WoodStuff")
        {
            other.GetComponent<WoodStuff>().BreakingWood();

            if (perkSystem.ExplosiveProjectiles == true)
            {
                GameObject Boom = (GameObject)Instantiate(explosive, this.transform.position, transform.rotation);

            }

            gameObject.SetActive(false);
        }

        else if (other.gameObject.tag == "Ground")
        {
            if (perkSystem.ExplosiveProjectiles == true)
            {
                GameObject Boom = (GameObject)Instantiate(explosive, this.transform.position, transform.rotation);
            }

            gameObject.SetActive(false);
        }
    }
}
