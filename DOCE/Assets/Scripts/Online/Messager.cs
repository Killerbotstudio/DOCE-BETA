using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Chat;
using System.Collections;

using ExitGames.Client.Photon;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif


public class Messager : MonoBehaviour, IChatClientListener
{



    [SerializeField]private string userID;
    //private string selectedChannelName;
    private ChatClient chatClient;

#if !PHOTON_UNITY_NETWORKING
    [SerializeField]
#endif

    protected internal ChatAppSettings chatAppSettings;


    [Header("Popup menu")]
    public GameObject popupPanel;
    public Button acceptButton;

    public Text senderText;
    public Text popUpInfoText;
    public Text popupMessage;

    public Text timerText;
    private float popUptimer;
    [SerializeField]
    private int popUpMessageTime;

    [SerializeField] private OnlineMenuController controller;

    #region ChatCallbacks

    public void Awake()
    {
        controller = FindObjectOfType<OnlineMenuController>();
    }
    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        Debug.Log("connected to chat");
        //throw new System.NotImplementedException();
        this.chatClient.SetOnlineStatus(ChatUserStatus.Online);
        JoinPublicChannel();
    }

    public void OnDisconnected()
    {
        Debug.Log("OnDisconnected");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        // as the ChatClient is buffering the messages for you, this GUI doesn't need to do anything here
        // you also get messages that you sent yourself. in that case, the channelName is determinded by the target of your msg
        //this.InstantiateChannelButton(channelName);


        ChatChannel privateChanel;
        this.chatClient.TryGetChannel(channelName, out privateChanel);

        Debug.Log("Private message from " + sender + " to " + userID + " >>>> " + message.ToString() + "channel: " + channelName);
        if (sender.ToString() != this.chatClient.AuthValues.UserId)
        {
            Debug.Log("IS IT THE SAME?!");
            if (!message.GetType().IsArray)
            {
                
                Debug.Log("rejected: " + message.ToString());
                controller.OnInvitationRejected();
            }
            else
            {


                //ACTIVATE MESSAGE;
                object[] input = (object[])message;
                string type = (string)input[0];
                string room = (string)input[1];



                
                controller.PopupStarter(room, type, sender);
                
                //controller.PopupStarter(room, type, sender.ToString());

            }
            
        }


        string[] channels = { channelName };
        this.chatClient.Unsubscribe(channels);

        Debug.Log("MESSAGES: " + privateChanel.MessageCount);
        Debug.Log("SUBSCRIBERS " + privateChanel.Subscribers.Count);
        Debug.Log("SENDERS " + privateChanel.Senders.Count);
        privateChanel.ClearMessages();
        Debug.Log("xxxMESSAGES: " + privateChanel.MessageCount);
        Debug.Log("xxxSUBSCRIBERS " + privateChanel.Subscribers.Count);
        Debug.Log("xxxSENDERS " + privateChanel.Senders.Count);


        //byte[] msgBytes = message as byte[];
        //if (msgBytes != null)
        //{
        //    Debug.Log("Message with byte[].Length: " + msgBytes.Length);
        //}
        //if (this.selectedChannelName.Equals(channelName))
        //{
        //    this.ShowChannel(channelName);
        //}
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }
    #endregion ChatCallbacks


    #region Others

    public void Update()
    {
        //        Debug.Log("pre" + this.chatClient.State.ToString());
        if (this.chatClient != null)
        {
            //Debug.Log("chat");
            this.chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }
        // Debug.Log(this.chatClient.State.ToString());

        // check if we are missing context, which means we got kicked out to get back to the Photon Demo hub.
        //if (this.StateText == null)
        //{
        //    Destroy(this.gameObject);
        //    return;
        //}

        //this.StateText.gameObject.SetActive(this.ShowState); // this could be handled more elegantly, but for the demo it's ok.
    }

    public void OnDestroy()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnApplicationQuit.</summary>
    public void OnApplicationQuit()
    {
        if (this.chatClient != null)
        {
            Debug.Log("Chat On Application Quit");
            Debug.Log(chatClient.UserId);
            this.chatClient.Disconnect();
            //Debug.Log(chatClient.State.ToString());
            //Debug.Log(chatClient.chatPeer.ToString());
            //Debug.Log(chatClient.CanChat.ToString());

            //Debug.Log(chatClient.UserId);
        }
    }


    #endregion Others

    #region Messager Functions

    //    public void JumpStart(string ID)
    //    {

    //        this.userID = ID;


    //        Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
    //        Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings().AppId);
    //        Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings().Server);

    //#if PHOTON_UNITY_NETWORKING
    //        this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
    //#endif

    //        bool appIdPresent = !string.IsNullOrEmpty(this.chatAppSettings.AppId);
    //        if (!appIdPresent)
    //        {
    //            Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
    //        }
    //    }

    public void DisconnectChat()
    {
        //this.OnDisconnected();
        if(this.chatClient!= null)
        {
            this.chatClient.Disconnect();
        }
        
    }
    public void ConnectChat(string ID)
    {


        this.userID = ID;


        Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
        Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings().AppId);
        Debug.Log(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings().Server);

#if PHOTON_UNITY_NETWORKING
        this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
#endif

        bool appIdPresent = !string.IsNullOrEmpty(this.chatAppSettings.AppId);
        if (!appIdPresent)
        {
            Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
        }


        this.chatClient = new ChatClient(this);
#if !UNITY_WEBGL
        this.chatClient.UseBackgroundWorkerForSending = true;
#endif
        this.chatClient.AuthValues = new AuthenticationValues(this.userID);
        Debug.Log("Chat authValues: " + chatClient.AuthValues.ToString());
        this.chatClient.ConnectUsingSettings(this.chatAppSettings);
        Debug.Log("Connecting chat as: " + this.userID);

       
    }

    public bool CheckIfUserExist(string userID)
    {
        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel("public", out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + "public");
            return false;
        }

        //ChatChannel publicChannel = this.chatClient.PublicChannels[0];
        ChatChannel publicChannel = channel;

        foreach (string pName in publicChannel.Subscribers)
        {

            if (pName == userID)
                return true;
        }
        return false;
    }


    public void JoinPublicChannel()
    {
        chatClient.Subscribe("public", creationOptions: new ChannelCreationOptions { PublishSubscribers = true });
    }

    public void OnClickSend(string message)
    {
        //TO SEND MESSAGE OR INVITE
        Debug.Log("Clicked Send!");
        Debug.Log("sending..." + message);
        this.chatClient.SendPrivateMessage(message, "invite", true);
        //if (this.UserIdText != null)
        //{


        //    this.SendChatMessage(this.UserIdText.text);
        //    this.UserIdText.text = "";


        //    Debug.Log("Send message to: " + UserIdText.text);
        //    this.chatClient.SendPrivateMessage(UserIdText.text, "invite", true);
        //}
    }

    //public void PopUpMessage(string message, string senderID)
    //{
    //    StartCoroutine(PopUpRoutine(message, senderID));
    //}
    //private IEnumerator PopUpRoutine(string message, string senderID)
    //{
    //    popupPanel.gameObject.SetActive(true);
    //    senderText.text = senderID;
    //    popUpInfoText.text = message;
    //    popupMessage.text = "<b>Invited you to join a</b>";
    //    popUptimer = popUpMessageTime;

    //    //PLAY ALERT SOUNDS
    //    //SETUP BUTTONS
    //    acceptButton.onClick.AddListener(delegate () { OnClickAcceptInvitation(message); });

    //    acceptButton.onClick.AddListener(delegate () { StopCoroutine(PopUpRoutine(message, senderID)); });
    //    acceptButton.interactable = true;

    //    while (popUptimer > 0)
    //    {

    //        popUptimer -= Time.deltaTime;
    //        timerText.text = popUptimer.ToString();
    //        yield return null;
    //    }

    //    timerText.text = "0.0";

    //    senderText.text = "<b>TIMEOUT</b>";
    //    popupMessage.text = "Invitation time expired";
    //    popUpInfoText.text = "";

    //    acceptButton.onClick.RemoveAllListeners();
    //    acceptButton.interactable = false;

    //    yield return new WaitForSeconds(3.5f);

    //    senderText.text = null;
    //    popUpInfoText.text = null;
    //    popupPanel.gameObject.SetActive(false);
    //    StopCoroutine(PopUpRoutine(message, senderID));

    //}

    public void SendInvitationForRoom(string roomName, string type, string receiver)
    {

        //TO SEND MESSAGE OR INVITE
        //string message = roomName;
        Debug.Log("sending..." + roomName + " to " + receiver);
       
        //Debug.Log(this.chatAppSettings);
        //Debug.Log(this.chatClient);
        //Debug.Log(receiver);

        string[] message = { type, roomName };

        this.chatClient.SendPrivateMessage(receiver, message);

    }

    public void SendReject(string receiver)
    {
        this.chatClient.SendPrivateMessage(receiver, "reject");
    }

    public void OnClickAcceptInvitation(string roomName)
    {

        OnlineMenuController onlineController = FindObjectOfType<OnlineMenuController>();
        onlineController.OnClickJoinInvitation(roomName);

        popupPanel.gameObject.SetActive(false);
    }


    #endregion Messager Functions
}
