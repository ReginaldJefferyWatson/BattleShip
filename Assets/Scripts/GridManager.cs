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

    public List<Tile> ourEnemyTileList;
    public Battleship enemyBattleship;
    public Carrier enemyCarrier;
    public Cruiser enemyCruiser;
    public Destroyer enemyDestroyer;
    public Submarine enemySubmarine;
    public List<int> rotations = new List<int> { -90, 0, 90, 180 };
    public bool properPosition = false;
    private bool collisionSwitch = false;

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

    //Because of how collisions work, we have to check at the next frame for ships touching tile. We
    //will loop this until proper positions are assigned
    private void Update()
    {
        if(!properPosition)
        {
            if (!collisionSwitch)
            {
                GenerateEnemyShips();
            }
            else
            {
                addEnemyCoords();
                checkEnemyValid();
            }
            collisionSwitch = !collisionSwitch;
        }
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
                ourEnemyTileList.Add(spawnedTile);
            }
        }
        //cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height + (distance / 2f), -10);
    }

    void GenerateShips()
    {
        //Friendly Ships
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

    void GenerateEnemyShips() 
    { 
        //Enemy Ships
        var enemy_Carrier_Tile = Instantiate(carrierPrefab, new Vector3(Random.Range(0, 9), Random.Range(12, 21)), Quaternion.Euler(0, 0, rotations[Random.Range(0, 3)]));
        enemy_Carrier_Tile.name = $"Carrier";
        enemyCarrier = enemy_Carrier_Tile;

        int rotateVal = rotations[Random.Range(0, 3)];
        int xVal = Random.Range(0, 9);
        int yVal = Random.Range(12, 21);
        var enemy_Battleship_Tile = Instantiate(battleshipPrefab, new Vector3(xVal, yVal), Quaternion.Euler(0, 0, rotateVal));
        //If object is rotated, adjust for weird angles
        if (rotateVal == -90 || rotateVal == 90)
        {
            enemy_Battleship_Tile.transform.position = new Vector3(xVal - 0.5f, yVal);
        }
        else
        {
            enemy_Battleship_Tile.transform.position = new Vector3(xVal, yVal - 0.5f);
        }
        enemy_Battleship_Tile.name = $"Battleship";
        enemyBattleship = enemy_Battleship_Tile;

        var enemy_Cruiser_Tile = Instantiate(cruiserPrefab, new Vector3(Random.Range(0, 9), Random.Range(12, 21)), Quaternion.Euler(0, 0, rotations[Random.Range(0, 3)]));
        enemy_Cruiser_Tile.name = $"Cruiser";
        enemyCruiser = enemy_Cruiser_Tile;

        rotateVal = rotations[Random.Range(0, 3)];
        xVal = Random.Range(0, 9);
        yVal = Random.Range(12, 21);
        var enemy_Destroyer_Tile = Instantiate(destroyerPrefab, new Vector3(xVal, yVal), Quaternion.Euler(0, 0, rotateVal));
        //Same rotate logic as above
        if (rotateVal == -90 || rotateVal == 90)
        {
            enemy_Destroyer_Tile.transform.position = new Vector3(xVal - 0.5f, yVal);
        }
        else
        {
            enemy_Destroyer_Tile.transform.position = new Vector3(xVal, yVal - 0.5f);
        }
        enemy_Destroyer_Tile.name = $"Destroyer";
        enemyDestroyer = enemy_Destroyer_Tile;

        var enemy_Sub_Tile = Instantiate(subPrefab, new Vector3(Random.Range(0, 9), Random.Range(12, 21)), Quaternion.Euler(0, 0, rotations[Random.Range(0, 3)]));
        enemy_Sub_Tile.name = $"Submarine";
        enemySubmarine = enemy_Sub_Tile;
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

    public void addEnemyCoords()
    {
        foreach (Tile ourTile in ourEnemyTileList)
        {
            //Only execute for occupied tiles
            if (ourTile.occupier)
            {
                if (ourTile.occupier.name == "Battleship")
                {
                    ourTile.occupier.GetComponent<Battleship>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                    //Debug.Log("BS");
                }
                else if (ourTile.occupier.name == "Carrier")
                {
                    ourTile.occupier.GetComponent<Carrier>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                    //Debug.Log("CA");
                }
                else if (ourTile.occupier.name == "Cruiser")
                {
                    ourTile.occupier.GetComponent<Cruiser>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                    //Debug.Log("CR");
                }
                else if (ourTile.occupier.name == "Destroyer")
                {
                    ourTile.occupier.GetComponent<Destroyer>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                    //Debug.Log("DS");
                }
                else if (ourTile.occupier.name == "Submarine")
                {
                    ourTile.occupier.GetComponent<Submarine>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                    //Debug.Log("SB");
                }
            }
        }

        Debug.Log(enemyBattleship.GetComponent<Battleship>().shipCoords.Count);
        Debug.Log(enemyCarrier.GetComponent<Carrier>().shipCoords.Count);
        Debug.Log(enemyCruiser.GetComponent<Cruiser>().shipCoords.Count);
        Debug.Log(enemyDestroyer.GetComponent<Destroyer>().shipCoords.Count);
        Debug.Log(enemySubmarine.GetComponent<Submarine>().shipCoords.Count);
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

            //Record actual final positions of ships
            foreach (Tile ourTile in ourTileList)
            {
                if (ourTile.occupied == true)
                {
                    ourTile.occupiedGameStart = true;
                }
            }
        }

    }

    public void checkEnemyValid()
    {
        if ((enemyBattleship.GetComponent<Battleship>().size != enemyBattleship.GetComponent<Battleship>().shipCoords.Count) ||
        (enemyCarrier.GetComponent<Carrier>().size != enemyCarrier.GetComponent<Carrier>().shipCoords.Count) ||
        (enemyCruiser.GetComponent<Cruiser>().size != enemyCruiser.GetComponent<Cruiser>().shipCoords.Count) ||
        (enemyDestroyer.GetComponent<Destroyer>().size != enemyDestroyer.GetComponent<Destroyer>().shipCoords.Count) ||
        (enemySubmarine.GetComponent<Submarine>().size != enemySubmarine.GetComponent<Submarine>().shipCoords.Count))
        {

            Destroy(enemyBattleship.gameObject);
            Destroy(enemyCarrier.gameObject);
            Destroy(enemyCruiser.gameObject);
            Destroy(enemyDestroyer.gameObject);
            Destroy(enemySubmarine.gameObject);

            Debug.Log("Incorrect Enemy Placement");
        }
        else
        {
            //Break out of assignment loop
            Debug.Log("PROPER!");
            properPosition = true;

            //Record that these tiles had the ships on them
            foreach (Tile ourTile in ourEnemyTileList)
            {
                if (ourTile.occupied == true)
                {
                    ourTile.occupiedGameStart = true;
                }
            }

            enemyBattleship.gameObject.SetActive(false);
            enemyCarrier.gameObject.SetActive(false);
            enemyCruiser.gameObject.SetActive(false);
            enemyDestroyer.gameObject.SetActive(false);
            enemySubmarine.gameObject.SetActive(false);

            //Disallow drag and drop for the enemy ships
            enemyBattleship.gameObject.GetComponent<box>().enabled = false;
            enemyCarrier.gameObject.GetComponent<box>().enabled = false;
            enemyCruiser.gameObject.GetComponent<box>().enabled = false;
            enemyDestroyer.gameObject.GetComponent<box>().enabled = false;
            enemySubmarine.gameObject.GetComponent<box>().enabled = false;
        }
    }

    IEnumerator timeDelay()
    {
        invalidPlacementButton.SetActive(true);
        yield return new WaitForSeconds(3);
        invalidPlacementButton.SetActive(false);
    }

    /*
    IEnumerator timeDelayWelcome()
    {
        yield return new WaitForSeconds(3);
    }
    */

    public void cheatButtonPress()
    {
        enemyBattleship.gameObject.SetActive(!enemyBattleship.gameObject.activeSelf);
        enemyCarrier.gameObject.SetActive(!enemyCarrier.gameObject.activeSelf);
        enemyCruiser.gameObject.SetActive(!enemyCruiser.gameObject.activeSelf);
        enemyDestroyer.gameObject.SetActive(!enemyDestroyer.gameObject.activeSelf);
        enemySubmarine.gameObject.SetActive(!enemySubmarine.gameObject.activeSelf);
    }
}
