using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_V2 : MonoBehaviour
{

    public GameObject player;
    public Transform target;
    private PlayerScript ps;

    public float speed = 36f;
    public float rotateSpeed = 200f;

    private Rigidbody2D rb;

    public float speed1 = 30f;
    public float speed2 = 32f;
    public float speed3 = 34f;
    public float speed4 = 36f;
    //public float maxSpeed = 40f;

    public float dist1 = 1000;
    public float dist2 = 2000;
    public float dist3 = 3000;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
        rb = GetComponent<Rigidbody2D>();
        ps = player.GetComponent<PlayerScript>();
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
        if (ps.maxYvalue < dist1)
        {
            speed = speed1;
        }
        if (ps.maxYvalue > dist1 && ps.maxYvalue < dist2)
        {
            speed = speed2;
        }
        else if (ps.maxYvalue > dist2 && ps.maxYvalue < dist3)
        {
            speed = speed3;
        }
        else if (ps.maxYvalue > dist3)
        {
            speed = speed4;
        }
    }

}
