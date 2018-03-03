using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGenManager : MonoBehaviour {
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
    // set size of game board (8x8)
    public int columns = 10;
    private static int columnsSize;
    public int rows = 25;
    private static int rowsSize;
    private static int nextZoneID = 0;

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
        public float difficulty;
        public Vector2 size;
        public BoxCollider2D collider;
        public int ID; //0, 1, or 2
        public bool beenEntered;

        public Zone(List<Tile> gridPos, Vector3 loc, float dif)
        {
            gridPositions = gridPos;
            location = loc;
            difficulty = dif;
            size = new Vector2(columnsSize, rowsSize);
            beenEntered = false;
            ID = nextZoneID;
            IncrementNextZoneId();
        }

        public void setCollider()
        {
            collider = zoneObject.GetComponent<BoxCollider2D>();
            collider.size = size;
            collider.offset = new Vector2(0, rowsSize / 2);
            collider.isTrigger = true;
        }

        
    }

    

    public List<Zone> Zones = new List<Zone>();
    public Vector3 startPosition = new Vector3(0,0,0);
    public Count postCount = new Count(50, 100);
    //public Count obstacleCount = new Count(1, 5); //use for constant number of obstacles
    public GameObject[] obstacles;
    public GameObject post;
    private GameController gameControllerScript;

    //private Transform boardHolder;
    //private List<List<Tile>> gridPositions;
    //private List<Tile> gridPositions0 = new List<Tile>();
    //private List<Tile> gridPositions1 = new List<Tile>();
    //private List<Tile> gridPositions2 = new List<Tile>();
    //private List<Tile>[] zoneList = new List<Tile>[3]; //array of lists of gridpositions to keep track of 3 zones

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

        //Zones[1] = gridPositions1;
        //Zones[2] = gridPositions2;
        columnsSize = columns * itemSize;
        rowsSize = rows * itemSize;
        

        List<Tile> gridPositionsNew = new List<Tile>();
        Zone tempZone = new Zone(gridPositionsNew, startPosition, 0);
        Zones.Add(tempZone);
        IncrementNextZoneId();
        //Zones[0].beenEntered = false; //because player starts in the first zone
        Vector3 prevZoneLocation = Zones[0].location;

        // initialize and clear three Zones
        for (int i = 0; i < 3; i++)
        {
            if (i > 0)
            {
                //gridPositions[i] = new List<Tile>();
                Vector3 newZoneLocation = new Vector3(prevZoneLocation.x, prevZoneLocation.y + rowsSize);
                Zones.Add(new Zone(gridPositionsNew, newZoneLocation, 0));
                IncrementNextZoneId();
                Debug.Log(Zones[i].location);
                prevZoneLocation = newZoneLocation;
            }
            GameObject newZone = Instantiate(ZonePrefab, Zones[i].location, Quaternion.identity);
            Zones[i].zoneObject = newZone; //set the zone class's zoneObject to be equal to the zone prefab
            Zones[i].setCollider(); //sets up the collider
            
            Zones[i].gridPositions.Clear(); // clear each list gridPositions
        }
    
        //base gridPositions on zone location

        foreach (Zone zone in Zones)
        {
            // initialize all 3 zones

            for (int x = -columnsSize; x < columnsSize; x += itemSize) //start at negative coordinate so that grid is centered
            {
                for (float y = zone.location.y; y < zone.location.y + rowsSize; y += itemSize)
                {
                    // creates a position for each grid position that are itemSize distance apart
                    zone.gridPositions.Add(new Tile(x, y));
                }
            }
        }
    }

    public void AddZone(Zone zone)
    {
        
        Debug.Log("RefreshZone");
        // given a zone and a difficulty, will reset it to be in front of the player
        // currently just makes a new zone
        gameControllerScript = GetComponent<GameController>();
        int obstacleCount = (int)Mathf.Log(gameControllerScript.difficulty, 2f); //add more obstables when difficulty is higher 
        Debug.Log(gameControllerScript.difficulty);
        Vector3 newZoneLocation;
        if (nextZoneID == 0)
        {
            newZoneLocation = new Vector3(Zones[2].location.x, Zones[2].location.y + rowsSize);
        }
        else if (nextZoneID == 1)
        {
            newZoneLocation = new Vector3(Zones[1].location.x, Zones[1].location.y + rowsSize);
        }
        else if (nextZoneID == 2)
        {
            newZoneLocation = new Vector3(Zones[0].location.x, Zones[0].location.y + rowsSize);
        }

        else //once past initial 3 zones
        {
            newZoneLocation = new Vector3(Zones[Zones.Count - 1].location.x,
                Zones[Zones.Count - 1].location.y + rowsSize);
        }

        Zone newZone = new Zone(zone.gridPositions, newZoneLocation, gameControllerScript.difficulty);
        //zone.location = newZoneLocation; //put new zone in the correct location

        GameObject newZoneObj = Instantiate(ZonePrefab, newZone.location, Quaternion.identity);
        newZone.zoneObject = newZoneObj; //set the zone class's zoneObject to be the zone that was just instatiated
        newZone.setCollider(); //sets up the collider

        newZone.gridPositions.Clear();

        for (int x = -columnsSize; x < columnsSize; x += itemSize) //start at negative coordinate so that grid is centered
        {
            
            
                for (float y = newZone.location.y; y < newZone.location.y + rowsSize; y += itemSize)
                {
                    // creates a position for each grid position that are itemSize distance apart
                    newZone.gridPositions.Add(new Tile(x, y));
                }
            
            
        }

        // set up new zone
        LayoutObstaclesAtRandom(zone, obstacles, obstacleCount, obstacleCount);
        LayoutPostsAtRandom(zone);

        Zones.Add(newZone);
        //IncrementNextZoneId();

    }

    void BoardSetup()
    {
        // they used to set up walls and floor
    }

    // creates a Random Position based on the currently available grid positions
    Vector3 RandomPosition(List<Tile> gridPositions)
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex].position;
        gridPositions.RemoveAt(randomIndex); //ensures that no grid position gets multiple objects
        return randomPosition;
    }

    void LayoutObstaclesAtRandom(Zone zone, GameObject[] obstacleArray, int minimum, int maximum)
    { //layout obstacles in the specified zone
        int objectCount = Random.Range(minimum, maximum + 1);

        // create objects until objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {

            Vector3 randomPosition = RandomPosition(zone.gridPositions);
            if (nextZoneID < 7)
            {
                // if the player is in the first 3 zones, ensure that no obstacles spawn on the y axis
                while (randomPosition.x > -5 & randomPosition.x < 5)
                {
                    randomPosition = RandomPosition(zone.gridPositions);
                }
            }

            GameObject obstacleChoice = obstacleArray[Random.Range(0, obstacleArray.Length)];
            Instantiate(obstacleChoice, randomPosition, Quaternion.identity);

        }
    }

    void LayoutPostsAtRandom(Zone zone)
    { //layout posts in the specified zone
        int postCountValue = Random.Range(postCount.minimum, postCount.maximum);
        for (int i = 0; i < postCountValue; i++)
        {
            Vector3 randomPosition = RandomPosition(zone.gridPositions);
            if (nextZoneID < 7)
            {
                // if the player is in the first 3 zones, ensure that no posts spawn on the y axis
                while (randomPosition.x > -5 & randomPosition.x < 5)
                {
                    randomPosition = RandomPosition(zone.gridPositions);
                }
            }
            

            //GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(post, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(float difficulty)
    {
        //BoardSetup();
        InitalizeList();
        int obstacleCount = (int)Mathf.Log(difficulty, 2f); //add more obstables when difficulty is higher 
        foreach (Zone zone in Zones)
        {
            // set up all zones
            LayoutObstaclesAtRandom(zone, obstacles, obstacleCount, obstacleCount);
            LayoutPostsAtRandom(zone);

        }
    }



    public static void IncrementNextZoneId()
    {
        /*
        //ZoneID can be 0, 1, or 2
        if (nextZoneID < 2)
        {
            nextZoneID++;
        }
        else
        {
            nextZoneID = 0;
        }
        */
        nextZoneID++;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }
}
