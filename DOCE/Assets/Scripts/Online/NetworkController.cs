using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Photon.Chat;

public class NetworkController : MonoBehaviourPunCallbacks
{

   
    
    public Text serverText;
    

    public Text authentificationName;

    //private NetworkUserID networkUser;
    private string userID;
    // Change Server Region when building
    [SerializeField]
    private GameObject lobbyControllerGO;
    [SerializeField]
    private GameObject lobbyConnectButton;

    [SerializeField]
    private MessageControllerStarter messageStarter;

    void Start()
    {


        //networkUser = this.GetComponent<NetworkUserID>();

        userID = GeneratedID();
        PlayerPrefs.SetString("NickName", userID);
        PhotonNetwork.NickName = userID;
        messageStarter.messageUserID = userID;
        messageStarter.enabled = true;
        PhotonNetwork.ConnectUsingSettings(); // Connects to master server;
        

        serverText.text = "connecting...";
    }


    public override void OnConnectedToMaster()
    {
        

        
        PhotonNetwork.AuthValues.UserId = userID;
        authentificationName.text = userID;

        int ping = PhotonNetwork.GetPing();
        serverText.text = "Server: " + PhotonNetwork.CloudRegion + " " + ping +" ms";
        Debug.Log(">>>>>>>We are online " + PhotonNetwork.CloudRegion + " server!<<<<<<<<");
        
                
        
        Debug.Log(userID + " is: " + PhotonNetwork.AuthValues.UserId + " and is online!");
        //lobbyControllerGO.SetActive(true);
        //lobbyConnectButton.SetActive(true);//active button for connecting to lobby
    }

    void ConnectToRegion(string region)
    {
        PhotonNetwork.ConnectToRegion(region);
    }

    void UserIDChange(string id)
    {
        AuthenticationValues authV = new AuthenticationValues();
        authV.UserId = id;
        PhotonNetwork.AuthValues = authV;
        
        

    }

    [Header ("ID Generator")]
    [SerializeField]
    private string[] words;
    [SerializeField]
    private string[] title;
    private int i1, i2, i3;



    private string GeneratedID()
    {
        string ID;

        int location = Random.Range(0, 2);
        i1 = Random.Range(0, 10);
        i2 = Random.Range(0, 10);
        i3 = Random.Range(0, 10);


        if (location == 0)
        {
            ID = i1.ToString() + i2.ToString() + i3.ToString()  + words[Random.Range(0, words.Length)] + title[Random.Range(0, title.Length)];

        }
        else 
        {
            ID = words[Random.Range(0, words.Length)] + title[Random.Range(0, title.Length)] + i1.ToString() + i2.ToString() + i3.ToString();
        }
        

        return ID;
    }
}
