using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;


public class MatchmakingLobbyController : MonoBehaviourPunCallbacks
{


    [SerializeField]
    private GameObject lobbyConnectButton; //button used for joining a lobby
    [SerializeField]
    private GameObject lobbyPanel; // panel for displaying lobby
    [SerializeField]
    private GameObject mainPanel; // panel for displaying the main menu
    [SerializeField]
    private Text userIDText; //Text field so player can show ther ID


    private string roomName; // string for saving room name
    private int roomSize = 2; // int for saving room size

    private List<RoomInfo> roomListings; //list of current rooms;
    [SerializeField]
    private Transform roomsContainer; // container for odlgin all the roomlistings;
    [SerializeField]
    private GameObject roomListingPrefab; //prefab for displayer each room in the lobby

    [Header("Players Online")]

    private List<Player> playerListings; //list of current players online
    [SerializeField]
    private Transform playersContainer;
    [SerializeField]
    private GameObject playerListingPrefab;

    [SerializeField]
    private Text serverInfo;


    private void Start()
    {
        
    }

    public override void OnConnectedToMaster() //Callback function for when the first connection is established
    {

        Debug.Log("OnConnectedToMaster2");

        PhotonNetwork.AutomaticallySyncScene = true;// make it so whatever scene the master client has....
        lobbyConnectButton.SetActive(true);//active button for connecting to lobby
        roomListings = new List<RoomInfo>(); //initialize roomListings

        //check for player name saved to player prefs

        lobbyConnectButton.SetActive(true);//active button for connecting to lobby
        string ID = PhotonNetwork.AuthValues.UserId;
        userIDText.text = "<color=#E07B00>ID:</color> " + "<b>" + ID.ToString() + "</b>";
        PhotonNetwork.JoinLobby();
        roomListings = new List<RoomInfo>(); //initialize roomListings
        Debug.Log("User is in lobby? " + PhotonNetwork.InLobby);
    }




    //public void PlayerNameUpdate(string nameInput)  //input function for player name. paired to player text
    //{
    //    PhotonNetwork.NickName = nameInput;
    //    PlayerPrefs.SetString("NickName", nameInput);

    //}


    public void JoinLobbyOnClick() //paired to the delay start button
    {
       
        Debug.Log("JoinLobbyClick");
        Debug.Log("User is in lobby? " + PhotonNetwork.InLobby);
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        
        //PhotonNetwork.JoinLobby(); //First tries to join an existing room
        //ListOfOnlinePlayers();

    }

    //private void SetUserID()
    //{
    //    AuthenticationValues authValue = new AuthenticationValues(PlayerPrefs.GetString("NickName"));
    //    PhotonNetwork.AuthValues = authValue;
    //    Debug.Log("this player ID" + PhotonNetwork.AuthValues.UserId);
    //    //PhotonNetwork.AuthValues.
    //}


    public void CreateRoom() //function paired to the create room button
    {
        Debug.Log("creating room...");
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom(roomName, roomOps); //attempting to create a new room
    }

    private ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtable();
    
    [SerializeField]
    MatchmakingRoomController roomController;


    public void SingleRoomCreate() //paired to the single game button
    {
        Debug.Log("creating a single game room...");
        //roomProperty.Add("Type", "none");

        roomProperty["type"] = "single";
        
        roomController.roomProperty = roomProperty;

        string check = roomProperty["type"].ToString();
        Debug.Log(check);
        Debug.Log("propertyLobby = " + roomProperty.ToString());
        RoomOptions singleRoom = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };

        //CREATE ROOM PROPETIES WITH ROOM OPTIONS
        PhotonNetwork.CreateRoom(PhotonNetwork.AuthValues.UserId + "'s Room" , singleRoom);


        //SET ROOMS PROPERTIES ONCE CREATED?
    }
    void CreateRoomOnJoinFaield(string type)
    {
        Debug.Log("creating a " + type + " game room...");
       
        roomProperty["type"] = type;

        roomController.roomProperty = roomProperty;


        Debug.Log("propertyLobby = " + roomProperty.ToString());
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };

        //CREATE ROOM PROPETIES WITH ROOM OPTIONS
        PhotonNetwork.CreateRoom(PhotonNetwork.AuthValues.UserId + "'s Room", roomOps);

    }

    public void SearchForRoomOnClick(string type)
    {
        List<RoomInfo> tempList = new List<RoomInfo>();
        RoomInfo selectedRoom = null;
        Debug.Log(roomListings.Count);

        foreach (RoomInfo room in roomListings)
        {
            if(room.CustomProperties.ContainsKey(type) && room.IsOpen && room.IsVisible)
            {
                tempList.Add(room);
                selectedRoom = room;
                Debug.Log("found this room: " + selectedRoom.Name);
            }
        }

        if(tempList.Count != 0)
        {
            Debug.Log("list of found rooms contains: " + tempList.Count);
        }
        else
        {
            Debug.Log("didn't find any " + type + " room...");
        }

        if(selectedRoom != null)
        {
            PhotonNetwork.JoinRoom(selectedRoom.Name);
        } else
        {
            Debug.Log(" attempting to create a" + type + " room");
            CreateRoomOnJoinFaield(type);
        }

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) //once in lobby this function is callback
    {
        Debug.Log("OnRoomListUpdate" + Time.time + "amount of rooms: " + roomList.Count);
        
        int tempIndex;
        foreach (RoomInfo room in roomList) //loop through each room in room list
        {
            if(roomListings != null) // try to find existing room listing
            {
                
                tempIndex = roomListings.FindIndex(ByName(room.Name));
                Debug.Log(tempIndex);
            }
            else
            {
                
                tempIndex = -1;
                Debug.Log(tempIndex);
            }
            if(tempIndex != -1) //remove listing because it has ben closed
            {
                Debug.Log(tempIndex);
                roomList.RemoveAt(tempIndex);
                Destroy(roomsContainer.GetChild(tempIndex).gameObject);
            }
            if (room.PlayerCount > 0) //add room listing because it is new
            {
                roomList.Add(room);
                ListRoom(room);
            }
        }
       
    }
   
    static System.Predicate<RoomInfo> ByName(string name) //predicate function for search through room
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
            
    }

    void ListRoom(RoomInfo room) //displays new room listing for the current room
    {
        if(room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }

    public void OnRoomNameChanged(string nameIn) //input function for changing room name. paired to rooms name
    {
        roomName = nameIn;
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //Create room will fail if room already exists
    {
        //add functions to display the player for errors
        Debug.Log("Tried to create a new room but failed, ther must be a room with the same name");
    }

    public void MatchmakingCancel() //Paired to the cancel button. used to go back to the main menu
    {
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        PhotonNetwork.LeaveLobby();
    }


    //void CheckPlayerPrefs()
    //{
    //    if (PlayerPrefs.HasKey("NickName"))
    //    {
    //        if (PlayerPrefs.GetString("Nickname") == " ")
    //        {
    //            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000); //random player name when not having nickname
    //        }
    //        else
    //        {
    //            PhotonNetwork.NickName = PlayerPrefs.GetString("NickName"); // get saved player name
    //        }
    //    }
    //    else
    //    {
    //        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000); //random player name when not....
    //    }

    //    userIDText.text = PhotonNetwork.NickName; //update input ield with player name
    //}


}
