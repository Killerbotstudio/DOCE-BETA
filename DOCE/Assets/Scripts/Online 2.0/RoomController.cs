using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class RoomController : MonoBehaviourPunCallbacks
{
    // RoomController Objects

    public static RoomController RoomControllerListener;

    // Start is called before the first frame update
    void Start()
    {
        RoomControllerListener = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region PUN Callbacks

 

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed: " + message);

        
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom initialized");

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("type"))
        {
            StartCoroutine("Failed to Join BackToLobby");
            return;
        }
        
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoom Failed: " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom initialized at RoomControl");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom initialized");
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom initialized");
    }

    #endregion

    #region RoomController

    public void LeaveRoom()
    {
        Debug.Log("Leave Room Initialized");
        Debug.Log("Attempting to leave room");
        PhotonNetwork.LeaveRoom();
    }


    #endregion
}
