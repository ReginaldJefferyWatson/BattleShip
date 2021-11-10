using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height, distance;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Tile enemyTilePrefab;
    [SerializeField] private Transform cam;


    [SerializeField] private Submarine subPrefab;
    [SerializeField] private Destroyer destroyerPrefab;
    [SerializeField] private Cruiser cruiserPrefab;
    [SerializeField] private Carrier carrierPrefab;
    [SerializeField] private Battleship battleshipPrefab;

    public List<Tile> ourTileList;
    public Battleship ourBattleship;
    

    void Start()
    {
        GenerateGrid();
        GenerateEnemyGrid();
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
                ourTileList.Add(spawnedTile);
            }
        }
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    void GenerateEnemyGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(enemyTilePrefab, new Vector3(x, y + height + distance), Quaternion.identity);
                spawnedTile.name = $"Enemy Tile {x} {y}";
            }
        }
        //cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height + (distance / 2f), -10);
    }

    void GenerateShips()
    {
        var carrier_Tile = Instantiate(carrierPrefab, new Vector3((float)10.5, (float) 7), Quaternion.identity);
        carrier_Tile.name = $"Carrier";

        var battleship_Tile = Instantiate(battleshipPrefab, new Vector3((float)12, (float)7.5), Quaternion.identity);
        battleship_Tile.name = $"Battleship";
        ourBattleship = battleship_Tile;

        var cruiser_Tile = Instantiate(cruiserPrefab, new Vector3((float)10.5, (float)2), Quaternion.identity);
        cruiser_Tile.name = $"Cruiser";

        var destroyer_Tile = Instantiate(destroyerPrefab, new Vector3((float)12.0, (float)4), Quaternion.identity);
        destroyer_Tile.name = $"Destroyer";

        var sub_Tile = Instantiate(subPrefab, new Vector3((float)12.0, (float)1), Quaternion.identity);
        sub_Tile.name = $"Submarine";
    }
    
    public void adjustCamera()
    {
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height + (distance / 2f), -10);
    }

    public void addCoords()
    {
        foreach(Tile ourTile in ourTileList)
        {
            //Only execute for occupied tiles
            if(ourTile.occupier)
            {
                if(ourTile.occupier.name == "Battleship")
                {
                    ourTile.occupier.GetComponent<Battleship>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                }
                else if(ourTile.occupier.name == "Carrier")
                {
                    ourTile.occupier.GetComponent<Carrier>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                }
                else if (ourTile.occupier.name == "Cruiser")
                {
                    ourTile.occupier.GetComponent<Cruiser>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                }
                else if (ourTile.occupier.name == "Destroyer")
                {
                    ourTile.occupier.GetComponent<Destroyer>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                }
                else if (ourTile.occupier.name == "Submarine")
                {
                    ourTile.occupier.GetComponent<Submarine>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                }
            }

        }
    }

    //To check the validity of ship placement, and to call subsequent transitions afterwards
    public void checkValid()
    {
        if(ourBattleship.GetComponent<Battleship>().size != ourBattleship.GetComponent<Battleship>().shipCoords.Count)
        {
            Debug.Log("Incorrect Ship Placement!");
        }
    }

}
