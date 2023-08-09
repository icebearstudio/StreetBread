using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class ScrollingVerticalUI : MonoBehaviour {
	public RectTransform yearsScrollingPanel;

	public GameObject yearsButtonPrefab;

	private GameObject[] yearsButtons;

	public RectTransform yearsCenter;

	UIVerticalScroller yearsVerticalScroller;
	public float coefficient;
	public float minCoefficient;
	public int starElement;
	private int yearsSet;

	private void InitializeYears () {
		int currentYear = int.Parse (System.DateTime.Now.ToString ("yyyy"));

		int[] arrayYears = new int[currentYear + 1 - 2000];

		yearsButtons = new GameObject[arrayYears.Length];

		for (int i = 0; i < arrayYears.Length; i++) {
			arrayYears [i] = 1900 + i;
			GameObject clone = (GameObject)Instantiate (yearsButtonPrefab, new Vector3 (0, i * 80, 0), Quaternion.Euler (new Vector3 (0, 0, 0))) as GameObject;
			clone.transform.SetParent (yearsScrollingPanel, false);
			clone.transform.localScale = new Vector3 (1, 1, 1);
			clone.GetComponentInChildren<Text> ().text = "" + arrayYears [i];
			clone.name = "Year_" + arrayYears [i];
			clone.AddComponent<CanvasGroup> ();
			//yearsButtons [i] = clone;
		}
	}

	// Use this for initialization
	public void Awake () {
		//InitializeYears ();

		int k = yearsScrollingPanel.childCount;
		yearsButtons = new GameObject[k];
		for (int i = 0; i < k; i++) {
			//yearsButtons [i] = yearsScrollingPanel.GetChild (k - i - 1).gameObject;
			yearsButtons [i] = yearsScrollingPanel.GetChild (i).gameObject;
		}
		yearsVerticalScroller = new UIVerticalScroller (yearsScrollingPanel, yearsButtons, yearsCenter, coefficient, minCoefficient, starElement);

		yearsVerticalScroller.Start ();
	}

	public void SetDate () {
		yearsVerticalScroller.SnapToElement (yearsSet);
	}

	void Update () {
		yearsVerticalScroller.Update ();
		string yearsString = yearsVerticalScroller.GetResults ();
	}

	public void YearsScrollUp () {
		yearsVerticalScroller.ScrollUp ();
	}

	public void YearsScrollDown () {
		yearsVerticalScroller.ScrollDown ();
	}
}
