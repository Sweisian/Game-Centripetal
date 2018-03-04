using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public ProceduralGenManager proceduralGenScript;
    public float difficulty = 15;
    public float difficultyModifier = 1;
    private Collider2D[] colliders = new Collider2D[3];
    private Collider2D playerCollider;
    public static ProceduralGenManager.Zone currZone;

    [SerializeField] private GameObject maxDistPrefab;
    private Dictionary<string, AudioSource> sounds;
    private Text alertText;
    public bool gameover = false;
    public GameObject gameoverText;
    [SerializeField] private int numTimesToFlashAlert;
    [SerializeField] private float secondsPerAlertFlash;

    void Awake()
    {
        //colliders = new Collider2D[proceduralGenScript.Zones.Length];
        proceduralGenScript = GetComponent<ProceduralGenManager>();
        InitGame();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<Collider2D>();
        //puts a new max distance prefab at the highest completed distance
        Instantiate(maxDistPrefab, new Vector3(0, PlayerPrefs.GetFloat("bestDistance"), 0), Quaternion.identity);
        alertText = GameObject.FindGameObjectWithTag("AlertText").GetComponent<Text>();
        Color c = alertText.color;
        c.a = 0f;
        alertText.color = c;
    }

    /* Unity is stupid and won't serialize dictionaries
    * So we'll just have to add the things manually to here.
    */
    void Start()
    {
        gameoverText.SetActive(false);
        sounds = new Dictionary<string, AudioSource>();
        AudioSource[] clips = GetComponents<AudioSource>();
        sounds.Add("cannonFire", clips[1]);
        sounds.Add("attach", clips[2]);
        sounds.Add("throw", clips[3]);
        sounds.Add("coinCollect", clips[4]);
        sounds.Add("detatch", clips[5]);
        sounds.Add("game_over", clips[6]);
        sounds.Add("snap", clips[7]);
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
	
	// Update is called once per frame
	void Update () {
		//Reset the game when the player presses R
		if (Input.GetKeyDown (KeyCode.R)) {
			restartGame ();
		}
        
	    increaseDifficulty();

        //temp code for reseting max distance and max score
	    if (Input.GetKeyDown(KeyCode.P))
	    {
	        PlayerPrefs.DeleteKey("bestDistance");
	        PlayerPrefs.DeleteKey("bestScore");
        }
    }


    public void increaseDifficulty()
    {
        difficulty += Time.deltaTime * difficultyModifier;
    }

    public void gameOver()
    {
        //Debug.Log("Game over state is true");
            gameover = true;
            gameoverText.SetActive(true);
            playSound("game_over");
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
        Debug.Log("Trying to play " + key);
        if (a != null)
        {
            Debug.Log("Playing "+key);
            a.Play();
        }
        else Debug.Log(key + " not found!");
    }

    public void sendAlert(string wannaSay, Color col)
    {
        Color c = col;
        c.a = 0f;
        alertText.color = c;
        alertText.text = wannaSay;
        StartCoroutine(fadeIn(secondsPerAlertFlash / 2));
    }

    /* Following two functions adapted from https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/*/
    private IEnumerator fadeIn(float seconds)
    {
        while (alertText.color.a<1.0f)
        {
            alertText.color = 
                new Color(alertText.color.r, alertText.color.g, alertText.color.b, alertText.color.a + (Time.deltaTime / seconds));
            yield return null;
        }
        StartCoroutine(fadeOut(secondsPerAlertFlash/2));
    }

    private IEnumerator fadeOut(float seconds)
    {
        while (alertText.color.a > 0f)
        {
            alertText.color =
                new Color(alertText.color.r, alertText.color.g, alertText.color.b, alertText.color.a - (Time.deltaTime / secondsPerAlertFlash));
            yield return null;
        }
    }
		
}
