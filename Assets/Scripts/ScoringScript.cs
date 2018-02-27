using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoringScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreText;
    private float score;
    private string bonus;
    [SerializeField] private float scoreIncreaseRate;
    [SerializeField] private float flashSpeed=0.5f;
    [SerializeField] private int timesToFlash=4;

    // Use this for initialization
    void Start () {
        bonus = "";
        scoreText.SetText("SCORE: 0");
	}
	
	// Update is called once per frame
	void Update ()
	{
	    updateScore();
	}

    public void updateScore ()
    {
        //Debug.Log(score);
        score = score + Time.deltaTime;
        scoreText.SetText("SCORE: " + (int) score+"    "+bonus);
    }

    public void addPoints(int points, string bonusText)
    {
        score += points;
        bonus = bonusText;
        StartCoroutine(flash(bonusText));
    }

    private IEnumerator flash(string bonusText)
    {
        for (int i = 0; i < timesToFlash; i++)
        {
            bonus = bonusText;
            yield return new WaitForSeconds(flashSpeed);
            bonus = "";
        }
        bonus = "";
    }
}
