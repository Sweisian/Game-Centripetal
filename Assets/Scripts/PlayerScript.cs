using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
	[SerializeField] private float speed;
    [SerializeField] private float appliedForce;
    [SerializeField] private float forcePerFrame;
    [SerializeField] private float maxSpeed;
	private GameController gc;

    private Vector2 direction;

    private Rigidbody2D rb;
    //private Camera playerCamera = Camera.main;

	// Use this for initialization
	void Start () {
	    rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * appliedForce);
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
    }
	
	// Update is called once per frame
	void Update () {
        DrawShotLine();
        applyMoarForce();
		//Solution adapted from https://answers.unity.com/questions/757118/rotate-towards-velocity-2d.html
		Vector2 dir = rb.velocity;
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg-90f;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

	/// <summary>
	/// Applies force to the player when necessary.
	/// </summary>
    void applyMoarForce()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(rb.velocity * forcePerFrame);
        }
    }

    void DrawShotLine() //mostly for debugging purposes
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 playerToMouse = mousePosition - transform.position;

        // Draw a debug line (transform.position = character position)
        Debug.DrawRay(transform.position, playerToMouse , Color.red, 0f);
    }

	void OnCollisionEnter2D(Collision2D c)
	{
		if (c.gameObject.tag != "Lasso") 
		{
			Debug.Log ("Ouch! You hit " + c.gameObject.name);
			gc.BroadcastMessage ("restartGame");
		}
	}
}
