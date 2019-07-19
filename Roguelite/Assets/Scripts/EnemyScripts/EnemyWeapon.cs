using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Original Author: Janne Karhu
V:1.0 | 20.6.2019 Janne Karhu
Made for the melee enemy weapon that will be used with animation to hit

*/
public class EnemyWeapon : MonoBehaviour
{
    [SerializeField]
    EnemyBaseController controller;
    int damage = 1;
    [SerializeField]
    bool isFlamington;
    [SerializeField]
    bool isBoomington;
    // Start is called before the first frame update
    void Start()
    {
        damage = controller.damage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
        // Damage playerand cause knockback if player has not been damaged recently.
        if (other.gameObject.tag == "Player")
        {
            if (isBoomington == false && controller.attacking == true)
            {
                GameManager.instance.damagePlayer(damage);
                if (isFlamington)
                {
                    GameManager.instance.burnPlayer(controller.damage / 5);
                }
                controller.attacking = false;
            }
            else
            {
                controller.health = 0;
            }
            //playerRigidbody.AddExplosionForce(knockbackToPlayer, ExplosionPoint.transform.position, 10, 0, ForceMode.Impulse);
            //selfbody.AddExplosionForce(knockbackToSelf, ExplosionPoint.transform.position, 10, 0, ForceMode.Impulse);
        }
    }
}
