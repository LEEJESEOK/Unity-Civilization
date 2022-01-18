using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


public class SoundManager : Singleton<SoundManager>
{
    public enum BGM_TYPE
    {
        BGM_START,
        BGM_INGAME,
        BGM_RESULT
    }

    public enum EFT_TYPE
    {
        EFT_INFANTRY_WALK,
        EFT_CAVALRY_WALK,
        EFT_UNIT_COMBAT,
        EFT_BUILD,
        EFT_VICTORY,
        EFT_DEFEAT
    }

    //BGM
    public AudioSource audioS_BGM;
    public AudioClip[] bgmAudio;

    //EFT
    public AudioSource audioS_EFT;
    public AudioClip[] eftAudio;

    void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "StartScene":
                PlayBGM(BGM_TYPE.BGM_START);
                break;
            case "GameScene":
                PlayBGM(BGM_TYPE.BGM_INGAME);
                break;
            case "Victory":
                PlayBGM(BGM_TYPE.BGM_RESULT);
                PlayEFT(EFT_TYPE.EFT_VICTORY);
                break;
            case "Defeat":
                PlayBGM(BGM_TYPE.BGM_RESULT);
                PlayEFT(EFT_TYPE.EFT_DEFEAT);
                break;
        }
    }

    public void PlayBGM(BGM_TYPE type)
    {
        audioS_BGM.clip = bgmAudio[(int)type];
        audioS_BGM.Play();
    }
    public void StopBGM()
    {
        audioS_BGM.Pause();
    }

    public void StopEFT()
    {
        audioS_EFT.Pause();
    }

    public void PlayEFT(EFT_TYPE type)
    {
        audioS_EFT.PlayOneShot(eftAudio[(int)type]);
    }
}
