using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SoundController : MonoBehaviour
{

    public static SoundController instance;
    List<(string, List<AudioSource>)> audioSource;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        loadAudioSource();
    }

    public void loadAudioSource()
    {
        audioSource = new List<(string, List<AudioSource>)>();
        foreach (Transform t in transform.GetChild(0))
        {
            audioSource.Add((t.name, new List<AudioSource>()));
            AudioSource[] temp = t.GetComponents<AudioSource>();
            foreach (AudioSource a in temp)
            {
                audioSource[audioSource.Count - 1].Item2.Add(a);
            }
        }
    }

    public int getIndex(string name)
    {
        for (int i = 0; i < audioSource.Count; i++)
        {
            if (audioSource[i].Item1 == name)
            {
                return i;
            }
        }
        return -1;
    }

    void Start()
    {
        loadSettingPanel();
    }

    public void loadSettingPanel()
    {
        if (UIController.instance != null && UIController.instance.settingPanel != null)
        {
            Transform settingPanel = UIController.instance.settingPanel;
            if (SaveGame.instance.getKey(StaticVar.keySaveGame.Sound.ToString(), 0) == 0)
            {
                settingPanel.GetChild(0).GetChild(2).GetChild(1).gameObject.SetActive(false);
                settingPanel.GetChild(0).GetChild(2).GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                settingPanel.GetChild(0).GetChild(2).GetChild(1).gameObject.SetActive(true);
                settingPanel.GetChild(0).GetChild(2).GetChild(2).gameObject.SetActive(false);
            }
            if (SaveGame.instance.getKey(StaticVar.keySaveGame.Music.ToString(), 0) == 0)
            {
                settingPanel.GetChild(0).GetChild(3).GetChild(1).gameObject.SetActive(false);
                settingPanel.GetChild(0).GetChild(3).GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                settingPanel.GetChild(0).GetChild(3).GetChild(1).gameObject.SetActive(true);
                settingPanel.GetChild(0).GetChild(3).GetChild(2).gameObject.SetActive(false);
            }
            if (SaveGame.instance.getKey(StaticVar.keySaveGame.Vibration.ToString(), 0) == 0)
            {
                settingPanel.GetChild(0).GetChild(4).GetChild(1).gameObject.SetActive(false);
                settingPanel.GetChild(0).GetChild(4).GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                settingPanel.GetChild(0).GetChild(4).GetChild(1).gameObject.SetActive(true);
                settingPanel.GetChild(0).GetChild(4).GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    public void setMusic()
    {
        if (SaveGame.instance.getKey(StaticVar.keySaveGame.Music.ToString(), 0) == 0)
        {
            Vibrate(40);
            SaveGame.instance.setKey(1, StaticVar.keySaveGame.Music.ToString());
            turnOffMusic(StaticVar.keyMusic.BG_Menu);
        }
        else
        {
            Vibrate(40);
            SaveGame.instance.setKey(0, StaticVar.keySaveGame.Music.ToString());
            if (GameplayController.instance != null)
            {
                turnOnMusic(StaticVar.keyMusic.BG_Menu);
            }
        }
        loadSettingPanel();
    }

    public void setSound()
    {
        if (SaveGame.instance.getKey(StaticVar.keySaveGame.Sound.ToString(), 0) == 0)
        {
            Vibrate(40);
            SaveGame.instance.setKey(1, StaticVar.keySaveGame.Sound.ToString());
        }
        else
        {
            Vibrate(40);
            SaveGame.instance.setKey(0, StaticVar.keySaveGame.Sound.ToString());
        }
        loadSettingPanel();
    }

    public void setVibration()
    {
        if (SaveGame.instance.getKey(StaticVar.keySaveGame.Vibration.ToString(), 0) == 0)
        {
            SaveGame.instance.setKey(1, StaticVar.keySaveGame.Vibration.ToString());
        }
        else
        {
            SaveGame.instance.setKey(0, StaticVar.keySaveGame.Vibration.ToString());
            Vibrate(40);
        }
        loadSettingPanel();
    }

    public bool checkVibration()
    {
        if (SaveGame.instance.getKey(StaticVar.keySaveGame.Vibration.ToString(), 0) == 0)
        {
            return true;
        }
        else
            return false;
    }

    public bool checkMusic()
    {
        if (SaveGame.instance.getKey(StaticVar.keySaveGame.Sound.ToString(), 0) == 0)
        {
            return true;
        }
        else
            return false;
    }

    public bool checkSound()
    {
        if (SaveGame.instance.getKey(StaticVar.keySaveGame.Music.ToString(), 0) == 0)
        {
            return true;
        }
        else
            return false;
    }

    public void Vibrate(long milliseconds)
    {
#if !UNITY_EDITOR
        if (SoundController.instance.checkVibration())
            Vibration.Vibrate(milliseconds);
#endif
    }

    public bool checkTurnOnSoundMusic(int i)
    {
        if (i == 0)
        {
            if (SaveGame.instance.getKey(StaticVar.keySaveGame.Sound.ToString(), 0) == 0)
            {
                return true;
            }
            else
            {
                int length = Enum.GetNames(typeof(StaticVar.keySound)).Length;
                for (int j = 0; j < length; j++)
                {
                    turnOffSound((StaticVar.keySound)j);
                }
                return false;
            }
        }
        else
        {
            if (SaveGame.instance.getKey(StaticVar.keySaveGame.Music.ToString(), 0) == 0)
            {
                return true;
            }
            else
            {
                int length = Enum.GetNames(typeof(StaticVar.keyMusic)).Length;
                for (int j = 0; j < length; j++)
                {
                    turnOffMusic((StaticVar.keyMusic)j);
                }
                return false;
            }
        }
    }

    public bool checkMusic(StaticVar.keyMusic name)
    {
        int k = getIndex(name.ToString());
        if (k != -1)
            foreach (AudioSource i in audioSource[k].Item2)
            {
                if (i.clip.name == name.ToString())
                {
                    if (i.isPlaying)
                        return true;
                    else
                        return false;
                }
            }
        return false;
    }

    public bool checkSound(StaticVar.keySound name)
    {
        int k = getIndex(name.ToString());
        if (k != -1)
            foreach (AudioSource i in audioSource[k].Item2)
            {
                if (i.clip.name == name.ToString())
                {
                    if (i.isPlaying)
                        return true;
                    else
                        return false;
                }
            }
        return false;
    }

    public bool turnOnMusic(StaticVar.keyMusic name)
    {
        if (checkTurnOnSoundMusic(1))
        {
            int k = getIndex(name.ToString());
            if (k != -1)
                foreach (AudioSource i in audioSource[k].Item2)
                {
                    if (i.clip.name == name.ToString())
                    {
                        i.Play();
                        return true;
                    }
                }
        }
        else
        {
        }
        return false;
    }

    public bool turnOffMusic(StaticVar.keyMusic name)
    {
        int k = getIndex(name.ToString());
        if (k != -1)
            foreach (AudioSource i in audioSource[k].Item2)
            {
                if (i.clip != null && i.clip.name == name.ToString())
                {
                    if (i.isPlaying)
                    {
                        i.Stop();
                        return true;
                    }
                }
            }
        return false;
    }

    public bool turnOnSound(StaticVar.keySound name, bool isLoop)
    {
        if (checkTurnOnSoundMusic(0))
        {
            int k = getIndex(name.ToString());
            if (k != -1)
                foreach (AudioSource i in audioSource[k].Item2)
                {
                    if (i.clip != null && i.clip.name == name.ToString())
                    {
                        {
                            // if (!i.isPlaying)
                            {
                                if (!isLoop)
                                    i.PlayOneShot(i.clip);
                                else
                                {
                                    if (!i.isPlaying)
                                        i.Play();
                                }
                                return true;
                            }
                        }
                    }
                }
        }
        return false;
    }
    public bool turnOnSoundWhileOff(StaticVar.keySound name)
    {
        if (checkTurnOnSoundMusic(0))
        {
            int k = getIndex(name.ToString());
            if (k != -1)
                foreach (AudioSource i in audioSource[k].Item2)
                {
                    if (i.clip != null && i.clip.name == name.ToString())
                    {
                        {
                            if (!i.isPlaying)
                            {
                                i.PlayOneShot(i.clip);
                                return true;
                            }
                        }
                    }
                }
        }
        return false;
    }


    public bool turnOffSound(StaticVar.keySound name)
    {
        int k = getIndex(name.ToString());
        if (k != -1)
            foreach (AudioSource i in audioSource[k].Item2)
            {
                if (i.clip.name == name.ToString())
                {
                    if (i.isPlaying)
                    {
                        i.Stop();
                    }
                }
            }
        return false;
    }

    public void turnOnSound(StaticVar.keySound name, float time)
    {
        StartCoroutine(dlaySound(name, time));
    }

    IEnumerator dlaySound(StaticVar.keySound name, float time, bool isLoop = false)
    {
        yield return new WaitForSeconds(time);
        turnOnSound(name, isLoop);
    }

    public void turnOnMusic(StaticVar.keyMusic name, float time)
    {
        StartCoroutine(dlayMusic(name, time));
    }

    IEnumerator dlayMusic(StaticVar.keyMusic name, float time)
    {
        yield return new WaitForSeconds(time);
        turnOnMusic(name);
    }
}
