using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoScript : MonoBehaviour {
	private Vector3 direction; //The direction the lasso was thrown
	private float distanceToMove; //The distance the lasso can move before returning
	private bool flying = true; //Whether the lasso is still seeking a post
	private GameObject player; //Reference to the player
	[SerializeField] private float flySpeed; //How fast the lasso flies midair
	[SerializeField] private float returnSpeed; //How fast the lasso returns to the player
	[SerializeField] private LineRenderer lassoLine; 


	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		lassoLine = GameObject.FindGameObjectWithTag ("Lasso Line").GetComponent<LineRenderer> ();
		lassoLine.enabled = true;
	}

	//Called by PlayerScript to set the initial values on this instance
	public void Initialize(Vector3 startLoc, Vector3 dir, float distToMove)
	{
		this.transform.position = startLoc;
		direction = dir;
		distanceToMove = distToMove;
	}

	// Update is called once per frame
	void Update () {
		if (flying)
		{
			this.transform.Translate (direction * flySpeed * Time.deltaTime);
			
		} 
		else //Returning to player
		{ 
		}

		if (lassoLine.enabled == true)
		{
			lassoLine.SetPosition(0, player.transform.position);
			lassoLine.SetPosition(1, this.transform.position);
		}
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		Debug.Log ("Lasso collided with " + c.gameObject.name);
		if (c.gameObject.tag == "Post") {
			player.GetComponent<GrapplingScript> ().connectGrapple (c.gameObject);
			lassoLine.enabled = false;
			GameObject.Destroy (this.gameObject);
		}
	}
}
