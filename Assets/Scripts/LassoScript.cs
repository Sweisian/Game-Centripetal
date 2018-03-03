using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoScript : MonoBehaviour
{
    private Vector3 direction; //The unit vector representing direction the lasso was thrown
    private float distanceToMove; //The distance the lasso can move before returning
    private bool flying = true; //Whether the lasso is still seeking a post
    private GameObject player; //Reference to the player
    private GameController gc;
    private ScoringScript s;
    private AudioManager a;
    [SerializeField] private float flySpeed; //How fast the lasso flies midair
    [SerializeField] private float returnSpeed; //How fast the lasso returns to the player
    [SerializeField] private LineRenderer lassoLine; //The visual representation of the lasso


    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lassoLine = GameObject.FindGameObjectWithTag("Lasso Line").GetComponent<LineRenderer>();
        lassoLine.enabled = true;
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        s = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoringScript>();
        a = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    /// <summary>
    /// Called externally by PlayerScript to set the initial values on this instance.
    /// </summary>
    /// <param name="startLoc">Where the lasso was thrown from.</param>
    /// <param name="dir">Vector representing the direction the lasso was thrown.</param>
    /// <param name="distToMove">How far the lasso can move before returning.</param>
    public void Initialize(Vector3 startLoc, Vector3 dir, float distToMove)
    {
        this.transform.position = startLoc;
        direction = dir;
        distanceToMove = distToMove;
        flying = true;
    }

    void FixedUpdate()
    {
        lassoLine.SetPosition(0, player.transform.position);
        lassoLine.SetPosition(1, this.transform.position);
        if (flying)
        {
            if (Input.GetMouseButtonDown(0)) { flying = false; }
            this.transform.Translate(direction * flySpeed * Time.deltaTime, Space.World);
            if (Vector3.Distance(this.transform.position, player.transform.position) > distanceToMove)
                flying = false;
        }
        else //Returning to player
        {
            float step = returnSpeed;
            this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, step);
        }

        if (lassoLine.enabled == true)
        {
            lassoLine.SetPosition(0, player.transform.position);
            lassoLine.SetPosition(1, this.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Post" || c.gameObject.tag=="Cattle") {
            a.Play("chain_connect");
            if (c.gameObject.GetComponent<CattleScript>())
            {
                c.gameObject.GetComponent<CattleScript>().run();
            }
			player.GetComponent<GrapplingScript> ().connectLasso (c.gameObject);
			lassoLine.enabled = false;
			GameObject.Destroy (this.gameObject);
		}
		else if (!flying && c.gameObject.tag=="Player")
		{
			lassoLine.enabled = false;
			GameObject.Destroy (this.gameObject);
			player.GetComponent<GrapplingScript> ().resetLasso ();
		}
        else if (flying && c.gameObject.tag!="Player")
        {
            Debug.Log("Lasso hit a " + c.name);
            flying = false;
            
        }
	}
}
