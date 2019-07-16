using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRoomPortal : MonoBehaviour
{
    public GameObject StartPoint;
    public GameObject player;
    public GameObject master;
    public EnemySpawner EnemySpawner;
    public ClutterSpawner ClutterSpawner;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 rotationVector = new Vector3(0, 30, 0);
        Quaternion rotation = Quaternion.Euler(0, 30, 0);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {


        if (other.tag == "Player")
        {
            /* if (nextLevel == 10)
             {
                 player.transform.position = BossLevel.transform.position;

             } */


            player.transform.position = StartPoint.transform.position;
            master.GetComponent<RoomSpawner>().KillYourChildren();
            master.GetComponent<RoomSpawner>().MakeMoreChildren();
            print("Spawned things");
            master.GetComponent<RoomSpawner>().Spawn();
            Cursor.lockState = CursorLockMode.None;
            player.GetComponent<MouseLook>().currentMouseLook = new Vector2(0, 0);
            player.GetComponent<MouseLook>().lookAngles = new Vector2(0, 0);
            Cursor.lockState = CursorLockMode.Locked;





        }
    }
   private void ReturnMouseControl()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


}
