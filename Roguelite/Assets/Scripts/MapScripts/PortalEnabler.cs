using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEnabler : MonoBehaviour
{
    public GameObject portal;
    public GameObject bossPortal;
    public GameObject necroPortal;
    public bool portalEnabled = false;

    public int enemies;
    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }




    //fuggen spaghetti how do magnets and bools work?
    /* public void PortalDisabler()
     {
         if (enemies == 0)
         {
             disablePortal = false;
         }
         else if (enemies >= 1)
         {
             disablePortal = true;
         }
     }*/
    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;



        if (enemies >= 1 && portalEnabled == false)
        {
            portal.SetActive(false);
            bossPortal.SetActive(false);
            necroPortal.SetActive(false);
        }
        else if (enemies == 0 && portalEnabled == false)
        {
            portal.SetActive(true);
            bossPortal.SetActive(true);
            necroPortal.SetActive(true);
            portalEnabled = true;
        }

    }
}
