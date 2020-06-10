//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class InputFieldAddOn : MonoBehaviour
{
    private InputField selectedInput;

    [DllImport("__Internal")]
    private static extern void OpenInputKeyboard(string str);
    [DllImport("__Internal")]
    private static extern void CloseInputKeyboard();
    [DllImport("__Internal")]
    private static extern void FixInputOnBlur();
    [DllImport("__Internal")]
    private static extern void FixInputUpdate();


    public void Deselect(InputField input)
    {
        Debug.Log("INPUT DESELECTED");
        selectedInput = null;
    }

    public void Select(InputField input)
    {

        Debug.Log("INPUT SELECTED");
        selectedInput = input;
        
        //TouchScreenKeyboard.Open(input.text);
        
    }

    public void ReceiveInputChange(string value)
    {
        if(selectedInput != null)
        {
            selectedInput.text = value;
        }
    }

    public void FocusInput(InputField input)
    {
        Debug.Log("INPUT SELECTED");
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (Application.isMobilePlatform)
            {
                Debug.Log("WEBGL is MOBILE");
                TouchScreenKeyboard.Open(input.text);
            }
            selectedInput = input;
            OpenInputKeyboard(input.text);
        }
    }

    public void LoseFocus()
    {
        if (selectedInput != null)
        {
            selectedInput = null;
        }  
    }

    public void ForceClose()
    {
        Debug.Log("INPUT DESELECTED");
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            
            CloseInputKeyboard();
        }
    }

    public void IsMobile()
    {
        TouchScreenKeyboard.Open(selectedInput.text);
    }




    
}
