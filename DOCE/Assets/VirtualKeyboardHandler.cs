using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class VirtualKeyboardHandler : MonoBehaviour
{
    [SerializeField] InputField textPlaceHolder;
    [SerializeField] GameObject charactersPanel;
    [SerializeField] GameObject numbersPanel;
    [SerializeField] GameObject shiftButton;
    [SerializeField] Text numbersButtonText;
    [SerializeField] List<Text> charactersList;

    private InputField targetField;

    [DllImport("__Internal")]
    private static extern void CheckIfMobile();

    public void WebGLMobilePass(InputField input)
    {

        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            CheckIfMobile();
            targetField = input;

        } 
    }

    public void IsMobile()
    {
        OpenKeyboard();
    }



    private void Awake()
    {
        InitializedKeyboard();
    }

    private void InitializedKeyboard()
    {
        if(charactersList.Count != 0)
        {
            charactersList.Clear();
        }
        foreach (Text text in charactersPanel.GetComponentsInChildren<Text>())
        {
            
            charactersList.Add(text);
        }
    }



    public void OpenKeyboard()
    {
        this.gameObject.SetActive(true);
        //targetField = field;
    }

    public void DeleteButton()
    {
        //        string value = targetField.text;

        Debug.Log(textPlaceHolder.text);
        if(textPlaceHolder.text.Length > 0)
        {

        //}
        //if(textPlaceHolder.text != null || textPlaceHolder.text != "")
        //{
            //targetField.text = value.Substring(0, value.Length - 1);

            //targetField.text = targetField.text.Remove(targetField.text.Length - 1);

            textPlaceHolder.text = textPlaceHolder.text.Remove(textPlaceHolder.text.Length - 1);

            UpdateTargetText();
        }
    }

    private bool shift;
    public void ShiftButton()
    {
        if (!shift)
        {
            shift = true;
            
            foreach (Text character in charactersList)
            {
                character.text = character.text.ToUpper();
            }
        }else
        {
            shift = false;
            
            foreach (Text character in charactersList)
            {
                character.text = character.text.ToLower();
            }
        }
    }

    public void SwitchCharactersAndNumbers()
    {

        if (charactersPanel.activeSelf)
        {
            charactersPanel.SetActive(false);
            shiftButton.SetActive(false);
            numbersPanel.SetActive(true);
            numbersButtonText.text = "abc";
        } else
        {
            charactersPanel.SetActive(true);
            shiftButton.SetActive(true);
            numbersPanel.SetActive(false);
            numbersButtonText.text = "&123";
        }

    }


    public void OnKeyPressed(Button btn)
    {
        
        textPlaceHolder.text += btn.GetComponentInChildren<Text>().text;
        UpdateTargetText();
    }


    public void UpdateTargetText()
    {
        targetField.text = textPlaceHolder.text;
    }

    public void EnterButton()
    {
        UpdateTargetText();
        ExitPanel();
        
    }

    public void ExitPanel()
    {
        targetField = null;
        this.gameObject.SetActive(false);
    }


}
