using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CattleScript : MonoBehaviour {
    [SerializeField] private float speed; //How fast the cow runs
    private GameController myGameController;
    private Vector3 direction;
    private bool lassoed;

	// Use this for initialization
	void Start () {
        //Pick a random direction
        myGameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        direction = Vector3.Normalize(new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.8f, 0.8f), 0f));
	}
	
	// Update is called once per frame
	void Update () {
        int multiplier = 1;
        if (lassoed) multiplier = 2;
        transform.Translate(direction*multiplier*speed*Time.deltaTime);
	}

    public void run(){
        myGameController.playSound("moo");
        lassoed = true;
    }

    public void calmDown()
    {
        lassoed = false;
    }
}
