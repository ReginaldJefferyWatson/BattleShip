using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height, distance;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Tile enemyTilePrefab;
    [SerializeField] private Transform cam;

    private bool paused = false;
    [SerializeField] private GameObject pauseBG;


    [SerializeField] private Submarine subPrefab;
    [SerializeField] private Destroyer destroyerPrefab;
    [SerializeField] private Cruiser cruiserPrefab;
    [SerializeField] private Carrier carrierPrefab;
    [SerializeField] private Battleship battleshipPrefab;

    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject cheatButton;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject invalidPlacementButton;
    [SerializeField] private GameObject enemyHitButton;
    private float enemyHitButtonTime = 3f;
    [SerializeField] private GameObject enemyDestroyedButton;
    private float enemyDestroyedButtonTime = 3f;
    [SerializeField] private GameObject enemyMissButton;
    private float enemyMissButtonTime = 3f;
    [SerializeField] private GameObject enemyTurnButton;
    private float enemyTurnButtonTime = 3f;
    //These lower ones are in regard to the player's ships
    [SerializeField] private GameObject friendlyHitButton;
    private float friendlyHitButtonTime = 3f;
    [SerializeField] private GameObject friendlyDestroyedButton;
    private float friendlyDestroyedButtonTime = 3f;
    [SerializeField] private GameObject friendlyMissButton;
    private float friendlyMissButtonTime = 3f;
    [SerializeField] private GameObject friendlyTurnButton;
    private float friendlyTurnButtonTime = 3f;
    private float disappearTime;

    public List<Tile> ourTileList;
    public List<Tile> ourTileListAttackTrack;
    public List<(int, int)> friendlyHitSpaces = new List<(int, int)>();
    public Battleship ourBattleship;
    public Carrier ourCarrier;
    public Cruiser ourCruiser;
    public Destroyer ourDestroyer;
    public Submarine ourSubmarine;

    public List<Tile> ourEnemyTileList;
    public List<(int, int)> enemyHitSpaces = new List<(int, int)>();
    public Battleship enemyBattleship;
    public Carrier enemyCarrier;
    public Cruiser enemyCruiser;
    public Destroyer enemyDestroyer;
    public Submarine enemySubmarine;
    public List<int> rotations = new List<int> { -90, 0, 90, 180 };
    public bool properPosition = false;
    private bool collisionSwitch = false;
    //For keeping track of the board offset
    public int enemyTileX = 0;
    public int enemyTileY = 12;

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
        if (!properPosition)
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

        //Enable "Enemy Hit" Button
        if (enemyHitButton.activeSelf && (Time.time >= disappearTime))
        {
            enemyHitButton.SetActive(false);

            //Call for "Enemy Turn" button
            enemyTurnDelay();
        }

        //Enable "Enemy Destroyed" Button
        if (enemyDestroyedButton.activeSelf && (Time.time >= disappearTime))
        {
            enemyDestroyedButton.SetActive(false);

            //Call for "Enemy Turn" button
            enemyTurnDelay();
        }

        //Enable "Enemy Miss" Button
        if (enemyMissButton.activeSelf && (Time.time >= disappearTime))
        {
            enemyMissButton.SetActive(false);

            //Call for "Enemy Turn" button
            enemyTurnDelay();
        }

        //Enable "Enemy Turn" Button
        if (enemyTurnButton.activeSelf && (Time.time >= disappearTime))
        {
            enemyTurnButton.SetActive(false);

            //Put the enemy attack function here
            enemyAttack();
        }



        //Enable "Friendly Hit" Button
        if (friendlyHitButton.activeSelf && (Time.time >= disappearTime))
        {
            friendlyHitButton.SetActive(false);

            //Call for "Enemy Turn" button
            friendlyTurnDelay();
        }

        //Enable "Friendly Destroyed" Button
        if (friendlyDestroyedButton.activeSelf && (Time.time >= disappearTime))
        {
            friendlyDestroyedButton.SetActive(false);

            //Call for "Enemy Turn" button
            friendlyTurnDelay();
        }

        //Enable "Friendly Miss" Button
        if (friendlyMissButton.activeSelf && (Time.time >= disappearTime))
        {
            friendlyMissButton.SetActive(false);

            //Call for "Enemy Turn" button
            friendlyTurnDelay();
        }

        //Enable "Friendly Turn" Button
        if (friendlyTurnButton.activeSelf && (Time.time >= disappearTime))
        {
            friendlyTurnButton.SetActive(false);

            //Put the enemy attack function here
            enableEnemyTiles();
        }

        if (Input.GetKeyDown("escape"))
        {
            if (!paused)
            {
                Debug.Log("PAUSED");
                paused = true;
                Time.timeScale = 0;
                pauseBG.SetActive(true);
                disableEnemyTiles();
            }
            else
            {
                Debug.Log("UNPAUSED");
                paused = false;
                Time.timeScale = 1;
                pauseBG.SetActive(false);
                //Need to fix this so it only happens when it's our turn during pause
                enableEnemyTiles();
            }
        }
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
                spawnedTile.GetComponent<tileAttack>().GridManager = this.gameObject;
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
        foreach (Tile ourTile in ourTileList)
        {
            //Only execute for occupied tiles
            if (ourTile.occupier)
            {
                if (ourTile.occupier.name == "Battleship")
                {
                    ourTile.occupier.GetComponent<Battleship>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y));
                }
                else if (ourTile.occupier.name == "Carrier")
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
                    ourTile.occupier.GetComponent<Battleship>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y - enemyTileY));
                    //Debug.Log(enemyBattleship.shipCoords[ourBattleship.shipCoords.Count - 1]);
                }
                else if (ourTile.occupier.name == "Carrier")
                {
                    ourTile.occupier.GetComponent<Carrier>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y - enemyTileY));
                    //Debug.Log("CA");
                }
                else if (ourTile.occupier.name == "Cruiser")
                {
                    ourTile.occupier.GetComponent<Cruiser>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y - enemyTileY));
                    //Debug.Log("CR");
                }
                else if (ourTile.occupier.name == "Destroyer")
                {
                    ourTile.occupier.GetComponent<Destroyer>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y - enemyTileY));
                    //Debug.Log("DS");
                }
                else if (ourTile.occupier.name == "Submarine")
                {
                    ourTile.occupier.GetComponent<Submarine>().shipCoords.Add(((int)ourTile.transform.position.x, (int)ourTile.transform.position.y - enemyTileY));
                    //Debug.Log("SB");
                }
            }
        }

        /*
        Debug.Log(enemyBattleship.GetComponent<Battleship>().shipCoords.Count);
        Debug.Log(enemyCarrier.GetComponent<Carrier>().shipCoords.Count);
        Debug.Log(enemyCruiser.GetComponent<Cruiser>().shipCoords.Count);
        Debug.Log(enemyDestroyer.GetComponent<Destroyer>().shipCoords.Count);
        Debug.Log(enemySubmarine.GetComponent<Submarine>().shipCoords.Count);
        */
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

            //Clear rotation values in box
            ourBattleship.gameObject.GetComponent<box>().rotZ = 0;
            ourCarrier.gameObject.GetComponent<box>().rotZ = 0;
            ourCruiser.gameObject.GetComponent<box>().rotZ = 0;
            ourDestroyer.gameObject.GetComponent<box>().rotZ = 0;
            ourSubmarine.gameObject.GetComponent<box>().rotZ = 0;

            //Reset our ships back to original positions
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
            skipButton.SetActive(true);
            cheatButton.SetActive(true);

            //Make ships unmovable
            ourBattleship.gameObject.GetComponent<box>().enabled = false;
            ourCarrier.gameObject.GetComponent<box>().enabled = false;
            ourCruiser.gameObject.GetComponent<box>().enabled = false;
            ourDestroyer.gameObject.GetComponent<box>().enabled = false;
            ourSubmarine.gameObject.GetComponent<box>().enabled = false;

            //Record actual final positions of ships
            foreach (Tile ourTile in ourTileList)
            {
                if (ourTile.occupied == true)
                {
                    ourTile.occupiedGameStart = true;
                    ourTile.occupierGameStart = ourTile.occupier;
                }
            }

            //Transfer these tiles to enemy attack list
            ourTileListAttackTrack = ourTileList;
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
                    ourTile.occupierGameStart = ourTile.occupier;
                }
            }

            //Remove visibility
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

            //Remove hitboxes
        }
    }

    IEnumerator timeDelay()
    {
        invalidPlacementButton.SetActive(true);
        yield return new WaitForSeconds(3);
        invalidPlacementButton.SetActive(false);
    }

    public void enemyHitDelay()
    {
        enemyHitButton.SetActive(true);
        disappearTime = Time.time + enemyHitButtonTime;
    }

    public void enemyDestroyedDelay()
    {
        enemyDestroyedButton.SetActive(true);
        disappearTime = Time.time + enemyDestroyedButtonTime;
    }

    public void enemyMissDelay()
    {
        enemyMissButton.SetActive(true);
        disappearTime = Time.time + enemyMissButtonTime;
    }

    public void enemyTurnDelay()
    {
        enemyTurnButton.SetActive(true);
        disappearTime = Time.time + enemyTurnButtonTime;
    }

    public void friendlyHitDelay()
    {
        friendlyHitButton.SetActive(true);
        disappearTime = Time.time + friendlyHitButtonTime;
    }

    public void friendlyDestroyedDelay()
    {
        friendlyDestroyedButton.SetActive(true);
        disappearTime = Time.time + friendlyDestroyedButtonTime;
    }

    public void friendlyMissDelay()
    {
        friendlyMissButton.SetActive(true);
        disappearTime = Time.time + friendlyMissButtonTime;
    }

    public void friendlyTurnDelay()
    {
        friendlyTurnButton.SetActive(true);
        disappearTime = Time.time + friendlyTurnButtonTime;
    }

    public void cheatButtonPress()
    {
        enemyBattleship.gameObject.SetActive(!enemyBattleship.gameObject.activeSelf);
        enemyCarrier.gameObject.SetActive(!enemyCarrier.gameObject.activeSelf);
        enemyCruiser.gameObject.SetActive(!enemyCruiser.gameObject.activeSelf);
        enemyDestroyer.gameObject.SetActive(!enemyDestroyer.gameObject.activeSelf);
        enemySubmarine.gameObject.SetActive(!enemySubmarine.gameObject.activeSelf);
    }

    public void skipButtonPress()
    {
        disappearTime = Time.time;
    }



    public bool checkDestroyedEnemy(string shipName)
    {
        bool destroyed = false;

        //For checking if ship has been totally destroyed
        int counter = 0;

        if (shipName == "Battleship")
        {
            foreach (var coord in enemyBattleship.gameObject.GetComponent<Battleship>().shipCoords)
            {
                if (enemyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == enemyBattleship.size)
                    {
                        Debug.Log("Enemy Battleship Destroyed!");
                        enemyBattleship.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }
        else if (shipName == "Carrier")
        {
            foreach (var coord in enemyCarrier.gameObject.GetComponent<Carrier>().shipCoords)
            {
                if (enemyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == enemyCarrier.size)
                    {
                        Debug.Log("Enemy Carrier Destroyed!");
                        enemyCarrier.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }
        else if (shipName == "Cruiser")
        {
            foreach (var coord in enemyCruiser.gameObject.GetComponent<Cruiser>().shipCoords)
            {
                if (enemyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == enemyCruiser.size)
                    {
                        Debug.Log("Enemy Cruiser Destroyed!");
                        enemyCruiser.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }
        else if (shipName == "Destroyer")
        {
            foreach (var coord in enemyDestroyer.gameObject.GetComponent<Destroyer>().shipCoords)
            {
                if (enemyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == enemyDestroyer.size)
                    {
                        Debug.Log("Enemy Destroyer Destroyed!");
                        enemyDestroyer.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }
        else if (shipName == "Submarine")
        {
            foreach (var coord in enemySubmarine.gameObject.GetComponent<Submarine>().shipCoords)
            {
                if (enemyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == enemySubmarine.size)
                    {
                        Debug.Log("Enemy Submarine Destroyed!");
                        enemySubmarine.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }

        checkEnemyDefeated();

        return destroyed;
    }

    public bool checkDestroyedFriendly(string shipName)
    {
        bool destroyed = false;

        //For checking if ship has been totally destroyed
        int counter = 0;

        if (shipName == "Battleship")
        {
            foreach (var coord in ourBattleship.gameObject.GetComponent<Battleship>().shipCoords)
            {
                if (friendlyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == ourBattleship.size)
                    {
                        Debug.Log("Our Battleship Destroyed!");
                        ourBattleship.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }
        else if (shipName == "Carrier")
        {
            foreach (var coord in ourCarrier.gameObject.GetComponent<Carrier>().shipCoords)
            {
                if (friendlyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == ourCarrier.size)
                    {
                        Debug.Log("Our Carrier Destroyed!");
                        ourCarrier.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }
        else if (shipName == "Cruiser")
        {
            foreach (var coord in ourCruiser.gameObject.GetComponent<Cruiser>().shipCoords)
            {
                if (friendlyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == ourCruiser.size)
                    {
                        Debug.Log("Our Cruiser Destroyed!");
                        ourCruiser.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }
        else if (shipName == "Destroyer")
        {
            foreach (var coord in ourDestroyer.gameObject.GetComponent<Destroyer>().shipCoords)
            {
                if (friendlyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == ourDestroyer.size)
                    {
                        Debug.Log("Our Destroyer Destroyed!");
                        ourDestroyer.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }
        else if (shipName == "Submarine")
        {
            foreach (var coord in ourSubmarine.gameObject.GetComponent<Submarine>().shipCoords)
            {
                if (friendlyHitSpaces.Contains(coord))
                {
                    counter++;

                    if (counter == ourSubmarine.size)
                    {
                        Debug.Log("Our Submarine Destroyed!");
                        ourSubmarine.intact = false;
                        destroyed = true;
                    }
                }
            }
            counter = 0;
        }

        checkFriendlyDefeated();

        return destroyed;
    }

    public void checkEnemyDefeated()
    {
        if (!(enemyBattleship.intact || enemyCarrier.intact || enemyCruiser.intact || enemyDestroyer.intact || enemySubmarine.intact))
        {
            Debug.Log("You are the winner :)");
        }
    }

    public void checkFriendlyDefeated()
    {
        if (!(ourBattleship.intact || ourCarrier.intact || ourCruiser.intact || ourDestroyer.intact || ourSubmarine.intact))
        {
            Debug.Log("You are the loser :(");
        }
    }

    public void disableEnemyTiles()
    {
        foreach (Tile ourTile in ourEnemyTileList)
        {
            ourTile.gameObject.GetComponent<tileAttack>().attackable = false;
        }
    }

    public void enableEnemyTiles()
    {
        foreach (Tile ourTile in ourEnemyTileList)
        {
            ourTile.gameObject.GetComponent<tileAttack>().attackable = true;
        }
    }


    public Tile mainHitTile;
    public Tile prevHitTile;
    public bool firstAttack = true;
    public void enemyAttack()
    {
        Tile ourTile = chooseTileAttack();
        ourTile.gameObject.GetComponent<Tile>().attacked = true;

        Debug.Log(ourTile.name);

        if (ourTile.gameObject.GetComponent<Tile>().occupiedGameStart)
        {
            Debug.Log("Hit our ship!");
            ourTile.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

            prevHitTile = ourTile;
            if (!mainHitTile)
            {
                mainHitTile = ourTile;
                //Remove later when ship is destroyed
            }

            //Add ship to list of hit ships
            friendlyHitSpaces.Add(((int)ourTile.gameObject.transform.position.x, (int)ourTile.gameObject.transform.position.y));

            if (checkDestroyedFriendly(ourTile.gameObject.GetComponent<Tile>().occupierGameStart.name))
            {
                friendlyDestroyedDelay();
                mainHitTile = null;
                prevHitTile = null;
                firstAttack = true;
            }
            else
            {
                friendlyHitDelay();
            }
        }
        else
        {
            Debug.Log("Miss our ship!");
            ourTile.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            friendlyMissDelay();

        }

        //Remove attacked tile from total list
        ourTileListAttackTrack.Remove(ourTile);
    }

    public Tile chooseTileAttack()
    {
        if (!mainHitTile)
        {
            return ourTileListAttackTrack[Random.Range(0, ourTileListAttackTrack.Count - 1)];
        }

        if (firstAttack)
        {
            firstAttack = false;

            //0-->Up, 1-->Right, 2-->Down, 3-->Left
            int attackDir = Random.Range(0, 4);

            if (attackDir == 0)
            {

            }
        }

        return ourTileListAttackTrack[Random.Range(0, ourTileListAttackTrack.Count - 1)];
    }

    public void quitGame()
    {
        SceneManager.LoadScene(0);
    }
}