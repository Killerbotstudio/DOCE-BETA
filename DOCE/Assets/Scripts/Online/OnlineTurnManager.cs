using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class OnlineTurnManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    [SerializeField]
    private PunTurnManager turnManager;
    private OnlineGameManager gameManager;

    public int turnInt;
    public bool isMyTurn;


    #region IPunTurnManagerCallbacks

    public void OnPlayerFinished(Player player, int turn, object move)
    {
        
        bool finishedByLocal = turnManager.GetPlayerFinishedTurn(PhotonNetwork.LocalPlayer);
        bool finishedByRemote = turnManager.GetPlayerFinishedTurn(PhotonNetwork.PlayerListOthers[0]);

        

        Debug.Log(">>>>>>> local:" + finishedByLocal + " VS remote: " + finishedByRemote + " <<<<<");
        if (player == PhotonNetwork.LocalPlayer) //Local is the one who finished
        {

            isMyTurn = false;
            gameManager.EndTurn();
            Debug.Log("I just finished! -------");
            Debug.Log("  -Local Player property 'turn' set to false and remote to true!");

        }
        else
        {
            Debug.Log("The other player just finished! -------");
            isMyTurn = true;
            gameManager.DecodeMove(move);

        }


        Debug.Log("Pieces: local: " + gameManager.controller.localPlayerInfo.listOfPlayedPositions.Count + " VS " + gameManager.controller.remotePlayerInfo.listOfPlayedPositions.Count);
        if (gameManager.CheckIfAnyWinner())
        {
            Debug.Log("Someone won or there is a tie");
            gameManager.EndOfROund();    
            Debug.Log("**********END ROUND********");

        } else if(isMyTurn)
        {
            
            turnManager.BeginTurn();
            Debug.Log("The round has ended. Because: " + gameManager.messageLog);
            
            //gameManager.SomethingHappened(msg);
        }
        Debug.Log("############TURN END############");
    }

    //Called when a blocker move is made. move is the information about that blocker movement
    public void OnPlayerMove(Player player, int turn, object move)
    {
        
        Debug.Log("OnPlayerMove Initialized (blocker moved) ------------");

        if (player != PhotonNetwork.LocalPlayer)
        {
            gameManager.DecodeMove(move);
        }
    }


    public void OnTurnBegins(int turn)
    {
        Debug.LogWarning("_______BEGIN TURN_____");
        //Debug.Log("OnTurnBegins Initialized");
        Debug.Log(PhotonNetwork.ServerTimestamp);
        if (!isMyTurn)
        {
            
            Debug.Log("goto wait again");
            
            if (!gameManager.CheckIfAvailableCells(gameManager.controller.remotePlayerInfo))
            {
                gameManager.EndOfROund();
                return;
            }
            gameManager.controller.remotePlayerInfo.turnIndicator.SetActive(true);
            return;
      
        }
       
        if (!gameManager.CheckIfAvailableCells(gameManager.controller.localPlayerInfo))
        {
            gameManager.EndOfROund();
            return;
        }
        
        gameManager.StartTurn();
                
    }

    public void OnTurnCompleted(int turn)
    {
        Debug.Log("OnTurnCompleted Initialized");
        Debug.Log("FINISHED! turn: " + turn);
        //if (turn % 2 == 1)
        //{
        //    Debug.Log("IT's ALSO MY TURN");
        //}
        //if (turnManager.GetPlayerFinishedTurn(PhotonNetwork.PlayerListOthers[0]))
        //{
        //    Debug.Log("IT's MY TURN NOW");
        //    turnManager.BeginTurn();
        //    switchButton.gameObject.SetActive(true);
        //}
        //throw new System.NotImplementedException();


    }

    public void OnTurnTimeEnds(int turn)
    {
        // throw new System.NotImplementedException();
    }


    #endregion

    #region MyMethods

    // Start is called before the first frame update
    public void InitializeTurnManager()
    {
        gameManager = FindObjectOfType<OnlineGameManager>();
        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;
        turnManager.TurnDuration = 60f;


    }

    #region messagesToSend

    public void SendBlockerMove(Cell block)
    {
        Debug.Log("Used Blocker");
        //string block = "BLOCKER MOFO";
        int val = 0;
        int row = block.row;
        int col = block.collum;
        string cellName = block.gameObject.name;

        object[] message = { val, row, col, cellName };

        turnManager.SendMove(message, false);

    }

    public void SendDiceMove(Cell cellPlayed)
    {
        Debug.Log("SendCellMove Initialized");
        int val = cellPlayed.value;
        int row = cellPlayed.row;
        int col = cellPlayed.collum;
        string cellName = cellPlayed.gameObject.name;
        Debug.Log("OOOO CELL NAME: " + cellName.ToString());
        object[] message = { val, row, col, cellName};



        bool turnFinished = true;
        turnManager.SendMove(message, turnFinished);



    }

    #endregion

    //public Button startGameButton;
    //public Button switchButton;

    #region OnSomething
    public void OnGameStart()
    {
        
        if (isMyTurn)
        {
            
            Debug.Log("##### Its time to begin");
            turnManager.BeginTurn();
        }
        else
        {
            Debug.Log("###### goto wait!");
            
        }
    }


    public bool OnGameOver()
    {
        return true;
    }

   
    //public void OnEndTurnx()
    //{
    //    Debug.Log("you clicked to end turn");
    //    // turnManager.BeginTurn();
    //    //turnInt = turnManager.Turn;
    //    //startGameButton.gameObject.SetActive(false);
    //    //switchButton.gameObject.SetActive(false);

    //    //PhotonNetwork.LocalPlayer.AddScore(0);
    //}
    #endregion
    #endregion

}

