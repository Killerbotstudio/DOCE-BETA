////////////ScoreManager////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public Text scoreRule1; //+12 for wining
    public Text scoreRule2; //+2 if all 4 dice of the same player;
    public Text scoreRule3; //+5 if unused blocker
    public Text scoreRule4; //+1 for every remaining cell
    public Text scoreRule5; //-2 for every 3 dice of the same
    public int roundScore;
    public int totalScore;
    public Text roundTotalScore;
    public GameObject scorePanel;
    private Vector2 pos;
    public void Start()
    {
        pos = scorePanel.transform.localPosition;
    }

    public void FillScoreTexts(int rule1, int rule2, int rule3, int rule4, int rule5)
    {
        scoreRule1.text = "Winning: \t" + rule1.ToString();
        scoreRule2.text = "Four of same: \t" + rule2.ToString();
        scoreRule3.text = "Use of blocker: \t" + rule3.ToString();
        scoreRule4.text = "Empty squares: \t" + rule4.ToString();
        scoreRule5.text = "Every 3 of same: \t" + rule5.ToString();
        roundTotalScore.text = "Total in round: \t\t" + (rule1 + rule2 + rule3 + rule4 + rule5).ToString();
        scorePanel.SetActive(true);
        MovePanel();
    }

    public void ErraseScoreTexts()
    {
        scoreRule1.text = "";
        scoreRule2.text = "";
        scoreRule3.text = "";
        scoreRule4.text = "";
        scoreRule5.text = "";
        roundTotalScore.text = "";
    }

    public void FillScoreTextsWhenDraw(int rule1, int rule2)
    {

        scoreRule1.text = "Draw: \t" + rule1.ToString();
        scoreRule2.text = "Use of blocker: \t" + rule2.ToString();
        scoreRule3.text = "-";
        scoreRule4.text = "-";
        scoreRule5.text = "-";
        roundTotalScore.text = "Total in round: \t\t" + (rule1 + rule2).ToString();
        scorePanel.SetActive(true);
        MovePanel();
    }

    public void MovePanel()
    {
        scorePanel.transform.localPosition = Vector2.zero;
    }
    public void ResetPanel()
    {
        scorePanel.transform.localPosition = pos;
    }

    public void ScoreWhenDraw(PlayerScript player)
    {
        int rule1 = 2;
        int rule2 = 0;
        if (!player.usedBlocker)
            rule2 = 5;
        player.score = rule1 + rule2;
        FillScoreTextsWhenDraw(rule1, rule2);
    }


}
