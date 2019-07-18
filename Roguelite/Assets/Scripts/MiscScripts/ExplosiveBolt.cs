using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBolt : MonoBehaviour
{
    bool explode;
    bool hasExploded;
    [SerializeField]
    GameObject explosionParticle;
    public PlayerController playerController;

    public IEnumerator Explosion()
    {
        hasExploded = false;
        explode = true;
        GameObject Boom = (GameObject)Instantiate(explosionParticle, this.transform.position, transform.rotation);
        gameObject.GetComponent<SphereCollider>().enabled = true;
        Destroy(Boom, 3);
        yield return new WaitForSeconds(0.1f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy" && explode == true)
        {
            if (hasExploded == false)
            {
                other.GetComponent<EnemyHealth>().damageEnemy(Mathf.RoundToInt(playerController.currentDamage * playerController.explosiveMultiplier));
                hasExploded = true;
            }
        }

        if (other.gameObject.tag == "WoodStuff" && explode == true)
        {
            other.GetComponent<WoodStuff>().BreakingWood();
            hasExploded = true;
        }
    }

    private void OnDisable()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;
    }
}
