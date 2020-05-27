using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class OpenURL : MonoBehaviour
{
	public void OpenLinkJSPlugin(string url)
	{
		Debug.Log("OpenLink");
	#if !UNITY_EDITOR
		openWindow(url);
		return;
	#endif

		Application.OpenURL(url);

	}

	[DllImport("__Internal")]
    private static extern void openWindow(string url);
}
