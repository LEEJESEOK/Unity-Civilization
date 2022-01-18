using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : Singleton<LoadScene>
{
    void Update()
    {
        //victory
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(LoadSceneAsnyc("Victory"));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(LoadSceneAsnyc("Defeat"));
        }
    }
    public void StartGameBTN()
    {
        StartCoroutine(LoadSceneAsnyc("GameScene"));
    }
    public void ExitGameBTN()
    {
        Application.Quit();
    }
    public void BackToMainBTN()
    {
        StartCoroutine(LoadSceneAsnyc("StartScene"));
    }
    public void Victory()
    {
        SoundManager.instance.PlayBGM(SoundManager.BGM_TYPE.BGM_RESULT);
        SoundManager.instance.PlayEFT(SoundManager.EFT_TYPE.EFT_VICTORY);
        StartCoroutine(LoadSceneAsnyc("Defeat"));
    }
    public async void Defeat()
    {
        SoundManager.instance.PlayBGM(SoundManager.BGM_TYPE.BGM_RESULT);
        SoundManager.instance.PlayEFT(SoundManager.EFT_TYPE.EFT_DEFEAT);
        StartCoroutine(LoadSceneAsnyc("Defeat"));
    }

    IEnumerator LoadSceneAsnyc(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        asyncOperation.allowSceneActivation = true;

    }
}