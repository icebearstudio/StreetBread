using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using System;
using UnityEngine.Experimental.Rendering;

public class SaveGame : MonoBehaviour
{

    public static SaveGame instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //reset();
            defaultKey();
            //checkTutorial();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //reset ();
    }

    void Start()
    {
        //checkTutorial ();
    }

    [ContextMenu("Reset")]
    public void reset()
    {
        PlayerPrefs.DeleteAll();
        defaultKey();
    }

    public bool checkTutorial()
    {
        string str = getKey(StaticVar.keySaveGame.Tutorial.ToString());
        string[] strs = str.Split(',');
        if (strs[0].Trim() == "0")
        {
            return false;
        }
        for (int i = 0; i < StaticVar.maxTutorialStep + 2; i++)
        {
            if (i >= StaticVar.maxTutorialStep + 1)
            {
                return true;
            }
            if (strs[i].Trim() == "0")
            {
                if (i > StaticVar.tutorialStepReset) return true;
                break;
            }
        }
        reset();
        return false;
    }

    public void defaultKey()
    {
        if (!checkKey("VersionDate"))
        {
            setKey(StaticVar.versionDate, "VersionDate");
        }
        if (!checkKey(StaticVar.keySaveGame.Cash.ToString()))
        {
            setKey(0, StaticVar.keySaveGame.Cash.ToString());
        }
        setKey(setDefaultSaveGame(), StaticVar.keySaveGame.DataSave.ToString());

        for (int i = 2; i <= 4; i++)
        {
            if (!checkKey(((StaticVar.keySaveGame)i).ToString()))
            {
                setKey(0, ((StaticVar.keySaveGame)i).ToString());
            }
        }
        if (!checkKey(StaticVar.keySaveGame.Tutorial.ToString()))
        {
            setKey(StaticVar.tutorialKey, StaticVar.keySaveGame.Tutorial.ToString());
        }

        save();
    }

    // JSONNode dataSave = JSONNode.Parse(getKey(StaticVar.keySaveGame.DataSave.ToString()));
    // setKey(dataSave.ToString(), StaticVar.keySaveGame.DataSave.ToString());
    string setDefaultSaveGame()
    {
        JSONNode dataSave = getDataSave();

        if (dataSave[StaticVar.keyDataSave.Level.ToString()] == null)
            dataSave[StaticVar.keyDataSave.Level.ToString()].AsInt = 0;

        if (dataSave[StaticVar.keyDataSave.LevelSpeedMove.ToString()] == null)
            dataSave[StaticVar.keyDataSave.LevelSpeedMove.ToString()].AsInt = 0;
        if (dataSave[StaticVar.keyDataSave.LevelCapacity.ToString()] == null)
            dataSave[StaticVar.keyDataSave.LevelCapacity.ToString()].AsInt = 0;
        if (dataSave[StaticVar.keyDataSave.LevelProfit.ToString()] == null)
            dataSave[StaticVar.keyDataSave.LevelProfit.ToString()].AsInt = 0;

        if (dataSave[StaticVar.keyDataSave.PhaseUnlock.ToString()] == null)
            dataSave[StaticVar.keyDataSave.PhaseUnlock.ToString()].AsInt = -1;

        return dataSave.ToString();
    }

    //   JSONNode dataSave = JSONNode.Parse(PlayerPrefs.GetString(StaticVar.keySaveGame.FaceCustomSave.ToString()));
    //  setKey(dataSave.ToString(), StaticVar.keySaveGame.FaceCustomSave.ToString());
    public void setValueNode(string keyWord, int value, bool isOverride)
    {
        JSONNode dataSave = getDataSave();
        if (dataSave[keyWord] == null)
            dataSave[keyWord].AsInt = 0;
        if (isOverride)
            dataSave[keyWord].AsInt = value;
        else
            dataSave[keyWord].AsInt += value;
        setDataSave(dataSave.ToString());
    }
    public void setValueNode(string keyWord, float value, bool isOverride)
    {
        JSONNode dataSave = getDataSave();
        if (dataSave[keyWord] == null)
            dataSave[keyWord].AsFloat = 0;
        if (isOverride)
            dataSave[keyWord].AsFloat = value;
        else
            dataSave[keyWord].AsFloat += value;
        setDataSave(dataSave.ToString());
    }
    public void setValueNode(string keyWord, string value, bool isOverride)
    {
        JSONNode dataSave = getDataSave();
        if (dataSave[keyWord] == null)
            dataSave[keyWord] = "";
        if (isOverride)
            dataSave[keyWord] = value;
        else
            dataSave[keyWord] += value;
        setDataSave(dataSave.ToString());
    }
    public void setValueNode(string keyWord, JSONNode value, bool isOverride)
    {
        JSONNode dataSave = getDataSave();
        dataSave[keyWord] = value;
        setDataSave(dataSave.ToString());
    }

    public bool hasValueNode(string keyWord)
    {
        JSONNode dataSave = getDataSave();
        if (dataSave[keyWord] != null)
            return true;
        else return false;
    }
    public int getValueNode(string keyWord, int value = 0)
    {
        JSONNode dataSave = getDataSave();
        if (dataSave[keyWord] != null)
            return dataSave[keyWord].AsInt;
        else return 0;
    }
    public float getValueNode(string keyWord, float value = 0)
    {
        JSONNode dataSave = getDataSave();
        if (dataSave[keyWord] != null)
            return dataSave[keyWord].AsFloat;
        else return 0;
    }
    public string getValueNode(string keyWord, string value = "")
    {
        JSONNode dataSave = getDataSave();
        if (dataSave[keyWord] != null)
            return dataSave[keyWord];
        else return "";
    }
    public JSONNode getValueNode(string keyWord, JSONObject value = null)
    {
        JSONNode dataSave = getDataSave();
        if (dataSave[keyWord] != null)
            return dataSave[keyWord];
        else return null;
    }

    public JSONNode getDataSave()
    {
        JSONNode dataSave = new JSONObject();
        string oldSave = PlayerPrefs.GetString(StaticVar.keySaveGame.DataSave.ToString());
        if (oldSave.Length > 0) dataSave = JSONNode.Parse(oldSave);
        return dataSave;
    }
    public void setDataSave(string dataSave)
    {
        setKey(dataSave, StaticVar.keySaveGame.DataSave.ToString());
    }
    //--------------------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------------------
    public JSONNode getSaveNode(string keySave, string keyNode, JSONObject value = null)
    {
        JSONNode dataSave = null;
        float timeCurrent = Time.realtimeSinceStartup;
        string oldSave = getKey(keySave.ToString());
        if (oldSave.Length > 0) dataSave = JSONNode.Parse(oldSave);
        else return null;
        if (keyNode.Length > 0)
        {
            if (dataSave[keyNode] != null)
            {
                return dataSave[keyNode];
            }
            else
            {
                return null;
            }
        }
        else
        {
            return dataSave;
        }
    }
    public void setSaveNode(string keySave, string keyNode, JSONNode value)
    {
        JSONNode dataSave = null;
        string oldSave = getKey(keySave.ToString());
        if (oldSave.Length > 0) dataSave = JSONNode.Parse(oldSave);
        else dataSave = new JSONObject();

        if (dataSave[keyNode] != null) dataSave[keyNode] = value;
        else dataSave.Add(keyNode, value);
        setKey(dataSave.ToString(), keySave);
    }
    public void setSaveNode(string keySave, string keyNode, int value, bool isOverride)
    {
        JSONNode dataSave = null;
        string oldSave = getKey(keySave.ToString());
        if (oldSave.Length > 0) dataSave = JSONNode.Parse(oldSave);
        else dataSave = new JSONObject();

        if (dataSave[keyNode] != null)
        {
            if (isOverride)
                dataSave[keyNode].AsInt = value;
            else
                dataSave[keyNode].AsInt += value;
        }
        else dataSave.Add(keyNode, value);
        print(dataSave.ToString());
        setKey(dataSave.ToString(), keySave);
    }
    public void setSaveNode(string keySave, string keyNode, string value)
    {
        JSONNode dataSave = null;
        float timeCurrent = Time.realtimeSinceStartup;
        string oldSave = getKey(keySave.ToString());
        if (oldSave.Length > 0) dataSave = JSONNode.Parse(oldSave);
        else dataSave = new JSONObject();
        if (dataSave[keyNode] != null) dataSave[keyNode] = value;
        else dataSave.Add(keyNode, value);
        setKey(dataSave.ToString(), keySave);
    }
    //--------------------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------------------
    public bool setKey(string str, string key)
    {
        PlayerPrefs.SetString(key, str);
        save();
        return true;
    }

    public bool setKey(int i, string key)
    {
        PlayerPrefs.SetInt(key, i);
        save();
        return true;
    }

    public string getKey(string key)
    {
        if (PlayerPrefs.HasKey(key))
            return PlayerPrefs.GetString(key);
        else
            return "";
    }

    public int getKey(string key, int i = 0)
    {
        if (PlayerPrefs.HasKey(key))
            return PlayerPrefs.GetInt(key);
        else
            return 0;
    }

    public void save()
    {
        PlayerPrefs.Save();
    }

    public void deleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        save();
    }


    public bool checkKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public int getCash()
    {
        return getKey(StaticVar.keySaveGame.Cash.ToString(), 0);
    }

    public bool checkCash(int i)
    {
        if (getKey(StaticVar.keySaveGame.Cash.ToString(), 0) >= Mathf.Abs(i))
        { return true; }
        else { return false; }
    }

    public bool changeCash(int i, string source = "")
    {
        if (i < 0)
        {
            if (getKey(StaticVar.keySaveGame.Cash.ToString(), 0) >= Mathf.Abs(i))
            {
                bool b = setKey(getKey(StaticVar.keySaveGame.Cash.ToString(), 0) + i, StaticVar.keySaveGame.Cash.ToString());
                UIController.instance.setCash();
                return b;
            }
            else
            {
                return false;
            }
        }
        else
        {
            bool b = setKey(getKey(StaticVar.keySaveGame.Cash.ToString(), 0) + i, StaticVar.keySaveGame.Cash.ToString());
            UIController.instance.setCash();
            return b;
        }
    }

    public bool checkRemoveAds()
    {
        return checkKey(StaticVar.keySaveGame.IsBuyRemoveAds.ToString());
    }

    public bool checkDoneTutorial()
    {
        return checkKey(StaticVar.keySaveGame.IsDoneTutorial.ToString());
    }

    // Save sprite
    public string getStringSprite(RenderTexture renderTexture)
    {
        Texture2D texture2D = toTexture2D(renderTexture);
        return getStringSprite(texture2D);
    }

    public string getStringSprite(Texture2D texture2D)
    {
        if (!texture2D.isReadable)
            texture2D = duplicateTexture(texture2D);
        byte[] texAsByte = texture2D.EncodeToPNG();
        string texAsString = Convert.ToBase64String(texAsByte);
        return texAsString;
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    Texture2D duplicateTexture(Texture2D source)
    {
        //RenderTexture renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        RenderTexture renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    public Sprite getSpriteByString(string dataSprite)
    {
        string base64Tex = dataSprite;
        if (!string.IsNullOrEmpty(base64Tex))
        {
            byte[] texByte = System.Convert.FromBase64String(base64Tex);
            Texture2D tex = new Texture2D(2, 2, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
            if (tex.LoadImage(texByte))
            {
                Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                return sprite;
            }
        }
        return null;
    }
}
