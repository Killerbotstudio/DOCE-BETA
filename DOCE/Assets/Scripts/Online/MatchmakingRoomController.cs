using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hastable = ExitGames.Client.Photon.Hashtable;




public class MatchmakingRoomController : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private int multiPlayerSceneIndex; //scene index for loading multiplayer scene

    [SerializeField]
    private GameObject lobbyPanel; //display for when in lobby
    [SerializeField]
    private GameObject roomPanel; //display for when in room

    [SerializeField]
    private GameObject startButton; // only for the master client. Used to start the game and load the scene

    [SerializeField]
    private Transform playersContainer; //used to display all the players in the current room
    [SerializeField]
    private GameObject playerListingPrefab; //Instantiate to display each player in the room

    [SerializeField]
    private Text roomNameDisplay; //display for the name of the room

    [SerializeField]
    private Text roomPropertyText;

    //[SerializeField]
    public ExitGames.Client.Photon.Hashtable roomProperty;



    public static MatchmakingRoomController roomController;

    public bool ready;



    public string roomType;
    object hashValue;
    object roomT;
    string roomTypeString = "type";



    [Header("Opponent Panel")]
    [SerializeField]
    private Text opponentPanelTitle;
    [SerializeField]
    private GameObject opponentPanel;
    [SerializeField]
    private Text opponentNickname;
    [SerializeField]
    private Text roomTypeText;
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private Button acceptButton;
    [SerializeField]
    private Text opponentInfoText;


    void ClearPlayerListings()
    {
        for (int i = playersContainer.childCount - 1; i >= 0; i--) //loop through all child object of players list
        {
            Destroy(playersContainer.GetChild(i).gameObject);
            //DESTROY HASHES
        }
        
    }

    void ListPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList) // loop through each player and create a players list
        {
            GameObject tempListing = Instantiate(playerListingPrefab, playersContainer);
            Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
            tempText.text = player.NickName;
            Debug.Log(player.NickName + " joined the room");

            string readyPlayer = "false";
            Hastable playerHash = new Hastable();
            //Hastable playerNewHash = player.CustomProperties;
            playerHash["ready"] = readyPlayer;
            
            //player.CustomProperties = playerHash;
            player.SetCustomProperties(playerHash);

          

        }

        //if (PhotonNetwork.PlayerList.Length == 1)
        //{
        //    //Assign MasterClient
        //    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        //}
        
        Debug.Log("room playerlist lenght = " + PhotonNetwork.PlayerList.Length);
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            Debug.Log("waiting for opponent");
            StartCoroutine(WaitingOpponent((string)PhotonNetwork.CurrentRoom.CustomProperties["type"]));
        }
        else if (PhotonNetwork.PlayerList.Length == 2)
        {
            Debug.Log("opponent found");
            StartCoroutine(OpponentFoundRoutine());
            //OPPONENT FOUND!
        } 
    }

    public override void OnJoinedRoom() //called when the local player joins the room
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        Debug.Log("joined");
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name; // update room name display
        string roomTypeDisplay = (string)PhotonNetwork.CurrentRoom.CustomProperties["type"];
        roomPropertyText.text = roomTypeDisplay;

        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);

        if (PhotonNetwork.IsMasterClient) // if master client then activate the startbutton
        {

            startButton.SetActive(true);
            //ACTIVATE SWTICH MODE BUTTONS
            if ((string)PhotonNetwork.CurrentRoom.CustomProperties["type"] == "invitation")
            {
                //SEND INVITATION;
                Debug.Log("INVITATION: " + (string)PhotonNetwork.CurrentRoom.CustomProperties["type"] + " ");
                MessageController messageC = FindObjectOfType<MessageController>();
                messageC.SendInvitationForRoom(PhotonNetwork.CurrentRoom.Name);
            }
        }
        else
        {
            //PhotonNetwork.PlayerListOthers[0].CustomProperties.Add("ready", false);
            //startButton.SetActive(false);
        }

        ClearPlayerListings(); //remove all old player listings
        ListPlayers(); //relist all current player listing
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //called whenever a new player enter the room
    {
        ClearPlayerListings(); //remove all old player listings
        ListPlayers(); //relist all current player Listing
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //called whenever a player leave the room
    {

        ClearPlayerListings(); //remove all old player lisitngs
        ListPlayers(); //relist all current players
        otherPlayer.CustomProperties.Clear();

        
        //PhotonNetwork.SetMasterClient();
        if (PhotonNetwork.IsMasterClient) //if the local player is now the new master client then we 
        {
            //SWITCH ROOM NAME

            startButton.SetActive(true);
            //ACTIVATE SWITHC BUTTONS
        }
    }



    private IEnumerator OpponentFoundRoutine()
    {
        opponentPanelTitle.text = "OPPONENT FOUND!";
        opponentPanel.SetActive(true);
        roomTypeText.text = "Type: " + PhotonNetwork.CurrentRoom.CustomProperties["type"].ToString().ToUpper();
        opponentNickname.text = PhotonNetwork.PlayerListOthers[0].NickName;
        acceptButton.gameObject.SetActive(true);
        acceptButton.interactable = true;
        opponentInfoText.text = "Waiting for other player to accept...";

        float time = 20;

        timerText.text = time.ToString("F2") + " s";
        bool allPlayersReady = false;

        //yield return new WaitForSeconds(1);

        while (PhotonNetwork.PlayerList.Length == 2 && time > 0)
        {
            string locaLPlayerReady = (string)PhotonNetwork.LocalPlayer.CustomProperties["ready"];
            string secondPlayerReady = (string)PhotonNetwork.PlayerListOthers[0].CustomProperties["ready"];
            Debug.Log(locaLPlayerReady + " vs " + secondPlayerReady);

            time -= 1;
            if (locaLPlayerReady == "true" && secondPlayerReady == "true")
            {
                //ALL PLAYERS READY
                allPlayersReady = true;
                time = 0;
                opponentInfoText.text = "All players are ready!";
                Debug.Log("All players ready");
            }

            
            timerText.text = time.ToString("F2") + " s";
            yield return new WaitForSeconds(1);
        }

        if(PhotonNetwork.PlayerList.Length != 2)
        {
            StartCoroutine(WaitingOpponent((string)PhotonNetwork.CurrentRoom.CustomProperties["type"]));
            StopCoroutine(OpponentFoundRoutine());
        }

        if (!allPlayersReady)
        {

            StartCoroutine(OpponentTookTooLong());
            StopCoroutine(OpponentFoundRoutine());
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //START GAME
                StartGame();
                StopCoroutine(OpponentFoundRoutine());
            }
            StopCoroutine(OpponentFoundRoutine());
        }
    }

    IEnumerator OpponentTookTooLong()
    {
        acceptButton.gameObject.SetActive(false);
        opponentPanelTitle.text = "SORRY...";
        opponentNickname.text = "Opponent took too long to answer";
        roomTypeText.text = "going back to lobby...";
        Debug.Log("Going back to lobby");
        opponentInfoText.text = "";
        yield return new WaitForSeconds(3);
        opponentPanel.SetActive(false);
        Debug.Log("OpponentTookTooLong Routine");
        BackOnClick();
        StopCoroutine(OpponentTookTooLong());

    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hastable changedProps)
    {
        
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public void OnClickPlayerReady()
    {

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("ready"))
        {

            Hastable playerHash = PhotonNetwork.LocalPlayer.CustomProperties;
            //Hastable playHash = new Hastable();
            //playHash.Add("ready", "true");
            //playHash["ready"] = "true";

            playerHash["ready"] = "true";

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);

            acceptButton.interactable = false;
            acceptButton.GetComponentInChildren<Text>().text = "READY";
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected reason: " + cause);
        base.OnDisconnected(cause);
    }

    public void StartGame() // paired to the start button will load ll players into the multiplatyer scene
    {
        PhotonNetwork.CurrentRoom.IsOpen = false; //comment out if you want player to join after the room started
        
        PhotonNetwork.LoadLevel(multiPlayerSceneIndex);
    }

    [Header("Waiting Panel")]
    [SerializeField]
    private GameObject waitingPanel;
    [SerializeField]
    private Text waitingRoomType;
    [SerializeField]
    private Text waitingTimerText;

    private IEnumerator WaitingOpponent(string text)
    {
        waitingTimerText.text = "";
        waitingPanel.SetActive(true);
        waitingRoomType.text = text.ToUpper();

        float time = 0;
        while (PhotonNetwork.PlayerList.Length == 1)
        {
            time += Time.deltaTime;

            waitingTimerText.text = time.ToString("F2") + " s";
            yield return null;
        }
        waitingPanel.SetActive(false);
        StopCoroutine(WaitingOpponent(text));
    }

    IEnumerator rejoinLobby()
    {
        Debug.Log(" back clicked ROOM CONTROLLER");

        while (!PhotonNetwork.IsConnectedAndReady)
        {
            //Debug.Log("NOT CONNECTED!");
            yield return null;
        }


        yield return new WaitForSeconds(1); //3
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Not In lobby. Joining One");
            PhotonNetwork.JoinLobby();
        }
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }

    public Button backToLobbyButton;

    public void BackOnClick() // paired to the back button in the room panel. will return the player to the lobby
    {
        
        PhotonNetwork.LeaveRoom();
        //if (PhotonNetwork.InLobby)
        //{
            
        //    PhotonNetwork.LeaveLobby();
        //}
        
        StartCoroutine(rejoinLobby());
    }


    private void Update()
    {
        
        if(PhotonNetwork.CurrentRoom == null && !PhotonNetwork.IsConnectedAndReady)
        {
            backToLobbyButton.interactable = false;
        } else
        {
            backToLobbyButton.interactable = true;
        }
    }
}
