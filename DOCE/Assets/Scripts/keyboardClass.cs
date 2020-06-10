using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;

public class keyboardClass : MonoBehaviour, ISelectHandler {

	public InputField input;


	[DllImport("__Internal")]
	private static extern void focusHandleAction (string _name, string _str);
	//private static extern void focusHandleAction(string _str);

	public void ReceiveInputData(string value) {
		input.text = value;
	}

	public void OnSelect(BaseEventData data) {
#if UNITY_WEBGL
		try{
			focusHandleAction (this.gameObject.name, input.text);
        //focusHandleAction (input.text);
		}
		catch(Exception error){Debug.Log("Errorrrrrrrrr");}
#endif
	}
}
