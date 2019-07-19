using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomPortal : MonoBehaviour
{
    public GameObject StartPoint;
    public GameObject player;
    public GameObject master;
    public EnemySpawner EnemySpawner;
    public ClutterSpawner ClutterSpawner;
    public GameObject ParentRoom;
    public PlayerController playerController;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            playerController.portalSource.Play();

            player.transform.position = StartPoint.transform.position;
            player.transform.rotation = StartPoint.transform.rotation;
            master.GetComponent<RoomSpawner>().KillYourChildren();
            master.GetComponent<RoomSpawner>().MakeMoreChildren();
            print("Spawned things");
            master.GetComponent<RoomSpawner>().Spawn();

            Cursor.lockState = CursorLockMode.None;
            player.GetComponent<MouseLook>().currentMouseLook = new Vector2(0, 0);
            player.GetComponent<MouseLook>().lookAngles = new Vector2(0, 0);
            Cursor.lockState = CursorLockMode.Locked;
            Destroy(GameObject.Find("Necromancer(Clone)"));
            ParentRoom.SetActive(false);

        }
    }
}
