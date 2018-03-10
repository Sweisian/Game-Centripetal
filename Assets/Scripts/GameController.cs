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
    //using singleton instance
    //public static GameController myGameController;

    public ProceduralGenManager proceduralGenScript;
    public float difficulty = 15;
    public float difficultyModifier = 1;
    private Collider2D[] colliders = new Collider2D[3];
    private Collider2D playerCollider;
    public static ProceduralGenManager.Zone currZone;

    private PlayerScript myPlayerScript;

    //game objects for best distance and prev best distance
    [SerializeField] private GameObject maxDistPrefab;
    [SerializeField] private GameObject prevBestPrefab;

    //previous run best distance stored here
    //private float previousBestDist;


    //min distance required to spawn a prev best prefab
    [SerializeField] private float prevBestMinThreshold = 10;

    private Dictionary<string, AudioSource> sounds;


    //alert text here
    private TextMeshProUGUI alertText;
    public bool gameover = false;
    public GameObject gameoverText;
    [SerializeField] private int numTimesToFlashAlert;
    [SerializeField] private float secondsPerAlertFlash;

    void Awake()
    {
        ////Singleton instance of this object
        //if (myGameController == null)
        //{
        //    myGameController = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //main cam is no longer destoryed on load
        //may cause problems with moving to a new scene later
        //DontDestroyOnLoad(gameObject);

        proceduralGenScript = GetComponent<ProceduralGenManager>();
        ProceduralGenManager.zoneID = 0; // reset the zone ID each time the game resets
        InitGame();

        //Gets player stuff
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        myPlayerScript = player.GetComponent<PlayerScript>();
        playerCollider = player.GetComponent<Collider2D>();

        //puts a new max distance prefab at the highest completed distance
        Instantiate(maxDistPrefab, new Vector3(0, PlayerPrefs.GetFloat("bestDistance"), 0), Quaternion.identity);

        //grabs the alert text from the UI
        alertText = GameObject.FindGameObjectWithTag("AlertText").GetComponent<TextMeshProUGUI>();
        Color c = alertText.color;
        c.a = 0f;
        alertText.color = c;

        if (PlayerPrefs.HasKey("prevBestDistance"))
        { 
            Instantiate(prevBestPrefab, new Vector3(0, PlayerPrefs.GetFloat("prevBestDistance"), 0),Quaternion.identity);
            Debug.Log("Instantiating a new prev distance prefab");

            PlayerPrefs.DeleteKey("prevBestDistance");
        }
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
        sounds.Add("smallPoints", clips[8]);
        sounds.Add("mediumPoints", clips[9]);
        sounds.Add("bigPoints", clips[10]);
        sounds.Add("powerUpEnd", clips[11]);
        sounds.Add("powerUpUse", clips[12]);
        sounds.Add("powerUpCollect", clips[13]);
        sounds.Add("davyJonesAppear", clips[14]);
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
		if (gameover == true && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
		{
			restartGame ();
		}

        if (Input.GetKeyDown(KeyCode.R))
        {
            restartGame();
        }
        
	    increaseDifficulty();

        //temp code for reseting max distance and max score
	    if (Input.GetKeyDown(KeyCode.P))
	    {
	        PlayerPrefs.DeleteKey("bestDistance");
	        PlayerPrefs.DeleteKey("bestScore");
	        PlayerPrefs.DeleteKey("prevBestDistance");
        }

        //updates prev best distance. Will get reset in nest game
	    if (myPlayerScript.maxYvalue > PlayerPrefs.GetFloat("prevBestDistance"))
	    {
	        PlayerPrefs.SetFloat("prevBestDistance", myPlayerScript.maxYvalue);
	        //Debug.Log("Prev best distance is currently: " + previousBestDist);
	    }
    }


    public void increaseDifficulty()
    {
        //difficulty += Time.deltaTime * difficultyModifier;

    }

    public void gameOver()
    {
        //Debug.Log("Game over state is true");
            gameover = true;
            gameoverText.SetActive(true);
            playSound("game_over");



            //slow game down upon player death
            Time.timeScale = .0f;
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
        try
        {
            AudioSource a = sounds[key];
            if (a != null)
            {
                //Debug.Log("Playing "+key);
                a.Play();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Sound not found. The error is: "+e.GetBaseException());
        }
        //Debug.Log("Trying to play " + key);

        //else Debug.Log(key + " not found!");
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
