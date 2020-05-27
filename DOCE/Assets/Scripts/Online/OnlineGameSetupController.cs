using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class OnlineGameSetupController : MonoBehaviourPunCallbacks
{


    [Header("Main")]
    public UIOnline ui;
    public OnlineTurnManager turnManager;
    public OnlineGameManager manager;

    public int multiPlayerSceneIndex;
    public int lobbySceneIndex;
    public bool gameBegan;

    [Header("THIS PLAYER")]
    public Text playerName;


    [Header("Room Configuration")]
    public string roomName;
    public string roomType;
    public int roundNumber;
    public int rounds;
    


    [Header("Players")]
    public PlayerInfo blackPlayerInfo;
    public PlayerInfo whitePlayerInfo;
    [HideInInspector] public PlayerInfo localPlayerInfo;            //TO CALL THE INFORMATION ON EACH PLAYER
    [HideInInspector] public PlayerInfo remotePlayerInfo;
    public List<PlayerInfo> playersInfoList;

    [Header("Board")]
    public GameObject board;

    private const byte START_ROUND_EVENT = 0;




    #region main setup
    private void Awake()
    {
        PhotonHandler handler = FindObjectOfType<PhotonHandler>();
        handler.ApplyDontDestroyOnLoad = false;
       
        

        Debug.Log(">>>>>>>>>Online Setup<<<<<<<");
        turnManager = FindObjectOfType<OnlineTurnManager>();
        turnManager.InitializeTurnManager();
        InitRoomSetup();
        OnAwakeSetupPlayers();
        UISetUp();
    }
    private void UISetUp()
    {
        ui = FindObjectOfType<UIOnline>();
        ui.controller = this;
        ui.whitePlayer = whitePlayerInfo;
        ui.blackPlayer = blackPlayerInfo;
        ui.blackStarterText.text = blackPlayerInfo.playerID;
        ui.whiteStarterText.text = whitePlayerInfo.playerID;

        if (roomType == "single")
        {
            ui.startText.text = "PLAYING SINGLE GAME";
        } else
        {
            ui.startText.text = "PLAYING A MATCH (4 GAMES)";
        }

        ui.StarterSetup();

       
    }
    private void InitRoomSetup()        //CALLED ONLY ON AWAKE TO SETUP THE ROOM
    {
        roomName = PhotonNetwork.CurrentRoom.Name;

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("type"))
        {
            StartCoroutine(LeaveToMenu());
            return;
        }
        else
        {
            roomType = (string)PhotonNetwork.CurrentRoom.CustomProperties["type"];
            Debug.Log("We are now playing in a room of type: " + roomType);
            if (roomType == "match")
            {
                rounds = 4;
            }
            else if (roomType == "single")
            {
                rounds = 1;
            }
        }
        Debug.Log(roomName);
        Debug.Log(roomType + " : " + rounds);
    }
    #region Players setup
    private void OnAwakeSetupPlayers()      //CALLED ONLY ONCE ON AWAKE METHOD
    {
        if (PhotonNetwork.PlayerList.Length != 2)
        {
            StartCoroutine(LeaveToMenu());
            return;
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient)
            {
                blackPlayerInfo.Player = player;
                playersInfoList.Add(blackPlayerInfo);
            }
            else
            {
                whitePlayerInfo.Player = player;
                playersInfoList.Add(whitePlayerInfo);
            }
        }
        foreach (PlayerInfo info in playersInfoList)
        {
            if (info.Player == PhotonNetwork.LocalPlayer)
            {
                info.playerID = "YOU";
                localPlayerInfo = info;
                //turnManager.localPlayerInfo = info;


                info.local = true;
            }
            else
            {
                info.playerID = info.Player.NickName;
                remotePlayerInfo = info;
                //turnManager.remotePlayerInfo = info;
            }
            //ADD HASH SETUP
            info.controller = this;
            ResetPlayerHashes(info.Player);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            RollDiceByMaster();
        }


    }
    public void ResetPlayerHashes(Player player)        //CLEARS AND RESETS PLAYER'S PROPERTIES
    {
        player.CustomProperties.Clear();
        Hashtable hash = new Hashtable();

        hash.Add("ready", false);
        hash.Add("turn", false);
        hash.Add("winner", false);
        hash.Add("blocker", false);
        hash.Add("score", 0);
        hash.Add("dice", 11);
        hash.Add("rematch", false);

        player.SetCustomProperties(hash);
    }
    #endregion Players setup

    #endregion main setup

//------------------------------------
    #region Game setup
    public void NextRound()
    {
        Debug.Log("@@@@@@@   NEXT ROUND   @@@@@@@");

        roundNumber += 1;
        Debug.Log("going to next round: " + roundNumber);
        Hashtable roomSetHash = new Hashtable();
        roomSetHash.Add("round", roundNumber);
        // roomSetHash.Add("type", roomType);
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomSetHash);

    }
    private void ResetForNewGame()             //WILL RESET VALUES TO START A NEW ROUND
    {
        Debug.Log("ResetForNewGame");
        PhotonNetwork.LoadLevel("OnlineGameScene");

    }
    #endregion Game Setup;


    #region Starters
    private void RollDiceByMaster()     //ROLLS DICE ONLY BY MASTER
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        else
        {
            StartCoroutine(RollDice());
        }
    }
    private IEnumerator RollDice()      //MASTER CLIENT ADDS THE PROPERTY "roll" TO THE ROOM
    {
        int blk = 0;
        int wht = 0;
        while (blk == wht)
        {
            blk = Random.Range(0, 6);
            wht = Random.Range(0, 6);
            yield return null;
        }
        int[] dice = { blk, wht };

        //string diceResult = blk.ToString() + ":" + wht.ToString();
        if (PhotonNetwork.IsMasterClient)
        {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                Hashtable hash = new Hashtable();
                hash.Add("roll", dice);
                player.SetCustomProperties(hash);
                if (player.CustomProperties.ContainsKey("roll"))
                    Debug.Log("Added roll");
            }
        }
    }
    public void RolledFromUINowToStartGame()                //CALLED FROM UI TO BEGIN SETUP READY
    {
        Debug.Log("roll dice check");
        Debug.Log(" XXXXXX After roll: " + PhotonNetwork.LocalPlayer.CustomProperties.ToStringFull());
        foreach(PlayerInfo player in playersInfoList)
        {
            if (player.turn == true) {
                Hashtable beginingHash = player.Player.CustomProperties;
                beginingHash["turn"] = true;
                player.Player.SetCustomProperties(beginingHash);
                Debug.Log(player.playerID.ToUpper() + "MOVES FIRST");
            }   
        }

       

        StartCoroutine(CheckPlayersReadyToPlay());
    }
    private IEnumerator CheckPlayersReadyToPlay()
    {
        bool allPlayersReady = false;
        float time = 10;
        while(PhotonNetwork.PlayerList.Length == 2 && time > 0 && !allPlayersReady)
        {
            Hashtable playerHash = PhotonNetwork.LocalPlayer.CustomProperties;
            if (playerHash.ContainsKey("ready"))
            {
                playerHash["ready"] = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);
            }
            bool localPlayerReady = (bool)PhotonNetwork.LocalPlayer.CustomProperties["ready"];
            bool secondPlayerReady = (bool)PhotonNetwork.PlayerListOthers[0].CustomProperties["ready"];
            time -= 1;
            if(localPlayerReady && secondPlayerReady)
            {
                allPlayersReady = true;
                time = 0;
                Debug.Log("All players ready!");
            }
            yield return new WaitForSeconds(1);
        }

        if (!allPlayersReady || PhotonNetwork.PlayerList.Length != 2)
        {
            string message = "there most be an error with the other player, going back to lobby";
            Debug.Log(message);
            StartCoroutine(LeaveToMenu());
        } else
        {
            Debug.Log("BEGIN!");

            ui.MainTimerVoid(3);

            while (!ui.ready)
            {
                yield return null;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                //SEND EVENT DO SOMETHING
                Hashtable startGameHash = new Hashtable();
                startGameHash.Add("start", true);

                PhotonNetwork.CurrentRoom.SetCustomProperties(startGameHash);
                StopCoroutine(CheckPlayersReadyToPlay());
            }

            Debug.Log("XXXXXXX ROOM PROPS: " + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());
            StopCoroutine(CheckPlayersReadyToPlay());
        }
    }

    #endregion Starters


    #region update room
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if(!gameBegan)
            Debug.Log("Room props changed: " + propertiesThatChanged.ToStringFull());

        if (propertiesThatChanged.ContainsKey("start") && !gameBegan)
        {
            
            //turnManager.InitializeTurnManager();
            Debug.Log("room properties summary: " + propertiesThatChanged.ToStringFull());
            Debug.Log("0000000000 ROOM PROPS: " + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());
            Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
            hash.Remove("start");
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            Debug.Log("removed start");

            //GAME STARTED
            manager.GameStart();
            gameBegan = true;
        }
    }
    
    
    #endregion update room


    #region player update info
    public bool rolledDice;
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        Debug.Log("OGSC Player props >> " + targetPlayer.NickName + " " + changedProps.ToStringFull());

        if(targetPlayer == PhotonNetwork.LocalPlayer)
        {
            //POPULATE LOCAL PLAYER INFO
            Debug.Log("it was local that changed");
            localPlayerInfo.SetPlayerInfo(targetPlayer);

            //Display dice to select the first player
            if(!rolledDice && changedProps.ContainsKey("roll"))
            {
                ui.StartCoroutine(ui.RollingDice((int[])changedProps["roll"]));
                changedProps.Remove("roll");
                targetPlayer.SetCustomProperties(changedProps);
                rolledDice = true;
            }            
        } else
        {
            //POPULATE REMOTE PLAYER INFO
            Debug.Log("it was remote that changed");
            remotePlayerInfo.SetPlayerInfo(targetPlayer);

            //if (targetPlayer.CustomProperties.ContainsKey("rematch"))
            //{
            //    if ((bool)targetPlayer.CustomProperties["rematch"] && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["rematch"])
            //    {
            //        Debug.Log("Xxxxxxxx Other wants to rematch");
            //        StartCoroutine(OtherPlayerWantsToRematch());
            //    }
            //}
        }
    }

    #endregion player update info

    #region PUN Callbacks

    public override void OnDisconnected(DisconnectCause cause)
    {

        Debug.Log("OnDisconnected because of cause: "+ cause.ToString());
        base.OnDisconnected(cause);

        if (!goingOffline){

            GoingOffline("You lost connection");
            return;
        }

        //SceneManager.LoadScene("MainScene");


    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("One of the Players left the room");

        if (otherPlayer != PhotonNetwork.LocalPlayer)
        {
            Debug.Log("The other player was the one that left the room.");
        }

        StopAllCoroutines();
        GoingOffline("Your opponent left the session...");
        base.OnPlayerLeftRoom(otherPlayer);
    }

    #endregion PUN Callbacks


    #region Rematch
    public void OnFinishedGame()
    {
        StartCoroutine(HoldForOffline(30));
    }

    public void OnClickPlayAgain()
    {
        StopAllCoroutines();
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["rematch"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        ui.WaitForRematch();
        ui.acceptButton.gameObject.SetActive(false);
        
        StartCoroutine(WaitingForOtherToRematch(30));
        //StartCoroutine(RematchRoutine());
        //WAIT FOR OTHER PLAYER
    }

    /// <summary>
    /// Timer to leave the room
    /// </summary>
    /// <param name="timer">The time to wait</param>
    /// <returns></returns>
    IEnumerator HoldForOffline(float timer)
    {
        Debug.Log("HoldForOffline");
        while (timer > 0)
        {
            timer -= 1;
            if (!PhotonNetwork.InRoom)
                yield break;
            if (PhotonNetwork.PlayerListOthers[0] == null)
                timer = 0;
            if (PhotonNetwork.PlayerListOthers[0] != null && (bool)PhotonNetwork.PlayerListOthers[0].CustomProperties["rematch"])
                ui.confirmationText.text = "OPPONNENT WAITING FOR REMATCH";
            
            yield return new WaitForSeconds(1);
        }

        GoingOffline("Leaving to main menu");
    }
    /// <summary>
    /// Routine for waiting the other player for rematch
    /// </summary>
    /// <param name="timer"> the time to wait if for other player</param>
    /// <returns></returns>
    IEnumerator WaitingForOtherToRematch(float timer)
    {
        Debug.Log("WaitingForOtherToRematch");
        ui.confirmationText.text = "WAITING FOR REMATCH";
        while (timer > 0)
        {
            timer -= 1;
            if (PhotonNetwork.PlayerList[0] == null)
                timer = 0;

            if(PhotonNetwork.PlayerListOthers[0] != null && (bool)PhotonNetwork.PlayerListOthers[0].CustomProperties["rematch"])
            {
                StartCoroutine(HoldForRematchStart());
                
                //StartCoroutine(HoldForRematchStart());
                yield break;
            }
            
            
            yield return new WaitForSeconds(1);
        }
        GoingOffline("Your opponent left...");
    }

    IEnumerator HoldForRematchStart()
    {
        Debug.Log("HoldForRematch");
        ui.confirmationText.text = "REMATCH!";
        ui.quitButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        ResetForNewGame();

    }

    #endregion Rematch



    #region Exits
    public void ExitOnlineGame()
    {
        Debug.Log("Attempting to exit Online Game");
        StopAllCoroutines();
        StartCoroutine(LeaveToMenu());
    }

    private IEnumerator LeaveToMenu()
    {
        goingOffline = true;
        Debug.Log("LeaveToMenu Routine");
        
        if (PhotonNetwork.InRoom){
            PhotonNetwork.LocalPlayer.CustomProperties.Clear();
            PhotonNetwork.LeaveRoom();
        }
        while (PhotonNetwork.InRoom)
        {
            Debug.Log("STILL IN ROOM WHILE ATTEMPTING TO LEAVE!");
            yield return null;
        }
        Debug.Log("Left Room");
        //yield return new WaitForSeconds(1); //3
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("Still in Lobby. Attempting to leave.");
            PhotonNetwork.LeaveLobby();
        }
        while (PhotonNetwork.InLobby)
        {
            Debug.Log("STILL IN LOBBY WHILE ATTEMPTING TO LEAVE!");
            yield return null;
        }
        Debug.Log("Left Lobby");

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Still connected. Attempting to disconnect.");
            PhotonNetwork.Disconnect();

        }
        while (PhotonNetwork.IsConnected)
        {
            Debug.Log("STILL CONNECTED WHILE ATTEMPTING TO LEAVE!");

            yield return null;
        }
        Debug.Log("Disconnected from Photon. Attempting to load main scene.");

        SceneManager.LoadScene("OnlineLobbyScene");
        Debug.Log("You should be in Main Scene.");

    }

    //private IEnumerator LeaveRoomRoutine(string message, float timer)       //CALLED WHEN FEW PLAYERS OR FAILED TO CREATE ROOM
    //{

    //    //Turn on ADDS
    //    goingOffline = true;
    //    Debug.Log("Leave room routine");
    //    Debug.Log(message);
    //    yield return new WaitForSeconds(timer);
    //    PhotonNetwork.LeaveRoom();
    //    PhotonNetwork.LeaveLobby();
    //    yield return new WaitForSeconds(1);
    //    PhotonNetwork.Disconnect();
    //    //PhotonNetwork.LoadLevel("MainScene");

    //}
    //public void OnClickQuit()
    //{
    //    StartCoroutine(LeaveToMenu());
    //}


    #endregion Exits

    #region Offline

    [Header("Offline")]
    public GameObject offlinePanel;
    public Text offlineMessage;


    void GoingOffline(string message)
    {
        StopAllCoroutines();
        StartCoroutine(OfflineRoutine(message));
    }
    bool goingOffline;
    IEnumerator OfflineRoutine(string message)
    {
        
        Debug.Log("OfflineRoutine");
        manager.PlaySound(manager.errorClip);
        offlinePanel.SetActive(true);
        offlineMessage.text = message;
        yield return new WaitForSeconds(3);
        StartCoroutine(LeaveToMenu());
    }
    #endregion Offline
}

