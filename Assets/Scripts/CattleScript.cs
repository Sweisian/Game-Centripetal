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

        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>() != null)
            myGameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        direction = Vector3.Normalize(new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.8f, 0.8f), 0f));

	    float angle = Random.value * 360f;
	    transform.rotation = Quaternion.Euler(0f,0f,angle);
    }
	
	// Update is called once per frame
	void Update () {
        int multiplier = 1;
        if (lassoed) multiplier = 4;


        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        //Debug.Log("transform.up is:" + transform.up);
	    //Debug.Log("Vector3.up is:" + Vector3.up);

        //BIG PROBLEMS WHEN USING TRANSFORM.UP VS VECTOR3.UP
        //I DON'T UNDERSTAND THIS AT ALL. IT SEEMS LIKE IT SHOULD BE DOING THE OPPOSITE OF WHATS HAPPENING
        //PLS HELP ME LARRY
        //always goes foward
        transform.Translate(Vector3.up*multiplier*speed*Time.deltaTime);


        //Solution adapted from https://answers.unity.com/questions/757118/rotate-towards-velocity-2d.html

        //var WorldPos = Camera.main.ScreenToWorldPoint(gameObject.transform.position);

        //var dir = WorldPos - transform.position;
        //var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //Vector2 moveDirection = gameObject.GetComponent<Rigidbody2D>().velocity;

        //if (moveDirection != Vector2.zero)
        //{
        //    float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        //    transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
        //}


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
        //Debug.Log("Cattle Crash Called: Collided with "+collision.gameObject.name);
        speed = -1*speed;
        gameObject.GetComponent<SpriteRenderer>().flipY = !gameObject.GetComponent<SpriteRenderer>().flipY;

        if (gameObject.tag == "Dingy" && collision.gameObject.tag == "Player")
        {
            gameObject.GetComponent<Destroyable>().DestroySelf();
        }
    }
}
