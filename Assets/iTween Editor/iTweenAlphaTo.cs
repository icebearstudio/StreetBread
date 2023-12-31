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

public class iTweenAlphaTo : iTweenEditor
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

    private SpriteRenderer _spriteRenderer = null;
    private Image _uiImage = null;
    private RawImage _uiRawImage = null;
    private Text _uiText = null;
    private CanvasGroup _uiCanvasGroup = null;
    private Material _MaterialMesh = null;

    // Use this for initialization
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _uiImage = GetComponent<Image>();
        _uiRawImage = GetComponent<RawImage>();
        _uiText = GetComponent<Text>();
        _uiCanvasGroup = GetComponent<CanvasGroup>();
        if (GetComponent<MeshRenderer>() != null)
            _MaterialMesh = GetComponent<MeshRenderer>().material;

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
            //_onUpdate (this.valueFrom);
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
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, valueFrom);
            }

            if (_uiImage != null)
            {
                _uiImage.color = new Color(_uiImage.color.r, _uiImage.color.g, _uiImage.color.b, valueFrom);
            }

            if (_uiRawImage != null)
            {
                _uiRawImage.color = new Color(_uiRawImage.color.r, _uiRawImage.color.g, _uiRawImage.color.b, valueFrom);
            }

            if (_uiText != null)
            {
                _uiText.color = new Color(_uiText.color.r, _uiText.color.g, _uiText.color.b, valueFrom);
            }

            if (_uiCanvasGroup != null)
            {
                _uiCanvasGroup.alpha = valueFrom;
            }

            if (_MaterialMesh != null)
            {
                _MaterialMesh.color = new Color(_MaterialMesh.color.r, _MaterialMesh.color.g, _MaterialMesh.color.b, valueFrom);
            }
        }

        iTween.ValueTo(this.gameObject, ht);
    }

    private void _onUpdate(float value)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, value);
        }

        if (_uiImage != null)
        {
            _uiImage.color = new Color(_uiImage.color.r, _uiImage.color.g, _uiImage.color.b, value);
        }

        if (_uiRawImage != null)
        {
            _uiRawImage.color = new Color(_uiRawImage.color.r, _uiRawImage.color.g, _uiRawImage.color.b, value);
        }

        if (_uiText != null)
        {
            _uiText.color = new Color(_uiText.color.r, _uiText.color.g, _uiText.color.b, value);
        }

        if (_uiCanvasGroup != null)
        {
            _uiCanvasGroup.alpha = value;
        }

        if (_MaterialMesh != null)
        {
            _MaterialMesh.color = new Color(_MaterialMesh.color.r, _MaterialMesh.color.g, _MaterialMesh.color.b, value);
        }
    }

    private void _onStart(float value)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, value);
        }

        if (_uiImage != null)
        {
            _uiImage.color = new Color(_uiImage.color.r, _uiImage.color.g, _uiImage.color.b, value);
        }

        if (_uiRawImage != null)
        {
            _uiRawImage.color = new Color(_uiRawImage.color.r, _uiRawImage.color.g, _uiRawImage.color.b, value);
        }

        if (_uiText != null)
        {
            _uiText.color = new Color(_uiText.color.r, _uiText.color.g, _uiText.color.b, value);
        }

        if (_uiCanvasGroup != null)
        {
            _uiCanvasGroup.alpha = value;
        }

        if (_MaterialMesh != null)
        {
            _MaterialMesh.color = new Color(_MaterialMesh.color.r, _MaterialMesh.color.g, _MaterialMesh.color.b, value);
        }
    }
}