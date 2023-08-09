using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SimpleJSON;

public class DontDestroyOnload : MonoBehaviour
{

    public static DontDestroyOnload instance;
    public int level = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        JSONNode dataSave = SaveGame.instance.getDataSave();
        level = SaveGame.instance.getValueNode(StaticVar.keyDataSave.Level.ToString(), 0);
    }
}
