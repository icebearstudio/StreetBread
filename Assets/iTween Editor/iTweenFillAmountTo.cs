﻿// Module Name:		iTweenAlphaTo.cs
// Project:			iTween Editor br Vortex Game Studios
// Version:			1.00.00
// Developed by:	Alexandre Ribeiro de Sá (@themonkeytail)
// Copyright(c) Vortex Game Studios LTDA ME.
// http://www.vortexstudios.com
// 
// iTween Alpha To component
// Use this component to create alpha tween from your component.
// 1.00.00 - First build
// 
// Check every tools and plugins we made for Unity at
// https://www.assetstore.unity3d.com/en/publisher/4888
// 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.


using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class iTweenFillAmountTo : iTweenEditor
{
    public float valueFrom = 0.0f;
    public float valueTo = 1.0f;

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

    private Image _uiImage = null;

    // Use this for initialization
    void Awake()
    {
        _uiImage = GetComponent<Image>();

        if (this.autoPlay)
            this.iTweenPlay();
    }

    void OnEnable()
    {
        iTweenPlay();
    }

    public override void iTweenPlay()
    {
        _onStart(valueFrom);
        Hashtable ht = new Hashtable();

        ht.Add("from", this.valueFrom);
        ht.Add("to", this.valueTo);
        ht.Add("time", this.tweenTime);
        ht.Add("delay", this.waitTime);

        ht.Add("looptype", this.loopType);
        ht.Add("easetype", this.easeType);

        ht.Add("onstart", (Action<object>)(newVal =>
        {
            _onUpdate(this.valueFrom);
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
        {
            if (_uiImage != null)
            {
                _uiImage.fillAmount = valueFrom;
            }
        }

        iTween.ValueTo(this.gameObject, ht);
    }

    private void _onUpdate(float value)
    {
        if (_uiImage != null)
        {
            _uiImage.fillAmount = value;
        }
    }

    private void _onStart(float value)
    {
        if (_uiImage != null)
        {
            _uiImage.fillAmount = value;
        }
    }
}