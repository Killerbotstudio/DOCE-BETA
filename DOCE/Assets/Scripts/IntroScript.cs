using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{


    public Image doceImage;


    public AudioSource source;
    public AudioClip pieceClip;
    public AudioClip whooshClip;
    public AudioClip whooshClip2;

    public Text welcomeText;
    public Text versionText;
    public Image line;
    public Text R;
    public Image RCover;

    
    public Image loadingImage;
    
    private Color textColor;

    private void Start()
    {
        textColor = versionText.color;
        versionText.color = Color.clear;
        welcomeText.color = Color.clear;
        StartCoroutine(IntroRoutine());
        
    }


    IEnumerator IntroRoutine()
    {

        yield return new WaitForSeconds(1.5f);

        //TEXT ANIM
        source.volume = 0.3f;
        source.clip = whooshClip;
        source.Play();
        while (doceImage.transform.localPosition.y < 0f)
        {
            doceImage.transform.localPosition = Vector2.MoveTowards(doceImage.transform.localPosition, Vector2.zero, 10);
            //doceImage.transform.localPosition = Vector2.Lerp(doceImage.transform.localPosition, Vector2.zero, Time.deltaTime * 10f);
            yield return null;
        }

       // yield return new WaitForSeconds(0.5f);

        //LINE ANIM
        source.clip = whooshClip;
        source.Play();
        while (line.rectTransform.sizeDelta.x < 280)
        {
            line.rectTransform.sizeDelta = Vector2.MoveTowards(line.rectTransform.sizeDelta, new Vector2(280f, 3.5f),  20f);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        R.enabled = true;
        source.volume = 2f;
        source.clip = pieceClip;
        source.Play();

        yield return new WaitForSeconds(1f);

        //TEXT COLORS
        versionText.enabled = true;
        welcomeText.enabled = true;
        while (versionText.color != textColor)
        {
            welcomeText.color = Color.Lerp(welcomeText.color, textColor, Time.deltaTime * 5);
            versionText.color = Color.Lerp(versionText.color, textColor, Time.deltaTime * 5);
            yield return null;
        }


        //yield return new WaitForSeconds(0);

        SceneManager.LoadScene("MainScene");

    }
}
