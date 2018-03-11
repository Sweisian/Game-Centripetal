using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGenManagerMainMenu : MonoBehaviour {
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public static int itemSize = 20;
    // set size of game board (10x25)
    public int columns = 7;
    private static int columnsSize;
    public int rows = 25;
    private static int rowsSize;
    public static int zoneID = 0;
    [SerializeField] private GameObject backgroundObj;

    public class Tile
    {
        public int width;
        public int height;
        public Vector3 position;

        public Tile(float xPos, float yPos)
        {
            width = height = itemSize;
            position = new Vector3(xPos, yPos, 0f);
        }
    }

    public GameObject ZonePrefab;

    public class Zone
    {
        public GameObject zoneObject;
        public List<Tile> gridPositions;
        public Vector3 location;
        public GameObject background;
        public float difficulty;
        public Vector2 size;
        public BoxCollider2D collider;
        public int ID; //0, 1, or 2
        public bool beenEntered;

        public Zone(Vector3 loc, float dif)
        {
            gridPositions = new List<Tile>();
            location = loc;
            difficulty = dif;
            size = new Vector2(columnsSize, rowsSize);
            beenEntered = false;
            ID = zoneID;
            zoneID++;
        }

        public void setCollider()
        {
            collider = zoneObject.GetComponent<BoxCollider2D>();
            collider.size = size;
            collider.offset = new Vector2(0, rowsSize / 2);
            collider.isTrigger = true;
        }

        public void setupZone(GameObject zonePrefab, GameObject backgroundObj)
        {
            // zone should already have been created using the Zone constructor
            GameObject newZoneObj = Instantiate(zonePrefab, this.location, Quaternion.identity);
            this.zoneObject = newZoneObj; //set the zone class's zoneObject to be the zone that was just instatiated
            this.setCollider(); //sets up the collider

            this.gridPositions.Clear();

            //this code puts in the background image and makes them children of the grid

            //Debug.Log("Putting in background image");
            this.background = backgroundObj;
            GameObject bg = Instantiate(backgroundObj, this.location, Quaternion.identity);
            
            bg.SetActive(true);
            bg.transform.parent = GameObject.FindGameObjectWithTag("Grid").transform;
            bg.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    

    public List<Zone> Zones = new List<Zone>();
    public Vector3 startPosition = new Vector3(0,0,0);
    public Count postCount = new Count(70, 100);
    public GameObject[] superEasyObstacles;
    public GameObject[] easyObstacles;
    public GameObject[] obstacles;
    public GameObject post;
    private GameController gameControllerScript;

    public Zone getZone(GameObject zoneGameObject)
    { //takes a collider and finds its assosiated zones
        foreach (Zone zone in Zones)
        {
            if (zone.zoneObject.transform.position == zoneGameObject.transform.position)
            {
                return zone;
            }
        }
        return Zones[0];
    }

    void InitalizeList()
    {
        columnsSize = columns * itemSize;
        rowsSize = rows * itemSize;
        //List<Tile> gridPositionsNew = new List<Tile>();
        Zone tempZone = new Zone(startPosition, 0);
        Zones.Add(tempZone);
        tempZone.setupZone(ZonePrefab, backgroundObj);
        Vector3 prevZoneLocation = Zones[0].location;
        for (int x = -columnsSize; x < columnsSize + itemSize; x += itemSize) //start at negative coordinate so that grid is centered
        {
            for (float y = tempZone.location.y; y < tempZone.location.y + rowsSize; y += itemSize)
            {
                // creates a position for each grid position that are itemSize distance apart
                tempZone.gridPositions.Add(new Tile(x, y));
            }
        }
        AddZone();
        AddZone();
    }

    public void AddZone()
    {
        //Debug.Log("RefreshZone");
        // makes a new zone in front of the most recently generated one
        gameControllerScript = GetComponent<GameController>();
        int obstacleCount = (int)Mathf.Log(gameControllerScript.difficulty, 2f); //add more obstables when difficulty is higher 
        //Debug.Log(gameControllerScript.difficulty);
        Vector3 newZoneLocation;
        
        if (zoneID == 1)
        {
            newZoneLocation = new Vector3(Zones[0].location.x, Zones[0].location.y + rowsSize);
        }
        else if (zoneID == 2)
        {
            newZoneLocation = new Vector3(Zones[1].location.x, Zones[1].location.y + rowsSize);
        }

        else //once past initial 3 zones
        {
            newZoneLocation = new Vector3(Zones[Zones.Count - 1].location.x,
                Zones[Zones.Count - 1].location.y + rowsSize);
        }

        Zone newZone = new Zone(newZoneLocation, gameControllerScript.difficulty);
        newZone.setupZone(ZonePrefab, backgroundObj);
        
        for (int x = -columnsSize; x < columnsSize + itemSize; x += itemSize) //start at negative coordinate so that grid is centered
        { 
                for (float y = newZone.location.y; y < newZone.location.y + rowsSize; y += itemSize)
                {
                    // creates a position for each grid position that are itemSize distance apart
                    newZone.gridPositions.Add(new Tile(x, y));
                }
        }

        // set up new zone
        LayoutObstaclesAtRandom(newZone, obstacleCount, obstacleCount);
        LayoutPostsAtRandom(newZone);
        Zones.Add(newZone);
    }

    // creates a Random Position based on the currently available grid positions
    Vector3 RandomPosition(List<Tile> gridPositions)
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex].position;
        gridPositions.RemoveAt(randomIndex); //ensures that no grid position gets multiple objects
        return randomPosition;
    }

    void LayoutObstaclesAtRandom(Zone zone, int minimum, int maximum)
    { //layout obstacles in the specified zone
        int objectCount = Random.Range(minimum, maximum + 1);

        // create objects until objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition(zone.gridPositions);

            // if the player is in the first 3 zones, ensure that no obstacles spawn on the y axis
            if (zone.ID <= 3)
            {
                while (randomPosition.x > -5 & randomPosition.x < 5)
                {
                    randomPosition = RandomPosition(zone.gridPositions);
                }
            }

            // chooses which array of obstacles to use based on the zone
            GameObject obstacleChoice;
            if (zone.ID == 0)
            {
                obstacleChoice = superEasyObstacles[Random.Range(0, superEasyObstacles.Length)];
            }
            else if (zone.ID <= 3)
            {
                obstacleChoice = easyObstacles[Random.Range(0, easyObstacles.Length)];
            }
            else
            {
                obstacleChoice = obstacles[Random.Range(0, obstacles.Length)];

            }
            //gives the shipwrecks a random rotation
            if(obstacleChoice.tag == "Shipwreck")
                Instantiate(obstacleChoice, randomPosition, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
            else
                Instantiate(obstacleChoice, randomPosition, Quaternion.identity);
        }
    }

    void LayoutPostsAtRandom(Zone zone)
    { //layout posts in the specified zone
        int postCountValue = Random.Range(postCount.minimum, postCount.maximum);
        for (int i = 0; i < postCountValue; i++)
        {
            Vector3 randomPosition = RandomPosition(zone.gridPositions);
           
            
            // if the player is in the first 3 zones, ensure that no posts spawn on the y axis
            if (zone.ID == 0)
            {
                while (randomPosition.x > -5 & randomPosition.x < 5)
                {
                    randomPosition = RandomPosition(zone.gridPositions);
                }
            }
            Instantiate(post, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(float difficulty)
    {
        //BoardSetup();
        InitalizeList();
        int obstacleCount = (int)Mathf.Log(difficulty, 2f); //add more obstables when difficulty is higher 
        // set up the first zone (the other zones get initialized in InitializeList
        LayoutObstaclesAtRandom(Zones[0], obstacleCount, obstacleCount);
        LayoutPostsAtRandom(Zones[0]);
    }
}
