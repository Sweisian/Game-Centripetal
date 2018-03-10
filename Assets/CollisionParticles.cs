using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionParticles : MonoBehaviour {

    public GameObject collisionPrefab;

    void OnCollisionEnter2D(Collision2D collision)
    {
       

        if (collision.collider.tag == "Player")
        {
            Debug.Log("Trying to create a collsion prefab");

            ContactPoint2D contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            Instantiate(collisionPrefab, pos, rot);
        }
    }

}
