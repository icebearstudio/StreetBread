using UnityEngine;
using System.Collections;
#if UNITY_IOS && !UNITY_EDITOR
using System.Collections;
using System.Runtime.InteropServices;
#endif

public static class Vibration
{

#if UNITY_ANDROID && !UNITY_EDITOR
	public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
	
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport ( "__Internal" )]
        private static extern bool _HasVibrator ();

        [DllImport ( "__Internal" )]
        private static extern void _Vibrate ();

        [DllImport ( "__Internal" )]
        private static extern void _VibratePop ();

        [DllImport ( "__Internal" )]
        private static extern void _VibratePeek ();

        [DllImport ( "__Internal" )]
        private static extern void _VibrateNope ();
#endif

    public static void VibratePop()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _VibratePop ();
#elif UNITY_ANDROID && !UNITY_EDITOR
		Vibrate(15);
#endif
    }

    ///<summary>
    /// Small peek vibration
    ///</summary>
    public static void VibratePeek()
    {
#if UNITY_IOS && !UNITY_EDITOR
        _VibratePeek ();
#elif UNITY_ANDROID && !UNITY_EDITOR
		Vibrate ( 25 );
#endif
    }

    ///<summary>
    /// 3 small vibrations
    ///</summary>
    public static void VibrateNope()
    {
#if UNITY_IOS && !UNITY_EDITOR
       _VibrateNope ();
#elif UNITY_ANDROID && !UNITY_EDITOR
		long [] pattern = { 0, 5, 5, 5 };
		Vibrate( pattern, -1 );
#endif
    }

    public static void Vibrate()
    {
        if (isAndroid())
            vibrator.Call("vibrate");
        else
        {
#if UNITY_IOS && !UNITY_EDITOR
        VibratePop();
#elif !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }
    }


    public static void Vibrate(long milliseconds)
    {
        if (isAndroid())
            vibrator.Call("vibrate", milliseconds);
        else
        {
#if UNITY_IOS && !UNITY_EDITOR
        VibratePop();
#elif !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
        if (isAndroid())
            vibrator.Call("vibrate", pattern, repeat);
        else
        {
#if UNITY_IOS && !UNITY_EDITOR
        VibratePop();
#elif !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }
    }

    public static bool HasVibrator()
    {
        return isAndroid();
    }

    public static void Cancel()
    {
        if (isAndroid())
            vibrator.Call("cancel");
    }

    private static bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		return true;
#else
        return false;
#endif
    }
}

