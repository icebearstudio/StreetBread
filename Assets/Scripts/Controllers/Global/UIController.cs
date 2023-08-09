using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Events;
using System;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public GameObject noticationTextPanel;
    public Text noticationText;
    public iTween.EaseType iTweenEaseTypeOff;
    public GameObject blackBG, whiteFade, notRaycast, notRaycastItem;
    public Transform settingPanel, ratePanel;
    public Transform popupEarn, popupEarnPack, loadingScenePanel;
    public Text FPSText;
    public List<Canvas> canvas = new List<Canvas>();
    public List<Camera> cameras = new List<Camera>();
    public List<GameObject> noAdsGameObject = new List<GameObject>();
    public GameObject noInternetNoticationGameObject, adsNotReadyNoticationGameObject;
    public GameObject loadingAdsPanel;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        //setOrthographicSize();
        Application.targetFrameRate = 60;
        if (loadingScenePanel != null)
            loadingScenePanel.gameObject.SetActive(true);
    }

    void Start()
    {
        init();
        StartCoroutine(delayActiveScene());
    }

    IEnumerator delayActiveScene()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);
        if (loadingScenePanel != null)
            loadingScenePanel.gameObject.SetActive(false);
    }

    public float sizeOrthographic = 9.6f;
    public float rateScreenToCamera;
    public bool setOrthographicSize(float size)
    {
        bool ishorizontalFieldOfView = true;
        float f = Screen.height / (Screen.width / 9f);
        if (f >= 15)
        {
            sizeOrthographic = size * (1f * Screen.height / Screen.width) * 9f;
            if (Camera.main != null)
                Camera.main.orthographicSize = sizeOrthographic;
            foreach (Camera c in cameras)
            {
                c.orthographicSize = sizeOrthographic;
            }
            foreach (Canvas c in canvas)
            {
                c.GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
            }
            ishorizontalFieldOfView = true;
        }
        else
        {
            sizeOrthographic = size * (1f * Screen.width / Screen.height) * 16f * (16f / 9f);
            if (Camera.main != null)
                Camera.main.orthographicSize = sizeOrthographic;
            foreach (Camera c in cameras)
            {
                c.orthographicSize = sizeOrthographic;
            }
            foreach (Canvas c in canvas)
            {
                c.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
            }
            ishorizontalFieldOfView = false;
        }
        //rateScreenToCamera = (Camera.main.orthographicSize / (Mathf.Sqrt(Screen.height * Screen.height + Screen.width * Screen.width)));
        return ishorizontalFieldOfView;
    }

    public void setupCamPer(Camera camera, float horizontalFieldOfView)
    {
        if (GameplayController.instance == null) return;
        camera.fieldOfView = Camera.HorizontalToVerticalFieldOfView(horizontalFieldOfView, Screen.width * 1f / Screen.height);
    }
    public void setupCamPerVertical(Camera camera, float verticalFieldOfView)
    {
        if (GameplayController.instance == null) return;
        camera.fieldOfView = verticalFieldOfView;
    }

    public void init()
    {
        iTweenEaseTypeOff = iTween.EaseType.easeOutQuint;
        Invoke("setNoAds", 0.1f);
        if (SoundController.instance != null)
            SoundController.instance.loadSettingPanel();
        setCash();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            SaveGame.instance.reset();
            reset(3);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            reset(SceneManager.GetActiveScene().name);
        }
    }

    public IEnumerator delayCountNumber(float timeDelay, Text text, int start, int finish, int count, UnityEvent e = null, UnityEvent delayEvent = null)
    {
        text.text = "" + start;
        int i = (finish - start) / count;
        for (int k = 0; k < count; k++)
        {
            text.text = "" + (i * k + start);
            if (delayEvent != null) delayEvent.Invoke();
            yield return new WaitForSeconds(timeDelay);
        }
        text.text = "" + finish;
        if (e != null)
            e.Invoke();
    }

    public void shakeCamera(Vector2 amount, float time, float delay = 0)
    {
        ShakePosition(Camera.main.gameObject, amount, time, delay);
    }

    public void reset(string nameScene)
    {
        SceneManager.LoadSceneAsync(nameScene);
    }

    public void reset(int i)
    {
        if (i == 0)
        {
            if (loadingScenePanel != null)
                StartCoroutine(loadScene("LoadingScene", loadingScenePanel.gameObject, loadingScenePanel.GetChild(1).GetChild(1).GetComponent<Text>(), loadingScenePanel.GetChild(1).GetChild(0).GetComponent<Image>()));
            else SceneManager.LoadSceneAsync("LoadingScene");
        }
        else if (i == 1)
        {
            if (loadingScenePanel != null)
                StartCoroutine(loadScene("Gameplay", loadingScenePanel.gameObject, loadingScenePanel.GetChild(1).GetChild(1).GetComponent<Text>(), loadingScenePanel.GetChild(1).GetChild(0).GetComponent<Image>()));
            else SceneManager.LoadSceneAsync("Gameplay");
        }
        else if (i == 2)
        {
        }
    }

    public void pause(bool b)
    {
        if (b)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void setNotRaycast()
    {
        if (!notRaycast.activeSelf)
        {
            notRaycast.SetActive(true);
            StartCoroutine(ienumeratorDelaySetActive(notRaycast, 0.2f, false));
        }
    }

    public void setVip()
    {
    }

    public void setNoAds()
    {
        foreach (GameObject g in noAdsGameObject)
        {
            g.SetActive(!SaveGame.instance.checkRemoveAds());
        }
    }

    public List<Transform> cashPos;
    public void setCash()
    {
        int cash = SaveGame.instance.getKey(StaticVar.keySaveGame.Cash.ToString(), 0);
        string cashString = StaticVar.convertCashIntToString(cash);
        foreach (Transform t in cashPos)
        {
            t.GetChild(0).GetComponent<Text>().text = cashString + "";
        }
    }
    public List<Transform> coinPos;
    public void setCoin()
    {
        foreach (Transform t in coinPos)
        {
            t.GetChild(0).GetComponent<Text>().text = SaveGame.instance.getKey(StaticVar.keySaveGame.Cash.ToString(), 0) + "";
        }
    }

    private IEnumerator loadScene(string sceneName, GameObject loadingPanel = null, Text textProcess = null, Image imageProcess = null)
    {
        //Time.timeScale = 1f;
        if (textProcess != null) textProcess.text = "0%";
        if (imageProcess != null) imageProcess.fillAmount = 0f;
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            int process = Mathf.RoundToInt(100 * (async.progress / 0.9f));
            if (textProcess != null) textProcess.text = Mathf.Min(100, process) + "%";
            if (imageProcess != null) imageProcess.fillAmount = Mathf.Min(1f, process / 100f);
            yield return null;
        }
    }

    public void setNoticationText(string s)
    {
        noticationText.text = s;
        noticationTextPanel.SetActive(true);
    }

    public void setNoticationText(string s, float f)
    {
        noticationText.text = s;
        noticationTextPanel.SetActive(true);
        StartCoroutine(ienumeratorDelaySetActive(noticationTextPanel, f, false));
    }

    public void setNoInternetNoti()
    {
        if (noInternetNoticationGameObject.activeSelf) noInternetNoticationGameObject.SetActive(false);
        noInternetNoticationGameObject.SetActive(true);
    }

    public void setAdsNotReadyNoti()
    {
        if (adsNotReadyNoticationGameObject.activeSelf) adsNotReadyNoticationGameObject.SetActive(false);
        adsNotReadyNoticationGameObject.SetActive(true);
    }

    public void setNotiAds()
    {
        if (!RealTime.instance.checkNetwork()) UIController.instance.setNoInternetNoti();
        else UIController.instance.setAdsNotReadyNoti();
    }

    public void setPopupEarn(int gold = 0, int gear = 0, string itemID = "", int itemCount = 0)
    {
        if (gold != 0)
        {
            popupEarn.GetChild(0).GetChild(0).gameObject.SetActive(true);
            popupEarn.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "+" + gold;
        }
        else
        {
            popupEarn.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        if (gear != 0)
        {
            popupEarn.GetChild(0).GetChild(1).gameObject.SetActive(true);
            popupEarn.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = "+" + gear;
        }
        else
        {
            popupEarn.GetChild(0).GetChild(1).gameObject.SetActive(false);
        }
        if (itemCount != 0)
        {
            popupEarn.GetChild(0).GetChild(2).gameObject.SetActive(true);
            popupEarn.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Sprites\\Item\\" + itemID);
            popupEarn.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = "+" + itemCount;
        }
        else
        {
            popupEarn.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }
        popupEarn.gameObject.SetActive(true);
    }


    public void setPopupEarnPack(List<(int, int, string)> anonymousType)
    {
        foreach (Transform t in popupEarnPack.GetChild(0))
        {
            t.GetChild(0).gameObject.SetActive(false);
            t.GetChild(1).gameObject.SetActive(false);
            t.GetChild(2).gameObject.SetActive(false);
        }
        for (int i = 0; i < anonymousType.Count; i++)
        {
            if (i < popupEarnPack.GetChild(0).childCount)
            {
                Transform tf = popupEarnPack.GetChild(0).GetChild(i);
                if (anonymousType[i].Item1 == 0)
                {
                    tf.GetChild(0).gameObject.SetActive(true);
                    tf.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = anonymousType[i].Item2 + "";
                }
                else if (anonymousType[i].Item1 == 1)
                {
                    tf.GetChild(1).gameObject.SetActive(true);
                    tf.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = anonymousType[i].Item2 + "";
                }
                else if (anonymousType[i].Item1 == 2)
                {
                    tf.GetChild(2).gameObject.SetActive(true);
                    tf.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = anonymousType[i].Item2 + "";
                    tf.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("Sprites\\Item\\" + anonymousType[i].Item3);
                }
                popupEarnPack.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
        }
        popupEarnPack.gameObject.SetActive(true);
    }

    public void CurveMove(GameObject target, Vector3 destination, float maxOffset, float time, string type, UnityEvent isDone, float delayTime, bool isDontDestroy = false, bool isRotate = true, bool isDisable = true)
    { // type = "X" cong theo X, type = "Y" cong theo Y
        StartCoroutine(CurveMoveCoroutine(target, destination, maxOffset, time, type, isDone, delayTime, isDontDestroy, isRotate, isDisable));
    }
    private IEnumerator CurveMoveCoroutine(GameObject target, Vector3 destination, float maxOffset, float time, string type, UnityEvent isDone, float delayTime, bool isDontDestroy = false, bool isRotate = true, bool isDisable = true)
    {
        float moveProgress = 0f;
        yield return new WaitForSeconds(delayTime);
        if (target == null) yield break;
        var startPos = target.transform.position;
        while (moveProgress <= 1.0)
        {
            if (target == null) break;
            moveProgress += (Time.deltaTime * (1f + Mathf.Sqrt(moveProgress))) / time;
            if (type == "Y")
            {
                var height = Mathf.Sin(Mathf.PI * moveProgress) * maxOffset;
                if (height < 0f)
                {
                    height = 0;
                }
                Vector2 temp = target.transform.position;
                target.transform.position = Vector3.Lerp(startPos, destination, moveProgress) + Vector3.up * height;
                if (isRotate)
                {
                    float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                yield return null;
            }
            else if (type == "X")
            {
                var width = Mathf.Sin(Mathf.PI * moveProgress) * maxOffset;
                if (startPos.x > destination.x)
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, moveProgress) + Vector3.right * width;
                    if (isRotate)
                    {
                        float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                        target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                    }
                }
                else
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, moveProgress) - Vector3.right * width;
                    if (isRotate)
                    {
                        float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                        target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                    }
                }
                yield return null;
            }
        }
        if (target != null)
            target.transform.position = destination;
        if (!isDontDestroy && target != null)
            Destroy(target);
        if (isDone != null)
            isDone.Invoke();
        if (isDisable && target != null && target.activeSelf)
            target.SetActive(false);
    }

    public void CurveMove(GameObject target, Transform destination, float maxOffset, float time, string type, UnityEvent isDone, float delayTime, bool isDontDestroy = false, bool isRotate = true, bool isDisable = true)
    { // type = "X" cong theo X, type = "Y" cong theo Y
        StartCoroutine(CurveMoveCoroutine(target, destination, maxOffset, time, type, isDone, delayTime, isDontDestroy, isRotate, isDisable));
    }
    private IEnumerator CurveMoveCoroutine(GameObject target, Transform destination, float maxOffset, float time, string type, UnityEvent isDone, float delayTime, bool isDontDestroy = false, bool isRotate = true, bool isDisable = true)
    {
        float moveProgress = 0f;
        yield return new WaitForSeconds(delayTime);
        if (target == null) yield break;
        var startPos = target.transform.position;
        while (moveProgress <= 1.0)
        {
            if (target == null || destination == null) break;
            moveProgress += (Time.deltaTime * (1f + Mathf.Sqrt(moveProgress))) / time;
            if (type == "Y")
            {
                var height = Mathf.Sin(Mathf.PI * moveProgress) * maxOffset;
                if (height < 0f)
                {
                    height = 0;
                }
                Vector2 temp = target.transform.position;
                target.transform.position = Vector3.Lerp(startPos, destination.position, moveProgress) + Vector3.up * height;
                if (isRotate)
                {
                    float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                yield return null;
            }
            else if (type == "X")
            {
                var width = Mathf.Sin(Mathf.PI * moveProgress) * maxOffset;
                if (startPos.x > destination.position.x)
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination.position, moveProgress) + Vector3.right * width;
                    if (isRotate)
                    {
                        float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                        target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                    }
                }
                else
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination.position, moveProgress) - Vector3.right * width;
                    if (isRotate)
                    {
                        float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                        target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                    }
                }
                yield return null;
            }
        }
        if (target != null)
            target.transform.position = destination.position;
        if (!isDontDestroy && target != null)
            Destroy(target);
        if (isDone != null)
            isDone.Invoke();
        if (isDisable && target != null && target.activeSelf)
            target.SetActive(false);
    }

    public void Throw(GameObject target, Vector3 destination, float maxOffset, float time, UnityEvent e)
    {
        StartCoroutine(ThrowCoroutine(target, destination, maxOffset, time, e));
    }

    private IEnumerator ThrowCoroutine(GameObject target, Vector3 destination, float maxOffset, float time, UnityEvent e)
    {
        float ThrowProgress = 0.0f;
        var startPos = target.transform.position;
        Vector3 tempScale = target.transform.localScale;
        while (ThrowProgress <= 1.0)
        {
            if (ThrowProgress <= 0.5f)
            {
                ThrowProgress += (Time.deltaTime * (1f + ThrowProgress * ThrowProgress)) / time;
            }
            else if (ThrowProgress >= 0.5f)
            {
                ThrowProgress += (Time.deltaTime * (1f + Mathf.Sqrt(ThrowProgress - 0.5f))) / time;
            }
            Vector2 temp = target.transform.position;
            target.transform.position = Vector3.Lerp(startPos, destination, ThrowProgress);
            if (ThrowProgress < 0.5f)
            {
                target.transform.localScale = tempScale * (1f + maxOffset * ThrowProgress * 2);
            }
            else
            {
                target.transform.localScale = tempScale * (1f + maxOffset - maxOffset * (ThrowProgress - 0.5f) * 2);
            }
            yield return null;
        }
        target.transform.localScale = tempScale;
        target.transform.position = destination;
        Destroy(target);
        if (e != null)
            e.Invoke();
    }

    public void delayInvoke(UnityEvent e, float timeDelay)
    {
        StartCoroutine(delayInvokeIEnumerator(e, timeDelay));
    }

    public IEnumerator delayInvokeIEnumerator(UnityEvent e, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        e.Invoke();
    }


    private IEnumerator activeJumpCoroutine1;
    public float JumpProgress1 { get; private set; }
    public void Jump1(GameObject target, Vector3 destination, float maxOffset, float time, int type, UnityEvent e, float delayTime, bool isDontDestroy = false, bool isRotate = true)
    {
        if (activeJumpCoroutine1 != null)
        {
            StopCoroutine(activeJumpCoroutine1);
            activeJumpCoroutine1 = null;
            JumpProgress1 = 0.0f;
        }
        activeJumpCoroutine1 = JumpCoroutine1(target, destination, maxOffset, time, type, e, delayTime, isDontDestroy);
        StartCoroutine(activeJumpCoroutine1);
    }
    private IEnumerator JumpCoroutine1(GameObject target, Vector3 destination, float maxOffset, float time, int type, UnityEvent e, float delayTime, bool isDontDestroy = false, bool isRotate = true)
    {
        yield return new WaitForSeconds(delayTime);
        var startPos = target.transform.position;
        while (JumpProgress1 <= 1.0)
        {
            JumpProgress1 += (Time.deltaTime * (1f + Mathf.Sqrt(JumpProgress1))) / time;
            if (type == 0)
            {
                var height = Mathf.Sin(Mathf.PI * JumpProgress1) * maxOffset;
                if (height < 0f)
                {
                    height = 0;
                }
                if (startPos.y > destination.y)
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, JumpProgress1) - Vector3.up * height;
                    float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                else
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, JumpProgress1) + Vector3.up * height;
                    float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                yield return null;
            }
            else
            {
                var width = Mathf.Sin(Mathf.PI * JumpProgress1) * maxOffset;
                if (width < 0f)
                {
                    width = 0;
                }
                if (startPos.x > destination.x)
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, JumpProgress1) + Vector3.right * width;
                    //float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    //target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                else
                {
                    Vector2 temp = target.transform.position;
                    target.transform.position = Vector3.Lerp(startPos, destination, JumpProgress1) - Vector3.right * width;
                    //float angle = Vector2.SignedAngle(Vector2.right, temp - (Vector2)target.transform.position);
                    //target.transform.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
                }
                yield return null;
            }
        }
        target.transform.position = destination;
        if (!isDontDestroy)
            Destroy(target);
        if (e != null)
            e.Invoke();
    }

    // -------------------------------------------------------------------------------------
    public void popupScale(GameObject g, Vector2 vec, float time)
    {
        g.transform.localScale = vec;
        iTween.ScaleTo(g, iTween.Hash("scale", Vector3.one, "time", time, "easetype", iTween.EaseType.easeOutBack));
    }

    public void popupScale(GameObject g, Vector2 vec)
    {
        g.transform.localScale = vec;
        iTween.ScaleTo(g, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack));
    }

    public void fly(GameObject target)
    {
        iTween.MoveTo(target, iTween.Hash("position", gameObject.transform.position + new Vector3(300f, 0f, 0f), "time", UnityEngine.Random.Range(0.5f, 1f), "easetype", iTween.EaseType.easeOutBack, "oncomplete", "setActive"));
    }

    public void popupScalePause(GameObject g)
    {
        g.transform.localScale = new Vector2(0f, 0f);
        iTween.ScaleTo(g, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack, "oncomplete", "pause", "oncompleteparams", 0));
    }

    public void popupScale(GameObject g)
    {
        g.transform.localScale = new Vector2(0f, 0f);
        iTween.ScaleTo(g, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack));
    }

    public void popupScaleHorizontal(GameObject g)
    {
        g.transform.localScale = new Vector2(1f, 0f);
        iTween.ScaleTo(g, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack));
    }

    public void popupScaleVertical(GameObject g)
    {
        g.transform.localScale = new Vector2(0f, 1f);
        iTween.ScaleTo(g, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack));
    }

    public void popupScaleVertical(GameObject g, float time)
    {
        g.transform.localScale = new Vector2(0f, 1f);
        iTween.ScaleTo(g, iTween.Hash("scale", Vector3.one, "time", time, "easetype", iTween.EaseType.easeOutBack));
    }

    public void ShakeRotation(GameObject g)
    {
        iTween.ShakePosition(g, iTween.Hash("x", 0.2f, "time", 0.2f, "easetype", iTween.EaseType.easeInOutBounce));
    }

    public void ShakeRotation(GameObject g, Vector2 amount)
    {
        iTween.ShakePosition(g, iTween.Hash("amount", amount, "time", 0.2f, "easetype", iTween.EaseType.easeInOutBounce));
    }

    public void ShakeRotation(GameObject g, Vector2 amount, float time)
    {
        iTween.ShakePosition(g, iTween.Hash("amount", amount, "time", time, "easetype", iTween.EaseType.easeInOutBounce));
    }

    public void ShakePosition(GameObject g)
    {
        iTween.ShakePosition(g, iTween.Hash("x", 0.2f, "time", 0.2f, "easetype", iTween.EaseType.easeInOutBounce));
    }

    public void ShakePosition(GameObject g, Vector2 amount)
    {
        iTween.ShakePosition(g, iTween.Hash("amount", amount, "time", 0.2f, "easetype", iTween.EaseType.easeInOutBounce));
    }

    public void ShakePosition(GameObject g, Vector2 amount, float time)
    {
        iTween.ShakePosition(g, iTween.Hash("amount", (Vector3)amount, "time", time, "easetype", iTween.EaseType.easeInOutBounce));
    }

    public void ShakePosition(GameObject g, Vector2 amount, float time, float delay)
    {
        iTween.ShakePosition(g, iTween.Hash("amount", (Vector3)amount, "time", time, "delay", delay, "easetype", iTween.EaseType.easeInOutBounce));
    }

    public void setScroll0f(Scrollbar g)
    {
        g.value = 0f;
    }

    public void setScroll1f(Scrollbar g)
    {
        g.value = 1f;
    }

    public void setScroll0_5f(Scrollbar g)
    {
        g.value = 0.5f;
    }

    public void setScroll0_25f(Scrollbar g)
    {
        g.value = 0.25f;
    }

    public void setScroll0_75f(Scrollbar g)
    {
        g.value = 0.75f;
    }

    public void popupScaleOff(GameObject g)
    {
        iTween.ScaleTo(g, iTween.Hash("scale", new Vector3(0f, 0f, 1f), "time", 0.4f, "easetype", iTweenEaseTypeOff, "oncomplete", "setActiveFalsePopupOff", "oncompleteparams", g));
    }

    public void popupScaleOff(GameObject g, Vector2 vec)
    {
        iTween.ScaleTo(g, iTween.Hash("scale", new Vector3(vec.x, vec.y, 1f), "time", 0.4f, "easetype", iTweenEaseTypeOff, "oncomplete", "setActiveFalsePopupOff", "oncompleteparams", g));
    }

    public void popupScaleOff(GameObject g, Vector2 vec, float time)
    {
        iTween.ScaleTo(g, iTween.Hash("scale", new Vector3(vec.x, vec.y, 1f), "time", time, "easetype", iTweenEaseTypeOff, "oncomplete", "setActiveFalsePopupOff", "oncompleteparams", g));
    }

    public void popupScaleHorizontalOff(GameObject g)
    {
        iTween.ScaleTo(g, iTween.Hash("scale", new Vector3(1f, 0f, 1f), "time", 0.4f, "easetype", iTweenEaseTypeOff, "oncomplete", "setActiveFalsePopupOff", "oncompleteparams", g));
    }

    public void popupScaleVerticalOff(GameObject g)
    {
        iTween.ScaleTo(g, iTween.Hash("scale", new Vector3(0f, 1f, 1f), "time", 0.4f, "easetype", iTweenEaseTypeOff, "oncomplete", "setActiveFalsePopupOff", "oncompleteparams", g));
    }

    public void iTweenValueTo(GameObject target, iTween.EaseType easeType, float from, float to, float time, float delay, string funcUpdate)
    {
        Hashtable ht = iTween.Hash("easetype", easeType, "from", from, "to", to, "time", time, "delay", delay, "onupdate", funcUpdate);
        iTween.ValueTo(this.gameObject, ht);
    }

    public void delaySetActive(GameObject g)
    {
        StartCoroutine(ienumeratorDelaySetActive(g, 0.25f, true));
    }

    public void delaySetActive(GameObject g, float time, bool b)
    {
        StartCoroutine(ienumeratorDelaySetActive(g, time, b));
    }

    public IEnumerator ienumeratorDelaySetActive(GameObject g, float time, bool b)
    {
        //yield return new WaitForSecondsRealtime(time);
        yield return new WaitForSeconds(time);
        if (g != null && g.activeSelf != b)
            g.SetActive(b);
    }

    public void setColorText(Text t)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, 0f);
    }

    public void setSound()
    {
        if (SoundController.instance != null)
        {
            SoundController.instance.setSound();
        }
    }

    public void setMusic()
    {
        if (SoundController.instance != null)
        {
            SoundController.instance.setMusic();
        }
    }

    public void setVibration()
    {
        if (SoundController.instance != null)
        {
            SoundController.instance.setVibration();
        }
    }

    public void onMusic(StaticVar.keyMusic i)
    {
        if (Time.timeScale > 0.01f)
            if (SoundController.instance != null)
                SoundController.instance.turnOnMusic(i);
    }
    public void offMusic(StaticVar.keyMusic i)
    {
        if (Time.timeScale > 0.01f)
            if (SoundController.instance != null)
                SoundController.instance.turnOffMusic(i);
    }

    public void onSound(StaticVar.keySound i, bool isLoop = false)
    {
        if (SoundController.instance != null)
            SoundController.instance.turnOnSound(i, isLoop);
    }
    public void onSoundWhileOff(StaticVar.keySound i)
    {
        if (SoundController.instance != null)
            SoundController.instance.turnOnSoundWhileOff(i);
    }
    public void offSound(StaticVar.keySound i)
    {
        if (Time.timeScale > 0.01f)
            if (SoundController.instance != null)
                SoundController.instance.turnOffSound(i);
    }


    public void onVibrate(int i)
    {
        try
        {
            if (SoundController.instance != null)
                SoundController.instance.Vibrate((long)i);
            else Vibration.Vibrate(i);
        }
        catch (Exception) { }
    }

    public void vibrate()
    {
        try
        {
            onVibrate(35);
        }
        catch (Exception) { }
    }

    public void buttonMusic()
    {
        try
        {
            vibrate();
            onSound(StaticVar.keySound.Button);
            setNotRaycast();
        }
        catch (Exception) { }
    }

    public void openUrl(string url)
    {
        print("open url " + url.Trim());
        Application.OpenURL(url.Trim());
    }

    // ---------------------------------------------------------------------------------------------
    public void gameObjectFlyToTarget(GameObject effectParticle, float delay, GameObject start, GameObject effectPool, GameObject target, float timeFly, int value, int type, float timeWait)
    {
        StartCoroutine(gameObjectFlyToTargetIEnumerator(effectParticle, delay, start, effectPool, target, timeFly, value, type, timeWait));
    }

    IEnumerator gameObjectFlyToTargetIEnumerator(GameObject effectParticle, float delay, GameObject start, GameObject effectPool, GameObject target, float timeFly, int value, int type, float timeWait)
    {
        effectPool.SetActive(true);
        yield return new WaitForSeconds(timeWait);
        if (effectParticle != null)
        {
            GameObject effect = null;
            foreach (Transform t in effectParticle.transform)
            {
                if (!t.gameObject.activeSelf)
                {
                    effect = t.gameObject;
                    effect.transform.position = new Vector3(start.transform.position.x, start.transform.position.y, effect.transform.position.z);
                    effect.SetActive(true);
                    break;
                }
            }
            yield return new WaitForSeconds(delay);
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[100];
            int k = effect.GetComponent<ParticleSystem>().GetParticles(particles);
            Vector2[] particlesPos = new Vector2[k];
            for (int i = 0; i < k; i++)
            {
                particlesPos[i] = (Vector2)particles[i].position;
            }
            if (k > 0)
            {
                List<int> list = caculatorEffectCount(k, value);
                effectPool.transform.position = new Vector3(start.transform.position.x, start.transform.position.y, effectPool.transform.position.z);
                for (int i = 0; i < k; i++)
                {
                    effectPool.transform.GetChild(i).transform.localPosition = particlesPos[i];
                    effectPool.transform.GetChild(i).gameObject.SetActive(true);
                    float time = UnityEngine.Random.Range(-0.1f, 0.1f) + timeFly;
                    Vector3 posTarget = new Vector3(target.transform.position.x, target.transform.position.y, effectPool.transform.GetChild(i).position.z);
                    float delayA = UnityEngine.Random.Range(0.2f, 0.5f);
                    iTween.MoveTo(effectPool.transform.GetChild(i).gameObject, iTween.Hash("position", posTarget, "time", time, "delay", delayA, "easetype", iTween.EaseType.easeOutQuart, "oncomplete", "gameObjectFlyToTargetDone", "oncompleteparams", (list[i], type, effectPool.transform.GetChild(i).gameObject), "oncompletetarget", this.gameObject));
                    UnityEvent e = new UnityEvent();
                    e.AddListener(() =>
                    {
                        //onSound(StaticVar.keySound.CoinFly);
                    });
                    delayInvoke(e, delayA);
                }
                effect.SetActive(false);
            }
        }
        else
        {
            yield return new WaitForSeconds(delay);
            foreach (Transform t in effectPool.transform)
            {
                if (!t.gameObject.activeSelf)
                {
                    t.position = (Vector2)start.transform.position;
                    t.gameObject.SetActive(true);
                    float time = UnityEngine.Random.Range(-0.1f, 0.1f) + timeFly;
                    Vector3 posTarget = new Vector3(target.transform.position.x, target.transform.position.y, t.position.z);
                    float delayA = UnityEngine.Random.Range(0.2f, 0.5f);
                    iTween.MoveTo(t.gameObject, iTween.Hash("position", posTarget, "time", time, "delay", delayA, "easetype", iTween.EaseType.easeOutQuart, "oncomplete", "gameObjectFlyToTargetDone", "oncompleteparams", (value, type, t.gameObject), "oncompletetarget", this.gameObject));
                    UnityEvent e = new UnityEvent();
                    e.AddListener(() =>
                    {
                        //onSound(StaticVar.keySound.CoinFly);
                    });
                    delayInvoke(e, delayA);
                    break;
                }
            }
        }
        UnityEvent ee = new UnityEvent();
        ee.AddListener(() =>
        {
            effectPool.SetActive(false);
        });
        delayInvoke(ee, timeFly + 1f);
    }

    void gameObjectFlyToTargetDone((int, int, GameObject) param)
    {
        if (param.Item2 == 0)
        {
            SaveGame.instance.changeCash(param.Item1, "");
            //onSound(StaticVar.keySound.CollectCoin);
        }
        param.Item3.SetActive(false);
    }

    public List<int> caculatorEffectCount(int count, int value)
    {
        int k = value / count + 1;
        int l = value % k;
        List<int> list = new List<int>();
        list.Add(l);
        value -= l;
        while (value > 0)
        {
            list.Add(k);
            value -= k;
        }
        while (list.Count < count)
            list.Add(0);
        return list;
    }

    //-------------------------------------------------------------------------------
    public void gameObjectFlyToTargetCanvas(GameObject effectParticle, float delay, GameObject start, GameObject effectPool, GameObject target, float timeFly, int value, int type, float timeWait)
    {
        StartCoroutine(gameObjectFlyToTargetIEnumeratorCanvas(effectParticle, delay, start, effectPool, target, timeFly, value, type, timeWait));
    }

    IEnumerator gameObjectFlyToTargetIEnumeratorCanvas(GameObject effectParticle, float delay, GameObject start, GameObject effectPool, GameObject target, float timeFly, int value, int type, float timeWait)
    {
        effectPool.SetActive(true);
        yield return new WaitForSeconds(timeWait);
        if (effectParticle != null)
        {
            GameObject effect = null;
            foreach (Transform t in effectParticle.transform)
            {
                if (!t.gameObject.activeSelf)
                {
                    effect = t.gameObject;
                    effect.transform.position = new Vector3(start.transform.position.x, start.transform.position.y, effect.transform.position.z);
                    effect.SetActive(true);
                    break;
                }
            }
            yield return new WaitForSeconds(delay);
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[100];
            int k = effect.GetComponent<ParticleSystem>().GetParticles(particles);
            Vector2[] particlesPos = new Vector2[k];
            for (int i = 0; i < k; i++)
            {
                particlesPos[i] = particles[i].position;
            }
            if (k > 0)
            {
                List<int> list = caculatorEffectCount(k, value);
                effectPool.transform.position = new Vector3(start.transform.position.x, start.transform.position.y, effectPool.transform.position.z);
                for (int i = 0; i < k; i++)
                {
                    effectPool.transform.GetChild(i).transform.localPosition = particlesPos[i];
                    effectPool.transform.GetChild(i).gameObject.SetActive(true);
                    float time = UnityEngine.Random.Range(-0.1f, 0.1f) + timeFly;
                    Vector3 posTarget = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
                    float delayA = UnityEngine.Random.Range(0.2f, 1f);
                    iTween.MoveTo(effectPool.transform.GetChild(i).gameObject, iTween.Hash("position", posTarget, "time", UnityEngine.Random.Range(time - 0.2f, time + 0.2f), "delay", delayA, "easetype", iTween.EaseType.easeOutQuart, "oncomplete", "gameObjectFlyToTargetDone", "oncompleteparams", (list[i], type, effectPool.transform.GetChild(i).gameObject), "oncompletetarget", this.gameObject));
                    UnityEvent e = new UnityEvent();
                    e.AddListener(() =>
                    {
                        //onSound(StaticVar.keySound.CoinFly);
                    });
                    delayInvoke(e, delayA);
                }
                effect.SetActive(false);
            }
        }
        else
        {
            yield return new WaitForSeconds(delay);
            foreach (Transform t in effectPool.transform)
            {
                if (!t.gameObject.activeSelf)
                {
                    t.position = (Vector2)start.transform.position;
                    t.gameObject.SetActive(true);
                    float time = UnityEngine.Random.Range(-0.1f, 0.1f) + timeFly;
                    Vector3 posTarget = new Vector3(target.transform.position.x, target.transform.position.y, t.position.z);
                    float delayA = UnityEngine.Random.Range(0.2f, 0.5f);
                    iTween.MoveTo(t.gameObject, iTween.Hash("position", posTarget, "time", time, "delay", delayA, "easetype", iTween.EaseType.easeOutQuart, "oncomplete", "gameObjectFlyToTargetDone", "oncompleteparams", (value, type, t.gameObject), "oncompletetarget", this.gameObject));
                    UnityEvent e = new UnityEvent();
                    e.AddListener(() =>
                    {
                        //onSound(StaticVar.keySound.CoinFly);
                    });
                    delayInvoke(e, delayA);
                    break;
                }
            }
        }
        UnityEvent ee = new UnityEvent();
        ee.AddListener(() =>
        {
            effectPool.SetActive(false);
        });
        delayInvoke(ee, timeFly + 3f);
    }

    //---------------------------------------------------------------------------------------------------------------

    public void share(string type)
    {
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
        }
    }

    public void rateGame()
    {
        if (StaticVar.GetPlatform() == StaticVar.PlatformType.IOS)
        {
            string id = "";
            if (id.IndexOf("id") >= 0)
                openUrl("itms-apps://itunes.apple.com/app/" + id);
            else
                openUrl("itms-apps://itunes.apple.com/app/id" + id);
        }
        else
            openUrl("https://play.google.com/store/apps/details?id=" + Application.identifier);
    }

    public string getUrlStore()
    {
        if (StaticVar.GetPlatform() == StaticVar.PlatformType.IOS)
        {
            string id = "";
            if (id.IndexOf("id") >= 0)
                return "itms-apps://itunes.apple.com/app/" + id;
            else
                return "itms-apps://itunes.apple.com/app/id" + id;
        }
        else
            return "https://play.google.com/store/apps/details?id=" + Application.identifier;
    }
}
