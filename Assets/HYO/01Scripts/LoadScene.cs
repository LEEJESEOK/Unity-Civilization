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
    void Update()
    {     
        //victory
        if (Input.GetKeyDown(KeyCode.V))
        {
            SoundManager.instance.PlayBGM(SoundManager.BGM_TYPE.BGM_RESULT);
            SoundManager.instance.PlayEFT(SoundManager.EFT_TYPE.EFT_VICTORY);
            SceneManager.LoadScene("Victory");
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
    public void BackToMainBTN()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void Victory()
    {
        SoundManager.instance.PlayBGM(SoundManager.BGM_TYPE.BGM_RESULT);
        SoundManager.instance.PlayEFT(SoundManager.EFT_TYPE.EFT_VICTORY);
        SceneManager.LoadScene("Victory");
    }
}
