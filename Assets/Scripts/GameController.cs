using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    /* Unity is stupid and won't serialize dictionaries
     * So we'll just have to add the things manually to here.
    */
    private Dictionary<string, AudioSource> sounds;

	// Use this for initialization
	void Start () {
        sounds = new Dictionary<string, AudioSource>();
        AudioSource[] clips = GetComponents<AudioSource>();
        sounds.Add("attach", clips[0]);
        sounds.Add("detach", clips[1]);
        sounds.Add("throw", clips[2]);
        sounds.Add("snap", clips[3]);
	}
	
	// Update is called once per frame
	void Update () {
		//Reset the game when the player presses R
		if (Input.GetKeyDown (KeyCode.R)) {
			restartGame ();
		}
	}

	/// <summary>
	/// Restarts the game by reloading this scene.
	/// </summary>
	public void restartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene ().buildIndex);
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
		
}
