using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingScript : MonoBehaviour
{

    [SerializeField] private DistanceJoint2D joint;
    private Vector3 targetPos;
    private RaycastHit2D hit;
    [SerializeField] private float distance;
    [SerializeField] private LayerMask myMask;
    [SerializeField] private LineRenderer myLine;
	[SerializeField] private GameObject lassoPrefab;
	private GameObject postAttached; //Used in drawing the line. We can probably find a better method.
	private bool canLasso;

    // Use this for initialization
    void Start ()
	{
	    joint = GetComponent<DistanceJoint2D>();
	    joint.enabled = false;
	    myLine.enabled = false;
		canLasso = true;
	}
	
	// Update is called once per frame
	void Update () {
		shootHook();
	}

    private void shootHook()
    {
        //this might be bad for performance

        if (canLasso && Input.GetMouseButtonDown(0))
		{
			
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0;

			GameObject lasso = GameObject.Instantiate (lassoPrefab);
			Vector3 directionToHead = targetPos - this.transform.position;
			lasso.GetComponent<LassoScript> ().Initialize (this.transform.position, directionToHead, 50f);
			canLasso = false;
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


		if (myLine.enabled == true && postAttached!=null)
        {
            myLine.SetPosition(0, transform.position);
			myLine.SetPosition(1, postAttached.transform.position);
        }


        if (Input.GetMouseButtonDown(1))
        {
            joint.enabled = false;
			myLine.enabled = false;
			canLasso = true;
        }
    }

	public void connectGrapple(GameObject postHit)
	{
		canLasso = false;
		joint.enabled = true;
		joint.connectedBody = postHit.GetComponent<Rigidbody2D>();
		//joint.anchor = this.transform.position;
		//joint.connectedAnchor = postHit.transform.position;
		//joint.distance = Vector2.Distance (this.transform.position, postHit.transform.position);
		postAttached = postHit;
		myLine.enabled=true;
	}
}
