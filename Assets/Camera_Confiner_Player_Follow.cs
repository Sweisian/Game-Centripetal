using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Confiner_Player_Follow : MonoBehaviour
{

    private GameObject myPlayer;

    // Use this for initialization
    void Start()
    {
        myPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(transform.position.x, myPlayer.transform.position.y, transform.position.z);
    }
}
