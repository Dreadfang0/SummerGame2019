using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *Original Author: Saku Petlin
 * 8.6.2019
 */

public class GunScript : MonoBehaviour
{
    public GameObject bullet;
    public Transform spawner1, spawner2, spawner3, spawner4, spawner5;
    public GameObject[] spawners;
    public PerkSystem perkSystem;
    public PlayerController playerController;

    public void Shoot()
    {
        //Instantiate(bullet, spawner.position, transform.rotation);
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
        if (bullet != null)
        {
            if (perkSystem.multishotUpgrade == 0)
            {
                //for (int i = 0; i < spawners.Length; i++)
                //{
                //    Debug.Log("Multishot 0");
                //    bullet.transform.position = spawners[i].transform.position;
                //    bullet.transform.rotation = spawners[i].transform.rotation;
                //    bullet.SetActive(true);
                //}
                Fire1();
            }

            else if (perkSystem.multishotUpgrade == 1)
            {
                Fire1();
                Fire2();
                Fire3();
            }

            else if (perkSystem.multishotUpgrade == 2)
            {
                Fire1();
                Fire2();
                Fire3();
                Fire4();
                Fire5();
            }
        }
    }

    private void Fire1()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
        bullet.transform.position = spawner1.transform.position;
        bullet.transform.rotation = spawner1.transform.rotation;
        bullet.SetActive(true);
        //Debug.Log("Fire1  "+playerController.currentDamage);
        playerController.currentDamage = playerController.baseDamage;
        //Debug.Log("Fire1After  " + playerController.currentDamage);

    }

    private void Fire2()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
        bullet.transform.position = spawner2.transform.position;
        bullet.transform.rotation = spawner2.transform.rotation;
        bullet.SetActive(true);
        //Debug.Log("Fire2  " +playerController.currentDamage);
        playerController.currentDamage = playerController.baseDamage;
        //Debug.Log("Fire2After  " + playerController.currentDamage);


    }

    private void Fire3()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
        bullet.transform.position = spawner3.transform.position;
        bullet.transform.rotation = spawner3.transform.rotation;
        bullet.SetActive(true);
        //Debug.Log("Fire3  "+playerController.currentDamage);
        playerController.currentDamage = playerController.baseDamage;
        //Debug.Log("Fire3After  " + playerController.currentDamage);


    }

    private void Fire4()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
        bullet.transform.position = spawner4.transform.position;
        bullet.transform.rotation = spawner4.transform.rotation;
        bullet.SetActive(true);
        //Debug.Log("Fire4  "+playerController.currentDamage);
        playerController.currentDamage = playerController.baseDamage;
        //Debug.Log("Fire4After  " + playerController.currentDamage);

    }

    private void Fire5()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Bullet");
        bullet.transform.position = spawner5.transform.position;
        bullet.transform.rotation = spawner5.transform.rotation;
        bullet.SetActive(true);
        //Debug.Log("Fire5  "+playerController.currentDamage);
        playerController.currentDamage = playerController.baseDamage;
        Debug.Log("Fire5After  " + playerController.currentDamage);

    }
}
