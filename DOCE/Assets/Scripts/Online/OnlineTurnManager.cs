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

    //public PlayerInfo localPlayerInfo;   //TO CALL THE INFORMATION ON EACH PLAYER
    //public PlayerInfo remotePlayerInfo;

    #region IPunTurnManagerCallbacks

    public void OnPlayerFinished(Player player, int turn, object move)
    {
        
        bool finishedByLocal = turnManager.GetPlayerFinishedTurn(PhotonNetwork.LocalPlayer);
        bool finishedByRemote = turnManager.GetPlayerFinishedTurn(PhotonNetwork.PlayerListOthers[0]);

        

        Debug.Log(">>>>>>> local:" + finishedByLocal + " VS " + finishedByRemote + ": remote <<<<<");
      //  Debug.Log("Move of type " + move.GetType().Name + " received");
        if (player == PhotonNetwork.LocalPlayer) //Local is the one who finished
        {

            //TODO leer mi jugada y actualizar el tablero acorde a move
            isMyTurn = false;
            gameManager.EndTurn();
            Debug.Log("I just finished! -------");
            Debug.Log("  -Local Player property 'turn' set to false and remote to true!");

        }
        else
        {
            //object[] message = { 0, 0, 0, "s" };

            //if (move.GetType().IsArray)
            //{

            //    message = (object[])move;
            //}


            //int val = (int)message[0];
            //int row = (int)message[1];
            //int col = (int)message[2];
            //string cellName = (string)message[3];

            //Debug.Log("DECODING... " + val.ToString() + "," + row.ToString() + "," + col.ToString() + "," + cellName);
            isMyTurn = true;
            gameManager.DecodeMove(move);
            //TODO: leer la jugada de mi oponente y actualizar el tablero acorde a move
           // gameManager.StartTurn();
          //  Debug.Log("  -Remote Player property 'turn' set to false and local to true!");

        }


        //TODO: Check if a player won the game (start OnlineGameManager.EndOfGame() routine )

        Debug.Log("Pieces: local: " + gameManager.controller.localPlayerInfo.listOfPlayedPositions.Count + " VS " + gameManager.controller.remotePlayerInfo.listOfPlayedPositions.Count);
        if (gameManager.CheckIfAnyWinner())
        {
            Debug.Log("Someone won or there is a tie");
            gameManager.EndOfROund();    
            Debug.Log("**********END ROUND********");

        } else
        {
            turnManager.BeginTurn();
            Debug.Log("The round has ended. Because: " + gameManager.messageLog);
            Debug.Log("############TURN END############");

            //gameManager.SomethingHappened(msg);
        }

    }

    //Called when a blocker move is made. move is the information about that blocker movement
    public void OnPlayerMove(Player player, int turn, object move)
    {
        
        Debug.Log("OnPlayerMove Initialized (blocker moved) ------------");
       // Debug.Log("Received move of type " + move.GetType().Name + ": value " + cell.value + ", blocker" + cell.blocked);

        //TODO Muestra el movivmiento del blocker en el tablero
        if (player == PhotonNetwork.LocalPlayer)
        {
            //TODO Destroy my blocker 
        }
        else
        {
           // object[] message = { 0, 0, 0, "s" };

            //if (move.GetType().IsArray)
            //{

            //    message = (object[])move;
            //}


            //int val = (int)message[0];
            //int row = (int)message[1];
            //int col = (int)message[2];
            //string cellName = (string)message[3];

            //Debug.Log("DECODING... " + val.ToString() + "," + row.ToString() + "," + col.ToString() + "," + cellName);
            gameManager.DecodeMove(move);
            //TODO actualizar el tablero para mostrar el movimiento de blocker del oponente
            //TODO Destruir el blocker del oponente (en mi tablero)


        }
    }


    public void OnTurnBegins(int turn)
    {
        Debug.LogWarning("_______BEGIN TURN_____");
        //Debug.Log("OnTurnBegins Initialized");

        if (!isMyTurn)
        {
            //TODO Desabilita el uso de mis dados y blocker: Llamar a funcion que haga esto. 
            //TODO: Desactiva el indicador de mi turno y activa el de mi oponente
            //gameManager.EndTurn();

            //Check if the other player has any cells available
            //if(!gameManager.CheckIfAvailableCells(remotePlayerInfo)){ //if there was a tie by not cells available
             
            //    //TODO: Show tie message
            //}
            Debug.Log("goto wait again");
            //gameManager.MyTurnEnds();
            
            if (!gameManager.CheckIfAvailableCells(gameManager.controller.remotePlayerInfo))
            {
                gameManager.EndOfROund();
                return;
            }
            gameManager.controller.remotePlayerInfo.turnIndicator.SetActive(true);
            return;
      
        }
        //else{

        //TODO: Habilita el uso de mis dados y blocker, llama a la rutina que espera que hagas un movimiento.  
        //TODO: Activa el indicador de mi turno y desactiva el de mi oponente
        //if (gameManager.CheckIfAvailableCellslocalPlayerInfo))
        //{
        //    //TIE
        //}
        //gameManager.MyTurnBegins();
       
        if (!gameManager.CheckIfAvailableCells(gameManager.controller.localPlayerInfo))
        {
            gameManager.EndOfROund();
            return;
        }

       
        
        gameManager.StartTurn();

        //switchButton.gameObject.SetActive(true);

        //start couroutine to wait for a local move
        
        

        

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
            //turnInt = turnManager.Turn;
            //startGameButton.gameObject.SetActive(false);
            //switchButton.gameObject.SetActive(true);

           // PhotonNetwork.LocalPlayer.AddScore(0);
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

   
    public void OnEndTurnx()
    {
        Debug.Log("you clicked to end turn");
        // turnManager.BeginTurn();
        //turnInt = turnManager.Turn;
        //startGameButton.gameObject.SetActive(false);
        //switchButton.gameObject.SetActive(false);

        //PhotonNetwork.LocalPlayer.AddScore(0);
    }
    #endregion
    #endregion

}

