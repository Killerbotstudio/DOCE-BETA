using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{

    [SerializeField] private GameObject offlinePanel;
    [SerializeField] private Text offlineText;
    [SerializeField] private Text loadingText;
    private void Start()
    {
        Debug.Log("Is connected:" + PhotonNetwork.IsConnected);
        SetupPlayer();
        StartCoroutine(Transitioning());
    }

    public void SetupPlayer()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        Hashtable hash = new Hashtable();
        hash.Add("scene", true);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

    }

    public IEnumerator Transitioning()
    {
       // SetupPlayer();
        bool localReady = false;
        bool remoteReady = false;
        float timeLimit = 5;

		yield return new WaitForSeconds(1);
        while (timeLimit > 0)
        {
            if(loadingText.text == "Loading...")
                loadingText.text = "Loading";

            

            if(PhotonNetwork.PlayerList.Length == 2)
            {
                localReady = (bool)PhotonNetwork.LocalPlayer.CustomProperties["scene"];
                remoteReady = (bool)PhotonNetwork.PlayerListOthers[0].CustomProperties["scene"];
                Debug.Log("TRANSITION: " + localReady + " vs " + remoteReady);
                if (localReady && remoteReady)
                    timeLimit = 0;
            }
            
            timeLimit -= 1;
            yield return new WaitForSeconds(1);
            loadingText.text += ".";
        }

        if(localReady && remoteReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("OnlineGameScene");
            }
        } else
        {
            Debug.Log("EXIT");
            PhotonNetwork.AutomaticallySyncScene = false;
            
            offlineText.text = "Connection issues";
            offlinePanel.SetActive(true);
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene("OnlineLobbyScene");
        }
    }

    
}
