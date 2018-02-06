using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrapplingScript : MonoBehaviour
{

    [SerializeField] private DistanceJoint2D joint;
    private Vector3 targetPos;
    private RaycastHit2D hit;
	[SerializeField] private float maxDistance; //Maximum distance the lasso can travel
	[SerializeField] private float chargeRate; //How fast the lasso charges
    [SerializeField] private LayerMask myMask;
    [SerializeField] private LineRenderer myLine;
	[SerializeField] private GameObject lassoPrefab;
	[SerializeField] private GameObject arrow;
	[SerializeField] private Text chargeDisplay; //UI Element for Displaying Charge
	private GameObject postAttached; //Used in drawing the line. We can probably find a better method.
	private bool canLasso;
	private bool grappleConnected;
	private float chargePercent; //Current charge

    // Use this for initialization
    void Start ()
	{
	    joint = GetComponent<DistanceJoint2D>();
	    joint.enabled = false;
	    myLine.enabled = false;
		canLasso = true;
		grappleConnected = false;
		chargePercent = 0f;
		arrow.GetComponent<SpriteRenderer> ().color=new Color(1f, 1f, 1f, 0.3f);
	}
	
	// Update is called once per frame
	void Update () {
		shootHook();
		chargeDisplay.text = "Charge: " + Mathf.Floor (chargePercent) + "%";
		//Following solution adapted from 
		//https://answers.unity.com/questions/599271/rotating-a-sprite-to-face-mouse-target.html
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 arrowPos = arrow.transform.position;
		Vector3 direction = mousePos - arrowPos;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		arrow.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
	}

    private void shootHook()
    {
        //this might be bad for performance

		if (canLasso && Input.GetMouseButton(0))
		{
			if (chargePercent <= 100) {
				chargePercent += (chargeRate * Time.deltaTime);
			}
			Debug.Log (chargePercent);
			/*
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
            */
        }
		if (canLasso && Input.GetMouseButtonUp (0)) {
			targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			targetPos.z = 0;

			if (chargePercent < 20)
				chargePercent = 20f;

			GameObject lasso = GameObject.Instantiate (lassoPrefab);
			Vector3 directionToHead = targetPos - this.transform.position;
			lasso.GetComponent<LassoScript> ().Initialize (this.transform.position, directionToHead, (chargePercent/100f)*maxDistance);
			canLasso = false;
			chargePercent = 0f;
		}


		if (myLine.enabled == true && postAttached!=null)
        {
            myLine.SetPosition(0, transform.position);
			myLine.SetPosition(1, postAttached.transform.position);
        }


		if (grappleConnected && Input.GetMouseButtonDown(1))
        {
            joint.enabled = false;
			myLine.enabled = false;
			canLasso = true;
			grappleConnected = false;
        }
    }

	public void connectGrapple(GameObject postHit)
	{
		canLasso = false;
		grappleConnected = true;
		joint.enabled = true;
		joint.connectedBody = postHit.GetComponent<Rigidbody2D>();
		//joint.anchor = this.transform.position;
		//joint.connectedAnchor = postHit.transform.position;
		//joint.distance = Vector2.Distance (this.transform.position, postHit.transform.position);
		postAttached = postHit;
		myLine.enabled=true;
	}

	public void resetGrapple()
	{
		canLasso = true;
		myLine.enabled = false;
	}
		
}
