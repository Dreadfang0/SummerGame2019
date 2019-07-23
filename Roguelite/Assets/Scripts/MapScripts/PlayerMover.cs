using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public GameObject StartPoint;
    public GameObject SkellyBossLevel;
    public GameObject NecroBossLevel;
    public GameObject player;
    public PlayerController playerController;
    public GameObject master;
    public EnemySpawner EnemySpawner;
    public int nextLevel;
    public int SkellyBoss = 5;
    public int NecroBoss = 10;
    public GameObject SkellyRoom, NecroRoom;
    // Start is called before the first frame update




    private void OnEnable()
    {
        //SkellyRoom = GameObject.Find("SkellyBossRoom");
        //NecroRoom = GameObject.Find("NecroBossRoom");
        if (nextLevel == 0)
        {
            //Fukken skip cause spaghet :-DDD
        }
        else
        { 
        Invoke("LevelUpCheck", 1.0f);
        }
        nextLevel++;
        master.GetComponent<RoomSpawner>().level++;
        
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

    }

    public void LevelUpCheck()
    {
        GameManager.instance.LevelUp();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerController.portalSource.Play();


            if (nextLevel == SkellyBoss)
            {
                SkellyRoom.SetActive(true);
                player.transform.position = SkellyBossLevel.transform.position;
                SkellyBoss += 10;
                EnemySpawner.GetComponent<EnemySpawner>().SpawnSkellyBoss();
            }
            else if (nextLevel == NecroBoss)
            {
                NecroRoom.SetActive(true);
                player.transform.position = NecroBossLevel.transform.position;
                NecroBoss += 10;
                EnemySpawner.GetComponent<EnemySpawner>().SpawnNecroBoss();
            }

            else
            {
                player.transform.position = StartPoint.transform.position;
                player.transform.rotation = StartPoint.transform.rotation;
                master.GetComponent<RoomSpawner>().KillYourChildren();
                master.GetComponent<RoomSpawner>().MakeMoreChildren();
                print("Spawned things");
                master.GetComponent<RoomSpawner>().Spawn();
            }
            GameObject.Find("StartStopper").GetComponent<Collider>().isTrigger = true;
            master.GetComponent<PortalEnabler>().portalEnabled = false;
            Cursor.lockState = CursorLockMode.None;
            player.GetComponent<MouseLook>().currentMouseLook = new Vector2(0, 0);
            player.GetComponent<MouseLook>().lookAngles = new Vector2(0, 0);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}

