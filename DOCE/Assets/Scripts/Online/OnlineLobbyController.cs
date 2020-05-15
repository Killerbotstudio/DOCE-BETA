using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.EventSystems;


public class OnlineLobbyController : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject lobbyConnectButton; //button used for joining a lobby
    [SerializeField]
    private GameObject lobbyPanel; //panel for lobby display
    [SerializeField]
    private GameObject mainPanel; //main menu panel

    [SerializeField]
    private Text playerID;

    private string roomName; //string for saving roomName;
    private int roomSize = 2;

    private List<RoomInfo> roomsListings; //list of current rooms
    [SerializeField]
    public Transform roomsContainer; //container for all roomsDisplay
    [SerializeField]
    private GameObject roomsListItem;

    [Header("Waiting Panel")]
    [SerializeField]
    private GameObject waitingPanel;
    [SerializeField]
    private Text waitingTimerText;
    [SerializeField]
    private Text waitingText;


    public EventTrigger singleButton;
    public EventTrigger matchButton;
    public EventTrigger anyButton;
    public Button inviteButton;

    [SerializeField]
    MatchmakingRoomController roomController;


    public void Update()
    {
        if(!PhotonNetwork.InLobby || !PhotonNetwork.IsConnectedAndReady)
        {
            singleButton.enabled = false;
            matchButton.enabled = false;
            anyButton.enabled = false;
            inviteButton.interactable = false;
        } else
        {
            singleButton.enabled = true;
            matchButton.enabled = true;
            anyButton.enabled = true;
            inviteButton.interactable = true;
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("lobby controller connected");
        PhotonNetwork.AutomaticallySyncScene = true; // make it so whatever shcen the master client....

        AutoConnectToLobby();

        roomsListings = new List<RoomInfo>(); // initialize lis
        //playerID.text = PhotonNetwork.AuthValues.UserId;
        string ID = PhotonNetwork.AuthValues.UserId;
        playerID.text = "<color=#E07B00>" + ID + "</color> ";

        
    }

    private void AutoConnectToLobby()
    {

        Debug.Log("auto connecting");
        lobbyConnectButton.SetActive(true);
        MessageControllerStarter starter = FindObjectOfType<MessageControllerStarter>();
        starter.StartMessager();
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("check1");
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        //roomProperties["type"] = "single";
        
        roomProperties.Add("type", "single");
        //room.SetCustomProperties()
        roomController.roomProperty = roomProperties;
        string roomType = (string)roomController.roomProperty["type"];

        Debug.Log(roomType);
    }
    public void OnClickCreateInvitationRoom()
    {
        StartCoroutine(WaitingTimer("Invitation sent"));
        //Debug.Log("creating a invitational " + type + " game room...");
        Debug.Log("creating an invitational game room...");

        RoomOptions roomOptions = new RoomOptions();
        string[] ops = { "type" };
        roomOptions.CustomRoomPropertiesForLobby = ops;

        string[] props = { "invitation" };

        //https://doc.photonengine.com/en-us/pun/current/lobby-and-matchmaking/matchmaking-and-lobby#matchmaking_checklist
        roomOptions.CustomRoomProperties = new Hashtable { { "type", "invitation" } };

        roomOptions.MaxPlayers = 2;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = false;
        roomOptions.EmptyRoomTtl = 0;
        roomController.roomType = "invitation";

        PhotonNetwork.CreateRoom(null, roomOptions);

    }
    public void JoinInvitation(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public void SearchForRoomOfType(string type)
    {
        List<RoomInfo> tempListOfRooms = new List<RoomInfo>();
        RoomInfo selectedRoom = null;
        Debug.Log("available rooms: " + roomsListings.Count);

        StartCoroutine(WaitingTimer(type));

        foreach(RoomInfo room in roomsListings)
        {
            Hashtable expectedCustomRoomProperties = new Hashtable() { { "type", type } };
            Debug.Log(room.Name + " is a " + room.CustomProperties["type"]);
            
        }


        foreach (RoomInfo room in roomsListings)
        {

            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties = room.CustomProperties;


            string roomType = (string)room.CustomProperties["type"];
            Debug.Log(room.Name + " is a " + roomType);

            string roomProps = (string)roomProperties["type"];
            Debug.Log(room.Name + " is a " + roomProps);
     
            if (room.CustomProperties.ContainsValue(type) && room.IsOpen && room.IsVisible)
            {
                tempListOfRooms.Add(room);
                selectedRoom = room;
                Debug.Log("found this room: " + selectedRoom.Name);
            }
        }
        if (tempListOfRooms.Count > 0)
        {
            Debug.Log("List of found rooms contains: " + tempListOfRooms.Count);
        }else
        {
            Debug.Log("didnt find any room of " + type + " type, now create one");
        }

        if (selectedRoom != null)
        {
            PhotonNetwork.JoinRoom(selectedRoom.Name);
        }else
        {
            //Promp to join a diferent type
            CreateRoomOnJoinFaield(type);
        }
    }


    //FUNCTION PAIRED TO THE ANY ROOM JOIN
    public void OnClickJoinAnyRoom()
    {
        StartCoroutine(WaitingTimer("Any"));
        Debug.Log("Joining any room");
        PhotonNetwork.JoinRandomRoom();

    }
    // IF JOINING RANDOM FAILED...
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        Debug.Log("joining random faield...");
        Debug.Log("now creating a room...");
        int i = Random.Range(0, 2);
        
        if (i == 0)
        {
            CreateRoomOnJoinFaield("single");
        }
        else
        {
            CreateRoomOnJoinFaield("match");
        }

        
    }




    void CreateRoomOnJoinFaield(string type)
    {
        Debug.Log("creating a " + type + " game room...");
     
        RoomOptions roomOptions = new RoomOptions();
        string[] ops = { "type" };
        roomOptions.CustomRoomPropertiesForLobby = ops;

        string[] props = { type };

        //https://doc.photonengine.com/en-us/pun/current/lobby-and-matchmaking/matchmaking-and-lobby#matchmaking_checklist

        Hashtable customProps = new Hashtable();
        customProps.Add("type", type);
        customProps.Add("round", 1);

        roomOptions.CustomRoomProperties = customProps;
        //roomOptions.CustomRoomProperties = new Hashtable { { "type",type } };
        
        roomOptions.MaxPlayers = 2;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomController.roomType = type;
        PhotonNetwork.CreateRoom(null, roomOptions);
        
        //PhotonNetwork.CreateRoom(PhotonNetwork.AuthValues.UserId + "'s" + type + " Room", roomOptions);
    }

    //Callback function when a room is added to the server
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("List changed");
        int tempIndex;
        foreach(RoomInfo room in roomList)
        {
            if(roomList != null)
            {
                tempIndex = roomsListings.FindIndex(ByName(room.Name));
            }else
            {
                tempIndex = -1;
            }
            if(tempIndex != -1) //remove listing because it has been closed
            {
                roomsListings.RemoveAt(tempIndex);
                Destroy(roomsContainer.GetChild(tempIndex).gameObject);
            }
            if (room.PlayerCount > 0) //add room because its new
            {
                roomsListings.Add(room);
                ListRoom(room);     //add room to list
            }
        }
    }
    //predicate function to go through room list
    static System.Predicate<RoomInfo> ByName(string name) 
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    void ListRoom(RoomInfo room) //displays new room listing for the current Room
    {
        if(room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomsListItem, roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }

    public bool OtherRoomsFound(string oldType)
    {
        List<RoomInfo> tempListOfRooms = new List<RoomInfo>();
        RoomInfo selectedRoom = null;
        Debug.Log("available rooms: " + roomsListings.Count);

        string newType;
        if (oldType == "single")
        {
            newType = "match";
        }
        else
        {
            newType = "single";
        }

        foreach (RoomInfo room in roomsListings)
        {
            Hashtable expectedCustomRoomProperties = new Hashtable() { { "type", newType } };
            Debug.Log(room.Name + " is a " + room.CustomProperties["type"]);
        }


        foreach (RoomInfo room in roomsListings)
        {

            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties = room.CustomProperties;


            string roomType = (string)room.CustomProperties["type"];
            Debug.Log(room.Name + " is a " + roomType);

            string roomProps = (string)roomProperties["type"];
            Debug.Log(room.Name + " is a " + roomProps);

            if (room.CustomProperties.ContainsValue(newType) && room.IsOpen && room.IsVisible)
            {
                tempListOfRooms.Add(room);
                selectedRoom = room;
                Debug.Log("found this room: " + selectedRoom.Name);
            }
        }
        if (tempListOfRooms.Count > 0)
        {
            Debug.Log("List of found rooms contains: " + tempListOfRooms.Count);
        }
        else
        {
            Debug.Log("didnt find any room of " + newType + " type, now create one");
        }


        return false;
    }

    //function for createing rooms
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("tried to create a room but failed, there must be a room with the same name, reconnect and try again");
    }
    //Paired to the cancel button. used to go back to the main menu
    public void MatchmakingCancel()
    {
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        PhotonNetwork.LeaveLobby();
    }

    private IEnumerator WaitingTimer(string text)
    {
        waitingTimerText.text = "";
        waitingPanel.SetActive(true);
        waitingText.text = text.ToUpper();

        float time = 0;
        while (!PhotonNetwork.InRoom)
        {
            time += Time.deltaTime;
            
            waitingTimerText.text = time.ToString("F2") + " s";
            yield return null;
        }
        waitingPanel.SetActive(false);
        StopCoroutine(WaitingTimer(text));

    }



    public void LeaveLobbyButtom()
    {

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Leaving Lobby");
            PhotonNetwork.Disconnect();
            //SEND TO BACK SCENE
        }

    }
    




}
