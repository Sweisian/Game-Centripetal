using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    public GameObject Zone;

    // Use this for initialization
    void Start () {
		
	}

    void OnTriggerEnter2D()
    {
        Debug.Log("Soething entered zone");
    }
}
