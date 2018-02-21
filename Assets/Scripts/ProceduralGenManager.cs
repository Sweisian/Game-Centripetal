using System;
using System.Collections;
using System.Collections.Generic;
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
    public int columns = 8;
    private static int columnsSize;
    public int rows = 8;
    private static int rowsSize;

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
        public int difficulty;
        public Vector2 size;
        public BoxCollider2D collider;

        public Zone(List<Tile> gridPos, Vector3 loc, int dif)
        { 
            gridPositions = gridPos;
            location = loc;
            difficulty = dif;
            size = new Vector2(columnsSize, rowsSize);
            
        }

        public void setCollider()
        {
            collider = zoneObject.GetComponent<BoxCollider2D>();
            collider.size = size;
            collider.offset = new Vector2(0, rowsSize / 2);
            collider.isTrigger = true;
        }
    }

    public Zone[] Zones = new Zone[1];
    public Vector3 startPosition = new Vector3(0,0,0);
    public Count postCount = new Count(10, 30);
    //public Count obstacleCount = new Count(1, 5); //use for constant number of obstacles
    public GameObject[] obstacles;
    public GameObject post;

    //private Transform boardHolder;
    //private List<List<Tile>> gridPositions;
    //private List<Tile> gridPositions0 = new List<Tile>();
    //private List<Tile> gridPositions1 = new List<Tile>();
    //private List<Tile> gridPositions2 = new List<Tile>();
    //private List<Tile>[] zoneList = new List<Tile>[3]; //array of lists of gridpositions to keep track of 3 zones

    void InitalizeList()
    {

        //Zones[1] = gridPositions1;
        //Zones[2] = gridPositions2;
        columnsSize = columns * itemSize;
        rowsSize = rows * itemSize;
        

        List<Tile> gridPositionsNew = new List<Tile>(); 
        Zones[0] = new Zone(gridPositionsNew, startPosition, 0);
        Vector3 prevZoneLocation = Zones[0].location;

        // initialize and clear all the Zones
        for (int i = 0; i < Zones.Length; i++)
        {
            if (i > 0)
            {
                //gridPositions[i] = new List<Tile>();
                Vector3 newZoneLocation = new Vector3(prevZoneLocation.x, prevZoneLocation.y + columnsSize);
                Zones[i] = new Zone(gridPositionsNew, newZoneLocation, 0);
                prevZoneLocation = newZoneLocation;
            }     
            Zones[i].zoneObject = ZonePrefab; //set the zone class's zoneObject to be equal to the zone prefab
            Zones[i].setCollider(); //sets up the collider
            Instantiate(Zones[i].zoneObject, Zones[i].location, Quaternion.identity); 
            Zones[i].gridPositions.Clear(); // clear each list gridPositions
        }
    
        //base gridPositions on zone location

        foreach (Zone zone in Zones)
        {
            // initialize all 3 zones

            for (int x = -columnsSize;
                x < columnsSize;
                x += itemSize) //start at negative coordinate so that grid is centered
            {
                for (int y = 0; y < rowsSize; y += itemSize)
                {
                    // creates a position for each grid position 5 positions apart
                    int xPosition = x;
                    int yPosition = y;
                    zone.gridPositions.Add(new Tile(xPosition, yPosition));
                }
            }
        }
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
            //GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(post, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int difficulty)
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

    public void RefreshZone(Zone zone, int difficulty)
    {
        // given a zone and a difficulty, will reset it to be in front of the player

        int obstacleCount = (int)Mathf.Log(difficulty, 2f); //add more obstables when difficulty is higher 
        
        {
            // set up all zones
            LayoutObstaclesAtRandom(zone, obstacles, obstacleCount, obstacleCount);
            LayoutPostsAtRandom(zone);

        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
