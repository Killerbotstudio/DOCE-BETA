using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOnline : MonoBehaviour
{
    [Header("Main")]
    public OnlineGameSetupController controller;

    [Header ("Players Info")]
    public Text playerIDText;
    public Text infoText;
    public GameObject infoPanel;

    [HideInInspector] public PlayerInfo blackPlayer;
    public Text blackScoreText;
    //public int[] blackRoundRecords = new int[4];
    [HideInInspector] public PlayerInfo whitePlayer;
    public Text whiteScoreText;
    //public int[] whiteRoundRecords = new int[4];
    

    [Header ("Rolling Dice") ]
    bool rolling = false;
    public GameObject rollingDiceGO;
    public List<Sprite> decals;
    public Image blackDecals;
    public Image whiteDecals;


    [Header("Win/Lost/Draw")]
    public GameObject victoryImage;
    public Text winText;
    public GameObject winPanel;
    public Text winnerPanelBlackScore;
    public Text winnerPanelWhiteScore;
    

    public Sprite diceSelectionSprite;
    public Color winColor;
    public Color lostColor;

    [Header ("Confirmation Panel")]
    public GameObject confirmationPanel;
    public Text confirmationText;
    public Button acceptButton;

    [Header("CURSOR")]
    public Texture2D pointerOver;
    


    public void CursorEnter()
    {
        Cursor.SetCursor(pointerOver, Vector2.zero, CursorMode.Auto);
    }
    public void CursorExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    public void PlayerScore()
    {
        blackScoreText.text = blackPlayer.score.ToString();
        whiteScoreText.text = whitePlayer.score.ToString();
    }

    public void PlayersNamesTextSetup()
    {
        blackPlayer.playerNameText.text = blackPlayer.playerID;
        whitePlayer.playerNameText.text = whitePlayer.playerID;
    }

    //SCRIPT TO CHECK IF IS A 4 ROUND GAME AND TURN ON INDICATORS
    public void RoundRecordStarter()
    {
        if (controller.rounds > 1)
        {
            blackPlayer.roundsRecordObject.SetActive(true);
            whitePlayer.roundsRecordObject.SetActive(true);
            SetupRoundRecord(blackPlayer);
            SetupRoundRecord(whitePlayer);
        }
        else
        {
            blackPlayer.roundsRecordObject.SetActive(false);
            whitePlayer.roundsRecordObject.SetActive(false);
        }
    }
    //TO ACTIVATE THE ROUND RECORD ITEMS
    public void SetupRoundRecord(PlayerInfo player)
    {
        for (int i = 0; i < player.roundRecord.Length; i++)
        {
            if (player.roundRecord[i] == 1)
            {
                player.roundsRecordObject.transform.GetChild(i).GetComponent<Image>().color = Color.white;

            }
        }
    }
    public void DiceRecord(PlayerInfo player)
    {
        player.diceCounter.text = player.dice.ToString();
    }

    /// <summary>
    /// Calculates the score of the player
    /// </summary>
    /// <param name="player">The player to calculate the score from</param>
    public void WinningScoreCalculation(PlayerInfo player)
    {
        int scoreRule1 = 0;
        int scoreRule2 = 0;
        int scoreRule3 = 0;
        int scoreRule4 = 0;
        int scoreRule5 = 0;
        if (player.winner)
            scoreRule1 += 12;
        if (controller.manager.winningCellsList[3].player == player)
            scoreRule2 += 2;
        if (!player.usedBlocker)
            scoreRule3 += 5;
        foreach (Cell cell in controller.manager.freePositions)
        {
            scoreRule4 += 1;
        }
        int one = 0;
        int two = 0;
        int three = 0;
        int four = 0;
        int five = 0;
        int six = 0;
        foreach (Cell cell in player.listOfPlayedPositions)
        {
            if (cell.value == 1)
                one += 1;
            if (cell.value == 2)
                two += 1;
            if (cell.value == 3)
                three += 1;
            if (cell.value == 4)
                four += 1;
            if (cell.value == 5)
                five += 1;
            if (cell.value == 6)
                six += 1;
        }
        if (one / 3 > 0)
            scoreRule5 += (-2 * (one / 3));
        if (two / 3 > 0)
            scoreRule5 += (-2 * (two / 3));
        if (three / 3 > 0)
            scoreRule5 += (-2 * (three / 3));
        if (four / 3 > 0)
            scoreRule5 += (-2 * (four / 3));
        if (five / 3 > 0)
            scoreRule5 += (-2 * (five / 3));
        if (six / 3 > 0)
            scoreRule5 += (-2 * (six / 3));
        int totalRoundScore = scoreRule1 + scoreRule2 + scoreRule3 + scoreRule4 + scoreRule5;


        player.scoreManager.FillScoreTexts(scoreRule1, scoreRule2, scoreRule3, scoreRule4, scoreRule5);
        player.score += totalRoundScore;
    }
    /// <summary>
    /// The score calculation when the game is tied
    /// </summary>
    /// <param name="player">The player's score in question</param>
    public void TieScoreCalculation(PlayerInfo player)
    {
        int rule1 = 2;
        int rule2 = 0;
        if (!player.usedBlocker)
            rule2 = 5;
        player.score = rule1 + rule2;
        player.scoreManager.FillScoreTextsWhenDraw(rule1, rule2);
    }

    /// <summary>
    /// Displays the rolling dice sequence
    /// </summary>
    /// <param name="diceResult"></param>
    public IEnumerator RollingDice(int[] diceResult)
    {
        
        if (!rolling)
        {
            rolling = true;
            Debug.Log("roll dice routine check");

            rollingDiceGO.SetActive(true);
            float timer = 3f;
            float interval = 0.4f;
            while (timer > 0)
            {
                blackDecals.sprite = decals[Random.Range(0, 6)];
                whiteDecals.sprite = decals[Random.Range(0, 6)];
                timer -= interval;
                yield return new WaitForSeconds(interval);
            }
             
            int blk = diceResult[0];
            int wht = diceResult[1];
            blackDecals.sprite = decals[blk];
            whiteDecals.sprite = decals[wht];
            yield return new WaitForSeconds(2.5f);
            
            if (blk > wht)
            {
                infoText.text = "FIRST MOVE: " + blackPlayer.playerID;
                controller.manager.blackPlayer.turn = true;
                if (controller.blackPlayerInfo.local)
                {
                    controller.turnManager.isMyTurn = true;
                    controller.manager.initialPlayer = controller.blackPlayerInfo;
                    //
                }
            }
            else
            {
                infoText.text = "FIRST MOVE: " + whitePlayer.playerID;
                 controller.manager.whitePlayer.turn = true;
                if (controller.whitePlayerInfo.local)
                {
                    controller.turnManager.isMyTurn = true;
                    controller.manager.initialPlayer = controller.whitePlayerInfo;
                }
            }
            infoPanel.SetActive(true);
            yield return new WaitForSeconds(2f);
            rollingDiceGO.SetActive(false);
            infoText.text = "ROUND: " + controller.roundNumber + "/" + controller.rounds;
            yield return new WaitForSeconds(2f);
            infoPanel.SetActive(false);
            controller.RolledFromUINowToStartGame();
        }
    }

    /// <summary>
    /// Shows a message in the 'infoText' component
    /// </summary>
    /// <param name="message">The message to be display</param>
    /// <param name="intro">The waiting time to show the text</param>
    /// <param name="outro">The time to turn off the message</param>
    public IEnumerator ShowMessage(string message, int intro, int outro)
    {

        infoText.text = message;

        yield return new WaitForSeconds(intro);

        infoPanel.SetActive(true);

        yield return new WaitForSeconds(outro);

        infoPanel.SetActive(false);
    }

    /// <summary>
    /// Shows a message for the rounds in the 'infoText' component depending on the player
    /// </summary>
    /// <param name="player">The player who won the round</param>
    /// <param name="intro">The time to wait for showing the message</param>
    /// <param name="outro">The time to wait to turn off the message </param>
    /// <returns></returns>
    public IEnumerator PlayerRoundMessage(PlayerInfo player, int intro, int outro)
    {

        if(player == controller.localPlayerInfo)
        {
            controller.manager.PlaySound(controller.manager.roundWinClip);
            HighlightCells(winColor);
            infoText.text = "YOU WON THIS ROUND!";
        }
        else
        {
            controller.manager.PlaySound(controller.manager.roundLostClip);
            HighlightCells(lostColor);
            infoText.text = "YOU LOST THIS ROUND";
        }

        yield return new WaitForSeconds(intro);
        infoPanel.SetActive(true);
        yield return new WaitForSeconds(outro);
        ResetCells();
        infoPanel.SetActive(false);


    }
    /// <summary>
    /// Highlights cells when winning
    /// </summary>
    private void HighlightCells(Color color)
    {
        foreach (Cell cell in controller.manager.winningCellsList)
        {
            cell.transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, -0.1f);
            SpriteRenderer sprite = cell.transform.GetChild(0).GetComponent<SpriteRenderer>();
            sprite.sprite = diceSelectionSprite;
            sprite.color = color;
            sprite.enabled = true;
            sprite.transform.localScale = Vector3.one * 1.05f;
        }
    }
    /// <summary>
    /// Resets all cells to its original apperance as well as its values
    /// </summary>
    private void ResetCells()
    {
        foreach (Cell cell in controller.manager.usedPositions)
        {
            //RESTORE VALUES
            cell.value = 0;
            cell.blocked = false;
            cell.free = true;
            cell.player = null;

            //RESTORES APPERANCE
            cell.transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, -0.26f);
            SpriteRenderer sprite = cell.transform.GetChild(0).GetComponent<SpriteRenderer>();
            sprite.sprite = null;
            sprite.enabled = false;
            sprite.transform.localScale = Vector3.one * 0.95f;
        }
    }


    private IEnumerator WinningRoundSEQ()
    {
        
        if (blackPlayer.winner && whitePlayer.winner)
        {
            if (controller.manager.numberOfRoundsInGame > 1)
            {
                blackPlayer.roundRecord[controller.manager.currentRoundNumber - 1] = 1;
                whitePlayer.roundRecord[controller.manager.currentRoundNumber - 1] = 1;
            }
            controller.manager.PlaySound(controller.manager.pieceClip);
            StartCoroutine(ShowMessage("TIE!", 2, 2));
            TieScoreCalculation(blackPlayer);
            TieScoreCalculation(whitePlayer);
            HighlightCells(winColor);
            yield return ShowMessage("TIE!", 2, 2);         // WAIT FOR COROUTINE TO END
            ResetCells();
        }
        else if (blackPlayer.winner)
        {
            if (controller.manager.numberOfRoundsInGame > 1)
            {
                blackPlayer.roundRecord[controller.manager.currentRoundNumber - 1] = 1;
            }
            StartCoroutine(PlayerRoundMessage(blackPlayer, 2, 3));
            WinningScoreCalculation(blackPlayer);
            blackScoreText.text = blackPlayer.score.ToString();
            yield return PlayerRoundMessage(blackPlayer, 2, 3);
        }
        else if (whitePlayer.winner)
        {
            if (controller.manager.numberOfRoundsInGame > 1)
            {
                whitePlayer.roundRecord[controller.manager.currentRoundNumber - 1] = 1;
            }
            StartCoroutine(PlayerRoundMessage(whitePlayer, 2, 3));
            WinningScoreCalculation(whitePlayer);
            whiteScoreText.text = whitePlayer.score.ToString();
            yield return PlayerRoundMessage(whitePlayer, 2, 3);
        }
        yield return new WaitForSeconds(1);
    }


    public IEnumerator WinGameSEQ(string text, int intro, int outro)
    {
        yield return new WaitForSeconds(intro);
        if(blackPlayer.score == whitePlayer.score)
        {
            controller.manager.PlaySound(controller.manager.roundWinClip);
            victoryImage.SetActive(false);
            winText.text = "DRAW!";

        }else 
        {
            if (controller.localPlayerInfo.score > controller.remotePlayerInfo.score)
            {
                controller.manager.PlaySound(controller.manager.winClip);
                victoryImage.SetActive(true);
                winText.text = "YOU WIN!";

            }
            else
            {
                controller.manager.PlaySound(controller.manager.lostClip);
                victoryImage.SetActive(false);
                winText.text = "YOU LOST";
            }
        }
        winText.gameObject.SetActive(true);
        winPanel.SetActive(true);
        winnerPanelBlackScore.text = blackPlayer.score.ToString();
        winnerPanelWhiteScore.text = whitePlayer.score.ToString();
        yield return new WaitForSeconds(outro);
        confirmationPanel.SetActive(true);      
        yield return new WaitForSeconds(outro);
    }


    public void ConfirmationPanel(Button button)
    {
        string buttonText = button.GetComponentInChildren<Text>().text;
        if (buttonText == "NEW GAME")
        {
            acceptButton.onClick.AddListener(delegate () { NewGame(); });
            GameManager.currentRound = 1;
            GameManager.player1Score = 0;
            GameManager.player2Score = 0;

            confirmationText.text = "DO YOU WISH TO START A NEW GAME?";
        }
        else if (buttonText == "RESTART SET")
        {
            //acceptButton.onClick.AddListener(delegate () { RestartSet(); });
            confirmationText.text = "DO YOU WISH TO RESTART THIS ROUND?";
        }
        else
        {
            confirmationText.text = "DO YOU WISH TO EXIT THE GAME?";
            acceptButton.onClick.AddListener(delegate () { ExitButton(); });
        }
    }

    
    public void NextRoundConfirmation()
    {
        controller.roundNumber += 1;
        //SceneManager.LoadScene("GameScene");
    }
    public void NewGame()
    {
        winPanel.SetActive(false);
        infoPanel.SetActive(false);
        controller.roundNumber = 1;
        //controller.NewGame();
        blackPlayer.score = 0;
        whitePlayer.score = 0;
        
        
    }

    public void ExitButton()
    {
        Debug.Log("Exit button pressed");
        controller.ExitOnlineGame();
    }

    public void OpenLink()
    {
        Application.OpenURL("https://smartiguanagames.com");
    }
}
