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
    // Start is called before the first frame update
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
            ParentRoom.SetActive(false);
        }
    }
}
