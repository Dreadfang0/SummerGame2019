using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Original Author: Janne Karhu
V:1.0 | 20.6.2019
Copied from BulletScript and adjusted to hit player only.
     */
public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody rb;
    public int damage;
    [SerializeField]
    bool isWizardton;
    [SerializeField]
    GameObject Explosion;
    void FixedUpdate()
    {
        rb.velocity = transform.TransformDirection(Vector3.forward * speed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isWizardton == false)
                GameManager.instance.damagePlayer(damage);
            else
            {
                GameObject.Instantiate(Explosion, this.transform.position, transform.rotation).GetComponent<Explosion>().setDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.tag == "Ground")
        {
            if (isWizardton == true)
            {
                GameObject.Instantiate(Explosion, this.transform.position, transform.rotation).GetComponent<Explosion>().setDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "WoodStuff")
        {
            if (isWizardton == false)
                other.GetComponent<WoodStuff>().BreakingWood();
            else
            {
                GameObject.Instantiate(Explosion, this.transform.position, transform.rotation).GetComponent<Explosion>().setDamage(damage);
            }
            Destroy(gameObject);
        }
    }

   public void SetDamage (int dmg)
    {
        damage = dmg;
    }
}
