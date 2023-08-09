// Module Name:		iTweenPositionTo.cs
// Project:			iTween Editor br Vortex Game Studios
// Version:			1.00.00
// Developed by:	Alexandre Ribeiro de Sá (@themonkeytail)
// Copyright(c) Vortex Game Studios LTDA ME.
// http://www.vortexstudios.com
// 
// iTween Position To component
// Use this component to create position tween from your component.
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

public class iTweenPositionPath : iTweenEditor {
	public string path = "";

	[System.Serializable]
	public class OnStart : UnityEvent {

	};

	public OnStart onStart;

	[System.Serializable]
	public class OnComplete : UnityEvent {

	};

	public OnComplete onComplete;

	// Use this for initialization
	void Awake () {
		if (this.autoPlay)
			this.iTweenPlay ();
	}

	void OnEnable () {
		iTweenPlay ();
	}

	public override void iTweenPlay () {
		Invoke ("delay", 0.2f);
	}

	void delay () {
		Hashtable ht = new Hashtable ();

		ht.Add ("path", iTweenPath.GetPath (path));
		ht.Add ("time", this.tweenTime);
		ht.Add ("delay", this.waitTime);

		ht.Add ("looptype", this.loopType);
		ht.Add ("easetype", this.easeType);

		ht.Add ("onstart", (Action<object>)(newVal => {
			_onUpdate (0.0f);
			if (onStart != null) {
				onStart.Invoke ();
			}
		}));
		ht.Add ("oncomplete", (Action<object>)(newVal => {
			if (onComplete != null) {
				onComplete.Invoke ();
			}
		}));

		ht.Add ("ignoretimescale", ignoreTimescale);

		iTween.MoveTo (this.gameObject, ht);
	}

	private void _onUpdate (float value) {
		//this.transform.localPosition = valueFrom + _transition * value;
	}
}