using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStopper : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            this.gameObject.GetComponent<Collider>().isTrigger = false;
        }
    }
}
