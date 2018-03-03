using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{

    //public GameController gameControllerScript;

    public ProceduralGenManager proceduralGenScript;

    private ProceduralGenManager.Zone thisZone;

    private bool isFirstZone = true;
	// Use this for initialization
	void Start ()
	{
	    proceduralGenScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<ProceduralGenManager>();
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Soething entered zone");
        if (other.gameObject.tag.Equals("Player") == true)
        {
            Collider2D thisCollider = this.GetComponent<BoxCollider2D>();
            thisZone = proceduralGenScript.getZone(thisCollider.gameObject);

            Debug.Log("Zone entered!");
            if (thisZone.beenEntered == false)
            {
                Debug.Log("Entered new zone");
                thisZone.beenEntered = true;

                proceduralGenScript.AddZone(thisZone); //refresh the previous zone
                //proceduralGenScript.AddZone(GameController.currZone); //refresh the previous zone

                //}
                //GameController.currZone = thisZone; //update the current zone to be the one that was just entered
            }
            else
            { 
                Debug.Log("re-entered zone");
                
                if (thisZone != null)
                {
                    
                    //GameController.currZone = thisZone;
                }
            }
            //set curZone to the zone related to this (other) collider
            
        }

    }
}
