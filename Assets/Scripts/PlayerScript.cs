﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {
	[SerializeField] private float minSpeed;
    [SerializeField] private float appliedForce;
    [SerializeField] private float forcePerFrame;
    [SerializeField] public float currentSpeed;
    [SerializeField] public float maxSpeed = 35f;
    [SerializeField] public float speedIncreasePerSecond;
    [SerializeField] public float timePerInvulnerability = 7f;
    [SerializeField] public float slowDownFraction;
    [SerializeField] public float davyJonesRespawnTime;
    [SerializeField] public float closeCallDistance;

    [SerializeField] public GameObject chaserPrefab;

    private GameController gc;
    private MainCamScript m;
    private GrapplingScript grappleScript;
    private ScoringScript s;
    private Vector2 direction;
    private bool invulnerable=false;
    private bool invulnerableStarter = false; //A hack to have the co-routine run correctly
    private float currentInvulnTimeLeft = 0f;

    private GameObject chaser;
    private bool waitingOnACloseCall = false; //Another hack for close call detection

    //stores max y value achieved by the player this run so far
    public float maxYvalue;

    private Rigidbody2D rb;
    //private Camera playerCamera = Camera.main;

    //Power up UI stuff. This should go in game controller but w/e
    [SerializeField] private Image[] powerUpImages ;
    [SerializeField] private int maxCharges = 5;
    [SerializeField] private int numCharges = 2;
    //private Image invulnBar;
    //private Text invulnText;

    // Use this for initialization
    void Start () {

	    rb = gameObject.GetComponent<Rigidbody2D>();
       // Debug.Log("On start, rb is :" + rb);
        rb.AddForce(transform.up * appliedForce);

		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
        m = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamScript>();
        s = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoringScript>();

        //invulnBar = GameObject.FindGameObjectWithTag("InvulnBar").GetComponent<Image>();
        //invulnText = GameObject.FindGameObjectWithTag("InvulnText").GetComponent<Text>();

        grappleScript = this.GetComponent<GrapplingScript>();
        chaser = GameObject.FindGameObjectWithTag("Chaser");
        StartCoroutine(speedUp());

        //handles initialization of power up bar
        //Has to be down here cause its throwing null refernce errors
        for (int i = 0; i < maxCharges; i++)
        {
            powerUpImages[i].enabled = false;
        }
        for (int j = 0; j < numCharges; j++)
        {
            powerUpImages[j].enabled = true;
        }

    }
	
	// Update is called once per frame
	void Update () {
        DrawShotLine();
        applyMoarForce();
		//Solution adapted from https://answers.unity.com/questions/757118/rotate-towards-velocity-2d.html

		Vector2 dir = rb.velocity;
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg-90f;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //invulnBar.fillAmount = (currentInvulnTimeLeft / timePerInvulnerability);

        //updates max y value of player
	    updateMaxYValue();
        StartCoroutine(invulnerabilityHandler());

        //alters the  invulnText
        //invulnText.text = ": " + numCharges;


        //This all should be its own function
        if (!invulnerable && canPowerUp() && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)))
        {
            Debug.Log("Tried to use a powerup");
            subInvulnCharge();
            currentInvulnTimeLeft = timePerInvulnerability;
            gc.playSound("powerUpUse");
            triggerInvulnerability();
        }
        if (currentInvulnTimeLeft > 0f)
        {
            currentInvulnTimeLeft -= Time.deltaTime;
            if (currentInvulnTimeLeft<=0f)
            {
                currentInvulnTimeLeft = 0f;
                invulnerable = false;

            }
        }
        //Code for close calls
        if (!waitingOnACloseCall && chaser != null && Vector3.Distance(this.transform.position, chaser.transform.position) < closeCallDistance)
        {
            waitingOnACloseCall = true;
        }
        if (chaser == null && waitingOnACloseCall)
            waitingOnACloseCall = false;
        if(waitingOnACloseCall && chaser!=null && Vector3.Distance(this.transform.position, chaser.transform.position) > closeCallDistance)
        {
            s.addPoints(10, "(+10 Close Call!)");
            gc.sendAlert("Close Call! +10", Color.blue);
            gc.playSound("mediumPoints");
            waitingOnACloseCall = false;
        }

	}

    IEnumerator invulnerabilityHandler()
    {
            if (invulnerable && invulnerableStarter)
            {
                invulnerableStarter = false;
                SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
                Color c = sprite.color;
                currentSpeed += 8;
                maxSpeed += 8;
                while (invulnerable)
                {
                    sprite.color = Color.blue;
                    yield return new WaitForSeconds(0.25f);
                    sprite.color = Color.yellow;
                    yield return new WaitForSeconds(0.25f);
                }
                sprite.color = c;
                currentSpeed -= 8;
                maxSpeed -= 8;
                gc.playSound("powerUpEnd");
            }
    }

	/// <summary>
	/// Applies force to the player when necessary.
	/// </summary>
    void applyMoarForce()
    {
        //Debug.Log("rb is: " + rb);

        if (currentSpeed>maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        else  if (rb.velocity.magnitude < minSpeed)
        {
            rb.velocity.Normalize();
            rb.velocity *= minSpeed;
        }
        else if (rb.velocity.magnitude < currentSpeed)
        {
            rb.AddForce(rb.velocity * forcePerFrame);
        }
        else if (rb.velocity.magnitude > currentSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, currentSpeed);
        }
    }

    IEnumerator speedUp()
    {
        while (true)
        {
            currentSpeed += speedIncreasePerSecond;
            yield return new WaitForSeconds(1f);
        }
    }

    void DrawShotLine() //mostly for debugging purposes
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 playerToMouse = mousePosition - transform.position;

        // Draw a debug line (transform.position = character position)
        Debug.DrawRay(transform.position, playerToMouse , Color.red, 0f);
    }

	void OnCollisionEnter2D(Collision2D c)
    {
        if (invulnerable)
        {
            if (c.gameObject.tag != "Wall")
            {
                if (grappleScript.isLassoConnected() && (c.gameObject.tag == "Post" || c.gameObject.tag == "Cow")) 
                {grappleScript.disconnectLasso(true);}
                else
                {
                    Vector3 vel = rb.velocity;

                    var theirDestroyable = c.collider.GetComponent<Destroyable>();
                    if (theirDestroyable != null)
                        theirDestroyable.DestroySelf();
                    else
                    {
                        Destroy(c.gameObject);
                    }
                    rb.velocity = vel;
                }
            }
        }

        else
        {
            if (c.gameObject.tag == "Chaser")
            {
                if (grappleScript.isLassoConnected())
                {
                    grappleScript.disconnectLasso(false);
                }
                gc.BroadcastMessage("gameOver");
                Destroy(gameObject);
            }

            if (c.gameObject.tag == "Post" || c.gameObject.tag == "Cattle" || c.gameObject.tag == "Wall")
            {
                m.shake();
                gc.playSound("hit");
                if (grappleScript.isLassoConnected())
                {
                    grappleScript.disconnectLasso(true);
                }
            }

            if (c.gameObject.tag == "Dingy")
            {
                gc.sendAlert("Slowed Down!", Color.red);
                currentSpeed = this.currentSpeed * slowDownFraction;
            }

            //Conditions where it is a gameover other than chaser
            if (c.gameObject.tag == "Shipwreck" || c.gameObject.tag == "Projectile" )
            {
                Debug.Log("Doing correct things");
                gc.BroadcastMessage("gameOver");
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag != "Camera Confiner")
        {
            if (c.gameObject.tag == "Coin")
            {
                s.addPoints(10, "(+10 Coin Collected)");
                gc.sendAlert("Treasure Stolen! +10", Color.yellow);
                gc.playSound("coinCollect");
                GameObject.Destroy(c.gameObject);
            }
            else if (c.gameObject.tag == "PowerUp")
            {
                addInvulnCharge();
                gc.playSound("powerUpCollect");
                GameObject.Destroy(c.gameObject);
            }
            
            else if (invulnerable && c.gameObject.tag == "Chaser")
            {
                Destroy(c.gameObject);
                gc.sendAlert("Davy Jones Vanished!", Color.green);
                gc.playSound("powerUpUse");
                StartCoroutine(spawnChaser());
            }
            else if (invulnerable && c.gameObject.tag != "Lasso")
            {
                Destroy(c.gameObject);
            }
        }
    }

    //spawns a new chaser. Called if previous chaser is destroyed
    IEnumerator spawnChaser()
    {
        //Transform myTransform = transform;
        //myTransform.position -= (new Vector3(0f, 50f, 0f));
        yield return new WaitForSeconds(davyJonesRespawnTime);
        Vector3 v = transform.position - (new Vector3(0f, 50f, 0f));
        gc.sendAlert("Davy Jones Reappeared!", Color.grey);
        gc.playSound("davyJonesAppear");
        chaser=Instantiate(chaserPrefab, v, transform.rotation);
        //Missile_V2 chaserScript = chaser.GetComponent<Missile_V2>();
        //Rigidbody2D chaserRB = chaser.GetComponent<Rigidbody2D>();
        //chaserRB.velocity.magnitude = chaserScript.speed;
    }

    private void updateMaxYValue()
    {
        maxYvalue = Mathf.Max(maxYvalue, transform.position.y);
        if (maxYvalue > PlayerPrefs.GetFloat("bestDistance"))
        {
            PlayerPrefs.SetFloat("bestDistance", maxYvalue);
            Debug.Log("the best distance is " + PlayerPrefs.GetFloat("bestDistance"));
        }
    }

    //Previously killed player when they went offscreen
    //void OnBecameInvisible()
    //{
    //    gc.BroadcastMessage("gameOver");
    //}

    public void triggerInvulnerability()
    {
        invulnerable = true;
        invulnerableStarter = true;
    }

    public void addInvulnCharge()
    {
        if (numCharges < maxCharges)
        {
            numCharges++;
            powerUpImages[numCharges - 1].enabled = true; //subtracting one to get array index
        }
    }

    public void subInvulnCharge()
    {
        if (numCharges > 0)
        {
            powerUpImages[numCharges - 1].enabled = false; //subtracting one to get array index
            numCharges--;
        }

    }

    public bool canPowerUp()
    {
        if (numCharges > 0)
            return true;
        else
        {
            return false;
        }
    }
}
