using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class OpenLink : MonoBehaviour
{
    public void OpenLinkJSPlugin(string url)
    {
       // #if !UNITY_EDITOR
       //openWindow(url);
       //#endif
       if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            openWindow(url);
        }
        else
        {
            
            Application.OpenURL(url);
        }
    }

    [DllImport("__Internal")]
    private static extern void openWindow(string url);
    
}
