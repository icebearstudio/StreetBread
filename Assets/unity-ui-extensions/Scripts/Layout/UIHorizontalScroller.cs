/// Credit Mrs. YakaYocha 
/// Sourced from - https://www.youtube.com/channel/UCHp8LZ_0-iCvl-5pjHATsgw
/// Please donate: https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RJ8D9FRFQF9VS

using UnityEngine.Events;

namespace UnityEngine.UI.Extensions {
	[RequireComponent (typeof(ScrollRect))]
	[AddComponentMenu ("Layout/Extensions/Vertical Scroller")]
	public class UIHorizontalScroller : MonoBehaviour {
		[Tooltip ("Scrollable area (content of desired ScrollRect)")]
		public RectTransform _scrollingPanel;
		[Tooltip ("Elements to populate inside the scroller")]
		public GameObject[] _arrayOfElements;
		[Tooltip ("Center display area (position of zoomed content)")]
		public RectTransform _center;
		[Tooltip ("Select the item to be in center on start. (optional)")]
		public int StartingIndex = -1;
		[Tooltip ("Button to go to the next page. (optional)")]
		public GameObject ScrollUpButton;
		[Tooltip ("Button to go to the previous page. (optional)")]
		public GameObject ScrollDownButton;
		[Tooltip ("Event fired when a specific item is clicked, exposes index number of item. (optional)")]
		public UnityEvent<int> ButtonClicked;


		private float[] distReposition;
		private float[] distance;
		//private int elementsDistance;
		private int minElementsNum;
		private int elementLength;
		//private int elementHalfLength;
		private float deltaX;
		private string result;
		float coefficient;
		float minCoefficient = 0.7f;
		int starElement = 0;

		public UIHorizontalScroller () {
		}

		public UIHorizontalScroller (RectTransform scrollingPanel, GameObject[] arrayOfElements, RectTransform center, float coefficient) {
			_scrollingPanel = scrollingPanel;
			_arrayOfElements = arrayOfElements;
			_center = center;
			this.coefficient = coefficient;
		}

		public UIHorizontalScroller (RectTransform scrollingPanel, GameObject[] arrayOfElements, RectTransform center, float coefficient, float minCoefficient) {
			_scrollingPanel = scrollingPanel;
			_arrayOfElements = arrayOfElements;
			_center = center;
			this.coefficient = coefficient;
			this.minCoefficient = minCoefficient;
		}

		public UIHorizontalScroller (RectTransform scrollingPanel, GameObject[] arrayOfElements, RectTransform center, float coefficient, float minCoefficient, int starElement) {
			_scrollingPanel = scrollingPanel;
			_arrayOfElements = arrayOfElements;
			_center = center;
			this.coefficient = coefficient;
			this.minCoefficient = minCoefficient;
			this.starElement = starElement;
		}

		public void Awake () {
			var scrollRect = GetComponent<ScrollRect> ();
			if (!_scrollingPanel) {
				_scrollingPanel = scrollRect.content;
			}
			if (!_center) {
				Debug.LogError ("Please define the RectTransform for the Center viewport of the scrollable area");
			}
			if (_arrayOfElements == null || _arrayOfElements.Length == 0) {
				var childCount = scrollRect.content.childCount;
				if (childCount > 0) {
					_arrayOfElements = new GameObject[childCount];
					for (int i = 0; i < childCount; i++) {
						_arrayOfElements [i] = scrollRect.content.GetChild (i).gameObject;
					}                    
				}
			}
		}

		public void Start () {
			if (_arrayOfElements.Length < 1) {
				return;
			}

			elementLength = _arrayOfElements.Length;
			distance = new float[elementLength];
			distReposition = new float[elementLength];

			deltaX = _arrayOfElements [starElement].GetComponent<RectTransform> ().rect.width * elementLength / 3 * 2;
			deltaX = _arrayOfElements [starElement].GetComponent<RectTransform> ().localPosition.x;
			Vector2 startPosition = new Vector2 (-deltaX, _scrollingPanel.anchoredPosition.y);
			//startPosition = new Vector2 (_center.sizeDelta.x / 2f, _scrollingPanel.anchoredPosition.y);
			_scrollingPanel.anchoredPosition = startPosition;

			for (var i = 0; i < _arrayOfElements.Length; i++) {
				AddListener (_arrayOfElements [i], i);
			}

			if (ScrollUpButton)
				ScrollUpButton.GetComponent<Button> ().onClick.AddListener (() => {
					ScrollUp ();
				});

			if (ScrollDownButton)
				ScrollDownButton.GetComponent<Button> ().onClick.AddListener (() => {
					ScrollDown ();
				});

			if (StartingIndex > -1) {
				StartingIndex = StartingIndex > _arrayOfElements.Length ? _arrayOfElements.Length - 1 : StartingIndex;
				SnapToElement (StartingIndex);
			}
		}

		private void AddListener (GameObject button, int index) {
			button.GetComponent<Button> ().onClick.AddListener (() => DoSomething (index));
		}

		private void DoSomething (int index) {
			if (ButtonClicked != null) {
				ButtonClicked.Invoke (index);
			}
		}

		public void Update () {
			if (_arrayOfElements.Length < 1) {
				return;
			}

			for (var i = 0; i < elementLength; i++) {
				distReposition [i] = _center.GetComponent<RectTransform> ().position.x - _arrayOfElements [i].GetComponent<RectTransform> ().position.x;
				distance [i] = Mathf.Abs (distReposition [i]);

				//Magnifying effect
				float scale = Mathf.Max (minCoefficient, 1 / (1 + distance [i] / coefficient));
				//Debug.Log (_arrayOfElements [i].name + "    " + scale);
				_arrayOfElements [i].GetComponent<RectTransform> ().transform.localScale = new Vector3 (scale, scale, 1f);
			}
			float minDistance = Mathf.Min (distance);

			for (var i = 0; i < elementLength; i++) {
				if (minDistance == distance [i]) {
					minElementsNum = i;
					//result = _arrayOfElements [i].GetComponentInChildren<Text> ().text;
				}
			}
			ScrollingElements (-_arrayOfElements [minElementsNum].GetComponent<RectTransform> ().anchoredPosition.x);
		}

		private void ScrollingElements (float position) {
			float newY = Mathf.Lerp (_scrollingPanel.anchoredPosition.x, position, Time.deltaTime * 10f);
			Vector2 newPosition = new Vector2 (newY, _scrollingPanel.anchoredPosition.y);
			_scrollingPanel.anchoredPosition = newPosition;
		}

		public string GetResults () {
			return result;
		}

		public void SnapToElement (int element) {
			float deltaElementPositionY = _arrayOfElements [0].GetComponent<RectTransform> ().rect.width * element;
			Vector2 newPosition = new Vector2 (-deltaElementPositionY, _scrollingPanel.anchoredPosition.y);
			_scrollingPanel.anchoredPosition = newPosition;

		}

		public void ScrollUp () {
			float deltaUp = _arrayOfElements [0].GetComponent<RectTransform> ().rect.width / 1.2f;
			Vector2 newPositionUp = new Vector2 (_scrollingPanel.anchoredPosition.x - deltaUp, _scrollingPanel.anchoredPosition.y);
			_scrollingPanel.anchoredPosition = Vector2.Lerp (_scrollingPanel.anchoredPosition, newPositionUp, 1);
		}

		public void ScrollDown () {
			float deltaDown = _arrayOfElements [0].GetComponent<RectTransform> ().rect.width / 1.2f;
			Vector2 newPositionDown = new Vector2 (_scrollingPanel.anchoredPosition.x + deltaDown, _scrollingPanel.anchoredPosition.y);
			_scrollingPanel.anchoredPosition = newPositionDown;
		}
	}
}