////////////PlayerScript////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PlayerScript : MonoBehaviour
{

    public string playerName;
    public Color playerColor;
    public int turns = 11;
    public bool usedBlocker; 
    public int score;


    public static string playerNameStatic;
    public static int scorePlayerStatic;

    public  bool playing;
    
    [HideInInspector]public int nameToString;         
    public List<Cell> listOfPlayedPositions;
    public GameManager manager;
    public PlayerPiece playingPiece;

    public Cell lastPositionPlayed;

    private float timer;
    public Text timerText;
    public Text diceCounter;
    public GameObject turnIndicator;

    public GameObject marker;

    public List<Cell> winingCells;

    public ScoreManager scoreManager;
    public GameObject roundsRecordObject;
    public int[] roundRecord = new int[4];
    public bool winner;


    [SerializeField] private BoxCollider dropBox;

    private void Start()
    { 
        manager = FindObjectOfType<GameManager>();
        scoreManager = this.GetComponent<ScoreManager>();
        if (GameManager.rounds > 1)
        {
            roundsRecordObject.SetActive(true);
            SetupRecord();
        } else
        {
            roundsRecordObject.SetActive(false);
        }
        diceCounter.text = turns.ToString();
    }
    public void SetupRecord()
    {
        for (int i  =0; i < roundRecord.Length; i++)
        {
            if (roundRecord[i] == 1)
            {
                roundsRecordObject.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                
            }
        }
    }
    public void PlayerSetup()
    {
        playing = true;
        foreach(PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
        {
            piece.GetComponent<BoxCollider>().enabled = true;
        }
        dropBox.gameObject.SetActive(false);
        dropBox.enabled = false;

        StartCoroutine(Turn());
    }
    private void SetupTimer()
    {
        timer += Time.deltaTime;
        float minutes = Mathf.Floor(timer / 60);
        float seconds = Mathf.RoundToInt(timer % 60);
        string m, s;
        if (minutes < 10)
        {
            m = "0" + minutes.ToString();
        }
        else
        {
            m = minutes.ToString();
        }
        if (seconds < 10)
        {
            s = "0" + seconds.ToString();
        }
        else
        {
            s = seconds.ToString();
        }
        timerText.text = m + ":" + s;
    }
    public IEnumerator MarkerMovement()
    {
        Vector2 destination = new Vector2(lastPositionPlayed.transform.position.x, lastPositionPlayed.transform.position.y);
        while(Vector2.Distance(destination, new Vector2(marker.transform.position.x, marker.transform.position.y)) > 0.1f)
        {
            marker.transform.position = Vector3.MoveTowards(marker.transform.position, new Vector3(destination.x, destination.y, -0.1f), 30 * Time.deltaTime);
            yield return null;
        }
        marker.transform.position = new Vector3(destination.x, destination.y, -1f);
        StopCoroutine(MarkerMovement());
    }
    public IEnumerator Turn()
    {
        turnIndicator.SetActive(true);
        while (playing)
        {
            SetupTimer();
            if (playingPiece == null)
            {
                dropBox.gameObject.SetActive(false);
                dropBox.enabled = false;
                
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log("click2");
                    LayerMask layerMaskPiece = LayerMask.GetMask("Pieces");
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit,Mathf.Infinity, layerMaskPiece))
                    {
                        Debug.Log("piece");
                        if (!IsPointerOverUIObject())
                        {
                            Debug.Log("uiCheck");
                            manager.PlaySound(manager.clickClip);
                            PlayerPiece piece = hit.collider.GetComponent<PlayerPiece>();
                            PlayerPiece playingPieceClone = Instantiate(piece, Input.mousePosition, Quaternion.identity);
                            playingPieceClone.name = playerName + "piece" + playingPieceClone.value;
                            playingPieceClone.gameObject.layer = 1;
                            piece.GetComponent<BoxCollider>().enabled = false;
                            piece.GetComponent<SpriteRenderer>().enabled = false;
                            playingPiece = playingPieceClone;
                            dropBox.gameObject.SetActive(true);
                            dropBox.enabled = true;
                            
                        }
                    }
                }
            } else
            {
                if (!Input.GetMouseButtonUp(1))
                {
                    Vector3 temp = Input.mousePosition;
                    temp.z = 10f;
                    playingPiece.transform.position = Camera.main.ScreenToWorldPoint(temp);
                    LayerMask layerMaskCell = LayerMask.GetMask("Cell");
                    LayerMask layermaskPieceAvoid = LayerMask.GetMask("Pieces");
                    LayerMask layerDropBox = LayerMask.GetMask("Drop");
                    layermaskPieceAvoid = ~layermaskPieceAvoid;
                    if (Input.GetMouseButtonUp(0))
                    {
                        Debug.Log("click");
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        //Drop on positions
                        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerDropBox)){
                            dropBox.gameObject.SetActive(false);
                            dropBox.enabled = false;
                            manager.PlaySound(manager.pieceClip);
                            Debug.Log("Droped piece " + hit.transform.name);
                            foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
                            {
                                piece.GetComponent<BoxCollider>().enabled = true;
                                piece.GetComponent<SpriteRenderer>().enabled = true;
                            }
                            Destroy(playingPiece.gameObject);
                        }

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskCell))
                        {
                            if (!IsPointerOverUIObject())
                            {
                                Cell playingCell = hit.collider.GetComponent<Cell>();
                                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                                if (!playingPiece.blocker)
                                {
                                    if (manager.freePositions.Contains(playingCell))
                                    {
                                        if (CheckLastPosition(lastPositionPlayed, playingCell))
                                        {
                                            manager.PlaySound(manager.pieceClip);
                                            Cell hitCell = hit.transform.GetComponent<Cell>();
                                            hitCell.CallPieceDATA(playingPiece);
                                            lastPositionPlayed = hitCell;
                                            listOfPlayedPositions.Add(lastPositionPlayed);
                                            manager.usedPositions.Add(lastPositionPlayed);
                                            manager.freePositions.Remove(playingCell);
                                            EndOfRound();
                                            Destroy(playingPiece.gameObject);
                                            StartCoroutine(MarkerMovement());
                                            turns -= 1;
                                            diceCounter.text = turns.ToString();
                                            playing = false;
                                        }
                                        else
                                        {
                                            manager.PlaySound(manager.errorClip);
                                            StartCoroutine(VibrateMarkerPiece());
                                        }
                                    }
                                    else
                                    {
                                        manager.PlaySound(manager.errorClip);
                                    }
                                }
                                else if (manager.freePositions.Contains(playingCell))
                                {
                                    usedBlocker = true;
                                    manager.PlaySound(manager.pieceClip);
                                    Cell hitCell = hit.transform.GetComponent<Cell>();
                                    hitCell.CallPieceDATA(playingPiece);
                                    manager.usedPositions.Add(hitCell);
                                    manager.freePositions.Remove(playingCell);
                                    Cell BlockerCell = hitCell;
                                    listOfPlayedPositions.Add(BlockerCell);
                                    Destroy(playingPiece.gameObject);
                                    foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
                                    {
                                        if (piece.blocker)
                                        {
                                            Destroy(piece.gameObject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    dropBox.gameObject.SetActive(false);
                    dropBox.enabled = false;
                    manager.PlaySound(manager.pieceClip);
                    foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
                    {
                        piece.GetComponent<BoxCollider>().enabled = true;
                        piece.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    Destroy(playingPiece.gameObject);
                }

            }
            yield return null;
        }
        dropBox.gameObject.SetActive(false);
        dropBox.enabled = false;
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

        } else
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
            marker.transform.position = random + pos;
            yield return null;
        }
        marker.transform.position = new Vector3(lastPositionPlayed.transform.position.x, lastPositionPlayed.transform.position.y, -1f);
        StopCoroutine(VibrateMarkerPiece());
    }
    public void EndOfRound()
    {
        foreach (PlayerPiece piece in this.GetComponentsInChildren<PlayerPiece>())
        {
            piece.GetComponent<Collider>().enabled = false;
            piece.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
