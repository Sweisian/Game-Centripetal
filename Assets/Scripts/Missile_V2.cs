using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_V2 : MonoBehaviour
{

    public GameObject player;
    public Transform target;
    private PlayerScript ps;
    private GameController gs;

    private float speed = 33f;
    public float rotateSpeed = 200f;

    private Rigidbody2D rb;

    private float speed1 = 33f;
    private float speed2 = 35f;
    private float speed3 = 38f;
    private float speed4 = 40f;
    //public float maxSpeed = 40f;

    private float time1 = 20;
    private float time2 = 40;
    private float time3 = 60;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        rb = GetComponent<Rigidbody2D>();
        ps = player.GetComponent<PlayerScript>();
        gs = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direction = (Vector2)target.position - rb.position;

            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.up).z;

            rb.angularVelocity = -rotateAmount * rotateSpeed;

            rb.velocity = transform.up * speed;
        }
        increaseSpeed();
    }

    void OnBecameInvisible()
    {
        //speed *= 2;
        //rotateSpeed *= 5;
    }

    void OnBecameVisible()
    {
        //speed /= 2;
        //rotateSpeed /= 5;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        //need to make this so the player detaches first or something
        if (target!=null && (c.gameObject.tag == "Post" || c.gameObject.tag == "Cattle" || c.gameObject.tag=="Shipwreck" || c.gameObject.tag=="Shooting"))
        {
            GrapplingScript g = target.GetComponent<GrapplingScript>();
            if (g.isLassoConnectedTo(c.gameObject))
                g.disconnectLasso(true);  
        }

        if (target != null && (c.gameObject.tag == "Wall"))
        {
            return;
        }

        Destroyable d = c.gameObject.GetComponent<Destroyable>();
        if (d != null)
        {
            //if the destroyable thing exsists, call the destroy self function
            //this will make an exploooosion
            d.DestroySelf();
        }
        else
        {
            Destroy(c.gameObject);
        }
    }

    public void increaseSpeed()
    {
        if (Time.timeSinceLevelLoad < time1)
        {
            speed = speed1;
        }
        else if (Time.timeSinceLevelLoad > time1 && Time.timeSinceLevelLoad < time2 && speed < speed2)
        {
            Debug.Log("Speed 2 INCREASE");
            speed = speed2;
            gs.sendAlert("Davy Jones is faster!", Color.black);
        }
        else if (Time.timeSinceLevelLoad > time2 && Time.timeSinceLevelLoad < time3 && speed < speed3)
        {
            speed = speed3;
            Debug.Log("Speed 3 INCREASE");
            gs.sendAlert("Davy Jones is faster!", Color.black);
        }
        else if (Time.timeSinceLevelLoad > time3 && speed < speed4)
        {
            speed = speed4;
            Debug.Log("Speed 4 INCREASE");
            gs.sendAlert("Davy Jones is faster!", Color.black);
        }
        else
        {
            speed = speed;
        }
    }

}
