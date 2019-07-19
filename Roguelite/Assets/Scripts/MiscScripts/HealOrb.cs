using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOrb : MonoBehaviour
{
    [SerializeField]
    int healAmount;
    [SerializeField]
    int armorAmount;
    [SerializeField]
    bool isArmor;
    [SerializeField]
    GameObject pickUpParticle;
    private void Start()
    {
        gameObject.transform.parent = GameObject.Find("RoomMaster").transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerController>().health < other.GetComponent<PlayerController>().healthMax && isArmor == false)
            {
                GameManager.instance.healPlayer(healAmount);
                GameObject Particle = (GameObject)Instantiate(pickUpParticle, this.transform.position, transform.rotation);
                Destroy(Particle, 2);
                Destroy(gameObject);
            }
            if (isArmor == true && other.GetComponent<PlayerController>().armor < other.GetComponent<PlayerController>().armorMax)
            {
                GameManager.instance.armorPlayer(armorAmount);
                GameObject Particle = (GameObject)Instantiate(pickUpParticle, this.transform.position, transform.rotation);
                Destroy(Particle, 2);
                Destroy(gameObject);
            }
        }
    }
}
