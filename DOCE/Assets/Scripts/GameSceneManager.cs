using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{

    public void MenuScene()
    {
        
        SceneManager.LoadScene("MainScene");
    }
    public void GameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void CreditScene()
    {
        SceneManager.LoadScene("TestScene");
    }
    public void IntroScene()
    {
        SceneManager.LoadScene("IntroScene");
    }

}
