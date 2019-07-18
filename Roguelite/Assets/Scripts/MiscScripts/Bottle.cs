using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Original Author: Janne Karhu
V:1.0 | 26.6.2019 Janne Karhu
Made this bottle projectile of necromancer boss to work.
     */
public class Bottle : MonoBehaviour
{
    int damage = 10;
    bool explode;
    bool hasExploded;
    [SerializeField]
    GameObject explosionParticle;
    private void FixedUpdate()
    {
        transform.Rotate(5, 0, 0);
    }
    IEnumerator Explosion()
    {
        explode = true;
        GameObject Boom =(GameObject)Instantiate(explosionParticle, this.transform.position, transform.rotation);
        Destroy(Boom, 3);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Explosion());
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && explode == true)
        {
            if (hasExploded == false)
            {
                GameManager.instance.damagePlayer(damage);
                hasExploded = true;
            }
        }
    }
}
