using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenScript : MonoBehaviour
{

    public Vector2 areaOfExistence;
    private float fieldOfView;
    private Camera cam;

	// Use this for initialization
	void Start ()
	{
	    cam = GameObject.Find("Main Camera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    
	    fieldOfView = cam.fieldOfView;
	    Debug.Log(fieldOfView);
    }
}
