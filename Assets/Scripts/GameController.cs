using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
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
		
}
