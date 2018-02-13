using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CattleScript : MonoBehaviour {
    [SerializeField] private float speed; //How fast the cow runs
    private Vector3 direction;

	// Use this for initialization
	void Start () {
        //Pick a random direction
        direction = Vector3.Normalize(new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.8f, 0.8f), 0f));
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(direction*speed*Time.deltaTime);
	}
}
