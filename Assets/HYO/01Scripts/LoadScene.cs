using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : Singleton<LoadScene>
{
    void Start()
    {
   
    }

    void Update()
    {
        
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
