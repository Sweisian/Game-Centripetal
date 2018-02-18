﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplingScript : MonoBehaviour
{

    [SerializeField] private DistanceJoint2D joint; //The joint representing the lasso
    [SerializeField] private float maxDistance; //Maximum distance the lasso can travel
    [SerializeField] private float chargeRate; //How fast the lasso charges
    [SerializeField] private LineRenderer myLine;
    [SerializeField] private GameObject lassoPrefab;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Text chargeDisplay; //UI Element for Displaying Charge
    private GameObject postAttached; //Used in drawing the line. We can probably find a better method.
    private bool canLasso; //Whether we can throw a lasso or not
    private bool lassoConnected; //Whether the lasso is connected or not.
    private float chargePercent; //Current charge
    private GameController gc;

    // Use this for initialization
    void Start()
    {
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        myLine.enabled = false;
        canLasso = true;
        lassoConnected = false;
        chargePercent = 0f;
        arrow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f);
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        throwLasso();
        chargeDisplay.text = "Charge: " + Mathf.Floor(chargePercent) + "%";
        //Update the arrow. Following solution adapted from 
        //https://answers.unity.com/questions/599271/rotating-a-sprite-to-face-mouse-target.html
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 arrowPos = arrow.transform.position;
        Vector3 direction = mousePos - arrowPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void FixedUpdate()
    {
        //makes sure the lasso distance can get smaller but not bigger
        if (lassoConnected)
        {
            this.joint.distance = Mathf.Min(joint.distance,
                Vector3.Distance(joint.connectedBody.position, transform.position)
                );
        }
    }

    /// <summary>
    /// Handles lots of things to do with throwing the lasso.
    /// </summary>
    private void throwLasso()
    {
        /*
		if (canLasso && Input.GetMouseButton(0))
		{
			if (chargePercent <= 100) {
				chargePercent += (chargeRate * Time.deltaTime);
			}
			//Debug.Log (chargePercent);

			//Original grapple script- Delete when ready

            hit = Physics2D.Raycast(transform.position, targetPos - transform.position, distance, myMask);

			if (hit.collider != null && hit.collider.gameObject.tag=="Post")
            {
				canLasso = false;
                joint.enabled = true;
                joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                //joint.connectedAnchor = hit.point - new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y);
                //joint.distance = Vector2.Distance(transform.position, hit.point)

                //makes the line render show
                myLine.enabled = true;
            }
        }
        */

        if (canLasso && Input.GetMouseButtonDown(0))
        {
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0;

            if (chargePercent < 20)
                chargePercent = 20f;

            GameObject lasso = GameObject.Instantiate(lassoPrefab);
            gc.playSound("throw");
            Vector3 directionToHead = targetPos - this.transform.position;
            lasso.GetComponent<LassoScript>().Initialize(this.transform.position, Vector3.Normalize(directionToHead), maxDistance);
            canLasso = false;
            chargePercent = 0f;
            arrow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.0f);
        }


        if (myLine.enabled == true && postAttached != null)
        {
            myLine.SetPosition(0, transform.position);
            myLine.SetPosition(1, postAttached.transform.position);
        }


        if (lassoConnected && Input.GetMouseButtonDown(0))
        {
            disconnectLasso(false);
        }
    }

    /// <summary>
    /// Attaches player and post to lasso. Called externally by an instance of LassoScript.
    /// </summary>
    /// <param name="postHit">The post that we wish to attach to.</param>
    public void connectLasso(GameObject postHit)
    {
        canLasso = false;
        lassoConnected = true;
        joint.enabled = true;
        joint.connectedBody = postHit.GetComponent<Rigidbody2D>();
        postAttached = postHit;
        myLine.enabled = true;
    }

    /// <summary>
    /// Resets the lasso throwing process. Called externally by an instance of LassoScript.
    /// </summary>
    public void resetLasso()
    {
        canLasso = true;
        myLine.enabled = false;
        arrow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f);
    }

    public void disconnectLasso(bool didItSnap)
    {
        if (didItSnap) gc.playSound("snap");
        else gc.playSound("detach");
        joint.enabled = false;
        myLine.enabled = false;
        canLasso = true;
        lassoConnected = false;
        arrow.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f);
    }

    public bool isLassoConnected()
    {
        return lassoConnected;
    }
		
}