using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
	
	bool Available = true;
	bool m_targetingPoint = false;
	float m_delay = 0.0f;
	Vector3 m_currentTarget;
	Vector2 m_speed;
	float m_targetSpeed;
	GameObject playerTarget;



    [SerializeField] private float ACCELERATION = 0.2f;
    [SerializeField] private float CHASE_TOLERANCE = 0.2f;
    [SerializeField] private float PURSUE_DISTANCE = 200f;
    [SerializeField] private float accelModifier = 0.2f;

    //Speed penalty for colliding with objects
    [SerializeField] private float collisionPenalty = 0;
    //this extra variable is declared to allow return to original acceleration
    private float tempACCELERATION;

    private GrapplingScript MyGrapplingScript;

    SpriteRenderer m_sprite;
	TrailRenderer trail;

	float timeOut = 0f;
	float targetTime = 0.6f;

	[SerializeField] private float m_spawnSpeed;

	void Start () {

	    MyGrapplingScript = GameObject.FindGameObjectWithTag("Player").GetComponent<GrapplingScript>();
        m_sprite = GetComponent<SpriteRenderer> ();
		m_currentTarget = new Vector2 ();
		m_speed = new Vector2 ();

	    tempACCELERATION = ACCELERATION;

        trail = GetComponent<TrailRenderer> ();
		if (GameObject.FindWithTag("Player")) {
			playerTarget = GameObject.FindWithTag("Player").gameObject;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (playerTarget != null) {
			m_currentTarget = playerTarget.transform.position;
			if (Vector3.Distance (m_currentTarget, transform.position) < PURSUE_DISTANCE) {
				chaseTarget ();
			}
		}
        IncreaseEnemyDifficulty();
	}

	void chaseTarget() {
		if (Vector3.Distance (transform.position, m_currentTarget) > CHASE_TOLERANCE) {
            m_speed.x += tempACCELERATION * Time.deltaTime * Mathf.Sign (m_currentTarget.x - transform.position.x);
			m_speed.y += tempACCELERATION * Time.deltaTime * Mathf.Sign (m_currentTarget.y - transform.position.y);
		} else {
			m_targetingPoint = false;
		}
		timeOut += Time.deltaTime;
		if (timeOut > targetTime) {
			m_targetingPoint = false;
		}
		m_speed *= 0.99f;
		orientToSpeed (m_speed);
		transform.Translate (m_speed,Space.World);
	}

	void orientToSpeed(Vector2 speed) {
        //the plus 90 is to orient the ship in the right direction
		m_sprite.transform.rotation = Quaternion.Euler (new Vector3(0f,0f,Mathf.Rad2Deg * Mathf.Atan2 (speed.y, speed.x) + 90));
	}

    void OnCollisionEnter2D(Collision2D c)
    {
        //need to make this so the player detaches first or something
        if (c.gameObject.tag == "Post" || c.gameObject.tag == "Cattle")
        {
            GrapplingScript g = playerTarget.GetComponent<GrapplingScript>();
            if (g.isLassoConnectedTo(c.gameObject))
                g.disconnectLasso(true);
            Destroy(c.gameObject);
        }

        //Supposed to provide a speed penalty on collisions. Doesn't work
        //m_speed = m_speed.normalized * (m_speed.magnitude - collisionPenalty);

        /*
        if (c.gameObject.tag == "Player")
        {
            Destroy(c.gameObject);
        }
        */
    }

    void OnBecameInvisible()
    {
        tempACCELERATION *= 2;
    }

    void OnBecameVisible()
    {
        tempACCELERATION = ACCELERATION;
    }

    void IncreaseEnemyDifficulty()
    {
        GameController gameControlScript = GetComponent<GameController>();
        PlayerScript ps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        float distance = ps.maxYvalue;
        float minAccel = 0.37f;
        float mediumAccel = 0.4f;
        float maxAccel = 0.5f;
        //float newAccel = distance * accelModifier;
        if (distance < 1000)
            ACCELERATION = minAccel;
        else if (distance < 2000)
            ACCELERATION = mediumAccel;
        else
            ACCELERATION = maxAccel;
        Debug.Log(distance);
    }
}