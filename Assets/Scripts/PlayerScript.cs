using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
	[SerializeField] private float minSpeed;
    [SerializeField] private float appliedForce;
    [SerializeField] private float forcePerFrame;
    [SerializeField] public float currentSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float speedIncreasePerSecond;
	private GameController gc;
    private MainCamScript m;
    private GrapplingScript grappleScript;
    private ScoringScript s;
    private Vector2 direction;

    private Rigidbody2D rb;
    //private Camera playerCamera = Camera.main;

	// Use this for initialization
	void Start () {
	    rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * appliedForce);
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
        m = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamScript>();
        s = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoringScript>();
        grappleScript = this.GetComponent<GrapplingScript>();
        StartCoroutine(speedUp());
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
        if (currentSpeed>maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        else  if (rb.velocity.magnitude < minSpeed)
        {
            rb.velocity.Normalize();
            rb.velocity *= minSpeed;
        }
        else if (rb.velocity.magnitude < currentSpeed)
        {
            rb.AddForce(rb.velocity * forcePerFrame);
        }
        else if (rb.velocity.magnitude > currentSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, currentSpeed);
        }
    }

    IEnumerator speedUp()
    {
        while (true)
        {
            currentSpeed += speedIncreasePerSecond;
            yield return new WaitForSeconds(1f);
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
	    if (c.gameObject.tag == "Chaser")
	    {
	        gc.BroadcastMessage("gameOver");
	        Destroy(gameObject);
        }

        if (c.gameObject.tag == "Post" || c.gameObject.tag == "Cattle")
        {
            m.shake();
            s.addPoints(-2, "(-2)");
            if (grappleScript.isLassoConnected())
            {
                grappleScript.disconnectLasso(true);
            }
        }


        if (c.gameObject.tag != "Lasso" 
            && c.gameObject.tag != "Post" 
            && c.gameObject.tag != "Cattle" 
            && c.gameObject.tag != "Wall") 
		{
			gc.BroadcastMessage ("gameOver");
            Destroy(gameObject);
		}

	    //if (c.gameObject.tag == "DustStorm")
	    //{
	    //    Debug.Log("You were consumed by the dust storm ");
	    //    gc.BroadcastMessage ("restartGame");
	    //}
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Coin")
        {
            s.addPoints(10, "(+10 Coin Collected)");
            gc.playSound("collectCoin");
            GameObject.Destroy(c.gameObject);
        }
    }

    void OnBecameInvisible()
    {
        gc.BroadcastMessage("gameOver");
    }
}
