using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.UI;

public static partial class StaticVar
{
    // version history note
    public static string versionDate = "2023/08/05 12:00";
    public static bool IsTestMode = false;

    public enum keySaveGame
    {
        Cash, DataSave, Sound, Music, Vibration,
        Tutorial, IsBuyBestPack, IsBuyRemoveAds, IsDoneTutorial, IsRestorePurchase,
        IsRateGame, IsStartPlay
    };

    public enum keyDataSave
    {
        None, Level, Cash, LevelSpeedMove, LevelCapacity, LevelProfit, PhaseUnlock
    };

    public enum keyMusic { BG_Menu };
    //int length = Enum.GetNames(typeof(StaticVar.keySound)).Length;
    //keyMusic keyMusic = (keyMusic)System.Enum.Parse(typeof(keyMusic), "");

    public enum keySound
    {
        Button, BuyClaim, sfx_drop, sfx_machines, sfx_money, sfx_order, sfx_pick,
        sfx_noise_1, sfx_noise_2, sfx_noise_3, sfx_noise_4, sfx_noise_5
    };
    //(StaticVar.keySound)System.Enum.Parse(typeof(StaticVar.keySound), "DinoSound_" + dinoAttackScriptEnemy.dinoDataDefault.ID + "_Hit")

    public static string tutorialKey = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";
    public static int maxTutorialStep = 22;
    public static int tutorialStepReset = 8;

    public enum PlatformType { UnityEditor, Android, IOS };
    public static PlatformType GetPlatform()
    {
        //return PlatformType.IOS;
#if UNITY_EDITOR
        {
#if UNITY_IOS
            return PlatformType.IOS;
#elif UNITY_ANDROID
            return PlatformType.Android;
#else
            return PlatformType.UnityEditor;
#endif
        }
#elif UNITY_ANDROID
        return PlatformType.Android;
#elif UNITY_IOS
        return PlatformType.IOS;
#endif
    }

    public static bool isTestVersion()
    {
        if (Application.identifier.IndexOf("test") >= 0)
            return true;
        else return false;
    }

    public static Transform getTransformByName(string nameTransform, Transform parent)
    {
        foreach (Transform item in parent)
            if (item.name == nameTransform) return item;
        return null;
    }

    public static List<int> getListByString(string s, string stringSplit)
    {
        List<int> result = new List<int>();
        if (s != null && s.Length > 0)
        {
            string[] strs = s.Split(stringSplit);
            for (int i = 0; i < strs.Length; i++)
            {
                strs[i] = strs[i].Trim();
                if (strs[i].Length > 0 && IsInt(strs[i]))
                    result.Add(int.Parse(strs[i]));
            }
        }
        return result;
    }

    public static string convertCashIntToString(int i)
    {
        if (i < 1000)
        {
            return i.ToString();
        }
        else if (i < 1000000)
        {
            if (((i % 1000) / 100) == 0)
                return (i / 1000) + "K";
            else return (i / 1000) + "." + ((i % 1000) / 100) + "K";
        }
        else if (i < 1000000000)
        {
            if (((i % 1000000) / 100000) == 0)
                return i / 1000000 + "M";
            else return (i / 1000000) + "." + ((i % 1000000) / 100000) + "M";
        }
        else
            return "0";
    }

    public static string convertGoldIntToString(string k)
    {
        int i = int.Parse(k);
        if (i < 10000)
        {
            return i.ToString();
        }
        else if (i < 1000000)
        {
            return i / 1000 + "K";
        }
        else if (i < 1000000000)
        {
            return i / 1000000 + "M";
        }
        else
            return "0";
    }

    public static string convertGoldIntToString(int i)
    {
        if (i < 10000)
        {
            return i.ToString();
        }
        else if (i < 1000000)
        {
            return i / 1000 + "K";
        }
        else if (i < 1000000000)
        {
            return i / 1000000 + "M";
        }
        else
            return "0";
    }

    public static List<int> splitCoinValue(int value, int splitValue)
    {
        List<int> listSplit = new List<int>();
        int k = value / splitValue;
        for (int i = 0; i < k; i++)
        {
            listSplit.Add(splitValue);
        }
        if (value % splitValue > 0)
            listSplit.Add(value % splitValue);
        return listSplit;
    }

    public static int totalArrayValue(int[] arr)
    {
        int total = 0;
        foreach (int i in arr) total += i;
        return total;
    }
    public static int totalArrayValue(JSONNode arr)
    {
        int total = 0;
        foreach (JSONNode i in arr) total += i.AsInt;
        return total;
    }

    public static int randomNumber(int[] probabilityArray)
    {
        return randomNumber(probabilityArray, totalArrayValue(probabilityArray));
    }

    public static string setArrayString(int[] probabilityArray)
    {
        string s = "{ ";
        foreach (int i in probabilityArray) s += i + " , ";
        s += " }";
        return s;
    }

    public static int randomNumber(int[] probabilityArray, int k)
    {
        List<int> randomNumber = NumberListController.instance.randomNumber;
        int i = randomNumber[UnityEngine.Random.Range(0, randomNumber.Count)] % k;
        List<int> intelligenceList = new List<int>();
        for (int j = 0; j < probabilityArray.Length; j++)
        {
            intelligenceList.Add(probabilityArray[j]);
        }
        int[] array = new int[intelligenceList.Count];
        for (int j = 0; j < intelligenceList.Count; j++)
        {
            if (j == 0)
            {
                array[0] = intelligenceList[0];
            }
            else
            {
                array[j] = intelligenceList[j] + array[j - 1];
            }
        }
        for (int j = 0; j < array.Length; j++)
        {
            if (i < array[j])
                return j;
        }
        return 0;
    }

    public static string randomId(string name)
    {
        string id = name;
        int length = UnityEngine.Random.Range(20, 30);
        length = 20;
        for (int i = 0; i < length; i++)
        {
            //char let = (char)('a' + Random.Range(0, 25));
            //id += let;
            id += UnityEngine.Random.Range(0, 10);
        }
        return id;
    }

    public static string loadText(string path)
    {
        TextAsset txt = (TextAsset)Resources.Load(path, typeof(TextAsset));
        return txt.text;
    }

    public static JSONNode loadTextData(string path)
    {
        TextAsset txt = (TextAsset)Resources.Load(path, typeof(TextAsset));
        return JSONArray.Parse(txt.text);
    }

    public static JSONNode getSave(int key)
    {
        JSONNode gunSave = JSONArray.Parse(SaveGame.instance.getKey(((StaticVar.keySaveGame)key).ToString()));
        return gunSave;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static void fitImage(Image image, Vector2 sizeFit)
    {
        image.SetNativeSize();
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        Vector2 sizeDelta = rectTransform.sizeDelta;
        Vector2 sizeDeltaNew = Vector2.zero;
        if (sizeFit.x / sizeDelta.x * sizeDelta.y > sizeFit.y)
        {
            sizeDeltaNew = new Vector2(sizeFit.y / sizeDelta.y * sizeDelta.x, sizeFit.y);
        }
        else
        {
            sizeDeltaNew = new Vector2(sizeFit.x, sizeFit.x / sizeDelta.x * sizeDelta.y);
        }
        rectTransform.sizeDelta = sizeDeltaNew;
    }

    public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 diference = (vec2 - vec1).normalized;
        float angle = Vector2.Angle(new Vector2(0f, 1f), diference);
        if (vec1.x <= vec2.x && vec1.y <= vec2.y)
        { }
        else if (vec1.x > vec2.x && vec1.y <= vec2.y)
        {
            angle *= -1;
        }
        else if (vec1.x <= vec2.x && vec1.y > vec2.y)
        { }
        else if (vec1.x > vec2.x && vec1.y > vec2.y)
        {
            angle *= -1;
        }
        return angle;
    }

    public static Transform setActiveChildTransform(GameObject[] list, int index)
    {
        Transform t = null;
        for (int i = 0; i < list.Length; i++)
        {
            if (i == index)
            {
                list[i].SetActive(true);
                t = list[i].transform;
            }
            else list[i].SetActive(false);
        }
        return t;
    }

    public static Transform setActiveChildTransform(List<GameObject> list, int index)
    {
        Transform t = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (i == index)
            {
                list[i].SetActive(true);
                t = list[i].transform;
            }
            else list[i].SetActive(false);
        }
        return t;
    }

    public static Transform setActiveChildTransform(Transform t_Parent, int index)
    {
        Transform t = null;
        for (int i = 0; i < t_Parent.childCount; i++)
        {
            if (i == index)
            {
                t_Parent.GetChild(i).gameObject.SetActive(true);
                t = t_Parent.GetChild(i);
            }
            else t_Parent.GetChild(i).gameObject.SetActive(false);
        }
        return t;
    }

    public static Transform setActiveChildTransform(Transform t_Parent, string nameChild)
    {
        Transform t = null;
        for (int i = 0; i < t_Parent.childCount; i++)
        {
            if (t_Parent.GetChild(i).name == nameChild)
            {
                t_Parent.GetChild(i).gameObject.SetActive(true);
                t = t_Parent.GetChild(i);
            }
            else t_Parent.GetChild(i).gameObject.SetActive(false);
        }
        return t;
    }

    public static Transform getTransformFree(Transform parent)
    {
        Transform result = null;
        foreach (Transform item in parent)
        {
            if (!item.gameObject.activeSelf)
            {
                result = item;
                break;
            }
        }
        return result;
    }

    public static string convertTimeLength(int i)
    {
        return (i.ToString().Length == 1 ? "0" : "") + i.ToString();
    }

    public static bool IsInt(string s)
    {
        if (String.IsNullOrEmpty(s))
            return false;
        int i;
        return int.TryParse(s, out i);
    }

    public static bool IsFloat(string s)
    {
        if (String.IsNullOrEmpty(s))
            return false;
        float i;
        return float.TryParse(s, out i);
    }

    public static bool IsLong(string s)
    {
        if (String.IsNullOrEmpty(s))
            return false;
        long i;
        return long.TryParse(s, out i);
    }

    public static bool IsDouble(string s)
    {
        if (String.IsNullOrEmpty(s))
            return false;
        double i;
        return double.TryParse(s, out i);
    }

    public static bool compareDateTime(string firstDate, string secondDate)
    {
        try
        {
            string[] date_1 = firstDate.Split(' ')[0].Split('/');
            string[] date_2 = secondDate.Split(' ')[0].Split('/');
            string[] time_1 = firstDate.Split(' ')[1].Split(':');
            string[] time_2 = secondDate.Split(' ')[1].Split(':');

            if (int.Parse(date_1[0]) > int.Parse(date_2[0]))
                return true;
            else if (int.Parse(date_1[0]) == int.Parse(date_2[0]))
            {
                if (int.Parse(date_1[1]) > int.Parse(date_2[1]))
                    return true;
                else if (int.Parse(date_1[1]) == int.Parse(date_2[1]))
                {
                    if (int.Parse(date_1[2]) > int.Parse(date_2[2]))
                        return true;
                    else if (int.Parse(date_1[2]) == int.Parse(date_2[2]))
                    {
                        if (int.Parse(time_1[0]) > int.Parse(time_2[0]))
                            return true;
                        else if (int.Parse(time_1[0]) == int.Parse(time_2[0]))
                        {
                            if (int.Parse(time_1[1]) > int.Parse(time_2[1]))
                                return true;
                            else if (int.Parse(time_1[1]) == int.Parse(time_2[1]))
                            {
                                return false;
                            }
                            else if (int.Parse(time_1[1]) < int.Parse(time_2[1]))
                                return false;
                        }
                        else if (int.Parse(time_1[0]) < int.Parse(time_2[0]))
                            return false;
                    }
                    else if (int.Parse(date_1[2]) < int.Parse(date_2[2]))
                        return false;
                }
                else if (int.Parse(date_1[1]) < int.Parse(date_2[1]))
                    return false;
            }
            else if (int.Parse(date_1[0]) < int.Parse(date_2[0]))
                return false;

            return false;
        }
        catch (Exception) { return false; }
    }

    // public static Color getColorByHex(string hex)
    // {
    //     System.Drawing.Color _color = System.Drawing.ColorTranslator.FromHtml(hex);
    //     return new Color(_color.R / 255f, _color.G / 255f, _color.B / 255f, _color.A / 255f);
    // }

    public static int getRandom(int count)
    {
        if (count == 0) return 0;
        return UnityEngine.Random.Range(0, 100000) % count;
    }

    public static List<string> EnumToList<T>()
    {
        var array = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
        List<string> lst = null;
        for (int i = 0; i < array.Length; i++)
        {
            if (lst == null)
                lst = new List<string>();
            string value = array[i].ToString();
            lst.Add(value);
        }
        return lst;
    }

    public static void openAppByPackageName(string packageName)
    {
        if (StaticVar.GetPlatform() == StaticVar.PlatformType.Android)
        {
            string bundleId = packageName;
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
            ca.Call("startActivity", launchIntent);
            up.Dispose();
            ca.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();
        }
        else if (StaticVar.GetPlatform() == StaticVar.PlatformType.IOS)
        {

        }
    }

    public static void openSetting_WIRELESS_SETTINGS()
    {
        if (StaticVar.GetPlatform() == StaticVar.PlatformType.Android)
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject =
                unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var intentObject = new AndroidJavaObject(
                //"android.content.Intent", "android.settings.WIFI_SETTINGS"))
                "android.content.Intent", "android.settings.WIRELESS_SETTINGS"))
            {
                currentActivityObject.Call("startActivity", intentObject);
            }
        }
        else if (StaticVar.GetPlatform() == StaticVar.PlatformType.IOS)
        {

        }
    }
}

public class Parameter
{
    public string key = "";
    public string string_value = "";
    public int int_value = -1;
    public float float_value = -1;

    public bool is_int_value;
    public bool is_float_value;
    public bool is_string_value;

    public Parameter(string key, string valueString)
    {
        this.key = key;
        this.string_value = valueString;
        this.is_string_value = true;
    }
    public Parameter(string key, int valueInt)
    {
        this.key = key;
        this.int_value = valueInt;
        this.is_int_value = true;
    }
    public Parameter(string key, float valueFloat)
    {
        this.key = key;
        this.float_value = valueFloat;
        this.is_float_value = true;
    }

    public Parameter(string key, int valueInt, float valueFloat, string valueString)
    {
        this.key = key;
        this.int_value = valueInt;
        this.float_value = valueFloat;
        this.string_value = valueString;
        this.is_int_value = true;
        this.is_float_value = true;
        this.is_string_value = true;
    }
}