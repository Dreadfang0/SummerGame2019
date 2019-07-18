using System.Collections;
using System.Collections.Generic;

using UnityEngine.AI;
using UnityEngine;


public class RoomSpawner : MonoBehaviour
{

    
    public float x_Start, z_Start;
    public float x_Space = 10, z_Space = 10;
    public int x_Length = 7, z_Length = 7;
    public GameObject[] chunks;
    public GameObject[] rareChunks;
    public GameObject RoomMaster;
    public EnemySpawner EnemySpawner;
    public ClutterSpawner ClutterSpawner;
    public GameObject[] skellys;
    

    //public NavMeshSurface[] surfaces;






    // Start is called before the first frame update
    void Start()
    {

        MakeMoreChildren();
        //Invoke("Spawn", 0.3f);

        /* for (int i = 0; i < x_Length * z_Length; i++)
         {
             Instantiate(chunks[Random.Range(0,chunks.Length)], new Vector3(x_Start + (x_Space * (i % x_Length)),0, z_Length + (-z_Space * (i / z_Length))), Quaternion.identity).gameObject.transform.parent = RoomMaster.transform;
             this.gameObject.transform.parent = RoomMaster.transform;
         } 



        for (int j = 0; j < surfaces.Length; j++)
         {
             surfaces[j].BuildNavMesh();
         }
         */


    }

    public void Spawn()
    {
        ClutterSpawner.Spawn();
        EnemySpawner.Spawn();
        print("Spawned things");
        //Invoke("EnableSkellys", 3);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Enabled");
            foreach (GameObject Enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Debug.Log("Found a skelly");
                if (Enemy.GetComponent<EnemyBaseController>() != null)
                {
                    //Enemy.GetComponent<EnemyRangedController>().enabled = true;
                    Enemy.GetComponent<EnemyBaseController>().enabled = true;
                    Debug.Log("Mele skelly");
                }
                else if (Enemy.GetComponent<EnemyRangedController>() != null)
                {
                    //Enemy.GetComponent<EnemyBaseController>().enabled = true;
                    Enemy.GetComponent<EnemyRangedController>().enabled = true;
                    Debug.Log("Ranged skelly");
                }
                else if (Enemy.GetComponent<DOOT>() != null)
                {
                    Enemy.GetComponent<DOOT>().enabled = true;
                    Debug.Log("DOOT skelly");
                }
            }
        }
    } 

    public void RealSpawn()
    {
        EnemySpawner.Spawn();
        print("Spawned things");
    }



    public void KillYourChildren()
    {
        foreach (Transform child in RoomMaster.transform)
            Destroy(child.gameObject);
    }

   public void MakeMoreChildren()
    {
        for (int i = 0; i < x_Length * z_Length; i++)
        {
            if (i == 24)
            {
                //fukken skip
            }
            else
            {
                int y = Random.Range(1 , 100);

                if (y > 5)
                {
                    Instantiate(chunks[Random.Range(0, chunks.Length)], new Vector3(x_Start + (x_Space * (i % x_Length)), 0
                    , z_Length + (-z_Space * (i / z_Length))), Quaternion.Euler(0, 90 * (Random.Range(0, 3)), 0)).gameObject.transform.parent = RoomMaster.transform;
                    //FUKKEN SPAGHET
                }
                else if (y <= 5)
                {
                    Instantiate(rareChunks[Random.Range(0, rareChunks.Length)], new Vector3(x_Start + (x_Space * (i % x_Length)), 0
                    , z_Length + (-z_Space * (i / z_Length))), Quaternion.Euler(0, 90 * (Random.Range(0, 3)), 0)).gameObject.transform.parent = RoomMaster.transform;
                }
            }

        }
    }

   /* void OnGUI()
    {
        if (GUI.Button(new Rect(200, 200, 150, 100), "New Room"))
        {
            print("Made a new room");
            KillYourChildren();
            MakeMoreChildren();
        }
        if (GUI.Button(new Rect(400, 200, 150, 100), "Spawn things"))
        {
            print("Spawned things");
            EnemySpawner.Spawn();
        }

    } */
}
