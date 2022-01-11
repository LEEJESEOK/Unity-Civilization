using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : Singleton<LoadScene>
{ 
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name == "StartScene")
        {
            SoundManager.instance.PlayBGM(SoundManager.BGM_TYPE.BGM_START);
        }       
    }
    public void StartGameBTN()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void ExitGameBTN()
    {
        Application.Quit();
    }
}
