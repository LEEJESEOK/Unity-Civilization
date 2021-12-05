using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class mapLoadManager : MonoBehaviour
{
	public GameObject LeftPanel;
	public GameObject RightPanel;
	public GameObject GameLodeMap;

    public bool gameMode;

    private void Start()
    {
        //if (gameMode)
        //{
        //    LeftPanel.SetActive(false);
        //    RightPanel.SetActive(false);
        //    GameLodeMap.SetActive(true);
        //}
        //else
        //{
        //    GameLodeMap.SetActive(false);
        //}

        if (gameMode)
        {
            SaveLoadMenu.instance.Load(Application.persistentDataPath + "/map1.map");
        }
        else
        {

        }

    }
}
