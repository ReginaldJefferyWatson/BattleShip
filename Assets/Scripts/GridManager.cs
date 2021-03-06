using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public bool enemyTurn = false;

    public AudioSource shipMissed;
    public AudioSource shipHit;
    public AudioSource enemyShipDestroyed;
    public AudioSource friendlyShipDestroyed;

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

            //Change turns
            enemyTurn = false;
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
                if (!enemyTurn)
                {
                    enableEnemyTiles();
                }

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
        enemy_Carrier_Tile.GetComponent<SpriteRenderer>().enabled = false;
        enemyCarrier = enemy_Carrier_Tile;

        int rotateVal = rotations[Random.Range(0, 3)];
        int xVal = Random.Range(0, 9);
        int yVal = Random.Range(12, 21);
        var enemy_Battleship_Tile = Instantiate(battleshipPrefab, new Vector3(xVal, yVal), Quaternion.Euler(0, 0, rotateVal));
        enemy_Battleship_Tile.GetComponent<SpriteRenderer>().enabled = false;
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
        enemy_Cruiser_Tile.GetComponent<SpriteRenderer>().enabled = false;
        enemyCruiser = enemy_Cruiser_Tile;

        rotateVal = rotations[Random.Range(0, 3)];
        xVal = Random.Range(0, 9);
        yVal = Random.Range(12, 21);
        var enemy_Destroyer_Tile = Instantiate(destroyerPrefab, new Vector3(xVal, yVal), Quaternion.Euler(0, 0, rotateVal));
        enemy_Destroyer_Tile.GetComponent<SpriteRenderer>().enabled = false;
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
        enemy_Sub_Tile.GetComponent<SpriteRenderer>().enabled = false;
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

            //Reactivate sprite visibility
            enemyCarrier.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            enemyBattleship.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            enemyCruiser.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            enemyDestroyer.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            enemySubmarine.gameObject.GetComponent<SpriteRenderer>().enabled = true;

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
        shipHit.Play();

        enemyHitButton.SetActive(true);
        disappearTime = Time.time + enemyHitButtonTime;
    }

    public void enemyDestroyedDelay()
    {
        enemyShipDestroyed.Play();

        enemyDestroyedButton.SetActive(true);
        disappearTime = Time.time + enemyDestroyedButtonTime;
    }

    public void enemyMissDelay()
    {
        shipMissed.Play();

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
        shipHit.Play();

        friendlyHitButton.SetActive(true);
        disappearTime = Time.time + friendlyHitButtonTime;
    }

    public void friendlyDestroyedDelay()
    {
        friendlyShipDestroyed.Play();

        friendlyDestroyedButton.SetActive(true);
        disappearTime = Time.time + friendlyDestroyedButtonTime;
    }

    public void friendlyMissDelay()
    {
        shipMissed.Play();

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

        //Allow for temporary list
        //tempHitTileList = hitTileList;

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

        if (destroyed)
        {

            Debug.Log("Ship was destroyed");

            prevHitTile = null;
            initialAttack = true;

            
            for(int i = 0; i < hitTileList.Count; i++)
            {
                //If it's not a destroyed tile
                if(hitTileList[i].gameObject.GetComponent<Tile>().occupierGameStart.name != shipName)
                {
                    finalHitTileList.Add(hitTileList[i]);
                }
            }

            hitTileList.Clear();

            //Update hitTileList with still viable hit points
            foreach(Tile ourTile in finalHitTileList)
            {
                hitTileList.Add(ourTile);
            }

            finalHitTileList.Clear();

            //If these aren't equal, that means mainHitTile still hasn't been destroyed, and should stay the same
            if(shipName == mainHitTile.GetComponent<Tile>().occupierGameStart.name)
            {
                //If this list isn't empty, there are more hit spots that haven't been destroyed
                if (hitTileList.Count != 0)
                {
                    mainHitTile = hitTileList[Random.Range(0, hitTileList.Count)];
                    Debug.Log("NEW MAIN TILE");
                    Debug.Log(mainHitTile.transform.position.x);
                    Debug.Log(mainHitTile.transform.position.y);
                }
                else
                {
                    mainHitTile = null;
                }
            }

        }

        return destroyed;
    }

    public void checkEnemyDefeated()
    {
        if (!(enemyBattleship.intact || enemyCarrier.intact || enemyCruiser.intact || enemyDestroyer.intact || enemySubmarine.intact))
        {
            Debug.Log("You are the winner :)");

            SceneManager.LoadScene(2);
        }
    }

    public void checkFriendlyDefeated()
    {
        if (!(ourBattleship.intact || ourCarrier.intact || ourCruiser.intact || ourDestroyer.intact || ourSubmarine.intact))
        {
            Debug.Log("You are the loser :(");

            SceneManager.LoadScene(3);
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


    public Tile mainHitTile = null;
    public Tile prevHitTile = null;
    //Need to reset this after a ship is destroyed
    public bool initialAttack = true;
    public int attackDir;
    //This is to keep track of tiles that are hit but not destroyed
    public List<Tile> hitTileList;
    public List<Tile> finalHitTileList;
    public void enemyAttack()
    {
        Tile ourTile = chooseTileAttack();
        ourTile.gameObject.GetComponent<Tile>().attacked = true;

        Debug.Log(ourTile.name);

        if (ourTile.gameObject.GetComponent<Tile>().occupiedGameStart)
        {
            Debug.Log("Hit our ship!");
            ourTile.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

            hitTileList.Add(ourTile);

            Debug.Log(hitTileList.Count);

            if (!mainHitTile)
            {
                //Debug.Log("HERE");
                mainHitTile = ourTile;
                //Remove later when ship is destroyed
            }

            //Set this as previously hit once one has already been hit
            if (!initialAttack)
            {
                prevHitTile = ourTile;
            }

            //If a tile hit isn't the same type as the main tile

            //Add ship to list of hit ships
            friendlyHitSpaces.Add(((int)ourTile.gameObject.transform.position.x, (int)ourTile.gameObject.transform.position.y));

            if (checkDestroyedFriendly(ourTile.gameObject.GetComponent<Tile>().occupierGameStart.name))
            {
                friendlyDestroyedDelay();
                //mainHitTile = null;
                //prevHitTile = null;
                //initialAttack = true;
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

        //Change turn
        enemyTurn = false;

        //Remove attacked tile from total list
        ourTileListAttackTrack.Remove(ourTile);
    }

    public Tile chooseTileAttack()
    {
        //A tile HAS TO be attacked here
        while (true) {
            Debug.Log("Back Here");

            if (!mainHitTile)
            {
                return ourTileListAttackTrack[Random.Range(0, ourTileListAttackTrack.Count)];
            }

            if (!prevHitTile)
            {
                //0-->Up, 1-->Right, 2-->Down, 3-->Left
                attackDir = Random.Range(0, 4);

                //Loop until a ship is hit
                while (true)
                {
                    if (attackDir == 0)
                    {
                        foreach (Tile ourTile in ourTileListAttackTrack)
                        {
                            if ((ourTile.transform.position.x == mainHitTile.transform.position.x) && (ourTile.transform.position.y == mainHitTile.transform.position.y + 1))
                            {
                                initialAttack = false;
                                return ourTile;
                            }
                        }

                        //If the tile was not found, attack elsewhere
                        attackDir = (attackDir + Random.Range(1, 3)) % 4;

                        //This will make it so we don't have to keep track of previously attacked, but missed, spaces
                    }
                    else if (attackDir == 1)
                    {
                        foreach (Tile ourTile in ourTileListAttackTrack)
                        {
                            if ((ourTile.transform.position.x == mainHitTile.transform.position.x + 1) && ((ourTile.transform.position.y) == mainHitTile.transform.position.y))
                            {
                                initialAttack = false;
                                return ourTile;
                            }
                        }

                        attackDir = (attackDir + Random.Range(1, 3)) % 4;
                    }
                    else if (attackDir == 2)
                    {
                        foreach (Tile ourTile in ourTileListAttackTrack)
                        {
                            if ((ourTile.transform.position.x == mainHitTile.transform.position.x) && ((ourTile.transform.position.y) == mainHitTile.transform.position.y - 1))
                            {
                                initialAttack = false;
                                return ourTile;
                            }
                        }

                        attackDir = (attackDir + Random.Range(1, 3)) % 4;
                    }
                    else if (attackDir == 3)
                    {
                        foreach (Tile ourTile in ourTileListAttackTrack)
                        {
                            if ((ourTile.transform.position.x == mainHitTile.transform.position.x - 1) && ((ourTile.transform.position.y) == mainHitTile.transform.position.y))
                            {
                                initialAttack = false;
                                return ourTile;
                            }
                        }

                        attackDir = (attackDir + Random.Range(1, 3)) % 4;
                    }
                }

            }
            //Now, there is a direction to move in
            else
            {
                if (attackDir == 0)
                {
                    Debug.Log("0");
                    foreach (Tile ourTile in ourTileListAttackTrack)
                    {
                        if ((ourTile.transform.position.x == prevHitTile.transform.position.x) && (ourTile.transform.position.y == prevHitTile.transform.position.y + 1))
                        {
                            return ourTile;
                        }
                    }

                    //If a tile isn't found in that direction, go the other way
                    foreach (Tile ourTile in ourTileListAttackTrack)
                    {
                        if ((ourTile.transform.position.x == prevHitTile.transform.position.x) && (ourTile.transform.position.y == mainHitTile.transform.position.y - 1))
                        {
                            attackDir = 2;
                            return ourTile;
                        }
                    }

                    //If neither of these work, it's a multiple ship scenario. Remove last hit tile, it's irrelevant
                    prevHitTile = null;
                }
                else if (attackDir == 1)
                {
                    Debug.Log("1");
                    foreach (Tile ourTile in ourTileListAttackTrack)
                    {
                        if ((ourTile.transform.position.x == prevHitTile.transform.position.x + 1) && (ourTile.transform.position.y == prevHitTile.transform.position.y))
                        {
                            return ourTile;
                        }
                    }

                    //If a tile isn't found in that direction, go the other way
                    foreach (Tile ourTile in ourTileListAttackTrack)
                    {
                        if ((ourTile.transform.position.x == mainHitTile.transform.position.x - 1) && (ourTile.transform.position.y == mainHitTile.transform.position.y))
                        {
                            attackDir = 3;
                            return ourTile;
                        }
                    }

                    //If neither of these work, it's a multiple ship scenario. Remove last hit tile, it's irrelevant
                    prevHitTile = null;
                }
                else if (attackDir == 2)
                {
                    Debug.Log("2");
                    foreach (Tile ourTile in ourTileListAttackTrack)
                    {
                        if ((ourTile.transform.position.x == prevHitTile.transform.position.x) && (ourTile.transform.position.y == prevHitTile.transform.position.y - 1))
                        {
                            return ourTile;
                        }
                    }

                    //If a tile isn't found in that direction, go the other way
                    foreach (Tile ourTile in ourTileListAttackTrack)
                    {
                        if ((ourTile.transform.position.x == prevHitTile.transform.position.x) && (ourTile.transform.position.y == mainHitTile.transform.position.y + 1))
                        {
                            attackDir = 0;
                            return ourTile;
                        }
                    }

                    //If neither of these work, it's a multiple ship scenario. Remove last hit tile, it's irrelevant
                    prevHitTile = null;
                }
                else if (attackDir == 3)
                {
                    Debug.Log("3");
                    foreach (Tile ourTile in ourTileListAttackTrack)
                    {
                        if ((ourTile.transform.position.x == prevHitTile.transform.position.x - 1) && (ourTile.transform.position.y == prevHitTile.transform.position.y))
                        {
                            return ourTile;
                        }
                    }

                    //If a tile isn't found in that direction, go the other way
                    foreach (Tile ourTile in ourTileListAttackTrack)
                    {
                        if ((ourTile.transform.position.x == mainHitTile.transform.position.x + 1) && (ourTile.transform.position.y == mainHitTile.transform.position.y))
                        {
                            attackDir = 1;
                            return ourTile;
                        }
                    }

                    //If neither of these work, it's a multiple ship scenario. Remove last hit tile, it's irrelevant
                    prevHitTile = null;
                }
            }

            //If no tile in either direction, choose random just to break out
            //return ourTileListAttackTrack[Random.Range(0, ourTileListAttackTrack.Count)];

            //If no tile is found in either direction, this means we have to go back to the original main tile and randomly guess again
        }
    }

    public void quitGame()
    {
        //Reset timescale
        Time.timeScale = 1;

        SceneManager.LoadScene(0);
    }
}