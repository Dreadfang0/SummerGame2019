using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public GameObject StartPoint;
    public GameObject BossLevel;
    public GameObject player;
    public GameObject master;
    public EnemySpawner EnemySpawner;
    public ClutterSpawner ClutterSpawner;
    public int nextLevel;
    public int SkellyBoss = 10;
    public int NecroBoss = 20;
    // Start is called before the first frame update




    private void OnEnable()
    {
        if (nextLevel == 0)
        {

        }
        else
        { 
        Invoke("LevelUpCheck", 1.0f);
        }
        nextLevel++;
        

    }

    public void LevelUpCheck()
    {
        GameManager.instance.LevelUp();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (nextLevel == SkellyBoss)
            {
                master.GetComponent<RoomSpawner>().KillYourChildren();
                player.transform.position = BossLevel.transform.position;
                SkellyBoss += 20;
                EnemySpawner.GetComponent<EnemySpawner>().SpawnBoss();

            }
          /*  else if (nextLevel == NecroBoss)
            {
                player.transform.position = NecroLevel.transform.position;
                NecroBoss += 20;
            } */

            else
            {
                player.transform.position = StartPoint.transform.position;
                player.transform.rotation = StartPoint.transform.rotation;
                master.GetComponent<RoomSpawner>().KillYourChildren();
                master.GetComponent<RoomSpawner>().MakeMoreChildren();
                print("Spawned things");
                master.GetComponent<RoomSpawner>().Spawn();
            }
            Cursor.lockState = CursorLockMode.None;
            player.GetComponent<MouseLook>().currentMouseLook = new Vector2(0, 0);
            player.GetComponent<MouseLook>().lookAngles = new Vector2(0, 0);
            Cursor.lockState = CursorLockMode.Locked;
        }


    }
}

