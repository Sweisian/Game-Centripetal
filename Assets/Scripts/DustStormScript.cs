using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustStormScript : MonoBehaviour
{

    [SerializeField] private float moveRate;
    [SerializeField] private GameObject thePlayer;
    [SerializeField] private GameController myGameController;
    [SerializeField] private float maxDistance;
    [SerializeField] private float startDistFromPlayer;
    //private float thePlayerX;

    void Start()
    {
        transform.position = new Vector3(thePlayer.transform.position.x, thePlayer.transform.position.y - startDistFromPlayer, transform.position.z);
    }

    // Update is called once per frame
    void Update ()
	{

	    transform.position = new Vector3(thePlayer.transform.position.x, transform.position.y, transform.position.z);
	    transform.Translate(moveRate * Vector3.up * Time.deltaTime, Space.World);

        if (thePlayer.transform.position.y - transform.position.y > maxDistance)
            transform.position = new Vector3(thePlayer.transform.position.x, thePlayer.transform.position.y - maxDistance, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.gameObject);

        if (other.gameObject.tag == "Player")
        {
            myGameController.restartGame();
        }
    }
}
