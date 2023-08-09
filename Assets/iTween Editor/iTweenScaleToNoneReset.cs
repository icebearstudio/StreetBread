using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class iTweenScaleToNoneReset : iTweenEditor
{
    public Vector3 valueFrom = Vector3.zero;
    public Vector3 valueTo = Vector3.one;

    [System.Serializable]
    public class OnStart : UnityEvent
    {

    };

    public OnStart onStart;

    [System.Serializable]
    public class OnComplete : UnityEvent
    {

    };

    public OnComplete onComplete;

    private Vector3 _transition = Vector3.zero;

    // Use this for initialization
    void Awake()
    {
        if (this.autoPlay)
            this.iTweenPlay();
    }

    void OnEnable()
    {
        iTweenPlay();
    }

    public override void iTweenPlay()
    {
        Hashtable ht = new Hashtable();

        ht.Add("from", 0.0f);
        ht.Add("to", 1.0f);
        ht.Add("time", this.tweenTime);
        ht.Add("delay", this.waitTime);

        ht.Add("looptype", this.loopType);
        ht.Add("easetype", this.easeType);

        ht.Add("onstart", (Action<object>)(newVal =>
        {
            //_onUpdate(0.0f);
            if (onStart != null)
            {
                onStart.Invoke();
            }
        }));
        ht.Add("onupdate", (Action<object>)(newVal =>
        {
            _onUpdate((float)newVal);
        }));
        ht.Add("oncomplete", (Action<object>)(newVal =>
        {
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        }));

        ht.Add("ignoretimescale", ignoreTimescale);
        //this.transform.localScale = valueFrom;

        _transition = valueTo - valueFrom;
        iTween.ValueTo(this.gameObject, ht);
    }

    private void _onUpdate(float value)
    {
        this.transform.localScale = valueFrom + _transition * value;
    }
}