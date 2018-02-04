using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
	[SerializeField] private float speed;
    [SerializeField] private float appliedForce;
    [SerializeField] private float forcePerFrame;
    [SerializeField] private float maxSpeed;

    private Vector2 direction;

    private Rigidbody2D rb;
    //private Camera playerCamera = Camera.main;

	// Use this for initialization
	void Start () {
	    rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * appliedForce);
    }
	
	// Update is called once per frame
	void Update () {
        DrawShotLine();
        applyMoarForce();
       
    }

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
}
