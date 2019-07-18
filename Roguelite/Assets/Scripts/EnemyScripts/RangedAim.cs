using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Original Author: Janne Karhu
V:1.0 | 18.6.2019 Janne Karhu
Made this for the ranged enemy to aim at player     
     */
public class RangedAim : MonoBehaviour
{
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindWithTag("Player"))
        {
            playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z));
    }
}
