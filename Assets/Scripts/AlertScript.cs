using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertScript : MonoBehaviour {
    [SerializeField] private float secondsToAppear;
    [SerializeField] private float speedMovingUp;
    public string message="...";
    public Color c=Color.red;
    private GameObject mainCamera;
    private TextMesh messageText;
    private float currentTime;

	// Use this for initialization
	void Start () {
        messageText = this.GetComponent<TextMesh>();
        currentTime = 0f;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        this.transform.position = mainCamera.transform.position-new Vector3(200f, 0f, -1f);
	}
	
	// Update is called once per frame
	void Update () {
        messageText.text = message;
        messageText.color = c;
        currentTime += Time.deltaTime;
        this.transform.Translate(speedMovingUp*Time.deltaTime, 0f, 0f);
        if (currentTime>secondsToAppear)
        {
            GameObject.Destroy(this.gameObject);
        }
	}

}
