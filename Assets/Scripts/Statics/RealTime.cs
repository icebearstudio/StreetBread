using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class RealTime : MonoBehaviour
{

    public static RealTime instance;
    private float i;
    public string dateString = "";
    public string dateStringOnline = "";
    public long seconds;
    public long secondsOnline;
    public string formatDate = "dd:MM:yyyy HH:mm:ss";
    public string defaultDate = "01:01:1970 12:00:00";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            return;
        }
        i = 0.25f;
        dateStringOnline = defaultDate;
        dateString = defaultDate;
        StartCoroutine(getTime());
        //StartCoroutine(getTimeOnlineLoop());
    }

    void Start()
    {
    }

    IEnumerator getTime()
    {
        //yield return new WaitForSeconds(0.1f);
        getTimeOffline();
        yield return new WaitForFixedUpdate();
        while (true)
        {
            getTimeOffline();
            yield return new WaitForSeconds(i);
        }
    }

    IEnumerator getTimeOnlineLoop()
    {
        yield return new WaitForSeconds(1.5f);
        while (true)
        {
            StartCoroutine(getTimeOnline());
            yield return new WaitForSeconds(60f);
        }
    }

    public IEnumerator getTimeOnline()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.2f);
        if (checkNetwork())
        {
            using (WWW www = new WWW("https://www.unixtimestamp.com/"))
            {
                yield return www;
                //print("WWW: " + www.text);
                if (www.text.Length > 0)
                {
                    //print("WWW: " + www.text);
                    try
                    {
                        string s = www.text.Split(new String[] { "<div class=\"value epoch\">" }, StringSplitOptions.None)[1].Split(new String[] { "</div>" }, StringSplitOptions.None)[0].Trim();
                        if (StaticVar.IsLong(s))
                        {
                            secondsOnline = long.Parse(s);
                            DateTime time = UnixTimeStampToDateTime(seconds);
                            time = time.ToUniversalTime();
                            dateStringOnline = time.ToString(formatDate);
                        }
                    }
                    catch
                    {
                        dateStringOnline = "01:01:1970 12:00:00";
                    }
                }
                else
                {
                    dateStringOnline = "01:01:1970 12:00:00";
                    // print("getTimeOffline");
                    // getTimeOffline();
                }
            }
        }
    }

    public void getTimeOffline()
    {
        try
        {
            DateTime time = DateTime.Now; // Use current time.
            time = time.ToUniversalTime();
            dateString = time.ToString(formatDate);
            //DateTimeOffset dto = new DateTimeOffset(DateTime.Now.Ticks, TimeSpan.Zero);
            //print(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + "   " + DateTimeOffset.Now.ToUnixTimeSeconds());
            //dto = dto.ToUniversalTime();
            //seconds = dto.ToUnixTimeSeconds();
            seconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        catch
        {
            dateString = "01:01:1970 12:00:00";
        }
    }

    public DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    public bool checkNetwork()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return false;
        else return true;
    }

    public string getShortDate(bool isOnline)
    {
        if (isOnline && dateStringOnline.Length > 0)
            return checkValidationDateString(dateStringOnline).Split(' ')[0];
        else
            return checkValidationDateString(dateString).Split(' ')[0];
    }

    public string getShortTime(bool isOnline)
    {
        if (isOnline && dateStringOnline.Length > 0 && checkValidationDateString(dateStringOnline).Split(' ').Length > 1)
            return checkValidationDateString(dateStringOnline).Split(' ')[1];
        else if (checkValidationDateString(dateStringOnline).Split(' ').Length > 1)
            return checkValidationDateString(dateString).Split(' ')[1];
        else return "";
    }

    public bool checkDateOnline()
    {
        return (checkNetwork() && getShortDate(true) != "01:01:1970" && dateStringOnline.Split(' ').Length == 2);
    }

    public string checkValidationDateString(string input)
    {
        if (input.Split(' ').Length == 2)
        {
            string[] strs = input.Split(' ');
            if (strs[0].Split(':').Length != 3) return defaultDate;
            else if (strs[1].Split(':').Length != 3) return defaultDate;
            else return input;
        }
        else return defaultDate;
    }
}