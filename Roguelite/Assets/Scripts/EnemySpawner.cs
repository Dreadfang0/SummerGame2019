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
public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    float spawnLimit;
    [SerializeField]
    GameObject[] gMeleeEnemy;
    [SerializeField]
    GameObject[] gRangedEnemy;
    [SerializeField]
    GameObject gMeleeBoss;
    [SerializeField]
    GameObject gRangedBoss;
   /* [SerializeField]
    GameObject rareMelee;
    [SerializeField]
    GameObject rareRanged; */

    //Sets a spawn area for the spawnable object
    [SerializeField]
    float MapXSize = 10;
    [SerializeField]
    float MapZSize = 10;
    [SerializeField]
    float navMeshSearchDist = 10; // This value is used to prevent enemies from spawning off the navmesh
    
    //These are used to calculate score and handle some of the UI, can be moved to another script.
    public Text EnemyCount;
    public GameObject EnemyPanel;
    public GameObject ScorePanel;
    public Text ScoreText;
    public int EnemyAmount;
    public int scoreGain = 5;
    public float difficulty = 1.1f;
    public float startLimit;
    public Transform SkellyBossSpawnPoint;
    public Transform NecroBossSpawnPoint;



    public void Spawn()
    {
        // Draws an outline for the map so we can determine how big it must be. 
        Debug.DrawLine(new Vector3(transform.position.x - MapXSize / 2, 1, 0), new Vector3(transform.position.x + MapXSize / 2, 1, 0), Color.red, 10f);
        Debug.DrawLine(new Vector3(0, 1, transform.position.z - MapZSize / 2), new Vector3(0, 1, transform.position.z + MapZSize / 2), Color.blue, 10f);

        // Spawn the enemies to randomized spots on the navmesh.
        for (int i = 0; i < spawnLimit; i++)
        {
            Debug.Log("YEEEET");
            Vector3 newPos = RandomNavPos(MapXSize, MapZSize, navMeshSearchDist, -1);
            float spawntype = Random.Range(0, 2);
            if (spawntype == 0)
            {
                int y = Random.Range(1, 100);
                if (y > 10)
                {
                    Debug.Log("Melee");
                    Instantiate(gMeleeEnemy[0], newPos, gMeleeEnemy[0].transform.rotation).gameObject.GetComponent<EnemyBaseController>().Increase(difficulty);
                }
                else if (y <= 10)
                {
                    int m = Random.Range(1, gMeleeEnemy.Length);
                    Debug.Log("RareMelee");
                    Instantiate(gMeleeEnemy[m], newPos, gMeleeEnemy[m].transform.rotation).gameObject.GetComponent<EnemyBaseController>().Increase(difficulty);
                }
            }
            if (spawntype == 1)
            { 
                /* {
                     Debug.Log("Ranged");
                     Instantiate(gRangedEnemy, newPos, gRangedEnemy.transform.rotation).gameObject.GetComponent<EnemyRangedController>().Increase(difficulty);
                 } */
                int y = Random.Range(1, 100);
                if (y > 10)
                {
                    Debug.Log("Ranged");
                    Instantiate(gRangedEnemy[0], newPos, gRangedEnemy[0].transform.rotation).gameObject.GetComponent<EnemyRangedController>().Increase(difficulty);
                }
                else if (y <= 10)
                {
                    int m = Random.Range(1, gRangedEnemy.Length);
                    Debug.Log("RareRanged");
                    Instantiate(gRangedEnemy[m], newPos, gRangedEnemy[m].transform.rotation).gameObject.GetComponent<EnemyRangedController>().Increase(difficulty);
                }
            }
        }
        IncreaseAmount();
    }

    public void SpawnSkellyBoss()
    {
        Instantiate(gMeleeBoss, SkellyBossSpawnPoint.position, gMeleeBoss.transform.rotation).gameObject.GetComponent<BGMController>();
    }
    public void SpawnNecroBoss()
    {
        Instantiate(gRangedBoss, NecroBossSpawnPoint.position, gRangedBoss.transform.rotation).gameObject.GetComponent<BGMController>();
    }


    public void IncreaseAmount()
    {
        //difficulty += 0.1f;
        float tenfoldDifficulty = difficulty * 10f;
        tenfoldDifficulty += 2f;
        difficulty = tenfoldDifficulty / 10f;
        //spawnLimit = startLimit * difficulty;
        print("double " + tenfoldDifficulty + " diff" + difficulty );
        //difficulty += 0.1;
        //spawnLimit = startLimit * difficulty;
        spawnLimit = spawnLimit + 3;
    }

    void Update()
    {
        //difficulty = 2.8f;
        //Debug.Log(difficulty);
        // Locates every enemy to count score.
        EnemyAmount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        //EnemyCount.text = "enemies: " + EnemyAmount;

        // Changes UI to display final score if we dont make the boss ending.
        if (EnemyAmount == 0)  
        {              
                EnemyPanel.SetActive(false);
                ScorePanel.SetActive(true);
            
            //ScoreText.text = "Game Over" + "\n\nYour Score: " + (EnemyAmount * scoreGain);
        }
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
