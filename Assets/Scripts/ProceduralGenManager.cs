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

    // set size of game board (8x8)
    public int columns = 8;
    public int rows = 8;
    public Count postCount = new Count(10, 30);
    //public Count obstacleCount = new Count(1, 5); //use for constant number of obstacles
    public GameObject[] obstacles;

    public GameObject post;

    //private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitalizeList()
    {
        // clear the list gridPositions
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                // creates a position for each grid position
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        // they used to set up walls and floor
    }

    // creates a Random Position based on the currently available grid positions
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex); //ensures that no grid position gets multiple objects
        return randomPosition;
    }

    void LayoutObstaclesAtRandom(GameObject[] obstacleArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        // create objects until objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {

            Vector3 randomPosition = RandomPosition();
            GameObject obstacleChoice = obstacleArray[Random.Range(0, obstacleArray.Length)];
            Instantiate(obstacleChoice, randomPosition, Quaternion.identity);

        }
    }

    void LayoutPostsAtRandom()
    {
        int postCountValue = Random.Range(postCount.minimum, postCount.maximum);
        for (int i = 0; i < postCountValue; i++)
        {
            Vector3 randomPosition = RandomPosition();
            //GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(post, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int difficulty)
    {
        BoardSetup();
        InitalizeList();
        int obstacleCount = (int)Mathf.Log(difficulty, 2f); //add more obstables when difficulty is higher 
        LayoutObstaclesAtRandom(obstacles, obstacleCount, obstacleCount);
        LayoutPostsAtRandom();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
