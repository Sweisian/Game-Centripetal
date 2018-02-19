﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoringScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreText;
    private int score;
    [SerializeField] private float scoreIncreaseRate;

    // Use this for initialization
    void Start () {
		scoreText.SetText("SCORE: 0");
	}
	
	// Update is called once per frame
	void Update ()
	{
	    updateScore();
	}

    public void updateScore ()
    {
        Debug.Log(score);
        score = score + (int)Time.time;
        scoreText.SetText("SCORE: " + score);
    }
}
