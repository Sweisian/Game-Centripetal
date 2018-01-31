using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
	[SerializeField] private float speed;
	private Vector2 direction;
    //private Camera playerCamera = Camera.main;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        DrawShotLine();
        //Debug.Log("Nigeria");
    }

    void DrawShotLine()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Draw a debug line (transform.position = character position)
        Debug.DrawRay(transform.position, mousePosition - transform.position, Color.red, 0f);
    }
}
