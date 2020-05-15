using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;




public class UIManager : MonoBehaviour
{
    [HideInInspector]public GameManager manager;


    public Button subMenuButton;

    

    public Text player1Score;
    public Text player2Score;

    public Text player1Name;
    public Text player2Name;


    [Header("STATUS PANEL")]
    public GameObject statusPanel;
    public Text statusText;
    public Text subStatusText;
    public GameObject roundWinnerImage;
    public Button nextRoundButton;

    public GameObject inGameMenuOption1;
    public GameObject confirmationPanel;
    public Text confirmationText;
    public Button acceptButton;
    public Button cancelButton;
    


    

    

    [Header("SCOREBOARD")]
    public GameObject player1Panel;
    public Text player1NameScoreBoard;
    public Text player1Timer;
    public Text player1TotalScore;
    
    public GameObject player2Panel;
    public Text player2NameScoreBoard;
    public Text player2Timer;
    public Text player2TotalScore;

    [Header("WINNER PANEL")]
    public GameObject victoryImage;
    public GameObject winPanel;
    public Text winnerPlayer;
    public Text winnerText;
    //public Text winnerPanelPlayer1Name;
    public Text winnerPanelPlayer1Score;
    //public Text winnerPanelPlayer1Timer;
    //public Text winnerPanelPlayer2Name;
    public Text winnerPanelPlayer2Score;
   // public Text winnerPanelPlayer2Timer;
    //public GameObject winnerImage;
    public GameObject restartConfirmation;

    public Color winColor;

    [Header("CURSOR")]
    public Texture2D pointerOver;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 cursorPos = Vector2.zero;

    private void Start()
    {
        manager = FindObjectOfType<GameManager>();
        InGameMenuSetup();
    }

    public void CursorEnter()
    {
        Cursor.SetCursor(pointerOver, cursorPos, cursorMode);
    }
    public void CursorExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }



    //public void StatusTextDisplay(string text)
    //{
    //    StartCoroutine(StatusTextRoutine(text));
    //}

    //public IEnumerator StatusTextRoutine(string text)
    //{
    //    ResumeGame();
    //    yield return new WaitForSeconds(3);
    //    statusPanel.SetActive(true);

    //    statusText.text = text;
        
    //    yield return new WaitForSeconds(2);
    //    statusPanel.SetActive(false);
    //    //roundWinnerImage.SetActive(false);
    //}
    public void RoundStatusTextDisplay(string text, int intro, int outro)
    {
        StartCoroutine(RoundStatusRoutine(text, intro, outro));
    }
    public IEnumerator RoundStatusRoutine(string text, int intro, int outro)
    {
        ResumeGame();
        yield return new WaitForSeconds(intro);
        statusPanel.SetActive(true);
        statusText.text = text;
        yield return new WaitForSeconds(outro);
        statusPanel.SetActive(false);
    }
    //public void WinnerStatusTextDisplay(string text, int intro, int outro)
    //{
    //    StartCoroutine(WinnerStatusRoutine(text, intro, outro));
    //}
    //public IEnumerator WinnerStatusRoutine(string text, int intro, int outro)
    //{
    //    ResumeGame();
    //    yield return new WaitForSeconds(intro);
    //    statusPanel.SetActive(true);
    //    statusText.text = text;
    //    yield return new WaitForSeconds(outro);
    //    statusPanel.SetActive(false);
    //}

    public void PlayerNames()
    {
        player1Name.text = GameManager.player1Name;
        player2Name.text = GameManager.player2Name;
    }

    public void PlayerScore()
    {
        player1Score.text = GameManager.player1Score.ToString();
        player2Score.text = GameManager.player2Score.ToString();
    }

    public void WinnerAnimations(string text, int intro, int outro)
    {
        StartCoroutine(WinSequence(text, intro, outro));
    }
    public IEnumerator WinSequence(string text, int intro, int outro)
    {
        yield return new WaitForSeconds(intro);
        if(text == "DRAW")
        {
            manager.PlaySound(manager.pieceClip);
            victoryImage.SetActive(false);
            winnerText.text = "IT'S A";
            winnerPlayer.text = "DRAW";

        } else
        {
            manager.PlaySound(manager.winClip);
            bool activation = true;
            victoryImage.SetActive(true);
            winnerText.text = "WINNER";
            winnerPlayer.text = text.ToUpper();
            StartCoroutine(RotationImageAnimation(activation));
        }
        winPanel.SetActive(true);
        winnerPanelPlayer1Score.text = GameManager.player1Score.ToString();
        winnerPanelPlayer2Score.text = GameManager.player2Score.ToString();
        yield return new WaitForSeconds(outro);
        restartConfirmation.SetActive(true);
    }
    public IEnumerator RotationImageAnimation(bool active)
    {
        Vector3 rotation = new Vector3();
        while (active)
        {
            
            rotation.z += 0.1f;
            //winnerImage.transform.localRotation = Quaternion.Euler(rotation);
            //winnerImage.rectTransform.localRotation.eulerAngles = rotation;
            yield return null;
        }
    }

    public void NextRoundConfirmation()
    {
        GameManager.currentRound += 1;
        SceneManager.LoadScene("GameScene");  
    }
    public void InGameMenuSetup()
    {
        if (manager.numberOfRoundsInGame > 1)
        {
            inGameMenuOption1.SetActive(true);

        }
        else
        {
            inGameMenuOption1.SetActive(false);
        }
    }
    public void ConfirmationPanel(Button button)
    {
        string buttonText = button.GetComponentInChildren<Text>().text;
        if(buttonText == "NEW GAME")
        {
            acceptButton.onClick.AddListener(delegate () { NewGame(); });
            GameManager.currentRound = 1;
            GameManager.player1Score = 0;
            GameManager.player2Score = 0;

            confirmationText.text = "DO YOU WISH TO START A NEW GAME?";
        } else if (buttonText == "RESTART SET")
        {
            acceptButton.onClick.AddListener(delegate () { RestartSet(); });
            confirmationText.text = "DO YOU WISH TO RESTART THIS ROUND?";
        }
        else
        {
            confirmationText.text = "DO YOU WISH TO EXIT THE GAME?";
            acceptButton.onClick.AddListener(delegate () { ExitButton(); });
        }
    }
    public void NewGame()
    {
        Debug.Log("New Game");
        ResumeGame();
        manager.ResetScoreRecords();
        if (GameManager.initialPlayerInt == 2)
        {
            GameManager.initialPlayerInt = 1;
        } else
        {
            GameManager.initialPlayerInt = 2;
        }
            

        GameManager.currentRound = 1;
        SceneManager.LoadScene("GameScene");
    }
    public void RestartSet()
    {
        ResumeGame();
        SceneManager.LoadScene("GameScene");
    }
    public void ExitButton()
    {
        ResumeGame();
        manager.ResetScoreRecords();
        SceneManager.LoadScene("MainScene");
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void OpenLink()
    {
        Application.OpenURL("https://smartiguanagames.com");
    }
    //string GetDATAValue(string data, string index)
    //{
    //    string value = data.Substring(data.IndexOf(index) + index.Length);
    //    if (value.Contains("|"))
    //        value = value.Remove(value.IndexOf("|"));
    //    return value;
    //}

}
