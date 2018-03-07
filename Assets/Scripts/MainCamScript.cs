using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamScript : MonoBehaviour
{

    [SerializeField] Transform target;
    //The following code has been adapted from https://gist.github.com/ftvs/5822103

    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;
    private float currentShakeTimeLeft;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    //stores the max y coord that has been achieved. Used to player can't move the camera down
    public float maxYcoord;

    //y offset from the bottom of the screen (used when going downward)
    public float yMaxDistFromCenter;

    //player gameobject
    private GameObject thePlayer;


    Vector3 originalPos;

    void Awake()
    {
        thePlayer = GameObject.FindGameObjectWithTag("Player");

        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
        currentShakeTimeLeft = shakeDuration;
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void LateUpdate()
    {
        if (target != null)
        {
        //updates the max y coord of the camera so far
        maxYcoord = Mathf.Max(target.position.y, maxYcoord);

        //gets player's distance from the center of the screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector3 camToPlayerVec = Camera.main.WorldToScreenPoint(thePlayer.transform.position) - screenPos;
        

            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

            if (currentShakeTimeLeft > 0)
            {
                Vector3 shakyShake = Random.insideUnitSphere;
                shakyShake = new Vector3(shakyShake.x, shakyShake.y, 0f);

                //temp changed y coord to be player coords instead of maxYcoord
                transform.position = new Vector3(target.position.x, target.position.y, transform.position.z) + shakyShake * shakeAmount;
                currentShakeTimeLeft -= Time.deltaTime * decreaseFactor;
            }
            
            //Should bound the camera to only move up, and then only move down if the player is too far from the center of the screen
            //if (Mathf.Abs(camToPlayerVec.y) < yMaxDistFromCenter)
            //{
            //    transform.position = new Vector3(target.position.x, maxYcoord, transform.position.z);
            //}
            //else if (Mathf.Abs(camToPlayerVec.y) >= yMaxDistFromCenter)
            //{
            //    //Vector3 offset = transform.position - thePlayer.transform.position;
            //    //offset.z = 0;
            //    Debug.Log("Player is further than bound");
            //    //Camera.main.ScreenToWorldPoint(new Vector3(0, yMaxDistFromCenter, 0));
            //    transform.position = (new Vector3(target.position.x, target.position.y + yMaxDistFromCenter, transform.position.z)) + Camera.main.ScreenToWorldPoint(new Vector3(0, yMaxDistFromCenter, 0));
            //}

            //Debug.Log("Camera Position: " + transform.position);
            //Debug.Log("Player Position: " + thePlayer.transform.position);
        }
    }

    public void shake()
    {
        currentShakeTimeLeft += shakeDuration;
    }
}


