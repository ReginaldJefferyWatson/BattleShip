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

    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject cheatButton;
    [SerializeField] private GameObject invalidPlacementButton;

    public List<Tile> ourTileList;
    public Battleship ourBattleship;
    public Carrier ourCarrier;
    public Cruiser ourCruiser;
    public Destroyer ourDestroyer;
    public Submarine ourSubmarine;

    float ourBattleshipX = 12f;
    float ourBattleshipY = 7.5f;
    float ourCarrierX = 10.5f;
    float ourCarrierY = 7f;
    float ourCruiserX = 10.5f;
    float ourCruiserY = 2f;
    float ourDestroyerX = 12f;
    float ourDestroyerY = 4f;
    float ourSubmarineX = 12f;
    float ourSubmarineY = 1f;

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
        var carrier_Tile = Instantiate(carrierPrefab, new Vector3(ourCarrierX, ourCarrierY), Quaternion.identity);
        carrier_Tile.name = $"Carrier";
        ourCarrier = carrier_Tile;

        var battleship_Tile = Instantiate(battleshipPrefab, new Vector3(ourBattleshipX, ourBattleshipY), Quaternion.identity);
        battleship_Tile.name = $"Battleship";
        ourBattleship = battleship_Tile;

        var cruiser_Tile = Instantiate(cruiserPrefab, new Vector3(ourCruiserX, ourCruiserY), Quaternion.identity);
        cruiser_Tile.name = $"Cruiser";
        ourCruiser = cruiser_Tile;

        var destroyer_Tile = Instantiate(destroyerPrefab, new Vector3(ourDestroyerX, ourDestroyerY), Quaternion.identity);
        destroyer_Tile.name = $"Destroyer";
        ourDestroyer = destroyer_Tile;

        var sub_Tile = Instantiate(subPrefab, new Vector3(ourSubmarineX, ourSubmarineY), Quaternion.identity);
        sub_Tile.name = $"Submarine";
        ourSubmarine = sub_Tile;
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
        if ((ourBattleship.GetComponent<Battleship>().size != ourBattleship.GetComponent<Battleship>().shipCoords.Count) ||
        (ourCarrier.GetComponent<Carrier>().size != ourCarrier.GetComponent<Carrier>().shipCoords.Count) ||
        (ourCruiser.GetComponent<Cruiser>().size != ourCruiser.GetComponent<Cruiser>().shipCoords.Count) ||
        (ourDestroyer.GetComponent<Destroyer>().size != ourDestroyer.GetComponent<Destroyer>().shipCoords.Count) ||
        (ourSubmarine.GetComponent<Submarine>().size != ourSubmarine.GetComponent<Submarine>().shipCoords.Count))
        {
            Debug.Log("Incorrect Ship Placement!");

            //Time Delay
            //invalidPlacementButton.SetActive(true);
            StartCoroutine(timeDelay());
            //invalidPlacementButton.SetActive(false);


            //Clear coordinates from ships
            ourBattleship.GetComponent<Battleship>().shipCoords.Clear();
            ourCarrier.GetComponent<Carrier>().shipCoords.Clear();
            ourCruiser.GetComponent<Cruiser>().shipCoords.Clear();
            ourDestroyer.GetComponent<Destroyer>().shipCoords.Clear();
            ourSubmarine.GetComponent<Submarine>().shipCoords.Clear();

            //Reset back to original positions
            ourBattleship.transform.position = new Vector3(ourBattleshipX, ourBattleshipY);
            ourBattleship.transform.rotation = Quaternion.Euler(0, 0, 0);
            ourCarrier.transform.position = new Vector3(ourCarrierX, ourCarrierY);
            ourCarrier.transform.rotation = Quaternion.Euler(0, 0, 0);
            ourCruiser.transform.position = new Vector3(ourCruiserX, ourCruiserY);
            ourCruiser.transform.rotation = Quaternion.Euler(0, 0, 0);
            ourDestroyer.transform.position = new Vector3(ourDestroyerX, ourDestroyerY);
            ourDestroyer.transform.rotation = Quaternion.Euler(0, 0, 0);
            ourSubmarine.transform.position = new Vector3(ourSubmarineX, ourSubmarineY);
            ourSubmarine.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            cam.GetComponent<Camera>().orthographicSize = 14;
            adjustCamera();
            startButton.SetActive(false);
            cheatButton.SetActive(true);
        }

    }

    IEnumerator timeDelay()
    {
        invalidPlacementButton.SetActive(true);
        yield return new WaitForSeconds(3);
        invalidPlacementButton.SetActive(false);
    }
}
