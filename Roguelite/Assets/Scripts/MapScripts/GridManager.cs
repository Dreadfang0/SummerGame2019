using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    /*public float x_Start, y_Start;

    public int columnLenght, rowLength;
    public float x_Space, y_Space;
    public GameObject prefab;*/
    // Start is called before the first frame update
    public GameObject plane;
    public int width = 10;
    public int height = 10;

    private GameObject[,] grid = new GameObject[10, 10];

    private void Awake()
    {
        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GameObject gridPlane = (GameObject)Instantiate(plane);
                gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + x,
                    gridPlane.transform.position.y, gridPlane.transform.position.z + z);
                grid[x,z] = gridPlane;
            }
        }
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Delet [3,3]"))
            Destroy(grid[3, 3]);
    }

    void Start()
    {
       /* for (int i = 0; i < columnLenght * rowLength; i++)
        {
            Instantiate(prefab, new Vector3(x_Start + (x_Space * (i % columnLenght)), y_Start + (-y_Space * (i / columnLenght))), Quaternion.identity);
        }*/
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
