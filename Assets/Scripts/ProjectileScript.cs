using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    private Rigidbody2D rb;
    [SerializeField] private float initialForce;

    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * initialForce);
        //I know this is jank to have it as "right" but it works for now
    }

    void OnBecameInvisible()
    {
        //Debug.Log("Destroyed Basic Projectile from invisibility");
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag != "Player" && c.gameObject.tag != "Shooting")
        {
            //Debug.Log("Destroyed Basic Projectile from collision with: " + c.gameObject.name);
            Destroy(gameObject);
        }

        if (c.gameObject.tag == "Player")
        {
            Destroy(c.gameObject);
        }
    }
}
