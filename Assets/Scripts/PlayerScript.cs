using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {
	[SerializeField] private float minSpeed;
    [SerializeField] private float appliedForce;
    [SerializeField] private float forcePerFrame;
    [SerializeField] public float currentSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float speedIncreasePerSecond;
    [SerializeField] public float timePerInvulnerability = 5f;
    [SerializeField] public float slowDownFraction;
    [SerializeField] public float davyJonesRespawnTime;

    [SerializeField] public GameObject chaserPrefab;

    private GameController gc;
    private MainCamScript m;
    private GrapplingScript grappleScript;
    private ScoringScript s;
    private Vector2 direction;
    private bool invulnerable=false;
    private bool invulnerableStarter = false; //A hack to have the co-routine run correctly
    private float currentInvulnTimeLeft = 0f;
    private Image invulnBar;
    private Text invulnText;
    private int numCharges;

    //stores max y value achieved by the player this run so far
    public float maxYvalue;

    private Rigidbody2D rb;
    //private Camera playerCamera = Camera.main;

	// Use this for initialization
	void Start () {
        numCharges = 1;
	    rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * appliedForce);
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
        m = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamScript>();
        s = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoringScript>();
        invulnBar = GameObject.FindGameObjectWithTag("InvulnBar").GetComponent<Image>();
        invulnText = GameObject.FindGameObjectWithTag("InvulnText").GetComponent<Text>();
        grappleScript = this.GetComponent<GrapplingScript>();
        StartCoroutine(speedUp());

    }
	
	// Update is called once per frame
	void Update () {
        DrawShotLine();
        applyMoarForce();
		//Solution adapted from https://answers.unity.com/questions/757118/rotate-towards-velocity-2d.html

		Vector2 dir = rb.velocity;
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg-90f;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        invulnBar.fillAmount = (currentInvulnTimeLeft / timePerInvulnerability);

        //updates max y value of player
	    updateMaxYValue();
        StartCoroutine(invulnerabilityHandler());
        invulnText.text = ": " + numCharges;

        if (!invulnerable && numCharges>0 && Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentInvulnTimeLeft = timePerInvulnerability;
            numCharges--;
            triggerInvulnerability();
        }
        if (currentInvulnTimeLeft>0f)
        {
            currentInvulnTimeLeft -= Time.deltaTime;
            if (currentInvulnTimeLeft<=0f)
            {
                currentInvulnTimeLeft = 0f;
                invulnerable = false;

            }
        }

	}

    IEnumerator invulnerabilityHandler()
    {
            if (invulnerable && invulnerableStarter)
            {
                invulnerableStarter = false;
                SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
                Color c = sprite.color;
                while (invulnerable)
                {
                    sprite.color = Color.blue;
                    yield return new WaitForSeconds(0.25f);
                    sprite.color = Color.yellow;
                    yield return new WaitForSeconds(0.25f);
                }
                sprite.color = c;
            }
    }

	/// <summary>
	/// Applies force to the player when necessary.
	/// </summary>
    void applyMoarForce()
    {
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
                    Destroy(c.gameObject);
                    rb.velocity = vel;
                }
            }
        }

        else
        {
            if (c.gameObject.tag == "Chaser")
            {
                gc.BroadcastMessage("gameOver");
                Destroy(gameObject);
            }


            if (c.gameObject.tag == "Post" || c.gameObject.tag == "Cattle" || c.gameObject.tag == "Wall")
            {
                m.shake();
                s.addPoints(-2, "(-2)");
                if (grappleScript.isLassoConnected())
                {
                    grappleScript.disconnectLasso(true);
                }
            }


            if (c.gameObject.tag != "Lasso"
                && c.gameObject.tag != "Post"
                && c.gameObject.tag != "Cattle"
                && c.gameObject.tag != "Wall")
            {
                gc.BroadcastMessage("gameOver");
                Destroy(gameObject);
            }

            //if (c.gameObject.tag == "DustStorm")
            //{
            //    Debug.Log("You were consumed by the dust storm ");
            //    gc.BroadcastMessage ("restartGame");
            //}
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
            else if (c.gameObject.tag == "Star")
            {
                numCharges++;
                GameObject.Destroy(c.gameObject);
            }
            else if (c.gameObject.tag == "Dingy")
            {
                gc.sendAlert("Slowed Down!", Color.red);
                currentSpeed = this.currentSpeed * slowDownFraction;
            }
            else if (invulnerable && c.gameObject.tag == "Chaser")
            {
                Destroy(c.gameObject);
                gc.sendAlert("Davy Jones Vanished!", Color.green);
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
        Instantiate(chaserPrefab, v, transform.rotation);
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
        
    }
}
