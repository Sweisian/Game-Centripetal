using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public ProceduralGenManager proceduralGenScript;
    public int difficulty = 15;
    private Collider2D[] colliders = new Collider2D[3];
    private Collider2D playerCollider;
    public static ProceduralGenManager.Zone currZone;

    [SerializeField] private GameObject maxDistPrefab;

    private Dictionary<string, AudioSource> sounds;
    public bool gameover = false;
    public GameObject gameoverText;
    [SerializeField] private GameObject alertPrefab;

    void Awake()
    {
        //colliders = new Collider2D[proceduralGenScript.Zones.Length];
        proceduralGenScript = GetComponent<ProceduralGenManager>();
        InitGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<Collider2D>();

        //puts a new max distance prefab at the highest completed distance
        Instantiate(maxDistPrefab, new Vector3(0, PlayerPrefs.GetFloat("bestDistance"), 0), Quaternion.identity);
    }

    void InitGame()
    {
        proceduralGenScript.SetupScene(difficulty);
        for (int i = 0; i < proceduralGenScript.Zones.Count; i++) 
        {
            // get collider objects
            colliders[i] = proceduralGenScript.Zones[i].collider;
        }

        currZone = proceduralGenScript.Zones[0]; //player must always start in the first zone
    }

    /* Unity is stupid and won't serialize dictionaries
     * So we'll just have to add the things manually to here.
    */


	// Use this for initialization
	void Start () {
        sounds = new Dictionary<string, AudioSource>();
        AudioSource[] clips = GetComponents<AudioSource>();
        sounds.Add("attach", clips[0]);
        sounds.Add("detach", clips[1]);
        sounds.Add("throw", clips[2]);
        sounds.Add("snap", clips[3]);
        sounds.Add("gameOver", clips[4]);
        sounds.Add("moo", clips[5]);
        sounds.Add("backgroundMusic", clips[6]);
        sounds.Add("collectCoin",clips[7]);
        gameoverText.SetActive(false);
	}

	
	// Update is called once per frame
	void Update () {
		//Reset the game when the player presses R
		if (Input.GetKeyDown (KeyCode.R)) {
			restartGame ();
		}

        //temp code for reseting max distance and max score
	    if (Input.GetKeyDown(KeyCode.P))
	    {
	        PlayerPrefs.DeleteKey("bestDistance");
	        PlayerPrefs.DeleteKey("bestScore");
        }
    }


    public void gameOver()
    {
        //Debug.Log("Game over state is true");
            gameover = true;
            gameoverText.SetActive(true);
            playSound("gameOver");
            Time.timeScale = 0f;

            if (Input.GetKey(KeyCode.R))
            {
                Debug.Log("Game over state is true and R is pressed");
                gameover = false;
                restartGame();
            }
    }

	/// <summary>
	/// Restarts the game by reloading this scene.
	/// </summary>
	public void restartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene ().buildIndex);
	    Time.timeScale = 1f;
        Debug.Log ("Game Reset");
	}

    public void playSound(string key)
    {
        AudioSource a = sounds[key];
        if (a!=null)
        {
            a.Play();
        }
    }

    public void sendAlert(string wannaSay, Color col)
    {
        GameObject alertMessage = GameObject.Instantiate(alertPrefab, GameObject.FindGameObjectWithTag("MainCamera").transform);
        AlertScript alertMessageScript = alertMessage.GetComponent<AlertScript>();
        alertMessageScript.message = wannaSay;
        alertMessageScript.c = col;
    }
		
}
