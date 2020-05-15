using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (MessageController))]
public class MessageControllerStarter : MonoBehaviour
{

    private const string UserNamePlayerPref = "NamePickUserName";
    public MessageController messageComponent;
    public Text idInput;
    public string messageUserID;

    
    void Start()
    {
        this.messageComponent = FindObjectOfType<MessageController>();


        string prefs = messageUserID;

        Debug.Log("message starter " + prefs);
        if (!string.IsNullOrEmpty(prefs))
        {
            this.idInput.text = prefs;
        }
    }

    public void StartMessager()
    {
        MessageController messageNewComponent = FindObjectOfType<MessageController>();
        //messageNewComponent.UserName = this.idInput.text.Trim();
        messageNewComponent.UserName = this.messageUserID;
        Debug.Log("Starting messager: " + idInput.text.Trim());
        messageComponent.enabled = true;
        messageNewComponent.Connect();
        enabled = false;

        PlayerPrefs.SetString("NickName", messageNewComponent.UserName);

    }

}
