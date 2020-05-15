using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyController : MonoBehaviourPunCallbacks
{


    public static LobbyController lobby;
    private List<RoomInfo> roomListings;



    private void Awake()
    {
        lobby = this;
    }

    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to master");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log((string)PhotonNetwork.CurrentRoom.CustomProperties["type"]);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("invitation"))
        {
           // SendInvite();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random");
        int i = Random.Range(0, 2);
        if (i == 0)
        {
            CreateRoomOfType("single");
        }
        else
        {
            CreateRoomOfType("match");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Faield to create a room there must be a room with the same name");
    }



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        roomListings = roomList;
        foreach(RoomInfo room in roomListings)
        {
            if (room.PlayerCount == 0 || room.PlayerCount == 2)
            {
                roomListings.Remove(room);
            }
        }
    }




    //------------------------------------------------------


    #region Non Callbacks
    public void OnClickLeaveRoomFromLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// Creates a room of the especified type
    /// </summary>
    /// <param name="type">The type of room to create</param>
    private  void CreateRoomOfType(string type)
    {
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
        PhotonNetwork.CreateRoom(null, roomOps);
    }

    /// <summary>
    /// Creates invitation rooms
    /// </summary>
    /// <param name="type">the type of room to create</param>
    public void OnClickCreateInvitationRoom(string type)
    {
        Debug.Log("Creating invitation room");

        Hashtable hash = new Hashtable();
        hash.Add("type", type);
        hash.Add("invitation", true);

        RoomOptions roomOps = new RoomOptions();
        roomOps.CustomRoomProperties = hash;
        roomOps.MaxPlayers = 2;
        roomOps.BroadcastPropsChangeToAll = true;
        roomOps.IsOpen = true;
        roomOps.IsVisible = false;
        roomOps.EmptyRoomTtl = 0;       // CHECK THIS;

        PhotonNetwork.CreateRoom(null, roomOps);
    }

    /// <summary>
    /// Joins to invitation rooms
    /// </summary>
    /// <param name="roomName">the name of the room to join</param>
    public void OnClickJoinInvitation(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// Search rooms of an specific type, if not found any creates a room
    /// </summary>
    /// <param name="type">the type of room to find</param>
    public void OnClickSearchForRoomOfType(string type)
    {
        Debug.Log("Search for rooms of type: " + type);
        //StartCoroutine(Search)
        List<RoomInfo> tempRoomList = new List<RoomInfo>();
        RoomInfo foundRoom = null;

        foreach(RoomInfo room in roomListings)
        {
            if(room.CustomProperties.ContainsValue(type) && room.IsOpen && room.IsVisible)
            {
                foundRoom = room;
            }
        }
        if(foundRoom != null)
        {
            PhotonNetwork.JoinRoom(foundRoom.Name);
        }else
        {
            CreateRoomOfType(type);
        }
    }

    public void OnClickJoinAnyRoom()
    {
        //StartCoroutine("Searching");
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClickLeaveLobby()
    {

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        
    }


    #endregion Non Callbacks

}
