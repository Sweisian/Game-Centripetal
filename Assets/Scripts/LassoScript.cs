using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoScript : MonoBehaviour {
	private Vector3 direction; //The direction the lasso was thrown
	private float distanceToMove; //The distance the lasso can move before returning
	private Vector3 startLocation; //Where the lasso was thrown from
	private bool flying = true; //Whether the lasso is still seeking a post
	[SerializeField] private float flySpeed; //How fast the lasso flies midair
	[SerializeField] private float returnSpeed; //How fast the lasso returns to the player


	// Use this for initialization
	void Start () {
		
	}

	//Called by PlayerScript to set the initial values on this instance
	public void Initialize(Vector3 startLoc, Vector3 dir, float distToMove)
	{
		startLocation = startLoc;
		direction = dir;
		distanceToMove = distToMove;
	}

	// Update is called once per frame
	void Update () {
		if (flying)
		{
			
		} 
		else //Returning to player
		{ 
		}
	}
}
