using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

///using UnityEditor.VersionControl;

public class OnlineMenuController : MonoBehaviourPunCallbacks
{

    [Header("Main")]
    [SerializeField] Text userIDText;
    public static string userID;
    public Text connectingText;
    public Text usersOnline;
    public GameObject onlineMenu;
    [SerializeField] GameObject searchPanel;
    [SerializeField] GameObject exitSearchButton;
    [SerializeField] Text searchingType;
    [SerializeField] Text searchingLog;
    [SerializeField] GameObject offlinePanel;
    [SerializeField] Text offlineMessage;
    [SerializeField] GameObject blockPanel;

    


    [Header("ID Generator")]
    [SerializeField]
    private string[] words;
    [SerializeField]
    private string[] title;
    private int i1, i2, i3;


    #region OnlineMenuController

    private static bool hasNickname;

    private static OnlineMenuController instance;
    internal static OnlineMenuController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<OnlineMenuController>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "PhotonMono";
                    instance = obj.AddComponent<OnlineMenuController>();
                }
            }

            return instance;
        }
    }

    public void Awake()
    {

        if (instance == null || ReferenceEquals(this, instance))
        {
            instance = this;
            
        }
        else
        {
            Destroy(this);
        }

        Debug.Log("AWAKE!!!");
        PhotonNetwork.AutomaticallySyncScene = true;


        Debug.Log("Attempting to Start Connection");
        StartCoroutine(StartConnection());
        //
        //0DontDestroyOnLoad(this.gameObject);

    }

    #endregion

    #region Connections


    //public void Connect()
    //{

    //    Debug.Log("Click connect");
    //    StartCoroutine(StartConnection());     
    //    //messageStarter.messageUserID = userID;
    //    //messageStarter.enabled = true;

    //}

        
    private IEnumerator StartConnection()
    {
        Debug.Log("StartConnectionRoutine");
        blockPanel.SetActive(true);

        ResetOnlineMenuMenus();
        connectingText.text = "Connecting";
        connectingText.gameObject.SetActive(true);

        

        //IDs
        if (!OnlineMenuController.hasNickname)
        {
            
            OnlineMenuController.userID = GeneratedID();
            userID = OnlineMenuController.userID;
            OnlineMenuController.hasNickname = true;
        }else
        {
            userID = OnlineMenuController.userID;
        }
        
            
        usersOnline.text = PhotonNetwork.CountOfPlayers.ToString();
        PlayerPrefs.SetString("NickName", userID);
        PhotonNetwork.ConnectUsingSettings();
        
               

        float timer = 5;
        while (!PhotonNetwork.IsConnectedAndReady && timer > 0)
        {
            if (connectingText.text == "Connecting...")
                connectingText.text = "Connecting";

            connectingText.text += ".";
            


            yield return new WaitForSeconds(1);
        }
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            connectingText.text = "Failed to connect";
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("MainScene");
            StopCoroutine(StartConnection());
            this.gameObject.SetActive(false);
        }
        //
        messager.ConnectChat(userID);
        blockPanel.SetActive(false);
    }

    /// <summary>
    /// Connects to the chat
    /// </summary>
    private void ConnectToChat()
    {
        Debug.Log("ConnectToChat");
    }
    /// <summary>
    /// Generates IDs for users
    /// </summary>
    /// <returns><c>string</c> The string ID for users to use</returns>
   #endregion Connections

    #region PunCallbacks
    public override void OnConnected()
    {
        Debug.Log("OnConnected");
        //connectingText.enabled = true;
        usersOnline.text = PhotonNetwork.CountOfPlayers.ToString();
        roomListings = new List<RoomInfo>();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.NickName = userID;
        PhotonNetwork.AuthValues.UserId = userID;
        usersOnline.text = PhotonNetwork.CountOfPlayers.ToString();
        userIDText.text = "<color=#E07B00>" + userID + "</color> ";
        
        onlineMenu.SetActive(true);
        //Maybe run if user is not in lobby
        StartCoroutine(UpdatePlayers());
        if (!PhotonNetwork.InLobby)
            AutoJoinLoby();
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        Debug.Log((string)PhotonNetwork.CurrentRoom.CustomProperties["type"]);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("invitation"))
        {
            SendInvite();
        } else
        {
            string type = (string)PhotonNetwork.CurrentRoom.CustomProperties["type"];
            searchingType.text = type.ToUpper();
        }
    }
    private int searchIndex;
    private IEnumerator WaitToJoinRandom()
    {

        StartCoroutine(SearchingLog("any"));
        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinRandomRoom();

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        searchIndex += 1;
        if(searchIndex < 8)
        {
            StopAllCoroutines();
            StartCoroutine(WaitToJoinRandom());

        } else
        {
            
            Debug.Log("OnJoinRandomFailed");
            int i = Random.Range(0, 2);
            if (i == 0)
            {
                CreateRoomOnJoinFailed("single");
            }
            else
            {
                CreateRoomOnJoinFailed("match");
            }
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        
        Debug.Log("OnJoinRoomFailed() Initialized");
        //if (PhotonNetwork.PlayerList.Length != 2)
        //{
        StartCoroutine(CancelInvite("Seems like the other player left"));
        //return;
           // }
        //}
    }
    //function for createing rooms
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("tried to create a room but failed, there must be a room with the same name, reconnect and try again");
    }

    private List<RoomInfo> otherList;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        otherList = roomList;

        


        Debug.Log("xxxxxOnRoomListUpdate" + otherList.Count);
        int temptIndex;
        foreach (RoomInfo room in roomList)
        {
            if (roomList != null)
            {
                temptIndex = roomListings.FindIndex(ByName(room.Name));
            }
            else
            {
                temptIndex = -1;
            }
            if (temptIndex != -1)
            {
                Debug.Log("removed room");
                roomListings.RemoveAt(temptIndex);
            }
            if (room.PlayerCount > 0)
            {
                Debug.Log("added room");
                roomListings.Add(room);
            }
        }
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        Debug.Log("Disable");
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        //OPEN OFFLINE PANNEL
        Debug.Log(cause.ToString());
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PhotonNetwork.InRoom)
            return;
        if (!PhotonNetwork.IsConnectedAndReady)
            return;
        usersOnline.text = PhotonNetwork.CountOfPlayers.ToString();
        Debug.Log("chagned: " +targetPlayer.NickName + " " + changedProps.ToStringFull());
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }
    #endregion PunCallbacks

    #region Lobby

    [Header("Lobby")]
    [SerializeField] GameObject lobbyPanel;
    public Button singleRoomButton;
    public Button matchRoomButton;
    public Button anyRoomButton;
    public Button inviteButton;

    /// <summary>
    /// Connects to the lobby in the game
    /// </summary>
    private void AutoJoinLoby()
    {
        Debug.Log("AutoConnectToLobby");
        //PhotonNetwork.AutomaticallySyncScene = true;
        lobbyPanel.SetActive(true);
        //CONNECT TO CHAT;
        usersOnline.text = PhotonNetwork.CountOfPlayers.ToString();
        PhotonNetwork.JoinLobby();
    }

    private void CreateRoomOnJoinFailed(string type)
    {
        Debug.Log("CreateRoomOnJoinFailed>" + type);

        Hashtable hash = new Hashtable();
        hash.Add("type", type);

        RoomOptions roomOps = new RoomOptions();
        string[] ops = { "type" };
        roomOps.CustomRoomPropertiesForLobby = ops;
        roomOps.CustomRoomProperties = new Hashtable { { "type", type } };

        roomOps.MaxPlayers = 2;
        roomOps.BroadcastPropsChangeToAll = true;
        roomOps.IsOpen = true;
        roomOps.IsVisible = true;
        roomOps.EmptyRoomTtl = 0;

        if (PhotonNetwork.IsConnectedAndReady  && !PhotonNetwork.InRoom)
        {
            attempt = 0;
            PhotonNetwork.CreateRoom(null, roomOps);
        }
    }

    public void OnClickCreateInvitationRoom(string type)
    {
        
        Debug.Log("OnClickCreateInvitationRoom>" + type);

        Hashtable hash = new Hashtable();
        hash.Add("type", type);
        hash.Add("invitation", true);

        RoomOptions roomOps = new RoomOptions();
        roomOps.CustomRoomProperties = hash;
        roomOps.MaxPlayers = 2;
        roomOps.BroadcastPropsChangeToAll = true;
        roomOps.IsOpen = true;
        roomOps.IsVisible = false;
        roomOps.EmptyRoomTtl = 0;

        if(PhotonNetwork.IsConnectedAndReady  && !PhotonNetwork.InRoom)
        {
            attempt = 0;
            PhotonNetwork.CreateRoom(null, roomOps);
        }
    }

    public void OnClickJoinInvitation(string roomName)
    {
        Debug.Log("OnClickJoinInvitation Initialized");
        Debug.Log("Is Connected: " + PhotonNetwork.IsConnectedAndReady);
        Debug.Log("In Room: " + PhotonNetwork.InRoom);
        Debug.Log("In Lobby: " + PhotonNetwork.InLobby);

        StopAllCoroutines();
        //if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom )//&& PhotonNetwork.InLobby)
        //{
        //    searchPanel.SetActive(false);
        //    Debug.Log("Im connected to everything.");
        //    roomPanel.SetActive(true);
        //    PhotonNetwork.JoinRoom(roomName);


        //}
        StartCoroutine(JoinInvitationRoutine(roomName));
        
    }

    private IEnumerator JoinInvitationRoutine(string roomName)
    {
        Debug.Log("JoinInvitationRoutine");
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        //while (PhotonNetwork.InRoom)
        //{
        //    yield return null;
        //}
        //yield return StartCoroutine(WaitToConnect());

        Debug.Log("left room");
        //float timer =0;
        while (!PhotonNetwork.InLobby)
        //while(timer < 3)
        {
          //  timer += Time.deltaTime;
            Debug.Log("not in lobby... waiting");
            yield return new WaitForSeconds(1);
            //yield return null;
        }
        
       
        Debug.Log("Is Connected: " + PhotonNetwork.IsConnectedAndReady);
        Debug.Log("In Room: " + PhotonNetwork.InRoom);

        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom)//&& PhotonNetwork.InLobby)
        {
            searchPanel.SetActive(false);
            Debug.Log("Im connected to everything.");
            roomPanel.SetActive(true);
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void OnClickSearchForRoomOfType(string type)
    {
        Debug.Log("OnClickSearchRoomOfType>" + type);
        attempt = 0;
        
        StartCoroutine(SearchingLog(type));
        List<RoomInfo> tempRoomList = new List<RoomInfo>();
        RoomInfo selectedRoom = null;

        foreach (RoomInfo room in roomListings)
        {
            Debug.Log("cheking room:" + (string)room.CustomProperties["type"] + room.IsOpen + room.IsVisible);
            if (room.CustomProperties.ContainsValue(type) && room.IsOpen && room.IsVisible)
            {
                Debug.Log("selected room");
                selectedRoom = room;
            }
        }

        if (selectedRoom != null)
        {
            Debug.Log("Join room");
            PhotonNetwork.JoinRoom(selectedRoom.Name);
        }
        else
        {
            Debug.Log("create room");
            CreateRoomOnJoinFailed(type);
        }

    }
    
    public void OnClickJoinAnyRoom()
    {
        Debug.Log("OnClickJoinAny");
        attempt = 0;
        searchIndex = 0;
        Debug.Log("OnClickJoinAnyRoom");
        StartCoroutine(SearchingLog("any"));
        PhotonNetwork.JoinRandomRoom();
    }

    //predicate function to go through room list
    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }



    public void LeaveLobbyButton()
    {
        Debug.Log("LeaveRoomButton");
        ResetLobbyMenu();
        ResetRoomMenu();
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    #endregion Lobby

    #region Room

    [Header("Rooms")]
    [SerializeField] GameObject roomPanel;
    [SerializeField] GameObject opponentPanel;
    [SerializeField] GameObject exitOpponentFoundButton;
    [SerializeField] GameObject timeOutPanel;
    [SerializeField] GameObject exitTimeOutButton;
    [SerializeField] Text timeOutText;
    [SerializeField] Text opponenName;
    [SerializeField] Text roomTypeText;
    [SerializeField] Text timerText;
    [SerializeField] Text opponentInfoText;
    [SerializeField] Button acceptButton;
    
    private int roomSize = 2;
    List<RoomInfo> roomListings = new List<RoomInfo>();

    public override void OnJoinedRoom()
    {


        Debug.Log("OnJoinedRoom");
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("type")) {
            StartCoroutine("Failed to Join BackToLobby");
            return;
        }

        if (PhotonNetwork.PlayerList.Length == 1 && !invite && !opponentFound && !searching)
        {
            Debug.Log("insufficient players");
            StartCoroutine(SearchingLog((string)PhotonNetwork.CurrentRoom.CustomProperties["type"]));
        }

        if (PhotonNetwork.PlayerList.Length == 2)
            StartCoroutine(OpponentFoundRoutine());
                 
        

        roomTypeText.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["type"];
        roomPanel.SetActive(true);
  
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            foreach(Player player in PhotonNetwork.PlayerList)
            {
                player.CustomProperties.Clear();
                Hashtable playerHash = new Hashtable();
                playerHash["ready"] = false;
                player.SetCustomProperties(playerHash); // BETTER SETUP ON JOIN TO ROOM
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("invitation"))
            {
                OnPlayerAcceptInvite();
            }
            else
            {
                StartCoroutine(OpponentFoundRoutine());
            }
            
        }

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom");
        opponentFound = false;
        
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("invitation"))
        {
            StartCoroutine(TimeOutRoutine("invitation", null));
        }
        else
        {
            Debug.Log("Doesnt contain invitation " + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());
        }

    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        //base.OnLeftRoom();
        opponentFound = false;
        searching = false;
        invite = false;
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
    }


    bool searching;

    private void OnPlayerAcceptInvite()
    {
        Debug.Log("OnPlayerAcceptedInvite");
        invite = false;
        searching = false;
        opponentFound = true;
        blockPanel.SetActive(true);
        StopAllCoroutines();
        StartGame();
    }

    private IEnumerator OpponentFoundRoutine()
    {
        Debug.Log("OpponentFoundRoutine");
        invite = false;
        searching = false;
        opponentFound = true;
        StopCoroutine(SearchingLog((string)PhotonNetwork.CurrentRoom.CustomProperties["type"]));
        exitOpponentFoundButton.SetActive(true);
        timeOutPanel.SetActive(false);
        invitationPanel.SetActive(false);
        searchPanel.SetActive(false);
        yield return new WaitForSeconds(1);
        FindObjectOfType<CanvasController>().SoundAlert2();
        
        
        
        
        roomPanel.SetActive(true);
        opponentPanel.SetActive(true);

        roomTypeText.text = "Type: " + (string)PhotonNetwork.CurrentRoom.CustomProperties["type"];
        opponenName.text = PhotonNetwork.PlayerListOthers[0].NickName;
        acceptButton.gameObject.SetActive(true);
        acceptButton.interactable = true;

        acceptButton.GetComponentInChildren<Text>().text = "ACCEPT";
        opponentInfoText.text = "Waiting for other player to accept...";


//        Debug.Log("local" + (bool)PhotonNetwork.LocalPlayer.CustomProperties["ready"]);
        //Debug.Log("local " + (bool)PhotonNetwork.LocalPlayer.CustomProperties["ready"]);
        //Debug.Log("remote " + (bool)PhotonNetwork.PlayerListOthers[0].CustomProperties["ready"]);


        string type = (string)PhotonNetwork.CurrentRoom.CustomProperties["type"];
        float timeLimit = 20;

        timerText.text = timeLimit.ToString("F2") + " s";
        bool allPlayersReady = false;
        bool localReady = false;
        bool remoteReady = false;

        
        while (PhotonNetwork.PlayerList.Length == 2 && timeLimit > 0)
        {
            localReady = (bool)PhotonNetwork.LocalPlayer.CustomProperties["ready"];
            remoteReady = (bool)PhotonNetwork.PlayerListOthers[0].CustomProperties["ready"];
            timeLimit -= 1;

            if (localReady && remoteReady)
            {
                allPlayersReady = true;
                timeLimit = 0;
                opponentInfoText.text = "All players are ready!";
            }
            timerText.text = timeLimit.ToString("F2") + " s";
            yield return new WaitForSeconds(1);
            FindObjectOfType<CanvasController>().SoundTick();
        }
        //Debug.Log("check local " + (bool)PhotonNetwork.LocalPlayer.CustomProperties["ready"]);
        //Debug.Log("check remote " + (bool)PhotonNetwork.PlayerListOthers[0].CustomProperties["ready"]);

        if (PhotonNetwork.PlayerList.Length != 2 && !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("invitation"))
        {
            Debug.Log("Missing one player");
            StartCoroutine(TimeOutRoutine("missing", type));
            StopCoroutine(OpponentFoundRoutine());
            yield break;
        }else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("invitation"))
        {
            StartCoroutine(TimeOutRoutine("invitation", type));
            StopCoroutine(OpponentFoundRoutine());
            yield break;
        }
        else if (localReady && remoteReady)
        {
            StartGame();
            StopCoroutine(OpponentFoundRoutine());
            yield break;
        }
        else
        {
            Debug.Log("!!!!!SOMETHING HAPPENED");

            if (!localReady && !remoteReady)
                StartCoroutine(TimeOutRoutine("check", type));
            if (localReady && !remoteReady)
                StartCoroutine(TimeOutRoutine("remote", type));
            if (!localReady && remoteReady)
                StartCoroutine(TimeOutRoutine("local", type));
            StopCoroutine(OpponentFoundRoutine());
            yield break;
        }

    }
    bool loading;
    int attempt;
    private IEnumerator TimeOutRoutine(string condition, string type)
    {
        Debug.Log("TimeOutRoutine Coroutine Initialized");
        exitTimeOutButton.SetActive(true);
        opponentPanel.SetActive(false);
        Debug.Log("Turned Off Panell");
        searchPanel.SetActive(false);
        timeOutPanel.SetActive(true);

        int outro = 0;


        if (condition == "check" && attempt == 0)
        {
            Debug.Log("Condition Check");
            attempt += 1;
            timeOutText.text = "Hey! you must click 'ACCEPT' to join";
            outro = 3;
            yield return new WaitForSeconds(outro);
            StartCoroutine(OpponentFoundRoutine());
            StopCoroutine(TimeOutRoutine(condition, type));
            yield break;
        } else if (attempt > 0 && condition == "check")
        {
            Debug.Log("Condition Second attempt");
            attempt = 0;
            PhotonNetwork.LeaveRoom();
            opponentFound = false;
            timeOutText.text = "Sorry, you don't seem to be around." +
                "\nThat's ok, get back in the queue when you're ready.";
            outro = 3;
            yield return new WaitForSeconds(outro);
            ExitSearch();
            StartCoroutine(RejoinLobby());
            StopCoroutine(TimeOutRoutine(condition, type));
            yield break;
        }
        if (condition == "local")
        {
            Debug.Log("Condition Local");
            PhotonNetwork.LeaveRoom();
            timeOutText.text = "Sorry, you don't seem to be around." +
                "\nThat's ok, get back in the queue when you're ready.";
            outro = 4;
            opponentFound = false;
            yield return new WaitForSeconds(outro);
            ExitSearch();
            StartCoroutine(RejoinLobby());
            StopCoroutine(TimeOutRoutine(condition, type));
            yield break;
        }
        if (condition == "remote")
        {
            Debug.Log("Condition Remote");
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("invitation"))
                {
                    Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
                    hash.Remove("invitation");
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                    PhotonNetwork.CurrentRoom.IsVisible = true;
                } else
                {
                    Debug.Log("Doesnt contain invitation " + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());
                }
            }
            timeOutText.text = "The other player didn’t answer in time." +
                "\nYou've been placed back in the queue";
            outro = 3;
            opponentFound = false;
            yield return new WaitForSeconds(outro);
            if(!searching)
                StartCoroutine(SearchingLog(type));
            Debug.Log("StopCoroutine");
            StopCoroutine(TimeOutRoutine(condition, type));
            Debug.Log("yield break");
            yield break;
            Debug.Log("broke");
        }
        if (condition == "missing")
        {
            Debug.Log("Condition Missing");

            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("invitation"))
                {
                    Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
                    hash.Remove("invitation");
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                    PhotonNetwork.CurrentRoom.IsVisible = true;
                }
                else
                {
                    Debug.Log("Doesnt contain invitation " + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());
                }
            }

            timeOutText.text = "Sorry, the other player is missing." +
                "\nYou've been placed back in the queue";
            outro = 2;
            opponentFound = false;
            yield return new WaitForSeconds(outro);
            if (!searching)
                StartCoroutine(SearchingLog(type));
            StopCoroutine(TimeOutRoutine(condition, type));
            yield break;
        }
        if(condition == "invitation")
        {
            //PhotonNetwork.LeaveRoom();
            Debug.Log("Condition Invitation");
            attempt = 0;
            opponentFound = false;
            timeOutText.text = "Sorry, the other player is missing or left";
            outro = 3;
            yield return new WaitForSeconds(outro);
            ExitSearch();
            StartCoroutine(RejoinLobby());
            StopCoroutine(TimeOutRoutine(condition, type));
            yield break;
        }
    }

    private IEnumerator SearchingLog(string room)
    {
        Debug.Log("SearchingLog Coroutine Initialized");
        searching = true;
        
        timeOutPanel.SetActive(false);
        exitSearchButton.SetActive(true);
        acceptButton.enabled = true;
        acceptButton.interactable = true;
        searchPanel.SetActive(true);
        Debug.Log("Set panels");
        if (room == "single")
        {
            searchingType.text = "(SINGLE GAME)";
        } else if (room == "match")
        {
            searchingType.text = "(MATCH - 4 GAMES)";
        } else
        {
            searchingType.text = "ANY";
        }
        Debug.Log("set names");
        searchingLog.text = "";
        if (searchPanel.activeSelf)
        {
            searchingLog.text = "";
            while (PhotonNetwork.PlayerList.Length < 2 || !PhotonNetwork.InRoom)
            {
                if (searchingLog.text == "...")
                    searchingLog.text = "";

                searchingLog.text += ".";

                yield return new WaitForSeconds(1);
            }
            Debug.Log(PhotonNetwork.PlayerList.Length + " " + PhotonNetwork.InRoom);
        }
        StopCoroutine(SearchingLog(room));
        searchPanel.SetActive(false);
        yield break;
    }


    private IEnumerator RejoinLobby()
    {
        Debug.Log("RejoinLobby Coroutine Initialized");
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        
        
        while (PhotonNetwork.InRoom)
        {
            Debug.Log("leaving room...");
            yield return null;
        }
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
        }
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
        yield return new WaitForSeconds(1);
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ResetRoomMenu();
        lobbyPanel.SetActive(true);
    }


    public void OnClickPlayerReady()
    {
        Debug.Log("OnClickPlayerReady");
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("ready"))
        {
            Hashtable playerHash = PhotonNetwork.LocalPlayer.CustomProperties;
            playerHash["ready"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);

            acceptButton.interactable = false;
            acceptButton.GetComponentInChildren<Text>().text = "READY";
        }
    }
    public void StartGame()
    {
        Debug.Log("StartGame");
        //PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsMasterClient && !loading)
        {
            loading = true;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("OnlineGameScene");
           
        }
        this.gameObject.SetActive(false);
        this.OnDisable();
        //SceneManager.LoadScene("OnlineGameScene");
    }
    #endregion Room

    #region Messager



    [Header("MESSAGE")]
    [SerializeField] Messager messager;
    [SerializeField] private GameObject invitationPanel;
    [SerializeField] private GameObject exitInvitationButton;
    [SerializeField] private GameObject rejectedPanel;

    [Header("Invite")]
    public InputField inputField;
    public Text inviteInputText;
    public Text invitingPlayerText;
    public Text inviteInfoText;
    private string friendID;
    public Button singleInvitation;
    public Button matchInvitation;
    public Button singleSendInvitation;
    public Button matchSendInvitation;
    public Text waitingForOtherText;

    [Header ("Popup")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Text popupText;
    [SerializeField] private Text popupTimer;
    [SerializeField] private Text popupType;
    [SerializeField] private Text popupInfo;
    public Button popupAcept;
    public Button popupDecline;

    


    public void OnClickSearchUser(InputField input)
    {
        Debug.Log("OnClickSearchUser");
        friendID = input.text;
        exitInvitationButton.SetActive(true);
        //inviteButton.enabled = false;
        //Debug.Log("text component" + input.textComponent);
        //Debug.Log("text" + input.text);
        //Debug.Log("check" + friendID);

        if (!messager.CheckIfUserExist(friendID))
        {
            StartCoroutine(FailedToFindUser());
        }else
        {
            OpenInvitationPanel();
        }
    }

    private void OpenInvitationPanel()
    {
        Debug.Log("OpenInvitationPanel");
        ResetInvitingMenu();
        invitationPanel.SetActive(true);
        invitingPlayerText.text = "<b>INVITING </b>" + friendID;
    }
    bool invite;
    public void SendInvite()
    {
        Debug.Log("SendInvite");
        if (!invite)
        {
            Debug.Log("SendInvite Initialized (!invite)");
            waitingForOtherText.gameObject.SetActive(true);
            string roomName = PhotonNetwork.CurrentRoom.Name;
            string type = (string)PhotonNetwork.CurrentRoom.CustomProperties["type"];
            messager.SendInvitationForRoom(roomName, type, friendID);
            invite = true;
            opponentFound = false;
            StartCoroutine(WaitForAccept());
        }

    }

    public void SendReject(string userID)
    {
        Debug.Log("SendRejected");
        messager.SendReject(userID);
        popupAcept.onClick.RemoveAllListeners();
        popupDecline.onClick.RemoveAllListeners();
        invite = false;
    }

    private bool opponentFound;
    private IEnumerator WaitForAccept()
    {
        Debug.Log("WaitForAcceptRoutine");
        float timer = 15f;

        while (timer > 0 && !opponentFound)
        {
            timer -= 1;
            yield return new WaitForSeconds(1);
        }

        if (opponentFound)
        {
            Debug.Log("opponent found from waiting");
            StartCoroutine(OpponentFoundRoutine());
            yield break; 
        }
        invitationPanel.SetActive(false);
        timeOutPanel.SetActive(true);
        timeOutText.text = "Looks like your friend is not around";
        yield return new WaitForSeconds(2);
        timeOutPanel.SetActive(false);
        
        ExitSearch(); 

    }

    private IEnumerator FailedToFindUser()
    {
        Debug.Log("FailedToFindUser");
        inputField.enabled = false; //To reset the search user by username input field
        inputField.text = "";
        inviteInputText.text = "Find by nickname...";
        inviteInfoText.text = "Nickname not found";
        yield return new WaitForSeconds(2);
        
        
        inputField.enabled = true;
        inviteInfoText.text = "Users' nicknames are case sensitive";
        inviteButton.enabled = true;
    }
    


    public void OnInvitationRejected()
    {
        Debug.Log("OnInvitationRejected");
        StartCoroutine(RejectedRoutine());
    }

    private IEnumerator RejectedRoutine()
    {
        Debug.Log("RejectedRoutine");
        if (invite)
        {
            FindObjectOfType<CanvasController>().SoundReject();
            rejectedPanel.SetActive(true);
            
            yield return new WaitForSeconds(2);
            rejectedPanel.SetActive(false);
            ResetInvitingMenu();
            ExitSearch();

        }
        
    }

    string tempInvitationRoomName;
    string tempSenderID;

    public void PopupStarter(string room, string type, string sender)
    {
        Debug.Log("popup");
        if (!invite)
        {
            tempInvitationRoomName = room;
            tempSenderID = sender;
            invite = true;
            StartCoroutine(PopupMessageRoutine(room, type, sender));
        }
    }

    public void OnClickPopupAccept()
    {
        Debug.Log("OnClickPopupAccet");
        StopAllCoroutines();

        OnClickJoinInvitation(tempInvitationRoomName);
        invite = false;
        
    }
    public void OnClickPopupDecline()
    {
        Debug.Log("OnClickPopupDecline");
        SendReject(tempSenderID);
        invite = false;
        StopAllCoroutines();
        tempSenderID = null;
    }



    private IEnumerator PopupMessageRoutine(string room, string type, string senderID)
    {
        FindObjectOfType<CanvasController>().SoundAlert();
        Debug.Log("POPUP ROUTINE");
        invite = true;
        popupPanel.gameObject.SetActive(true);
        popupText.text = "INVITATION FROM: " + senderID;
        popupInfo.text = "to join a";

        popupType.text = type;
        if(type == "single")
        {
            popupType.text = "(SINGLE GAME)";
        } else
        {
            popupType.text = "(MATCH - 4 GAMES)";
        }
        

        int time = 10;

        //PLAY ALERT SOUNDS
        //SETUP BUTTONS
        popupAcept.gameObject.SetActive(true);
        popupDecline.gameObject.SetActive(true);

        //popupAcept.onClick.AddListener(delegate () { OnClickJoinInvitation(room); });

        //popupAcept.onClick.AddListener(delegate () { StopCoroutine(PopupMessageRoutine(room, type, senderID)); });

        //popupDecline.onClick.AddListener(delegate () { SendReject(senderID); });
        //popupDecline.onClick.AddListener(delegate () { StopCoroutine(PopupMessageRoutine(room, type, senderID)); });
        

        while (time > 0)
        {

            time -= 1;
            popupTimer.text = time.ToString();
            yield return new WaitForSeconds(1);
        }

        popupTimer.text = "0.0";

        popupText.text = "<b>TIMEOUT</b>";
        popupType.text = "Invitation time expired";
        popupInfo.text = "";

        //popupAcept.onClick.RemoveAllListeners();
        //popupDecline.onClick.RemoveAllListeners();
        invite = false;

        yield return new WaitForSeconds(3.5f);

        
        popupPanel.gameObject.SetActive(false);
        StopCoroutine(PopupMessageRoutine(room, type, senderID));
    }
    #endregion Messager





    #region Resets

    /// <summary>
    /// Resets all elements in the OnlineMenu object
    /// </summary>
    public void ResetOnlineMenuMenus()
    {
        Debug.Log("ResetOnlineMenu");
        connectingText.gameObject.SetActive(false);
        usersOnline.text = "...";


        

        onlineMenu.SetActive(false);
        ResetLobbyMenu();
        ResetRoomMenu();
        ResetInvitingMenu();
    }


    public void ResetLobbyMenu()
    {
        Debug.Log("ResetLobbyMenu");
        lobbyPanel.SetActive(true);
        singleRoomButton.gameObject.SetActive(true);
        matchRoomButton.gameObject.SetActive(true);
        anyRoomButton.gameObject.SetActive(true);
        inputField.gameObject.SetActive(true);

        inviteInfoText.text = "Users' nicknames are case sensitive";
        inviteInputText.text = "Filter by nickname";
        inviteButton.GetComponentInChildren<Text>().text = "Search";
        inviteButton.interactable = true;
        invitationPanel.SetActive(false);
        
    }

    public void ResetRoomMenu()
    {
        Debug.Log("ResetRoomMenu");

        roomPanel.SetActive(false);
        opponentPanel.SetActive(false);
        timeOutPanel.SetActive(false);

        acceptButton.gameObject.SetActive(true);
        acceptButton.interactable = true;
        acceptButton.GetComponentInChildren<Text>().text = "ACCEPT";
    }

    public void ResetInvitingMenu()
    {
        Debug.Log("ResetInvitationMenu");
        //Panel
        invitationPanel.SetActive(false);
        rejectedPanel.SetActive(false);
        invitingPlayerText.text = "...";
        waitingForOtherText.text = "Waiting for other player to accept...";
        waitingForOtherText.gameObject.SetActive(false);
        //Single
        singleInvitation.gameObject.SetActive(true);
        singleInvitation.interactable = true;
        singleInvitation.GetComponent<EventTrigger>().enabled = true;
        singleSendInvitation.interactable = true;
        singleSendInvitation.gameObject.SetActive(false);
        //Match
        matchInvitation.gameObject.SetActive(true);
        matchInvitation.interactable = true;
        matchInvitation.GetComponent<EventTrigger>().enabled = true;
        matchSendInvitation.interactable = true;
        matchSendInvitation.gameObject.SetActive(false);

    }
    public void OnClickBackToLocalOnlineMenu()
    {
        Debug.Log("OnClickBackToLocalOnlineMenu");
        ResetOnlineMenuMenus();
        messager.enabled = false;
        messager.DisconnectChat();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainScene");
        StopAllCoroutines();
    }

    public void ExitSearch()
    {
        Debug.Log("ExitSearch");
        StopAllCoroutines();
        
        acceptButton.enabled = true;
        acceptButton.interactable = true;
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        opponentPanel.SetActive(false);
        invitationPanel.SetActive(false);
        searchPanel.SetActive(false);
        inputField.enabled = false; //To reset the search user by username input field
        inputField.text = "";
        inviteInputText.text = "Find by nickname...";
        inputField.enabled = true;

        opponentFound = false;
        invite = false;

        
        StartCoroutine(WaitToConnect());

        

    }
    IEnumerator WaitToConnect()
    {
        Debug.Log("WaitToConnect");
        float timer = 10;
        while (!PhotonNetwork.IsConnectedAndReady && timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            OnClickBackToLocalOnlineMenu();
            StopCoroutine(WaitToConnect());
            yield break;
        }

        //if (!PhotonNetwork.InLobby)
        //{
        //    PhotonNetwork.JoinLobby();
        //}

        roomPanel.SetActive(false);
        ResetLobbyMenu();
        Debug.Log("finished connection");
    }

    
    IEnumerator UpdatePlayers()
    {
        while (PhotonNetwork.IsConnectedAndReady)
        {
            usersOnline.text = PhotonNetwork.CountOfPlayers.ToString();
            yield return new WaitForSeconds(1);
        }
        StopCoroutine(UpdatePlayers());
    }


    private IEnumerator LeaveToMenu()
    {
        Debug.Log("LeaveToMenu");
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LeaveRoom();
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
        SceneManager.LoadScene("MainScene");
        Debug.Log("You should be in Main Scene.");

    }

    private IEnumerator CancelInvite(string msg)
    {
        Debug.Log("CancelInvite() initialized");
        popupPanel.SetActive(false);
        rejectedPanel.SetActive(false);
        invitationPanel.SetActive(false);
        roomPanel.SetActive(true);
        timeOutPanel.SetActive(true);
        timeOutText.text = msg;
        yield return new WaitForSeconds(2);
        timeOutPanel.SetActive(false);
        roomPanel.SetActive(false);
        ExitSearch();

    }



        #endregion Resets

        #region Miscellaneous
        private string GeneratedID()
    {
        string ID;
        int location = Random.Range(0, 2);
        i1 = Random.Range(0, 10);
        i2 = Random.Range(0, 10);
        i3 = Random.Range(0, 10);

        if (location == 0)
        {
            ID = i1.ToString() + i2.ToString() + i3.ToString() + words[Random.Range(0, words.Length)] + title[Random.Range(0, title.Length)];
        }
        else
        {
            ID = words[Random.Range(0, words.Length)] + title[Random.Range(0, title.Length)] + i1.ToString() + i2.ToString() + i3.ToString();
        }
        return ID;
    }
    #endregion Miscellaneous
}
