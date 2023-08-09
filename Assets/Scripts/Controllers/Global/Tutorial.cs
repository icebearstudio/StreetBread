using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    public static Tutorial instance;
    public Transform tutorialPanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            tutorialPanel = transform.GetChild(0);
            //StartCoroutine(delayCheckTutorial());
        }
        else
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(delayCheckTutorial());
    }

    public int checkTutorial(string s = "")
    {
        string str = SaveGame.instance.getKey(StaticVar.keySaveGame.Tutorial.ToString());
        string[] strs = str.Split(',');
        for (int i = 0; i < strs.Length; i++)
        {
            if (strs[i].Trim() == "0")
            {
                //Debug.Log("check  " + i);
                return i;
            }
        }
        return strs.Length + 1;
    }

    public bool checkTutorial(int k)
    {
        string str = SaveGame.instance.getKey(StaticVar.keySaveGame.Tutorial.ToString());
        string[] strs = str.Split(',');
        for (int i = 0; i < StaticVar.maxTutorialStep + 2; i++)
        {
            if (i >= StaticVar.maxTutorialStep + 1)
            {
                return true;
            }
            if (strs[i].Trim() == "0")
            {
                break;
            }
        }
        SaveGame.instance.setKey(StaticVar.tutorialKey, StaticVar.keySaveGame.Tutorial.ToString());
        return false;
    }

    public bool checkTutorial()
    {
        string str = SaveGame.instance.getKey(StaticVar.keySaveGame.Tutorial.ToString());
        string[] strs = str.Split(',');
        for (int i = 0; i < StaticVar.maxTutorialStep + 2; i++)
        {
            if (i >= StaticVar.maxTutorialStep + 1)
            {
                return true;
            }
            if (strs[i].Trim() == "0")
            {
                if (!tutorialPanel.GetChild(i).gameObject.activeSelf)
                {
                    tutorialPanel.GetChild(i).gameObject.SetActive(true);
                }
                break;
            }
        }
        if (!SaveGame.instance.checkKey(StaticVar.keySaveGame.IsDoneTutorial.ToString()))
            StartCoroutine(delayCheckTutorial());
        return false;
    }

    IEnumerator delayCheckTutorial()
    {
        yield return new WaitForSeconds(0.2f);
        checkTutorial();
    }

    public void setTutorial(int i)
    {
        string str = SaveGame.instance.getKey(StaticVar.keySaveGame.Tutorial.ToString());
        string[] strs = str.Split(',');
        for (int j = 0; j <= i; j++)
        {
            strs[j] = "1";
        }
        str = strs[0];
        for (int j = 1; j < strs.Length; j++)
        {
            str += "," + strs[j];
        }
        SaveGame.instance.setKey(str, StaticVar.keySaveGame.Tutorial.ToString());
        if (tutorialPanel.GetChild(i) != null)
            tutorialPanel.GetChild(i).gameObject.SetActive(false);
        if (i == StaticVar.maxTutorialStep && !SaveGame.instance.checkKey(StaticVar.keySaveGame.IsDoneTutorial.ToString()))
        {
            SaveGame.instance.setKey(0, StaticVar.keySaveGame.IsDoneTutorial.ToString());
            //UIController.instance.logEvent(StaticVar.eventKey.A1_HitMaster_DoneTutorial);
        }
        // if (!SaveGame.instance.checkDoneTutorial())
        // {
        //     if (i == 4 || i == 8 || i == 11 || i == 15)
        //     {
        //         //UIController.instance.logEvent(StaticVar.eventKey.A1_HitMaster_TutorialStep_.ToString() + i);
        //     }
        // }
    }
}