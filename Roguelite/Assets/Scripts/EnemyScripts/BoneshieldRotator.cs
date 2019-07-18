using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Original Author: Janne Karhu
V:1.0 | 26.6.2019 Janne Karhu
Made this for rotating boneshields to protect necromancer who is unable to move due to being drunk
     */
public class BoneshieldRotator : MonoBehaviour
{
    public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0, rotationSpeed, 0);
    }
}
