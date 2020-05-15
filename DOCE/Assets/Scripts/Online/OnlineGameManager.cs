using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GENERAL MANAGER FOR THE GAME
public class OnlineGameManager : MonoBehaviour
{


    [Header("General")]
    ///<sumary>The reference to the UI controller</sumary>
    public UIOnline ui;
    public OnlineGameSetupController controller;
    public bool gameStarted;
    public string messageLog;

    /// <summary>If the game already started</summary>
    /// <value><c>true</c> if the game already started; otherwise, <c>false</c></value>
    

    [Header("Board DATA")]
    ///<summary>The reference to the board GameObject</summary>
    public GameObject board;

    public List<Cell> freePositions;

    /// <summary>List of board cells used in the game</summary>
    public List<Cell> usedPositions;

    /// <summary>List to store the winning dice</summary>
    public List<Cell> winningCellsList;

    /// <summary>Highligher of dice</summary>
    public Sprite diceSelectionSprite;

    /// <summary>Stores the value of the player who moved first in the room</summary>
    public static int inititalPlayerNumber; //this should just be updated when the players entered the game scene

    /// <summary>The current round in the game</summary>
   // public int currentRoundNumber;      // this is used if scene is restarted

    /// <summary>The total number of rounds</summary>
    public int numberOfRoundsInGame;    // this is used if scene is restarted

    /// <summary>An array in which to store values of rounds won as [i,(0 or 1)] for black player as [0,i] and white player as [1,i]</summary>
    //public int[,] roundsRecord = new int[2, 4];


    [Header("Audio Clips")]
    public AudioSource audioSource;
    public AudioClip pieceClip;
    public AudioClip clickClip;
    public AudioClip click2Clip;
    public AudioClip errorClip;
    public AudioClip roundWinClip;
    public AudioClip winClip;
    public AudioClip roundLostClip;     //new clip for when the local player looses a round
    public AudioClip lostClip;          //new clip for when the local player looses the game
    public AudioClip rollClip;
    

  
    [Header("Players setup")]           //sets the players as black & white independently if they are locals
    public PlayerInfo blackPlayer;
    public PlayerInfo whitePlayer;

    /// <summary>Gets or sets the player who is currently playing</summary>
    public PlayerInfo initialPlayer;
    

    


    /// <summary>Resets the board and the game, also calls the respective methods in players</summary>
    //public void NewGamex()
    //{
        

    //    ResetBoard();
    //    ResetPlayers();
    //    StartTurn();
        
    //}

    public void NextRound()
    {
        Debug.Log("NEXT ROUND!!!");
        blackPlayer.controller.turnManager.isMyTurn = false;
        whitePlayer.controller.turnManager.isMyTurn = false;
        controller.roundNumber += 1;
        if (controller.roundNumber > 1)
            SwitchPlayersForNextRound();

        ResetBoard();
        ResetPlayers();
        GameStart();
        //controller.localPlayerInfo.scoreManager.ResetPanel();
        //controller.remotePlayerInfo.scoreManager.ResetPanel();
        //ResetsUi();

        //ui.subMenuButton.gameObject.SetActive(true);
        //ui.PlayersNames();
        //ui.PlayesScores();
        //blackPlayer.gameObject.SetActive(true);
        //whitePlayer.gameObject.SetActive(true);
    }


    //Verica si es un juego single o match para llamar a CheckEndOfGame o a EndGame 
    public void EndOfROund()
    {
        //ShowEndGameMsg
        controller.turnManager.isMyTurn = false;
        Debug.Log("ROUND ENDED BECASUE: " + messageLog);
        StartCoroutine(GoForNextRoundRoutine());     
    }

    IEnumerator GoForNextRoundRoutine()
    {
        ui.EndOfRound();
        while (!ui.ready)
        {
            yield return null;
        }
        Debug.Log("UI IS READY!");
        NextRound();


    }

    public void ResetBoard()
    {
        freePositions.Clear();
        foreach (Cell cell in board.GetComponentsInChildren<Cell>())
        {
            cell.CellReset();
            freePositions.Add(cell);
        }
        winningCellsList.Clear();
        usedPositions.Clear();
    }

    public void ResetPlayers()
    {

        blackPlayer.Reset();
        whitePlayer.Reset();
        
        
    }

    /// <summary>Sets players round records </summary>
    public void SetupPlayersRoundMarkers()
    {
        if (numberOfRoundsInGame > 1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (GameManager.roundsRecord[0, i] == 1)
                {
                    blackPlayer.roundRecord[i] = 1;
                    //perhaps run the method PlayerInfo.SetupRoundRecord() here
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (GameManager.roundsRecord[1, i] == 1)
                {
                    whitePlayer.roundRecord[i] = 1;
                    //perhaps run the method PlayerInfo.SetupRoundRecord() here
                }
            }
        }
    }



    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }


    /// <summary>
    /// Alternates the players in between rounds
    /// </summary>
    public void SwitchPlayersForNextRound()
    {
        Debug.Log("Switch Players!");
        if(initialPlayer == blackPlayer)
        {
            if (whitePlayer.local)
                controller.turnManager.isMyTurn = true;
            //whitePlayer.controller.turnManager.isMyTurn = true;
            initialPlayer = whitePlayer;
            //blackPlayer.controller.turnManager
        } else
        {
            if (blackPlayer.local)
                controller.turnManager.isMyTurn = true;
            //blackPlayer.controller.turnManager.isMyTurn = true;
            initialPlayer = blackPlayer;
        }
    }

    public void GameStart()
    {
        Debug.Log("GameStarted Manager");
        messageLog = "GAME STARTED";
        ResetPlayers();
        ResetBoard();
        controller.turnManager.enabled = true;
        controller.turnManager.OnGameStart();

    }

    /// <summary>
    /// Method to be called at the begining of the turn
    /// </summary>
    public void StartTurn()
    {
        //Debug.Log("START TURN!!!");
        messageLog = "NOW IS YOUR TURN";
        controller.localPlayerInfo.PiecesSwitch(true);

        controller.turnManager.isMyTurn = true;
        //controller.turnManager.enabled = true;
        //controller.turnManager.OnGameStart();
        //controller.remotePlayerInfo.turnIndicator.SetActive(false);
    }
    /// <summary>
    /// Method to be called at the end of player's turn
    /// </summary>
    public void EndTurn()
    {
        messageLog = "YOUR TURN HAS FINISHED";
        controller.localPlayerInfo.PiecesSwitch(false);
        

        controller.turnManager.isMyTurn = false;

        //controller.remotePlayerInfo.turnIndicator.SetActive(true);
    }

    #region Game Logic
    /// <summary>
    /// Checks if there are any winner in the game
    /// </summary>
    /// <value><c>True</c> if any player is a Winner, or if it's a Draw; otherwise, <c>False</c></value>
    public bool CheckIfAnyWinner()
    {
        if (usedPositions.Count > 6)
        {
            CheckAllWinners(controller.localPlayerInfo);
            if (controller.localPlayerInfo.winner || controller.remotePlayerInfo.winner)
            {
                if (controller.localPlayerInfo.winner && !controller.remotePlayerInfo.winner)
                {
                    messageLog = "YOU WIN!";
                    return true;
                    
                }
                else if (!controller.localPlayerInfo.winner && controller.remotePlayerInfo.winner)
                {
                    messageLog = "YOU LOST :(";
                    return true;
                }
                else
                {
                    messageLog = "IT'S A DRAW!";
                    return true;
                }
            }
        }
        messageLog = "ROUND CONTINUES";
        return false;
    }


    

    /// <summary>
    /// Check if the player has any available cell where it can play
    /// </summary>
    /// <param name="player">The player to check for cells</param>
    /// <value><c>True</c> If there are any cell where player can play; otherwise, <c>False</c></value>
    public bool CheckIfAvailableCells(PlayerInfo player)
    {
        if(freePositions.Count <= 9)
        {
            if (player.dice == 0)
                return false;
            if (CheckLastAvailableCells(player).Count > 0)
            {
                messageLog = "CELLS AVAILABLE TO PLAY";
                return true;
            }
            else
            {
                Debug.Log("ITS A DRAW, NO REMAINING PIECES TO PLAY");
                messageLog = "ITS A DRAW, NO REMAINING PIECES TO PLAY";
                return false;
            }
        }
        return true;
        
    }
  
 
    /// <summary>
    /// Goes through the list of all free cell in the board and compare if there are any free to be played in
    /// </summary>
    /// <param name="player">The player to check</param>
    /// <returns>A list of free positions where the player can play</returns>
    private List<Cell> CheckLastAvailableCells(PlayerInfo player)
    {
        List<Cell> cells = new List<Cell>();
        foreach (Cell cell in freePositions)
        {
            if (!CheckSurroundingPositions(player.lastPositionPlayed, cell))
            {
                cells.Add(cell);
            }
        }
        return cells;
    }

    /// <summary>
    /// Check for the surrounding positions of the last cell played
    /// </summary>
    /// <param name="lastCell">The last cell played by  player</param>
    /// <param name="newCell">The next cell, input all remaining cells in </param>
    /// <value><c>True</c>if the new cell is surrounding the last cell; otherwise, <c>False</c></value>
    private bool CheckSurroundingPositions(Cell lastCell, Cell newCell)
    {
        if (lastCell.row == newCell.row + 1)
        {
            if (lastCell.collum == newCell.collum - 1 ||
                lastCell.collum == newCell.collum ||
                lastCell.collum == newCell.collum + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (lastCell.row == newCell.row)
        {
            if (lastCell.collum == newCell.collum - 1 ||
                lastCell.collum == newCell.collum + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (lastCell.row == newCell.row - 1)
        {
            if (lastCell.collum == newCell.collum - 1 ||
                lastCell.collum == newCell.collum ||
                lastCell.collum == newCell.collum + 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check if the player has won by using only its cell for the winning row
    /// </summary>
    /// <param name="player"></param>
    public void WinCheckPlayersLastPosition(PlayerInfo player)
    {
        List<Cell> tempPlayerPositionsPlayed = player.listOfPlayedPositions;
        if (!player.winner)
        {
            for (int i = 0; i < player.listOfPlayedPositions.Count; i++)
            {
                if (!player.listOfPlayedPositions[i].blocked)
                {
                    List<Cell> tempWinningList = new List<Cell>();
                    Cell firstCell = player.listOfPlayedPositions[i];
                    tempWinningList.Add(firstCell);
                    for (int j = 0; j < player.listOfPlayedPositions.Count; j++)
                    {
                        if (!tempWinningList.Contains(player.listOfPlayedPositions[j]))
                        {
                            if (CheckSurroundingPositions(firstCell, player.listOfPlayedPositions[j]))
                            {
                                if (!player.listOfPlayedPositions[j].blocked)
                                {
                                    if (player.listOfPlayedPositions[j].value + firstCell.value <= 10)
                                    {
                                        Cell secondCell = player.listOfPlayedPositions[j];
                                        Vector2 orientation = new Vector2(firstCell.row - secondCell.row, firstCell.collum - secondCell.collum);
                                        tempWinningList.Add(secondCell);
                                        for (int k = 0; k < player.listOfPlayedPositions.Count; k++)
                                        {
                                            if (!player.listOfPlayedPositions[k].blocked)
                                            {
                                                if (!tempWinningList.Contains(player.listOfPlayedPositions[k]))
                                                {
                                                    int xOrientation = Mathf.FloorToInt(orientation.x);
                                                    int yOrientation = Mathf.FloorToInt(orientation.y);
                                                    if (player.listOfPlayedPositions[k].row == secondCell.row - xOrientation && player.listOfPlayedPositions[k].collum == secondCell.collum - yOrientation)
                                                    {
                                                        if (firstCell.value + secondCell.value + player.listOfPlayedPositions[k].value <= 11)
                                                        {
                                                            Cell thirdCell = player.listOfPlayedPositions[k];
                                                            tempWinningList.Add(thirdCell);
                                                            for (int l = 0; l < player.listOfPlayedPositions.Count; l++)
                                                            {
                                                                if (!player.listOfPlayedPositions[l].blocked)
                                                                {
                                                                    if (!tempWinningList.Contains(usedPositions[l]))
                                                                    {
                                                                        if (player.listOfPlayedPositions[l].row == thirdCell.row - xOrientation && player.listOfPlayedPositions[l].collum == thirdCell.collum - yOrientation)
                                                                        {
                                                                            if (player.listOfPlayedPositions[l].value + thirdCell.value + secondCell.value + firstCell.value == 12)
                                                                            {
                                                                                Cell fourthCell = player.listOfPlayedPositions[l];
                                                                                tempWinningList.Add(fourthCell);
                                                                                winningCellsList.Add(firstCell);
                                                                                winningCellsList.Add(secondCell);
                                                                                winningCellsList.Add(thirdCell);
                                                                                winningCellsList.Add(fourthCell);
                                                                                player.winner = true;
                                                                                return;

                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Check if the player won by using all cells used as the last cell of its winning row
    /// </summary>
    /// <param name="player"></param>
    public void WinCheckAllPositionForLastCell(PlayerInfo player)
    {
        if (!player.winner)
        {
            for (int i = 0; i < player.listOfPlayedPositions.Count; i++)
            {
                if (!player.listOfPlayedPositions[i].blocked)
                {
                    List<Cell> tempWinningList = new List<Cell>();
                    Cell firstCell = player.listOfPlayedPositions[i];
                    tempWinningList.Add(firstCell);
                    for (int j = 0; j < player.listOfPlayedPositions.Count; j++)
                    {
                        if (!tempWinningList.Contains(player.listOfPlayedPositions[j]))
                        {
                            if (CheckSurroundingPositions(firstCell, player.listOfPlayedPositions[j]))
                            {
                                if (!player.listOfPlayedPositions[j].blocked)
                                {
                                    if (player.listOfPlayedPositions[j].value + firstCell.value <= 10)
                                    {
                                        Cell secondCell = player.listOfPlayedPositions[j];
                                        Vector2 orientation = new Vector2(firstCell.row - secondCell.row, firstCell.collum - secondCell.collum);
                                        tempWinningList.Add(secondCell);
                                        for (int k = 0; k < player.listOfPlayedPositions.Count; k++)
                                        {
                                            if (!player.listOfPlayedPositions[k].blocked)
                                            {
                                                if (!tempWinningList.Contains(player.listOfPlayedPositions[k]))
                                                {
                                                    int xOrientation = Mathf.FloorToInt(orientation.x);
                                                    int yOrientation = Mathf.FloorToInt(orientation.y);
                                                    if (player.listOfPlayedPositions[k].row == secondCell.row - xOrientation && player.listOfPlayedPositions[k].collum == secondCell.collum - yOrientation)
                                                    {
                                                        if (firstCell.value + secondCell.value + player.listOfPlayedPositions[k].value <= 11)
                                                        {
                                                            Cell thirdCell = player.listOfPlayedPositions[k];
                                                            tempWinningList.Add(thirdCell);
                                                            for (int l = 0; l < usedPositions.Count; l++)
                                                            {
                                                                if (!usedPositions[l].blocked)
                                                                {
                                                                    if (!tempWinningList.Contains(usedPositions[l]))
                                                                    {
                                                                        if (usedPositions[l].row == thirdCell.row - xOrientation && usedPositions[l].collum == thirdCell.collum - yOrientation)
                                                                        {
                                                                            if (usedPositions[l].value + thirdCell.value + secondCell.value + firstCell.value == 12)
                                                                            {
                                                                                Cell fourthCell = usedPositions[l];
                                                                                tempWinningList.Add(fourthCell);
                                                                                winningCellsList.Add(firstCell);
                                                                                winningCellsList.Add(secondCell);
                                                                                winningCellsList.Add(thirdCell);
                                                                                winningCellsList.Add(fourthCell);
                                                                                player.winner = true;
                                                                                return;

                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calls to check if any player won 
    /// </summary>
    /// <param name="player">The local player to be check</param>
    private void CheckAllWinners(PlayerInfo player)
    {
        if (player == controller.localPlayerInfo)
        {
            WinCheckPlayersLastPosition(controller.localPlayerInfo);
            if (!controller.localPlayerInfo.winner)
            {
                WinCheckAllPositionForLastCell(controller.localPlayerInfo);
            }
            WinCheckPlayersLastPosition(controller.remotePlayerInfo);
            if (!controller.remotePlayerInfo.winner)
            {
                WinCheckAllPositionForLastCell(controller.remotePlayerInfo);
            }
        }
        else
        {
            WinCheckPlayersLastPosition(controller.remotePlayerInfo);
            if (!controller.remotePlayerInfo.winner)
            {
                WinCheckAllPositionForLastCell(controller.remotePlayerInfo);
            }
            WinCheckPlayersLastPosition(controller.localPlayerInfo);
            if (!controller.localPlayerInfo.winner)
            {
                WinCheckAllPositionForLastCell(controller.localPlayerInfo);
            }
        }
    }


    #endregion Game Logic



    #region Remote Player Moves
    /// <summary>
    /// Translate the message sent by other players
    /// </summary>
    /// <param name="move">The codded message</param>
    public void DecodeMove(object move)
    {
        object[] message = { 0, 0, 0, "s" };

        if (move.GetType().IsArray)
        {
            
            message = (object[])move;
        }


        int val = (int)message[0];
        int row = (int)message[1];
        int col = (int)message[2];
        string cellName = (string)message[3];

        Debug.Log("DECODING... " + val.ToString() + "," + row.ToString() + "," + col.ToString() + "," + cellName);


        Cell doubleCheckCell =null ;

        foreach (Cell cell in board.GetComponentsInChildren<Cell>())
        {

            if (cell.row == row && cell.collum == col)
            {
                doubleCheckCell = cell;
            }

        }

        if(val == 0)
        {
            doubleCheckCell.blocked = true;
        } else
        {
            doubleCheckCell.value = val;
        }
        


        if (val > 0)
        {
            PlaySound(pieceClip);
            doubleCheckCell.decal.sprite = controller.remotePlayerInfo.GetComponentsInChildren<PlayerPiece>()[doubleCheckCell.value - 1].gameObject.GetComponent<SpriteRenderer>().sprite;
            doubleCheckCell.decal.enabled = true;
            controller.remotePlayerInfo.lastPositionPlayed = doubleCheckCell;
            controller.remotePlayerInfo.listOfPlayedPositions.Add(doubleCheckCell);
            controller.remotePlayerInfo.StartCoroutine(controller.remotePlayerInfo.MarkerMovement(doubleCheckCell));
            controller.remotePlayerInfo.dice -= 1;
            controller.remotePlayerInfo.diceCounter.text = controller.remotePlayerInfo.dice.ToString();
        }
        else
        {
            PlaySound(pieceClip);
            doubleCheckCell.decal.sprite = controller.remotePlayerInfo.GetComponentsInChildren<PlayerPiece>()[6].gameObject.GetComponent<SpriteRenderer>().sprite;
            doubleCheckCell.decal.enabled = true;
            controller.remotePlayerInfo.blocker.SetActive(false);
            controller.remotePlayerInfo.listOfPlayedPositions.Add(doubleCheckCell);
        }

        usedPositions.Add(doubleCheckCell);
        freePositions.Remove(doubleCheckCell);
        
    }

    #endregion Remote Player Moves


}





