using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOrb : MonoBehaviour
{
    [SerializeField]
    int healAmount;
    [SerializeField]
    bool isArmor;
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
                Destroy(gameObject);
            }
            if (isArmor == true)
            {
                GameManager.instance.armorPlayer(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
