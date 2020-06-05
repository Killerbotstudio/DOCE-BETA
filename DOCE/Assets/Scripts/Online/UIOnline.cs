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

    public Text subRoundText;

    [HideInInspector] public PlayerInfo blackPlayer;
    public Text blackScoreText;
    //public int[] blackRoundRecords = new int[4];
    [HideInInspector] public PlayerInfo whitePlayer;
    public Text whiteScoreText;
    //public int[] whiteRoundRecords = new int[4];


    [Header("Start Display")]
    public Text startText;
    public GameObject startPanel;
    public Text mainTimer;
    [Header ("Rolling Dice") ]
    public GameObject rollingDiceGO;
    public List<Sprite> decals;
    public Image blackDecals;
    public Image whiteDecals;
    public bool rolling = false;
    public GameObject blackStartGO;
    public Text blackStarterText;
    public GameObject blackMarker;
    public GameObject whiteStartGO;
    public Text whiteStarterText;
    public GameObject whiteMaker;
    


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
    public Text confirmationTimer;
    public Text confirmationWinner;
    public Button acceptButton;
    public Button quitButton;

    [Header("CURSOR")]
    public Texture2D pointerOver;
    

    public void StarterSetup()
    {
        if(controller.roomType == "match")
        {
            startText.text = "PLAYING A MATCH (4 GAMES)";
        } else
        {
            startText.text = "PLAYING A SINGLE GAME";
        }
        startText.gameObject.SetActive(true);
        blackStarterText.gameObject.SetActive(true);
        whiteStarterText.gameObject.SetActive(true);

        startPanel.SetActive(true);
        RoundRecordStarter();
        PlayersNamesTextSetup();
    }

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
          //  SetupRoundRecord(blackPlayer);
           // SetupRoundRecord(whitePlayer);
        }
        else
        {
            blackPlayer.roundsRecordObject.SetActive(false);
            whitePlayer.roundsRecordObject.SetActive(false);
        }
    }
    [SerializeField] private Sprite drawRoundSprite;
    //TO ACTIVATE THE ROUND RECORD ITEMS
    public void SetupRoundRecord(PlayerInfo player)
    {
        for (int i = 0; i < player.roundRecord.Length; i++)
        {
            if (player.roundRecord[i] == 1)
            {
                player.roundsRecordObject.transform.GetChild(i).GetComponent<Image>().color = Color.white;

            } else if(player.roundRecord[i] == 2)
            {
                //USE THIS WHEN IS A TIE
                player.roundsRecordObject.transform.GetChild(i).GetComponent<Image>().sprite = drawRoundSprite;
                player.roundsRecordObject.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }
        }
    }
    private void RoundRecords(PlayerInfo player, int value)
    {
        if (controller.rounds > 1)
        {
            player.roundRecord[controller.roundNumber - 1] = value;
        }
        SetupRoundRecord(player);
    }
    public void DiceRecord(PlayerInfo player)
    {
        player.diceCounter.text = player.dice.ToString();
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
            controller.manager.PlaySound(controller.manager.rollClip);
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
                // OnlineGameManager.initialPlayer = controller.blackPlayerInfo
                //SETS THE INITIAL PLAYER
                InitialPlayer.GetSetIni = controller.blackPlayerInfo;
                Debug.Log("Player::::::::::" + InitialPlayer.GetSetIni);
                InitialPlayer.PlayerIndex = 1;
                Debug.Log("Index::::::::::" + InitialPlayer.PlayerIndex);
                blackMarker.SetActive(true);
                if (blackPlayer == controller.localPlayerInfo)
                {
                    infoText.text = "YOU MOVE FIRST";
                }
                else
                {
                    infoText.text = "FIRST MOVE: " + blackPlayer.playerID;
                }
                
                controller.manager.blackPlayer.turn = true;
                if (controller.blackPlayerInfo.local)
                {
                    controller.turnManager.isMyTurn = true;
                    //controller.manager.initialPlayer = controller.blackPlayerInfo;
                    //
                }
            }
            else
            {
                //OnlineGameManager.initialPlayer = controller.whitePlayerInfo;
                //SETS THE INITIAL PLAYER
                InitialPlayer.GetSetIni = controller.whitePlayerInfo;
                Debug.Log("Player::::::::::" + InitialPlayer.GetSetIni);
                InitialPlayer.PlayerIndex = 2;
                Debug.Log("Index::::::::::" + InitialPlayer.PlayerIndex);
                if (whitePlayer == controller.localPlayerInfo)
                {
                    infoText.text = "YOU MOVE FIRST";
                }
                else
                {
                    infoText.text = "FIRST MOVE: " + whitePlayer.playerID;
                }

                whiteMaker.SetActive(true);
                //infoText.text = "FIRST MOVE: " + whitePlayer.playerID;
                 controller.manager.whitePlayer.turn = true;
                if (controller.whitePlayerInfo.local)
                {
                    controller.turnManager.isMyTurn = true;
                    //controller.manager.initialPlayer = controller.whitePlayerInfo;
                }
            }
            StartCoroutine(StartGame());
            
        }
    }

    //HANDLES SINGLE MATCH REMATCHES
    public void SingleRematch()
    {
        Debug.Log("SINGLE REMATCH: " + InitialPlayer.GetSetIni);

        PlayerInfo initial = InitialPlayer.GetSetIni;
        Debug.Log("SINGLE REMATCH INDEX: " + InitialPlayer.PlayerIndex);
        int index = InitialPlayer.PlayerIndex;


        if (index == 2)
        {
            // OnlineGameManager.initialPlayer = controller.blackPlayerInfo
            //SETS THE INITIAL PLAYER
            
            blackMarker.SetActive(true);
            if (blackPlayer == controller.localPlayerInfo)
            {
                infoText.text = "YOU MOVE FIRST";
            }
            else
            {
                infoText.text = "FIRST MOVE: " + blackPlayer.playerID;
            }

            controller.manager.blackPlayer.turn = true;
            if (controller.blackPlayerInfo.local)
            {
                controller.turnManager.isMyTurn = true;
                //controller.manager.initialPlayer = controller.blackPlayerInfo;
                //
            }
            InitialPlayer.PlayerIndex = 1;
        }
        else
        {
            //OnlineGameManager.initialPlayer = controller.whitePlayerInfo;
            //SETS THE INITIAL PLAYER
            
            if (whitePlayer == controller.localPlayerInfo)
            {
                infoText.text = "YOU MOVE FIRST";
            }
            else
            {
                infoText.text = "FIRST MOVE: " + whitePlayer.playerID;
            }

            whiteMaker.SetActive(true);
            //infoText.text = "FIRST MOVE: " + whitePlayer.playerID;
            controller.manager.whitePlayer.turn = true;
            if (controller.whitePlayerInfo.local)
            {
                controller.turnManager.isMyTurn = true;
                //controller.manager.initialPlayer = controller.whitePlayerInfo;
            }
            InitialPlayer.PlayerIndex = 2;
        }


        StartCoroutine(StartGame());
        
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
        controller.manager.PlaySound(controller.manager.roundWinClip);
        yield return new WaitForSeconds(outro);

        infoPanel.SetActive(false);
    }

    public IEnumerator NextRound()

    {
        Debug.Log("Next Round Routine");
        //RoundRecordStarter();


        if (controller.rounds == 1 || controller.roundNumber >= 4)
        {
            StartCoroutine(WinGameSEQ(0, 4));
            StopCoroutine(NextRound());
            yield break;
            
        } else {
            infoPanel.SetActive(true);
            infoText.enabled = true;
            int round = controller.roundNumber + 1;
            subRoundText.text = "ROUND: " + round;
            infoText.text = "ROUND: " + round + "/" + controller.rounds;
        }
            
        

        
        
        

        yield return new WaitForSeconds(2f);
        //rollingDiceGO.SetActive(false);
        
        //yield return new WaitForSeconds(2f);
        infoPanel.SetActive(false);
        StartCoroutine(MainTimer(3));
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
        AudioClip clip = null;
        if(player == controller.localPlayerInfo)
        {
            controller.manager.PlaySound(controller.manager.roundWinClip);
            HighlightCells(winColor);
            infoText.text = "YOU WON THIS ROUND!";
            clip = controller.manager.roundWinClip;
        }
        else
        {
            controller.manager.PlaySound(controller.manager.roundLostClip);
            HighlightCells(winColor);
            infoText.text = "YOU LOST THIS ROUND";
            clip = controller.manager.roundLostClip;
        }

        yield return new WaitForSeconds(intro);
        infoPanel.SetActive(true);
        controller.manager.PlaySound(clip);
        yield return new WaitForSeconds(outro);
        ResetCells();
        infoPanel.SetActive(false);


    }
    
    
    public void EndOfRound()
    {
        Debug.Log("EndRound Void");
        StartCoroutine(WinningRoundSEQ());
    }

    private IEnumerator WinningRoundSEQ()
    {
        Debug.Log("WinningRoundSEQ Routine");
        Debug.Log("QQQQQQQQQQQQQ:" + controller.manager.numberOfRoundsInGame + " vs " + controller.rounds);
        
        ready = false;
        if (blackPlayer.winner && whitePlayer.winner)
        {
            RoundRecords(blackPlayer, 2);
            RoundRecords(whitePlayer, 2);
            //if (controller.manager.numberOfRoundsInGame > 1)
            //{

            //    blackPlayer.roundRecord[controller.roundNumber - 1] = 1;
            //    whitePlayer.roundRecord[controller.roundNumber - 1] = 1;
            //}
            controller.manager.PlaySound(controller.manager.pieceClip);
            StartCoroutine(ShowMessage("DRAW", 2, 2));
            TieScoreCalculation(blackPlayer);
            TieScoreCalculation(whitePlayer);
            blackScoreText.text = blackPlayer.score.ToString();
            whiteScoreText.text = whitePlayer.score.ToString();
            HighlightCells(winColor);
            Debug.Log("!!!!!!!!!!!!!!TIE!!!!!!!!!!!");
            yield return ShowMessage("DRAW", 2, 2);         // WAIT FOR COROUTINE TO END
            ResetCells();
        }
        else if (blackPlayer.winner)
        {
            RoundRecords(blackPlayer,1);
            //if (controller.manager.numberOfRoundsInGame > 1)
            //{
            //    blackPlayer.roundRecord[controller.roundNumber - 1] = 1;
            //}
            StartCoroutine(PlayerRoundMessage(blackPlayer, 2, 3));
            WinningScoreCalculation(blackPlayer);
            blackScoreText.text = blackPlayer.score.ToString();
            yield return PlayerRoundMessage(blackPlayer, 2, 3);
        }
        else if (whitePlayer.winner)
        {
            RoundRecords(whitePlayer,1);
            //if (controller.manager.numberOfRoundsInGame > 1)
            //{
            //    whitePlayer.roundRecord[controller.roundNumber - 1] = 1;
            //}
            StartCoroutine(PlayerRoundMessage(whitePlayer, 2, 3));
            WinningScoreCalculation(whitePlayer);
            whiteScoreText.text = whitePlayer.score.ToString();
            yield return PlayerRoundMessage(whitePlayer, 2, 3);
        }else
        {
            //OLD TIE
            StartCoroutine(ShowMessage("DRAW", 2, 2));
            RoundRecords(blackPlayer, 2);
            RoundRecords(whitePlayer, 2);
            TieScoreCalculation(blackPlayer);
            TieScoreCalculation(whitePlayer);
            blackScoreText.text = blackPlayer.score.ToString();
            whiteScoreText.text = whitePlayer.score.ToString();
            yield return ShowMessage("DRAW", 2, 2);         // WAIT FOR COROUTINE TO END
            ResetCells();
        }
        yield return new WaitForSeconds(0);

        StartCoroutine(NextRound());
        
    }


    public IEnumerator WinGameSEQ(int intro, int outro)
    {
        Debug.Log("WinningGame SEQ");
        yield return new WaitForSeconds(intro);
        if(blackPlayer.score == whitePlayer.score)
        {
            controller.manager.PlaySound(controller.manager.roundWinClip);
            victoryImage.SetActive(false);
            winText.text = "DRAW!";
            controller.manager.PlaySound(controller.manager.roundWinClip);

        }else 
        {
            if (controller.localPlayerInfo.score > controller.remotePlayerInfo.score)
            {
                victoryImage.SetActive(true);
                controller.manager.PlaySound(controller.manager.winClip);
                victoryImage.SetActive(true);
                winText.text = "YOU WIN!";
                confirmationWinner.text = "You won this game (" + controller.localPlayerInfo.score.ToString() + " pts)";
                //controller.manager.PlaySound(controller.manager.winClip);
            }
            else
            {
                controller.manager.PlaySound(controller.manager.lostClip);
                victoryImage.SetActive(false);
                winText.text = "BETTER LUCK NEXT TIME";
                confirmationWinner.text = "WINNER: " + controller.remotePlayerInfo.playerID + " (" + controller.remotePlayerInfo.score.ToString() + " pts)";
                //controller.manager.PlaySound(controller.manager.lostClip);
            }
        }
        winText.gameObject.SetActive(true);
        winPanel.SetActive(true);
        winnerPanelBlackScore.text = blackPlayer.score.ToString();
        winnerPanelWhiteScore.text = whitePlayer.score.ToString();
        yield return new WaitForSeconds(outro);
        winText.gameObject.SetActive(false);

        confirmationText.text = "PLAY AGAIN?";
        confirmationTimer.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        confirmationPanel.SetActive(true);
        Debug.Log("End of UI routines");
        controller.OnFinishedGame();
        
    }



    public void WaitForRematch()
    {
        Debug.Log("Waiting");
        
        //confirmationText.text = "WAITING FOR YOUR OPPONENT";
        //acceptButton.gameObject.SetActive(false);
        //StartCoroutine(RematchRoutine());
    }

    public void MainTimerVoid(int time)
    {

        StartCoroutine(MainTimer(time));
    }
    public IEnumerator MainTimer(int time)
    {
        ready = false;
        mainTimer.gameObject.SetActive(true);

        while (time > 0)
        {
            controller.manager.PlaySound(controller.manager.click2Clip);
            mainTimer.text = time.ToString();
            time -= 1;
            yield return new WaitForSeconds(1);
        }
        mainTimer.text = "0";
        yield return new WaitForSeconds(1);

        mainTimer.gameObject.SetActive(false);
        ready = true;
    }
    public bool ready;

    public IEnumerator StartGame()
    {
        Debug.Log("StartGame Routine");
        yield return new WaitForSeconds(2);
        startPanel.SetActive(false);
        blackMarker.SetActive(false);
        whiteMaker.SetActive(false);
        startText.gameObject.SetActive(false);


        infoPanel.SetActive(true);
        //infoText.enabled = true;
        yield return new WaitForSeconds(2f);
        rollingDiceGO.SetActive(false);
        infoText.text = "ROUND: " + controller.roundNumber + "/" + controller.rounds;
        //yield return new WaitForSeconds(2f);
        infoPanel.SetActive(false);
        controller.RolledFromUINowToStartGame();


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

    
    //public void NextRoundConfirmation()
    //{
    //    controller.roundNumber += 1;
    //    //SceneManager.LoadScene("GameScene");
    //}
    public void NewGame()
    {
        Debug.Log("@@@@@UI NEW GAME");
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
        
        player.scoreManager.FillScoreTextsWhenDraw(rule1, rule2);
        player.score += rule1 + rule2;
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


    public void OpenLink()
    {
        Application.OpenURL("https://smartiguanagames.com");
    }
}


