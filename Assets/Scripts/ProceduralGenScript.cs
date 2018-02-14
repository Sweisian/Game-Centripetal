using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ProceduralGenScript : MonoBehaviour
{

    public Vector2 areaOfExistence;
    private float fieldOfView;
    private Camera cam;

    public GameObject post;
    public Transform postParent;
    public GameObject player;
    private Rigidbody2D rb;

    public float maxRange = 200;
    private float newXPos;
    private float newYPos;
    private Vector3 randomPosition;
    private Vector3 lastPosition;
    public float DistanceFromLastPost = 50;
    private int numOfPosts = 0;
    public int numOfPostsMaxStart = 50;
    public int nummOfPostsPerArea = 50;
    public int maxNumOfPostsTotal = 2000;

    private int numberOfZones = 9;
    private GameObject[] zones;
    private Collider2D[] colliders;

    // Use this for initialization
    void Start ()
    {

        zones = new GameObject[numberOfZones];
        colliders = new Collider2D[numberOfZones];
        rb = player.GetComponent<Rigidbody2D>();

	    cam = GameObject.Find("Main Camera").GetComponent<Camera>();

	    // very basic post generation for starting area
	    for (int i = numOfPosts; i < numOfPostsMaxStart; i++)
	    {
	        newXPos = Random.Range(-maxRange, maxRange);
	        newYPos = Random.Range(player.transform.position.y - 10, maxRange);
	        randomPosition = new Vector2(newXPos, newYPos);
	        Object.Instantiate(post, randomPosition, Quaternion.identity, postParent); //Quaternion.identity is no rotation
	        numOfPosts++;
	        lastPosition = randomPosition;
	    }

        // Find all existing zones and their colliders
        zones = GameObject.FindGameObjectsWithTag("Zone");
        for (int i = 0; i < zones.Length; i++)
        {
            colliders[i] = zones[i].GetComponent<Collider2D>();
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
        // check if player has entered any other existing zone
	    for (int i  = 0; i < zones.Length; i++)
	    {
	        OnTriggerEnter2D(colliders[i]);
	    }
	    /*
        
        //fieldOfView = cam.fieldOfView;
        //Debug.Log(fieldOfView);

        // very basic post generation

        
        // check if max number of posts has been reached and if player is past the position of the last post spawned + an amount
        if (numOfPosts < maxNumOfPostsTotal && Mathf.Abs(player.transform.position.x + DistanceFromLastPost) > Mathf.Abs(lastPosition.x) 
            && Mathf.Abs(player.transform.position.y + DistanceFromLastPost) > Mathf.Abs(lastPosition.y))
        {   // based on direction
            //if (rb.velocity.y > 0) // if player is moving up the screen
            //{
               Debug.Log("rb x velocity is positive!");
                while (postParent.childCount < nummOfPostsPerArea)
                {
                    newXPos = Random.Range(player.transform.position.x + DistanceFromLastPost,
                        maxRange + DistanceFromLastPost);
                    newYPos = Random.Range(player.transform.position.y + DistanceFromLastPost,
                        maxRange + DistanceFromLastPost);
                    randomPosition = new Vector2(newXPos, newYPos);
                    Object.Instantiate(post, randomPosition, Quaternion.identity,
                        postParent); //Quaternion.identity is no rotation
                    numOfPosts++;
                }

            while (postParent.childCount < nummOfPostsPerArea)
            {
                newXPos = Random.Range(player.transform.position.x + DistanceFromLastPost,
                    maxRange + DistanceFromLastPost);
                newYPos = Random.Range(player.transform.position.y,
                    maxRange + DistanceFromLastPost);
                randomPosition = new Vector2(newXPos, newYPos);
                Object.Instantiate(post, randomPosition, Quaternion.identity,
                    postParent); //Quaternion.identity is no rotation
                numOfPosts++;
            }

            while (postParent.childCount < nummOfPostsPerArea)
            {
                newXPos = Random.Range(player.transform.position.x,
                    maxRange + DistanceFromLastPost);
                newYPos = Random.Range(player.transform.position.y + DistanceFromLastPost,
                    maxRange + DistanceFromLastPost);
                randomPosition = new Vector2(newXPos, newYPos);
                Object.Instantiate(post, randomPosition, Quaternion.identity,
                    postParent); //Quaternion.identity is no rotation
                numOfPosts++;
            } 
            */
	    //}
	    //} 


	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("New Zone entered!");
        //Create a new zone

        // but where??

        // Get new zones
        zones = GameObject.FindGameObjectsWithTag("Zone");
        for (int i = 0; i < zones.Length; i++)
        {
            colliders[i] = zones[i].GetComponent<Collider2D>();
        }
    }
        

        /* next:
         * remove posts that are a set distance behind the player
         *      check if between a distance from player and start?
         *      or check if behind dust storm
         * generate posts a set distance in front of player as player moves
         *      
         * */


    
}
