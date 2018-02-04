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

    // Use this for initialization
    void Start ()
	{
	    joint = GetComponent<DistanceJoint2D>();
	    joint.enabled = false;
	    myLine.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		shootHook();
	}

    private void shootHook()
    {
        //this might be bad for performance


        if (Input.GetMouseButtonDown(0))
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0;

            hit = Physics2D.Raycast(transform.position, targetPos - transform.position, distance, myMask);

            if (hit.collider != null)
            {
                joint.enabled = true;
                joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                //joint.connectedAnchor = hit.point - new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y);
                //joint.distance = Vector2.Distance(transform.position, hit.point)

                //makes the line render show
                myLine.enabled = true;
            }
        }

        if (myLine.enabled == true)
        {
            myLine.SetPosition(0, transform.position);
            myLine.SetPosition(1, hit.point);
        }

        if (Input.GetMouseButtonDown(1))
        {
            joint.enabled = false;
            myLine.enabled = false;
        }
    }
}
