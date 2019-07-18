using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    
    bool explode;
    bool hasExploded;
    [SerializeField]
    bool PlayerBoom;
    PlayerController playerController;
    public int damage = 5;
    public bool isBoomington;
    bool hasExplodedOnEnemy;
    bool hasExplodedOnPlayer;
    bool failsafe = false;
    private void Start()
    {
        StartCoroutine(Boom());
        
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void setBoom(int dmg)
    {
        isBoomington = true;
        setDamage(dmg);
        Debug.Log(isBoomington);
    }

    public void setDamage(int dmg)
    {
        damage = dmg;
    }    

    public IEnumerator Boom()
    {
        hasExploded = false;
        explode = true;
        Destroy(gameObject, 1f);
        yield return new WaitForSeconds(0.05f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (PlayerBoom == true)
        {
            if (hasExploded == false)
            {
                if(other.GetComponent<Rigidbody>() != null)
                    other.GetComponent<Rigidbody>().AddExplosionForce(1, this.transform.position, 3, 0f, ForceMode.Impulse);
            }
            if (other.tag == "Enemy" && explode == true)
            {
                if (hasExploded == false)
                {
                    other.GetComponentInParent<EnemyHealth>().damageEnemy(Mathf.RoundToInt(playerController.currentDamage * playerController.explosiveMultiplier));
                    hasExploded = true;
                }
            }

            if (other.gameObject.tag == "WoodStuff" && explode == true)
            {
                other.GetComponent<WoodStuff>().BreakingWood();
                hasExploded = true;
            }
        }
        if(PlayerBoom == false && isBoomington == false)
        {
            if (hasExploded == false)
            {
                if (other.GetComponent<Rigidbody>() != null)
                    other.GetComponent<Rigidbody>().AddExplosionForce(1, this.transform.position, 3,0,ForceMode.Impulse);
            }
            if (other.tag == "Player" && explode == true)
            {
                if (hasExploded == false)
                {
                    GameManager.instance.damagePlayer(damage);
                    hasExploded = true;
                    
                }
            }
            if (other.gameObject.tag == "WoodStuff" && explode == true)
            {
                other.GetComponent<WoodStuff>().BreakingWood();
                hasExploded = true;
            }
        }
        if(isBoomington == true)
        {
            
            if (failsafe == false)
            {
                hasExplodedOnEnemy = false;
                hasExplodedOnPlayer = false;
                failsafe = true;
                Debug.Log(failsafe);
            }
            if (hasExploded == false)
            {
                if (other.GetComponent<Rigidbody>() != null)
                    other.GetComponent<Rigidbody>().AddExplosionForce(20, this.transform.position, 5);
            }
            if (other.tag == "Enemy" && explode == true)
            {
                if (hasExplodedOnEnemy == false)
                {
                    other.GetComponentInParent<EnemyHealth>().damageEnemy(Mathf.RoundToInt(playerController.currentDamage * playerController.explosiveMultiplier));
                    hasExplodedOnEnemy = true;
                }
            }
            if (other.tag == "Player" && explode == true)
            {
                if (hasExplodedOnPlayer == false)
                {
                    GameManager.instance.damagePlayer(damage);
                    hasExplodedOnPlayer = true;
                    Debug.Log(hasExplodedOnPlayer);
                }
            }
            if (other.gameObject.tag == "WoodStuff" && explode == true)
            {
                other.GetComponent<WoodStuff>().BreakingWood();
                hasExploded = true;
            }
        } 
    }
}
