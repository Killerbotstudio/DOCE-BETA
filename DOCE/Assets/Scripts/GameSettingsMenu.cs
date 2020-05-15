//////////////GameSettingsMenu//////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameSettingsMenu : MonoBehaviour
{
    public int playerStart;
    public GameObject dicePlayer1;
    public GameObject dicePlayer2;
    public GameObject marker1;
    public GameObject marker2;
    public List<Sprite> decals;
    public Text statusText;
    public InputField player1InputName;
    public InputField player2InputName;
    public RectTransform player1Panel;
    public RectTransform player2Panel;
    public int rounds;
    public Text nameAndVersion;
    private Texture2D cursorOriginal;
    public Texture2D cursorOver;
    public GameObject vsImage;
    public Image rollDice1;
    public Image rollDice2;
    public GameObject settingsOption1;
    public GameObject settingsOption2;
    public GameObject infoTextPlayer1;
    public GameObject infoTextPlayer2;
    public Button playButton;
    private void Start()
    {
        //nameAndVersion.text = Application.productName + " v: " + Application.version;
    }
    public void Rounds(int i)
    {
        rounds = i;
    }
    public void MoveNamesButton()
    {
        StartCoroutine(MoveNames());
    }
    public IEnumerator MoveNames()
    {
        Vector3 positionOfPlayer1 = new Vector3(-300, 18, 0);
        Vector3 positionOfPlayer2 = new Vector3(300, 18,0);
        while(Vector3.Distance(player1Panel.localPosition, positionOfPlayer1) > 1 && Vector3.Distance(player2Panel.localPosition, positionOfPlayer2) > 1)
        {
            player1Panel.localPosition = Vector3.MoveTowards(player1Panel.localPosition, positionOfPlayer1, 5);
            player2Panel.localPosition = Vector3.MoveTowards(player2Panel.localPosition, positionOfPlayer2, 5);
            yield return null;
        }
        InitialPlayerButton();
    }

   
    public void InitialPlayerButton()
    {
        StartCoroutine(SelectInitialPlayer());
    }
    public void CursorOver()
    {
        Cursor.SetCursor(cursorOver, Vector2.zero, CursorMode.Auto);
    }
    public void CursorExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    public void TextColorMenu(Text text)
    {
        text.color = Color.gray;
    }
    public void ResetColorMenu(Text text)
    {
        text.color = Color.white;
    }
    public void RollDices()
    {
        StartCoroutine(RollingDicesRoutine());
    }
    public IEnumerator RollingDicesRoutine()
    {
        float timer = 1f;
        float counter = 0;
        int diceValue1 = 0;
        int diceValue2 = 0;
        dicePlayer1.SetActive(true);
        dicePlayer2.SetActive(true);
        while (counter < timer)
        {
            diceValue1 = Random.Range(1, 7);
            diceValue2 = Random.Range(1, 7);
            rollDice1.sprite = decals[diceValue1 - 1];
            rollDice2.sprite = decals[diceValue2 - 1];
            counter += 0.15f;
            yield return new WaitForSeconds(0.2f);
        }
        while (diceValue1 == diceValue2)
        {
            diceValue1 = Random.Range(1, 7);
            diceValue2 = Random.Range(1, 7);
            rollDice1.sprite = decals[diceValue1 - 1];
            rollDice2.sprite = decals[diceValue2 - 1];
            yield return null;
        }
        if (diceValue1 > diceValue2)
        {
            marker1.SetActive(true);
            marker2.SetActive(false);
            playerStart = 1;
        }else
        {
            marker1.SetActive(false);
            marker2.SetActive(true);
            playerStart = 2;
        }
        playButton.gameObject.SetActive(true);
    }
    public IEnumerator SelectInitialPlayer()
    {
        int diceValue1 = Random.Range(1, 7);
        int diceValue2 = Random.Range(1, 7);
        if (diceValue1 == diceValue2)
        {
            StopCoroutine(SelectInitialPlayer());
            PlayerSelectionDiceDrawing(diceValue1, diceValue2);
            StartCoroutine(SelectInitialPlayer());
        }
        else if (diceValue1 > diceValue2)
        {
            playerStart = 1;
            PlayerSelectionDiceDrawing(diceValue1, diceValue2);
            yield return new WaitForSeconds(1);
            marker1.SetActive(true);
            marker2.SetActive(false);
            yield return new WaitForSeconds(2);
            StopCoroutine(SelectInitialPlayer());
        }
        else
        {
            playerStart = 2;
            PlayerSelectionDiceDrawing(diceValue1, diceValue2);
            yield return new WaitForSeconds(1);
            marker1.SetActive(false);
            marker2.SetActive(true);
            yield return new WaitForSeconds(2);
            StopCoroutine(SelectInitialPlayer());
        }
    }
    public void PassStartingRound(int startingPlayer)
    {
        playerStart = startingPlayer;
    }

    public void StartRound()
    {
        Debug.Log("Start Round");
        GameManager.player1Name = player1InputName.text;
        GameManager.player2Name = player2InputName.text;
        GameManager.rounds = rounds;
        GameManager.initialPlayerInt = playerStart;
        GameManager.currentRound = 1;
        SceneManager.LoadScene("GameScene");
    }
    public void OnClickOnlineGame()
    {
        SceneManager.LoadScene("OnlineLobbyScene");
    }

    private void PlayerSelectionDiceDrawing(int valuePlayer1, int valuePlayer2)
    {
        if (valuePlayer1 != 0)
        {
            dicePlayer1.SetActive(true);
            dicePlayer1.GetComponentsInChildren<Image>()[1].sprite = decals[valuePlayer1 - 1];
            dicePlayer1.GetComponentsInChildren<Image>()[1].color = Color.white;
        }
        if (valuePlayer2 != 0)
        {
            dicePlayer2.SetActive(true);
            dicePlayer2.GetComponentsInChildren<Image>()[1].sprite = decals[valuePlayer2 - 1];
            dicePlayer2.GetComponentsInChildren<Image>()[1].color = Color.black;
        }
    }

    public IEnumerator StatusTextDisplay(string text)
    {
        statusText.gameObject.SetActive(true);
        statusText.enabled = true;
        statusText.text = text;
        yield return new WaitForSeconds(2);
        statusText.gameObject.SetActive(false);
    }

    public void ResetGameSettings()
    {
        marker1.SetActive(false);
        marker2.SetActive(false);
        foreach (Transform obj in marker1.transform) 
        {
            obj.gameObject.SetActive(true);
        }

        foreach (Transform obj in marker2.transform)
        {
            obj.gameObject.SetActive(true);
        }

        infoTextPlayer1.SetActive(true);
        infoTextPlayer2.SetActive(true);
        playButton.gameObject.SetActive(false);
        player1InputName.GetComponent<EventTrigger>().enabled = true;
        player2InputName.GetComponent<EventTrigger>().enabled = true;
        settingsOption1.SetActive(true);
        settingsOption2.SetActive(true);
        settingsOption1.GetComponentInChildren<Button>().interactable = true;
        settingsOption2.GetComponentInChildren<Button>().interactable = true;
        settingsOption1.GetComponentInChildren<EventTrigger>().enabled = true;
        settingsOption2.GetComponentInChildren<EventTrigger>().enabled = true;
        settingsOption1.GetComponentInChildren<Text>().color = Color.white;
        settingsOption2.GetComponentInChildren<Text>().color = Color.white;
        vsImage.SetActive(false);
    }
}
