using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;


    [SerializeField] private Submarine subPrefab;
    [SerializeField] private Destroyer destroyerPrefab;
    [SerializeField] private Cruiser cruiserPrefab;
    [SerializeField] private Carrier carrierPrefab;
    [SerializeField] private Battleship battleshipPrefab;
    

    void Start()
    {
        GenerateGrid();
        GenerateShips();
    }

    void GenerateGrid()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
            }
        }
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }
    
    void GenerateShips()
    {
        var carrier_Tile = Instantiate(carrierPrefab, new Vector3((float)12.8, (float) 8.8), Quaternion.identity);
        carrier_Tile.name = $"Carrier";

        var battleship_Tile = Instantiate(battleshipPrefab, new Vector3((float)12.5, (float)7.5), Quaternion.identity);
        battleship_Tile.name = $"Battleship";

        var cruiser_Tile = Instantiate(cruiserPrefab, new Vector3((float)12.5, (float)6.6), Quaternion.identity);
        cruiser_Tile.name = $"Cruiser";

        var destroyer_Tile = Instantiate(destroyerPrefab, new Vector3((float)12.5, (float)5.8), Quaternion.identity);
        destroyer_Tile.name = $"Destroyer";

        var sub_Tile = Instantiate(subPrefab, new Vector3((float)12.0, (float)4.8), Quaternion.identity);
        sub_Tile.name = $"Submarine";
        
        
        
        
        //cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }
    

}
