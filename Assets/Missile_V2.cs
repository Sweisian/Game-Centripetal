using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_V2 : MonoBehaviour
{

    public Transform target;

    public float speed = 5f;
    public float rotateSpeed = 200f;

    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 direction = (Vector2) target.position - rb.position;

        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;

        rb.velocity = transform.up * speed;
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
        if (c.gameObject.tag == "Post" || c.gameObject.tag == "Cattle")
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

}
