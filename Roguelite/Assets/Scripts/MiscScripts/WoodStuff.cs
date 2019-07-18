using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodStuff : MonoBehaviour
{
    [SerializeField]
    GameObject woodSplinter;
    [SerializeField]
    GameObject explosionParticle;
    [SerializeField]
    bool explosive;
    int damage = 10;
    bool explode;
    bool hasExplodedOnPlayer = false;
    bool hasExplodedOnEnemy = false;
    [SerializeField]
    bool isBoomingtonBarrel;

    public void setDamage(int dmg)
    {
        damage = dmg;
    }
    public void BreakingWood()
    {
        GameObject Wood = (GameObject)Instantiate(woodSplinter, new Vector3(this.transform.position.x, this.transform.position.y + 0.65f, this.transform.position.z), transform.rotation);
        Destroy(Wood, 1);
        if (explosive == true && explode == false)
        {
            StartCoroutine(Explosion());
        }
        else
            Destroy(gameObject);

    }
    IEnumerator Explosion()
    {
        explode = true;
        if (isBoomingtonBarrel == false)
        {
            GameObject Boom = (GameObject)Instantiate(explosionParticle, new Vector3(this.transform.position.x, this.transform.position.y + 0.65f, this.transform.position.z), transform.rotation);
            Destroy(Boom, 1);
        }
        else
        {
            GameObject.Instantiate(explosionParticle, new Vector3 (this.transform.position.x, this.transform.position.y + 0.65f, this.transform.position.z), transform.rotation).GetComponent<Explosion>().setBoom(damage);
        }
        gameObject.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && explode == true)
        {
            if (hasExplodedOnPlayer == false)
            {
                GameManager.instance.damagePlayer(damage);
                hasExplodedOnPlayer = true;
            }
        }
        if (other.tag == "Enemy" && explode == true)
        {
            if (hasExplodedOnEnemy == false)
            {
                other.GetComponentInParent<EnemyHealth>().damageEnemy(damage);
                hasExplodedOnEnemy = true;
            }
        }
        if (other.gameObject.tag == "WoodStuff" && explode == true)
        {
            other.GetComponent<WoodStuff>().BreakingWood();
        }
    }
}
