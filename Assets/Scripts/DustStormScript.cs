using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustStormScript : MonoBehaviour
{

    [SerializeField] private float moveRate;
    [SerializeField] private GameObject thePlayer;
    [SerializeField] private GameController myGameController;
    //private float thePlayerX;

    // Update is called once per frame
    void Update ()
	{
	    transform.position = new Vector3(thePlayer.transform.position.x, transform.position.y, transform.position.z);

        transform.Translate(moveRate * Vector3.up * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            myGameController.restartGame();
        }
    }
}
