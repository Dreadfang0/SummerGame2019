using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarreltonSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject barrelton;
    [SerializeField]
    GameObject WoodParticle;
    public int health = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            GameObject.Instantiate(barrelton, this.transform.position, barrelton.transform.rotation);
            GameObject Wood = (GameObject)Instantiate(WoodParticle, this.transform.position, barrelton.transform.rotation);
            Destroy(Wood, 1);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.Instantiate(barrelton, this.transform.position, barrelton.transform.rotation);
            GameObject.Instantiate(WoodParticle, this.transform.position, barrelton.transform.rotation);
            Destroy(gameObject);
        }
    }
}
