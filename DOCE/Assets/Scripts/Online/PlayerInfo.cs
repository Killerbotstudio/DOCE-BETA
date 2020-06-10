using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.EventSystems;

public class PlayerInfo : MonoBehaviourPunCallbacks
{

    [HideInInspector] public OnlineGameSetupController controller;
    //[HideInInspector] public OnlineGameManager manager;

    [Header("Player info")]
    public string playerID;
    public Color playerColor;
    public Player Player; //{ get; private set; }
    public bool local;
    //public bool rolledDice;

    public bool ready = false;
    public bool turn = false;
    public bool usedBlocker = false;
    public bool winner = false;
    public int dice = 11;
    public int score = 0;
    
    
    [Header ("Game info")]
    public bool playing;
    public PlayerPiece currentPlayingPiece;
    public Cell lastPositionPlayed;
    public List<Cell> listOfPlayedPositions;
    public GameObject markerPiece;
    private Vector3 markerPiecePosition;
    public GameObject turnIndicator;
    public GameObject blocker;



    public static int scorePlayerStatic;
    public GameObject roundsRecordObject;
    public int[] roundRecord = new int[4];

    [Header("Player UI")]
    public Text playerNameText;
    public Text diceCounter;
    public ScoreManager scoreManager;

    [SerializeField] private BoxCollider dropBox;

    #region Initialize

    public void Start()
    {
        markerPiecePosition = markerPiece.transform.position;

    }
    /// <summary>Resets the players to initial values</summary>
    public void Reset()
    {
        winner = false;
        ready = false;
        turn = false;
        usedBlocker = false;
        dice = 11;
        diceCounter.text = dice.ToString();
        turnIndicator.SetActive(false);
        markerPiece.transform.position = markerPiecePosition;
        PiecesSwitch(false);
        blocker.GetComponent<SpriteRenderer>().enabled = true;
        blocker.SetActive(true);
        scoreManager.ResetPanel();
        lastPositionPlayed = null;
        listOfPlayedPositions.Clear();
        currentPlayingPiece = null;
        scoreManager.ErraseScoreTexts();        //ERRASES THE SCORES IN TABLE


    }


    public void PiecesSwitch(bool activation) {

        foreach(PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
        {
            //piece.gameObject.SetActive(activation);
            piece.GetComponent<BoxCollider>().enabled = activation;
//            Debug.Log(piece.name);
        }
        turnIndicator.SetActive(activation);
        controller.remotePlayerInfo.turnIndicator.SetActive(!activation);
    }


    

    #endregion Initialize


    #region Player update
    public void SetPlayerInfo(Player player)
    {
        if (player.CustomProperties.ContainsKey("turn"))
        {
            turn = (bool)player.CustomProperties["turn"];
        }
        if (player.CustomProperties.ContainsKey("blocker"))
        {
            usedBlocker = (bool)player.CustomProperties["blocker"];
        }
        //if (player.CustomProperties.ContainsKey("dice"))
        //{
        //    dice = (int)player.CustomProperties["dice"];
        //}
        if (player.CustomProperties.ContainsKey("winner"))
        {
            winner = (bool)player.CustomProperties["winner"];
        }
        if (player.CustomProperties.ContainsKey("ready"))
        {
            ready = (bool)player.CustomProperties["ready"];
        }
    }

    public void GetPlayerInfo(Player player)
    {

    }

    #endregion Player update




    bool running;
    public void TurnMovement()
    { 
        playing = true;
        dropBox.gameObject.SetActive(false);
        dropBox.enabled = false;
        if (!running)
        {
            running = true;
            StartCoroutine(PlayingRoutine());
        }
            
    }

    
    IEnumerator PlayingRoutine()
    {
        turnIndicator.SetActive(true); //CALL FROM TURNS
        Debug.Log("playing");

        while (playing)
        {
            if (currentPlayingPiece == null)
            {
                dropBox.gameObject.SetActive(false);
                dropBox.enabled = false;

                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log("click2" + this.gameObject);
                    LayerMask layerMaskPiece = LayerMask.GetMask("Pieces");
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskPiece))
                    {
                        Debug.Log("piece");
                        if (!IsPointerOverUIObject())
                        {
                            Debug.Log("uiCheck");
                            controller.manager.PlaySound(controller.manager.clickClip);
                            PlayerPiece piece = hit.collider.GetComponent<PlayerPiece>();
                            PlayerPiece playingPieceClone = Instantiate(piece, Input.mousePosition, Quaternion.identity);
                            playingPieceClone.name = playerID + "piece" + playingPieceClone.value;
                            playingPieceClone.gameObject.layer = 1;
                            piece.GetComponent<BoxCollider>().enabled = false;
                            piece.GetComponent<SpriteRenderer>().enabled = false;
                            currentPlayingPiece = playingPieceClone;
                            dropBox.gameObject.SetActive(true);
                            dropBox.enabled = true;
                        }
                    }
                }
            }
            else
            {
                if (!Input.GetMouseButtonUp(1))
                {
                    Vector3 temp = Input.mousePosition;
                    temp.z = 10f;
                    currentPlayingPiece.transform.position = Camera.main.ScreenToWorldPoint(temp);
                    LayerMask layerMaskCell = LayerMask.GetMask("Cell");
                    LayerMask layermaskPieceAvoid = LayerMask.GetMask("Pieces");
                    LayerMask layerDropBox = LayerMask.GetMask("Drop");
                    layermaskPieceAvoid = ~layermaskPieceAvoid;
                    if (Input.GetMouseButtonUp(0))
                    {
                        Debug.Log("click");
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        //if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerDropBox))
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerDropBox))
                        {
                            dropBox.gameObject.SetActive(false);
                            dropBox.enabled = false;
                            controller.manager.PlaySound(controller.manager.pieceClip);
                            Debug.Log("Droped piece " + hit.transform.name);
                            foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
                            {
                                piece.GetComponent<BoxCollider>().enabled = true;
                                piece.GetComponent<SpriteRenderer>().enabled = true;
                            }
                            Destroy(currentPlayingPiece.gameObject);
                        }

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskCell))
                        {
                            if (!IsPointerOverUIObject())
                            {
                                Cell playingCell = hit.collider.GetComponent<Cell>();
                                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                                if (!currentPlayingPiece.blocker)
                                {
                                    if (controller.manager.freePositions.Contains(playingCell))
                                    {
                                        if (CheckLastPosition(lastPositionPlayed, playingCell))
                                        {
                                            controller.manager.PlaySound(controller.manager.pieceClip);
                                            Cell hitCell = hit.transform.GetComponent<Cell>();
                                            hitCell.CallPieceDATA(currentPlayingPiece);
                                            lastPositionPlayed = hitCell;
                                            listOfPlayedPositions.Add(lastPositionPlayed);
                                            controller.manager.usedPositions.Add(lastPositionPlayed);
                                            controller.manager.freePositions.Remove(playingCell);


                                            //END OF ROUND
                                            controller.turnManager.SendDiceMove(lastPositionPlayed); //SendMessage move to turn manager
                                            
                                            
                                            Destroy(currentPlayingPiece.gameObject);
                                            StartCoroutine(MarkerMovement(hitCell));
                                            //turns -= 1;
                                            foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
                                            {
                                                piece.GetComponent<SpriteRenderer>().enabled = true;
                                            }
                                            controller.localPlayerInfo.dice -= 1;
                                            controller.localPlayerInfo.diceCounter.text = controller.localPlayerInfo.dice.ToString();
                                            ExitGames.Client.Photon.Hashtable newHash = PhotonNetwork.LocalPlayer.CustomProperties;
                                            newHash["dice"] = dice - 1;
                                            PhotonNetwork.LocalPlayer.SetCustomProperties(newHash);

                                            Debug.Log("played piece");
                                            playing = false;
                                        }
                                        else
                                        {
                                            controller.manager.PlaySound(controller.manager.errorClip);
                                            StartCoroutine(VibrateMarkerPiece());
                                        }
                                    }
                                    else
                                    {
                                        controller.manager.PlaySound(controller.manager.errorClip);
                                    }
                                }
                                else if (controller.manager.freePositions.Contains(playingCell))
                                {
                                    usedBlocker = true;
                                    controller.manager.PlaySound(controller.manager.pieceClip);
                                    Cell hitCell = hit.transform.GetComponent<Cell>();
                                    hitCell.CallPieceDATA(currentPlayingPiece);
                                    listOfPlayedPositions.Add(hitCell);
                                    controller.manager.usedPositions.Add(hitCell);
                                    controller.manager.freePositions.Remove(playingCell);

                                    //Cell BlockerCell = hitCell;
                                    controller.turnManager.SendBlockerMove(hitCell); //Send moves to turn manager
                                    Destroy(currentPlayingPiece.gameObject);

                                    blocker.SetActive(false);
                                    //foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
                                    //{
                                    //    if (piece.blocker)
                                    //    {
                                    //        piece.gameObject.SetActive(false);
                                    //        //Destroy(piece.gameObject);
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                }
                else
                {
                    dropBox.gameObject.SetActive(false);
                    dropBox.enabled = false;

                    controller.manager.PlaySound(controller.manager.pieceClip);
                    Debug.Log("Canceled droped");
                    foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
                    {
                        piece.GetComponent<BoxCollider>().enabled = true;
                        piece.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    Destroy(currentPlayingPiece.gameObject);
                }

            }
            yield return null;
        }
        Debug.Log("end of turn");
        dropBox.gameObject.SetActive(false);
        dropBox.enabled = false;
        running = false;
    }

    public bool CheckLastPosition(Cell lastCell, Cell newCell)
    {
        if (lastPositionPlayed)
        {
            if (lastCell.row == newCell.row + 1)
            {
                if (lastCell.collum == newCell.collum - 1 ||
                    lastCell.collum == newCell.collum ||
                    lastCell.collum == newCell.collum + 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (lastCell.row == newCell.row)
            {
                if (lastCell.collum == newCell.collum - 1 ||
                    lastCell.collum == newCell.collum ||
                    lastCell.collum == newCell.collum + 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (lastCell.row == newCell.row - 1)
            {
                if (lastCell.collum == newCell.collum - 1 ||
                    lastCell.collum == newCell.collum ||
                    lastCell.collum == newCell.collum + 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    public IEnumerator VibrateMarkerPiece()
    {
        Vector3 pos = new Vector3(lastPositionPlayed.transform.position.x, lastPositionPlayed.transform.position.y, -1f);
        float timer = 0.5f;
        float counter = 0;
        while (counter < timer)
        {
            counter += Time.deltaTime;
            Vector3 random = Random.insideUnitCircle * 0.1f;
            markerPiece.transform.position = random + pos;
            yield return null;
        }
        markerPiece.transform.position = new Vector3(lastPositionPlayed.transform.position.x, lastPositionPlayed.transform.position.y, -1f);
        StopCoroutine(VibrateMarkerPiece());
    }

    public IEnumerator MarkerMovement(Cell cell)
    {
        Vector2 destination = new Vector2(cell.transform.position.x, cell.transform.position.y);
        while (Vector2.Distance(destination, new Vector2(markerPiece.transform.position.x, markerPiece.transform.position.y)) > 0.1f)
        {
            markerPiece.transform.position = Vector3.MoveTowards(markerPiece.transform.position, new Vector3(destination.x, destination.y, -0.1f), 30 * Time.deltaTime);
            yield return null;
        }
        markerPiece.transform.position = new Vector3(destination.x, destination.y, -1f);
        StopCoroutine(MarkerMovement(cell));
    }

   

    //TO PREVENT FROM CLICKING THE PIECE PICKED BY PLAYER
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }






}
