using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/*
Original Author Janne Karhu
V.1.0: Janne Karhu
Taken from another project. Might require changes.
v1.1 |19.6.2019 Teemu Karhusaari
Void start vaihdettu void spawniin

 */
public class ClutterSpawner : MonoBehaviour
{

    [SerializeField]
    float spawnLimit;
    [SerializeField]
    GameObject[] RandomShit;

    //Sets a spawn area for the spawnable object
    [SerializeField]
    float MapXSize = 10;
    [SerializeField]
    float MapZSize = 10;
    [SerializeField]
    float navMeshSearchDist = 10; // This value is used to prevent enemies from spawning off the navmesh

    //These are used to calculate score and handle some of the UI, can be moved to another script.



    public GameObject RoomMaster;



    public void Spawn()
    {
        // Draws an outline for the map so we can determine how big it must be. 
        Debug.DrawLine(new Vector3(transform.position.x - MapXSize / 2, 1, 0), new Vector3(transform.position.x + MapXSize / 2, 1, 0), Color.red, 10f);
        Debug.DrawLine(new Vector3(0, 1, transform.position.z - MapZSize / 2), new Vector3(0, 1, transform.position.z + MapZSize / 2), Color.blue, 10f);

        // Spawn the enemies to randomized spots on the navmesh.
        for (int i = 0; i < spawnLimit; i++)
        {
            Vector3 newPos = RandomNavPos(MapXSize, MapZSize, navMeshSearchDist, -1);

                Instantiate(RandomShit[Random.Range(0, RandomShit.Length)], newPos, Quaternion.Euler(0, (Random.Range(0, 360)), 0)).gameObject.transform.parent = RoomMaster.transform;
            }

        }
    


    void Update()
    {
      
    }
    // This is used to spawn enemies on the navmesh.
    Vector3 RandomNavPos(float xSize, float zSize, float dist, int layermask)
    {
        NavMeshHit navHit;
        // Randomly gets a position for an enemy to spawn in.
        NavMesh.SamplePosition(new Vector3(Random.Range(transform.position.x - xSize / 2, transform.position.x + xSize / 2), 2,
                                           Random.Range(transform.position.z - zSize / 2, transform.position.z + zSize / 2)), out navHit, dist, layermask);
        return navHit.position;
    }
}
