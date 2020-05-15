using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;

public class RaiseEventExample : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    PunTurnManager turnManager = new PunTurnManager();

    public bool turn;

    private const byte SWITCH_TURN_EVENT = 0;

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }
    
    private void NetworkingClient_EventReceived(EventData obj)
    {
        if(obj.Code == SWITCH_TURN_EVENT)
        {
            object[] data = (object[])obj.CustomData;
            bool finishedTurn = (bool)data[0];
            int row = (int)data[1];
            int col = (int)data[2];
            int value = (int)data[3];
            bool blocked = (bool)data[4];
        }

        turnManager.TurnDuration = 10;
    }

    private void SwitchTurn()
    {

        bool finishedTurn = true;
        int row = 3;
        int col = 2;
        int value = 3;
        bool blocked = false;

        object[] datas = { finishedTurn, row, col, value, blocked };


        PhotonNetwork.RaiseEvent(SWITCH_TURN_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    private void Update()
    {
        if (base.photonView.IsMine)
        {
            SwitchTurn();
        }
    }


    
    public void OnTurnBegins(int turn)
    {

        throw new System.NotImplementedException();
    }

    public void OnTurnCompleted(int turn)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerMove(Player player, int turn, object move)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerFinished(Player player, int turn, object move)
    {
        throw new System.NotImplementedException();
    }

    public void OnTurnTimeEnds(int turn)
    {
        throw new System.NotImplementedException();
    }
}
