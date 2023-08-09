using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class ScrollingHorizontalUI : MonoBehaviour {
	public RectTransform yearsScrollingPanel;

	public GameObject yearsButtonPrefab;

	private GameObject[] yearsButtons;

	public RectTransform yearsCenter;

	UIHorizontalScroller yearsVerticalScroller;
	public float coefficient;
	public float minCoefficient;
	public int starElement;

	private int yearsSet;

	// Use this for initialization
	public void Awake () {
		//InitializeYears ();
		setUp ();
	}

	public void setUp () {
		int k = yearsScrollingPanel.childCount;
		yearsButtons = new GameObject[k];
		for (int i = 0; i < k; i++) {
			yearsButtons [i] = yearsScrollingPanel.GetChild (i).gameObject;
		}
		yearsVerticalScroller = new UIHorizontalScroller (yearsScrollingPanel, yearsButtons, yearsCenter, coefficient, minCoefficient, starElement);

		yearsVerticalScroller.Start ();
	}

	void Update () {
		yearsVerticalScroller.Update ();
	}

	public void YearsScrollUp () {
		yearsVerticalScroller.ScrollUp ();
	}

	public void YearsScrollDown () {
		yearsVerticalScroller.ScrollDown ();
	}
}
