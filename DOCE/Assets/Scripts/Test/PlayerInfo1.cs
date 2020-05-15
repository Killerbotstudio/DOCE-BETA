//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Realtime;
//using Photon.Pun;
//using UnityEngine.UI;
//using ExitGames.Client.Photon;
//using UnityEngine.EventSystems;

//public class PlayerInfo : MonoBehaviourPunCallbacks
//{

//    [HideInInspector] public OnlineGameSetupController controller;
//    [HideInInspector] public OnlineGameManager manager;

//    [Header("Player info")]
//    public string playerID;
//    public Color playerColor;
//    public Player Player; //{ get; private set; }
//    public bool local;
//    //public bool rolledDice;

//    public bool ready = false;
//    public bool turn = false;
//    public bool usedBlocker = false;
//    public bool winner = false;
//    public int dice = 11;
//    public int score = 0;

//    [Header ("")]
    
    
//    [Header ("Game info")]
//    public PlayerPiece currentPlayingPiece;
//    public Cell lastPositionPlayed;
//    public List<Cell> listOfPlayedPositions;
//    public GameObject markerPiece;
//    public GameObject turnIndicator;



//    public static int scorePlayerStatic;
//    public GameObject roundsRecordObject;
//    public int[] roundRecord = new int[4];

//    [Header("Player UI")]
//    public Text playerNameText;
    

//    #region Initialize

//    public void RoundRecordStarter()
//    {
//        if (controller.rounds > 1)
//        {
//            roundsRecordObject.SetActive(true);
//            SetupRoundRecord();
//        }
//        else
//        {
//            roundsRecordObject.SetActive(false);
//        }


//    }
//    public void SetupRoundRecord()
//    {
//        for (int i = 0; i < roundRecord.Length; i++)
//        {
//            if (roundRecord[i] == 1)
//            {
//                roundsRecordObject.transform.GetChild(i).GetComponent<Image>().color = Color.white;

//            }
//        }
//    }
//    public void DiceRecord(Text diceCounter)
//    {
//        diceCounter.text = dice.ToString();
//    }

//    //public void InitializePlayer(Player iPlayer)
//    //{
//    //    Player = iPlayer;
//    //    initHASH();
//    //}
//    //public void initHASH()
//    //{
//    //    Player.CustomProperties.Clear();
//    //    //ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();

//    //    //hash.Add("turn", false);
//    //    //hash.Add("color", "new");
//    //    //hash.Add("dice", 0);

//    //    //Player.SetCustomProperties(hash);
//    //}
//    //public void StartHASH(bool isTurn, string isColor, int nDice)
//    //{
//    //    ExitGames.Client.Photon.Hashtable hash = Player.CustomProperties;

//    //    hash["turn"] = isTurn;
//    //    hash["color"] = isColor;
//    //    hash["dice"] = nDice;

//    //    Player.SetCustomProperties(hash);
//    //}
//    #endregion Initialize


//    #region Player update
//    public void SetPlayerInfo(Player player)
//    {
//        if (player.CustomProperties.ContainsKey("turn"))
//        {
//            turn = (bool)player.CustomProperties["turn"];
//        }
//        if (player.CustomProperties.ContainsKey("blocker"))
//        {
//            usedBlocker = (bool)player.CustomProperties["blocker"];
//        }
//        if (player.CustomProperties.ContainsKey("dice"))
//        {
//            dice = (int)player.CustomProperties["dice"];
//        }
//        if (player.CustomProperties.ContainsKey("winner"))
//        {
//            winner = (bool)player.CustomProperties["winner"];
//        }
//        if (player.CustomProperties.ContainsKey("ready"))
//        {
//            ready = (bool)player.CustomProperties["ready"];
//        }
//    }
//    #endregion Player update




//    public bool playing;
//    public void TurnMovement()
//    {

//        playing = true;
//        StartCoroutine(PlayingRoutine());



//    }

//    IEnumerator PlayingRoutine()
//    {
//        turnIndicator.SetActive(true); //CALL FROM TURNS
//        while (playing)
//        {
//            if (currentPlayingPiece == null)
//            {
//                if (Input.GetMouseButtonUp(0))
//                {
//                    LayerMask layerMaskPiece = LayerMask.GetMask("Pieces");
//                    RaycastHit hit;
//                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskPiece))
//                    {
//                        if (!IsPointerOverUIObject())
//                        {
//                            manager.PlaySound(manager.clickClip);
//                            PlayerPiece piece = hit.collider.GetComponent<PlayerPiece>();
//                            PlayerPiece playingPieceClone = Instantiate(piece, Input.mousePosition, Quaternion.identity);
//                            //playingPieceClone.name = playerName + "piece" + playingPieceClone.value;
//                            playingPieceClone.gameObject.layer = 1;
//                            piece.GetComponent<BoxCollider>().enabled = false;
//                            piece.GetComponent<SpriteRenderer>().enabled = false;
//                            currentPlayingPiece = playingPieceClone;
//                        }
//                    }
//                }
//            }
//            else
//            {
//                if (!Input.GetMouseButtonUp(1))
//                {
//                    Vector3 temp = Input.mousePosition;
//                    temp.z = 10f;
//                    currentPlayingPiece.transform.position = Camera.main.ScreenToWorldPoint(temp);
//                    LayerMask layerMaskCell = LayerMask.GetMask("Cell");
//                    LayerMask layermaskPieceAvoid = LayerMask.GetMask("Pieces");
//                    layermaskPieceAvoid = ~layermaskPieceAvoid;
//                    if (Input.GetMouseButtonUp(0))
//                    {
//                        RaycastHit hit;
//                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskCell))
//                        {
//                            if (!IsPointerOverUIObject())
//                            {
//                                Cell playingCell = hit.collider.GetComponent<Cell>();
//                                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
//                                if (!currentPlayingPiece.blocker)
//                                {
//                                    if (manager.freePositions.Contains(playingCell))
//                                    {
//                                        if (CheckLastPosition(lastPositionPlayed, playingCell))
//                                        {
//                                            manager.PlaySound(manager.pieceClip);
//                                            Cell hitCell = hit.transform.GetComponent<Cell>();
//                                            hitCell.CallPieceDATA(currentPlayingPiece);
//                                            lastPositionPlayed = hitCell;
//                                            listOfPlayedPositions.Add(lastPositionPlayed);
//                                            manager.usedPositions.Add(lastPositionPlayed);
//                                            manager.freePositions.Remove(playingCell);
//                                            EndOfRound();
//                                            Destroy(currentPlayingPiece.gameObject);
//                                            StartCoroutine(MarkerMovement());
//                                            //turns -= 1;
//                                            ExitGames.Client.Photon.Hashtable newHash = PhotonNetwork.LocalPlayer.CustomProperties;
//                                            newHash["dice"] = dice - 1;
//                                            PhotonNetwork.LocalPlayer.SetCustomProperties(newHash);


//                                            playing = false;
//                                        }
//                                        else
//                                        {
//                                            manager.PlaySound(manager.errorClip);
//                                            StartCoroutine(VibrateMarkerPiece());
//                                        }
//                                    }
//                                    else
//                                    {
//                                        manager.PlaySound(manager.errorClip);
//                                    }
//                                }
//                                else if (manager.freePositions.Contains(playingCell))
//                                {
//                                    usedBlocker = true;
//                                    manager.PlaySound(manager.pieceClip);
//                                    Cell hitCell = hit.transform.GetComponent<Cell>();
//                                    hitCell.CallPieceDATA(currentPlayingPiece);
//                                    manager.usedPositions.Add(hitCell);
//                                    manager.freePositions.Remove(playingCell);
//                                    listOfPlayedPositions.Add(lastPositionPlayed);
//                                    Destroy(currentPlayingPiece.gameObject);
//                                    foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
//                                    {
//                                        if (piece.blocker)
//                                        {
//                                            Destroy(piece.gameObject);
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    manager.PlaySound(manager.click2Clip);
//                    foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
//                    {
//                        piece.GetComponent<BoxCollider>().enabled = true;
//                        piece.GetComponent<SpriteRenderer>().enabled = true;
//                    }
//                    Destroy(currentPlayingPiece.gameObject);
//                }

//            }
//            yield return null;
//        }
//    }

//    public bool CheckLastPosition(Cell lastCell, Cell newCell)
//    {
//        if (lastPositionPlayed)
//        {
//            if (lastCell.row == newCell.row + 1)
//            {
//                if (lastCell.collum == newCell.collum - 1 ||
//                    lastCell.collum == newCell.collum ||
//                    lastCell.collum == newCell.collum + 1)
//                {
//                    return false;
//                }
//                else
//                {
//                    return true;
//                }
//            }
//            else if (lastCell.row == newCell.row)
//            {
//                if (lastCell.collum == newCell.collum - 1 ||
//                    lastCell.collum == newCell.collum ||
//                    lastCell.collum == newCell.collum + 1)
//                {
//                    return false;
//                }
//                else
//                {
//                    return true;
//                }
//            }
//            else if (lastCell.row == newCell.row - 1)
//            {
//                if (lastCell.collum == newCell.collum - 1 ||
//                    lastCell.collum == newCell.collum ||
//                    lastCell.collum == newCell.collum + 1)
//                {
//                    return false;
//                }
//                else
//                {
//                    return true;
//                }
//            }
//            else
//            {
//                return true;
//            }
//        }
//        else
//        {
//            return true;
//        }
//    }

//    public IEnumerator VibrateMarkerPiece()
//    {
//        Vector3 pos = new Vector3(lastPositionPlayed.transform.position.x, lastPositionPlayed.transform.position.y, -1f);
//        float timer = 0.5f;
//        float counter = 0;
//        while (counter < timer)
//        {
//            counter += Time.deltaTime;
//            Vector3 random = Random.insideUnitCircle * 0.1f;
//            markerPiece.transform.position = random + pos;
//            yield return null;
//        }
//        markerPiece.transform.position = new Vector3(lastPositionPlayed.transform.position.x, lastPositionPlayed.transform.position.y, -1f);
//        StopCoroutine(VibrateMarkerPiece());
//    }

//    public IEnumerator MarkerMovement()
//    {
//        Vector2 destination = new Vector2(lastPositionPlayed.transform.position.x, lastPositionPlayed.transform.position.y);
//        while (Vector2.Distance(destination, new Vector2(markerPiece.transform.position.x, markerPiece.transform.position.y)) > 0.1f)
//        {
//            markerPiece.transform.position = Vector3.MoveTowards(markerPiece.transform.position, new Vector3(destination.x, destination.y, -0.1f), 30 * Time.deltaTime);
//            yield return null;
//        }
//        markerPiece.transform.position = new Vector3(destination.x, destination.y, -1f);
//        StopCoroutine(MarkerMovement());
//    }

//    public void EndOfRound()
//    {
//        foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
//        {
//            piece.GetComponent<Collider>().enabled = false;
//            piece.GetComponent<SpriteRenderer>().enabled = true;
//        }
//    }

//    private bool IsPointerOverUIObject()
//    {
//        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
//        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
//        List<RaycastResult> results = new List<RaycastResult>();
//        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
//        return results.Count > 0;
//    }






//}
