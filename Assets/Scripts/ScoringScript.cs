using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//maybe should move tracking of best distance to this script instead of inside player script
public class ScoringScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestDistanceText;

    //the scale to alter the distance traveled by into score
    [SerializeField] private float distanceScoreScaler;

    private float distanceScore;
    private float bonusScore;
    private float totalScore;
    private string bonus;

    private GameObject myPlayer;
    [SerializeField] private float scoreIncreaseRate;
    [SerializeField] private float flashSpeed=0.5f;
    [SerializeField] private int timesToFlash=4;

    // Use this for initialization
    void Start () {
        bonus = "";
        scoreText.SetText("SCORE: 0");
        myPlayer = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (myPlayer != null)
        {
            updateScore();
        }
	}

    public void updateScore ()
    {
        //Debug.Log(score);

        //updates score to be the score plus the change in max value of y
        distanceScore = myPlayer.GetComponent<PlayerScript>().maxYvalue / distanceScoreScaler;
        totalScore = bonusScore + distanceScore;
        scoreText.SetText("SCORE: " + (int) totalScore + "    " + bonus);
        if (totalScore > PlayerPrefs.GetFloat("bestScore"))
        {
            PlayerPrefs.SetFloat("bestScore", totalScore);
        }

        //we don't need to update the max score and distance on every frame, but I'm not concerned wit performance so I don't really care
        bestDistanceText.SetText("Best Distance: " + (int)PlayerPrefs.GetFloat("bestDistance"));
        bestScoreText.SetText("Best Score: " + (int)PlayerPrefs.GetFloat("bestScore"));
    }

    public void addPoints(int points, string bonusText)
    {
        bonusScore += points;
        //bonus = bonusText;
        //StartCoroutine(flash(bonusText));
    }

    private IEnumerator flash(string bonusText)
    {
        for (int i = 0; i < timesToFlash; i++)
        {
            bonus = "";
            yield return new WaitForSeconds(flashSpeed);
            bonus = bonusText;
            yield return new WaitForSeconds(flashSpeed);
        }
        bonus = "";
    }
}
