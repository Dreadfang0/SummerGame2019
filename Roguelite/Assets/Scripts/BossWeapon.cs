using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Original Author: Janne Karhu
V:1.0 | 24.6.2019 Janne Karhu
Made this as a copy from EnemyWeapon and adjusted for boss
     */
public class BossWeapon : MonoBehaviour
{
    [SerializeField]
    BGMController BGMcontroller;
    int damage = 10;

    void Start()
    {
        if(BGMcontroller != null)
            damage = BGMcontroller.damage;
    }

    private void OnTriggerEnter(Collider other)
    {

        // Damage playerand cause knockback if player has not been damaged recently.
        if (other.gameObject.tag == "Player")
        {
            GameManager.instance.damagePlayer(damage);
            //playerRigidbody.AddExplosionForce(knockbackToPlayer, ExplosionPoint.transform.position, 10, 0, ForceMode.Impulse);
            //selfbody.AddExplosionForce(knockbackToSelf, ExplosionPoint.transform.position, 10, 0, ForceMode.Impulse);
        }
    }
}
