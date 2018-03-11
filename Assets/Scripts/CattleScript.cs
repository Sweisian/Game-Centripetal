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
        //Solution adapted from https://answers.unity.com/questions/757118/rotate-towards-velocity-2d.html
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

    public void run(){
        lassoed = true;
    }

    public void calmDown()
    {
        lassoed = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Cattle Crash Called: Collided with "+collision.gameObject.name);
        speed = -1*speed;
    }
}
