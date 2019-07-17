using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
Original Author: Janne Karhu
V:1.0 | 20.6.2019 Janne Karhu
Made to disable enemy when it dies so we can use pooling instead of creating new ones to prevent memory blot
V:1.0.1 | 25.6.2019 Teemu Karhusaari
Changed health into public
V:2.0 | 25.6.2019 Janne Karhu
Made it to be called by bolt then calling a function in parent. Finding parent is based on EnemyType value. Dont ask me why..
     */
public class EnemyHealth : MonoBehaviour
{
    // 0 = EGM, 1 = EGR, 2 = EFM, 3 = EFR, 4 = BGM, 5 = BGR, 6 = BFM, 7 = BFR, 8 = BarrelEnemy, 9 = DOOT
    // E = enemy, B = boss, G = ground, F = flying, M = mele, R = ranged
    public int EnemyType; 
    int fireDamage = 2;
    public int fireTickAmount;
    public float fireTickRate;
    [SerializeField]
    GameObject BurningParticle;
    [SerializeField]
    GameObject FreezeParticle;
    [SerializeField]
    GameObject HurtParticle;
    [SerializeField]
    bool isFlamington;
    bool isSlowed;
    float previousSpeed;
    public void damageEnemy(int dmg)
    {
        HurtParticle.GetComponent<ParticleSystem>().Play();
        if (EnemyType == 0)
        {
            gameObject.GetComponent<EnemyBaseController>().health -= dmg;
        }
        if (EnemyType == 1)
        {
            gameObject.GetComponent<EnemyRangedController>().health -= dmg;
        }
        //if (EnemyType == 2)
        //    gameObject.GetComponent<EnemyBaseController>().health -= dmg;
        //if (EnemyType == 3)
        //    gameObject.GetComponent<EnemyBaseController>().health -= dmg;
        if (EnemyType == 4)
        {
            gameObject.GetComponent<BGMController>().health -= dmg;
        }
        if (EnemyType == 5)
        {
            gameObject.GetComponent<NecroController>().health -= dmg;
        }
        //if (EnemyType == 6)
        //    gameObject.GetComponent<EnemyBaseController>().health -= dmg;
        //if (EnemyType == 7)
        //    gameObject.GetComponent<EnemyBaseController>().health -= dmg;
        if (EnemyType == 8)
        {
            gameObject.GetComponent<BarreltonSpawn>().health -= dmg;
        }
        if (EnemyType == 9)
        {
            gameObject.GetComponent<DOOT>().health -= dmg;
        }
    }
    public void burnEnemy(int dmg)
    {
        if (isFlamington == false)
        {
            fireDamage = dmg;
            StartCoroutine(burning());
        }
    }
    IEnumerator burning()
    {
        BurningParticle.GetComponent<ParticleSystem>().Play();
        for (int i = 0; i < fireTickAmount; i++)
        {
            yield return new WaitForSeconds(fireTickRate);
            damageEnemy(fireDamage);
            Debug.Log(i);
        }
        BurningParticle.GetComponent<ParticleSystem>().Stop();
    }
    public void slowed(float slowAmount)
    {
        StartCoroutine(Slow(slowAmount));
    }
    IEnumerator Slow(float slow)
    {
        if (GetComponentInParent<NavMeshAgent>() != null)
        {
            FreezeParticle.GetComponent<ParticleSystem>().Play();
            if (isSlowed == false)
            {
                previousSpeed = GetComponentInParent<NavMeshAgent>().speed;
                isSlowed = true;
            }
            GetComponentInParent<NavMeshAgent>().speed -= previousSpeed * slow;
            yield return new WaitForSeconds(3);
            GetComponentInParent<NavMeshAgent>().speed = previousSpeed;
            FreezeParticle.GetComponent<ParticleSystem>().Stop();
            isSlowed = false;
        }
    }
}
