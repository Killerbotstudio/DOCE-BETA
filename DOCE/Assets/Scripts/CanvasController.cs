using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] Texture2D pointerOver;
    [SerializeField] AudioClip click1;
    [SerializeField] AudioClip click2;
    [SerializeField] AudioClip alertSound;
    [SerializeField] AudioClip rejectSound;
    [SerializeField] AudioClip alert2Sound;
    [SerializeField] AudioSource source;


    private void Awake()
    {
        Debug.Log("Awake :)");
        CursorExit();
    }
    public void CursorEnter()
    {
        //Debug.Log("Sound Click2");
        Cursor.SetCursor(pointerOver, Vector2.zero, CursorMode.Auto);
        source.clip = click2;
        source.Play();
    }
    public void CursorExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
       
    }

    public void ClickSound()
    {
        //Debug.Log("Sound Click");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        source.clip = click1;
        source.Play();
    }
    public void SoundAlert()
    {
        source.clip = alertSound;
        source.Play();
    }
    public void SoundAlert2()
    {
        source.clip = alert2Sound;
        source.Play();
    }
    public void SoundReject()
    {
        source.clip = rejectSound;
        source.Play();
    }
    public void SoundTick()
    {
        source.clip = click2;
        source.Play();
    }
}
