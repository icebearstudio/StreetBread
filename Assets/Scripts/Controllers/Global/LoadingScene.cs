using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene instance;
    public GameObject loadingPanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        loadingPanel.SetActive(true);
        StartCoroutine(loadScene("Gameplay", loadingPanel.transform.GetChild(1).GetChild(0).GetComponent<Image>(), loadingPanel.transform.GetChild(1).GetChild(1).GetComponent<Text>()));
        //Debug.Log("loadMapPrefab TimeStart:  " + Time.realtimeSinceStartup);
    }

    public IEnumerator loadScene(string nameScene, Image imageProcess, Text textProcess)
    {
        if (textProcess != null) textProcess.text = "0%";
        if (imageProcess != null) imageProcess.fillAmount = 0f;
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        yield return new WaitForSeconds(0f);
        //StartCoroutine(loadingLog());
        int fakeStep = UnityEngine.Random.Range(0, 10000) % 20 + 60;
        for (int i = 0; i < fakeStep; i++)
        {
            yield return new WaitForSeconds(0.04f);
            if (textProcess != null) textProcess.text = "" + i + "%";
            if (imageProcess != null) imageProcess.fillAmount = i * 0.01f;
        }
        yield return new WaitForSeconds(3.2f - 0.04f * fakeStep);
        yield return new WaitForSeconds(0.8f);
        AsyncOperation async = SceneManager.LoadSceneAsync(nameScene);
        while (!async.isDone)
        {
            int process = Mathf.RoundToInt(60 * (async.progress / 0.9f)) + fakeStep;
            if (textProcess != null) textProcess.text = Mathf.Min(100, process) + "%";
            if (imageProcess != null) imageProcess.fillAmount = Mathf.Min(1f, process / 100f);
            yield return null;
        }
    }
}
