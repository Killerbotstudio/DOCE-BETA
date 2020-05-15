////////////GameManager////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public Text debugText;
    public List<Cell> cellsOnBoard;
    public List<Cell> usedPositions;
    public List<Cell> freePositions;
    public PlayerScript player1;
    public PlayerScript player2;
    public PlayerScript currentPlayer;
    public Sprite diceSelectionSprite;
    private int cellLayer = 8;
    public BoardScript board;
    public UIManager ui;
    PlayerPiece playerPiece;
    public List<Cell> winingCellsList;
    public static string player1Name;
    public static string player2Name;
    public static int player1Score;
    public static int player2Score;
    public static int initialPlayerInt;
    public static int currentRound;
    public static int rounds;
    public int numberOfRoundsInGame;
    public int currentRoundInGame;
    public static int[,] roundsRecord = new int[2, 4];
    [Header("AUDIO CLIPS")]
    public AudioSource audioSource;
    public AudioClip clickClip;
    public AudioClip click2Clip;
    public AudioClip errorClip;
    public AudioClip pieceClip;
    public AudioClip roundWinClip;
    public AudioClip winClip;

    private void Start()
    {
        board = FindObjectOfType<BoardScript>();
        audioSource = this.GetComponent<AudioSource>();
        SetupGame(rounds);
        numberOfRoundsInGame = rounds;
        currentRoundInGame = currentRound;
        SetupPlayersRoundMarkers();
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void SetupPlayersRoundMarkers()
    {
        if (numberOfRoundsInGame > 1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (GameManager.roundsRecord[0, i] == 1)
                {
                    player1.roundRecord[i] = 1;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (GameManager.roundsRecord[1, i] == 1)
                {
                    player2.roundRecord[i] = 1;
                }
            }
        }
    }
    public void SelectIniPlayer(int current)
    {
        Debug.Log("Select Initial");
       if(current == 1)
        {
            if(initialPlayerInt == 1)
            {
                currentPlayer = player1;

            } else
            {
                currentPlayer = player2;
            }
        } else if (current %2 == 0)
        {
            if(initialPlayerInt == 1)
            {
                currentPlayer = player2;
            } else
            {
                currentPlayer = player1;
            }
        } else
        {
            if (initialPlayerInt == 1)
            {
                currentPlayer = player1;

            }
            else
            {
                currentPlayer = player2;
            }
        }
    }
    public void SetupGame(int numberOfrounds)
    {
        ui.subMenuButton.gameObject.SetActive(true);
        ui.PlayerNames();
        ui.PlayerScore();
        board.gameObject.SetActive(true);
        player1.gameObject.SetActive(true);
        player2.gameObject.SetActive(true);
        foreach (Cell piece in board.GetComponentsInChildren<Cell>())
        {
            freePositions.Add(piece);
        }
        SelectIniPlayer(currentRound);
        StartCoroutine(RoundStart());
    }
    public IEnumerator RoundStart()
    {
        ui.subStatusText.gameObject.SetActive(false);
        ui.RoundStatusTextDisplay("ROUND: " + currentRound.ToString(),0,2);
        yield return ui.RoundStatusRoutine("ROUND: " + currentRound.ToString(), 0, 2);
        ui.subStatusText.gameObject.SetActive(true);
        ui.subStatusText.text = "ROUND: " + currentRound.ToString();
        player1.turnIndicator.SetActive(false);
        player2.turnIndicator.SetActive(false);
        StartCoroutine(Round(currentPlayer));
    }    
    public IEnumerator Round(PlayerScript player)
    {
        currentPlayer = player;
        player.PlayerSetup();
        if (freePositions.Count <= 9)
        {
            if(CheckLastAvailableCells(player).Count > 0)
            {
            } else
            {
                ui.RoundStatusTextDisplay("DRAW", 2, 2);
                yield return ui.RoundStatusRoutine("DRAW", 2, 2);
                ScoreWhenDraw();
                StartCoroutine(NextRound(4));
                StopCoroutine(Round(player));
            }
        }
        while (player.playing && player.turns > 0)
        {
            yield return null;
        }
        if (usedPositions.Count > 6)
        {
            CheckAllWinners(player);
            if (player1.winner || player2.winner)
            {
                StartCoroutine(WinningSEQ());
                StartCoroutine(NextRound(4));
                StopCoroutine(Round(player));
            }
            else if (player1.turns > 0 || player2.turns > 0)
            {
                SwitchPlayer(player);
            }
            else
            {
                ui.RoundStatusTextDisplay("DRAW", 2, 2);
                yield return ui.RoundStatusRoutine("DRAW", 2, 2);
                ScoreWhenDraw();
                StartCoroutine(NextRound(4));
                StopCoroutine(Round(player));
            }
        }
        else
        {
            SwitchPlayer(player);
        }
    }
    IEnumerator NextRound(int timer)
    {
        player1Score += player1.score;
        player2Score += player2.score;
        ui.PlayerScore();
        yield return new WaitForSeconds(timer);
        if (rounds > 1 && currentRound <= 3)
        {
            ui.nextRoundButton.gameObject.SetActive(true);
        } else
        {
            StartCoroutine(EndOfGame());
        }       
    }
    IEnumerator EndOfGame()
    {
        string text;
        if(player1Score > player2Score)
        {
            text = player1Name;
        } else if (player2Score > player1Score)
        {
            text = player2Name;   
        } else
        {
            text = "DRAW";
        }
        ui.WinnerAnimations(text,0,3);
        yield return  ui.WinSequence(text,0,3);
    }
    private List<Cell> CheckLastAvailableCells(PlayerScript player)
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
    private void CheckAllWinners(PlayerScript player)
    {
        if (player == player1)
        {
            WinCheckPlayersLastPosition(player1);
            if (!player1.winner)
            {
                WinCheckAllPositionForLastCell(player1);
            }
            WinCheckPlayersLastPosition(player2);
            if (!player2.winner)
            {
                WinCheckAllPositionForLastCell(player2);
            }
        }
        else
        {
            WinCheckPlayersLastPosition(player2);
            if (!player2.winner)
            {
                WinCheckAllPositionForLastCell(player2);
            }
            WinCheckPlayersLastPosition(player1);
            if (!player1.winner)
            {
                WinCheckAllPositionForLastCell(player1);
            }
        }
    }
    IEnumerator WinningSEQ()
    {
        foreach(Cell cell in usedPositions)
        {
            debugText.text += " " + cell.name + ",";
        }
        if(player1.winner && player2.winner)
        {
            if (GameManager.rounds > 1)
            {
                GameManager.roundsRecord[0, currentRound - 1] = 1;
                GameManager.roundsRecord[1, currentRound - 1] = 1;
            }
            PlaySound(pieceClip);
            ui.RoundStatusTextDisplay("DRAW", 2, 2);
            ScoreWhenDraw();
            foreach(Cell cell in winingCellsList)
            {
                cell.transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, -0.1f);   
                SpriteRenderer sprite = cell.transform.GetChild(0).GetComponent<SpriteRenderer>();
                sprite.sprite = diceSelectionSprite;
                sprite.enabled = true;
                sprite.transform.localScale = Vector3.one * 1.05f;
            }
            yield return ui.RoundStatusRoutine("DRAW", 2, 2);
            
        } else if (player1.winner)
        {
            if (GameManager.rounds > 1)
            {
                GameManager.roundsRecord[0, currentRound-1] = 1;
            }
            PlaySound(roundWinClip);
            ui.RoundStatusTextDisplay("ROUND FOR " + player1Name, 2, 2);
            foreach (Cell cell in winingCellsList)
            {
                cell.transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, -0.1f);
                SpriteRenderer sprite = cell.transform.GetChild(0).GetComponent<SpriteRenderer>();
                sprite.sprite = diceSelectionSprite;
                sprite.enabled = true;
                sprite.transform.localScale = Vector3.one * 1.05f;
            }
            WinningScoreCalculation(player1);
            ui.player1Score.text = player1.score.ToString();
            yield return ui.RoundStatusRoutine("ROUND FOR " + player1Name, 2, 2);
        }
        else if (player2.winner)
        {
            PlaySound(roundWinClip);
            ui.RoundStatusTextDisplay("ROUND FOR " + player2Name, 2, 2);
            if (GameManager.rounds > 1)
            {
                GameManager.roundsRecord[1, currentRound - 1] = 1;
            }
            foreach (Cell cell in winingCellsList)
            {
                cell.transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, -0.1f);
                SpriteRenderer sprite = cell.transform.GetChild(0).GetComponent<SpriteRenderer>();
                sprite.sprite = diceSelectionSprite;
                sprite.enabled = true;
                sprite.transform.localScale = Vector3.one * 1.05f;
            }
            WinningScoreCalculation(player2);
            ui.player2Score.text = player2.score.ToString();
            yield return ui.RoundStatusRoutine("ROUND FOR " + player2Name, 2, 2);
        } else
        {
            PlaySound(pieceClip);
            GameManager.roundsRecord[0, currentRound - 1] = 1;
            GameManager.roundsRecord[1, currentRound - 1] = 1;
            ui.RoundStatusTextDisplay("DRAW", 2, 2);
            ScoreWhenDraw();
            yield return ui.RoundStatusRoutine("DRAW", 2, 2);
        }
        yield return new WaitForSeconds(1);
    }
    void ScoreWhenDraw()
    {
        player1.scoreManager.ScoreWhenDraw(player1);
        player2.scoreManager.ScoreWhenDraw(player2);
        ui.player1Score.text = player1.score.ToString();
        ui.player2Score.text = player2.score.ToString();
    }
    void WinningScoreCalculation(PlayerScript player)
    {
        int scoreRule1 = 0;
        int scoreRule2 = 0;
        int scoreRule3 = 0;
        int scoreRule4 = 0;
        int scoreRule5 = 0;
        if (player.winner)
            scoreRule1 += 12;
        if (winingCellsList[3].player == player)
            scoreRule2 += 2;
        if (!player.usedBlocker)
            scoreRule3 += 5;
        foreach (Cell cell in freePositions)
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
    public bool CheckSurroundingPositions(Cell lastCell, Cell newCell)
    {   
        if (lastCell.row == newCell.row + 1)
        {
            if (lastCell.collum == newCell.collum - 1 ||
                lastCell.collum == newCell.collum ||
                lastCell.collum == newCell.collum + 1)
            {
                return true;
            } else
            {
                return false;
            }
        } else if (lastCell.row == newCell.row)
        {
            if (lastCell.collum == newCell.collum - 1 ||
                lastCell.collum == newCell.collum + 1)
            {
                return true;
            } else
            {
                return false;
            }
        } else if (lastCell.row == newCell.row - 1)
        {
            if (lastCell.collum == newCell.collum - 1 ||
                lastCell.collum == newCell.collum ||
                lastCell.collum == newCell.collum + 1)
            {
                return true;
            } else
            {
                return false;
            }
        } else
        {
            return false;
        }
    }

    public void WinCheckPlayersLastPosition(PlayerScript player)
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
                                                                                winingCellsList.Add(firstCell);
                                                                                winingCellsList.Add(secondCell);
                                                                                winingCellsList.Add(thirdCell);
                                                                                winingCellsList.Add(fourthCell);
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
    public void WinCheckAllPositionForLastCell(PlayerScript player)
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
                                                                                winingCellsList.Add(firstCell);
                                                                                winingCellsList.Add(secondCell);
                                                                                winingCellsList.Add(thirdCell);
                                                                                winingCellsList.Add(fourthCell);
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
    public void SwitchPlayer(PlayerScript player)
    {
        if (player == player1)
        {
            player1.turnIndicator.SetActive(false);
            StartCoroutine(Round(player2));
        }
        else
        {
            player2.turnIndicator.SetActive(false);
            StartCoroutine(Round(player1));
        }
    }
    public void ResetScoreRecords()
    {
        GameManager.player1Score = 0;
        GameManager.player2Score = 0;
        if (GameManager.rounds > 1)
        {
            for (int i = 0; i < 4; i++)
            {
                GameManager.roundsRecord[0, i] = 0;
                GameManager.roundsRecord[1, i] = 0;
            }
        }
    }

}
