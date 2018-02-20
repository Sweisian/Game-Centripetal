using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
	[SerializeField] private float minSpeed;
    [SerializeField] private float appliedForce;
    [SerializeField] private float forcePerFrame;
    [SerializeField] public float maxSpeed;
	private GameController gc;
    private GrapplingScript grappleScript;
    private Vector2 direction;

    private Rigidbody2D rb;
    //private Camera playerCamera = Camera.main;

	// Use this for initialization
	void Start () {
	    rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * appliedForce);
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
        grappleScript = this.GetComponent<GrapplingScript>();
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
        if (rb.velocity.magnitude < minSpeed)
        {
            rb.velocity.Normalize();
            rb.velocity *= minSpeed;
        }
        else if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(rb.velocity * forcePerFrame);
        }
        else if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
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
        if (c.gameObject.tag == "Post")
        {
            if (grappleScript.isLassoConnected())
            {
                Debug.Log("Rope has snapped");
                grappleScript.disconnectLasso(true);
            }
        }

		if (c.gameObject.tag != "Lasso" && c.gameObject.tag != "Post") 
		{
			Debug.Log ("Ouch! You hit " + c.gameObject.name);
			gc.BroadcastMessage ("gameOver");
            Destroy(gameObject);
		}

	    //if (c.gameObject.tag == "DustStorm")
	    //{
	    //    Debug.Log("You were consumed by the dust storm ");
	    //    gc.BroadcastMessage ("restartGame");
	    //}
    }
}
