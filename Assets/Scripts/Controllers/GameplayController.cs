using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleJSON;
using UnityEngine.Events;
using System;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;
    public List<IngredientsRecipe> ingredientsRecipesCurrent;
    public Transform draftPos;
    public List<Camera> listCamPer;
    public List<CounterMachineScript> counterMachineScriptActives;
    public Transform pointSpawnCustomer;
    public List<StaffScript> staffRentScripts;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        initStart();
        Invoke(nameof(startPlay), .1f);
    }

    void initStart()
    {
        bool ishorizontalFieldOfView = UIController.instance.setOrthographicSize(0.6f);
        foreach (Camera cam in listCamPer)
            if (ishorizontalFieldOfView)
                UIController.instance.setupCamPer(cam, 36);
            else UIController.instance.setupCamPerVertical(cam, 60);
    }

    public void startPlay()
    {
        UIController.instance.onMusic(StaticVar.keyMusic.BG_Menu);
        loadUpgradePanel();
        setUI_IngredientsRecipe();
        spawnCustomerCoroutine = StartCoroutine(spawnCustomerIEnumerator());
        unlockMachine();
        if (!SaveGame.instance.checkKey(StaticVar.keySaveGame.IsStartPlay.ToString()))
        {
            initMoneyStart();
        }
        else
        {
            moneyStackScriptStartPlay.gameObject.SetActive(false);
        }
        foreach (StaffScript item in staffRentScripts)
        {
            if (item.gameObject.activeSelf)
                item.setupAction();
        }
    }

    public Coroutine spawnCustomerCoroutine;
    public IEnumerator spawnCustomerIEnumerator()
    {
        yield return new WaitForFixedUpdate();
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 3f));
            spawnCustomer();
        }
    }

    void spawnCustomer()
    {
        List<CounterMachineScript> counterMachineScripts = new List<CounterMachineScript>();
        foreach (CounterMachineScript item in counterMachineScriptActives)
        {
            if (item.customerStackWaitTransform.Count > item.customerStackWaitScript.Count && item.isUnlock)
                counterMachineScripts.Add(item);
        }
        if (counterMachineScripts.Count > 0)
        {
            CustomerScript customerScript = Instantiate(PrefabsList.instance.customerPrefabs[0]).GetComponent<CustomerScript>();
            customerScript.transform.position = pointSpawnCustomer.position;
            CounterMachineScript counterMachineScript = counterMachineScripts[NumberListController.instance.getNumberRandom(counterMachineScripts.Count)];
            customerScript.setupTargetBuy(counterMachineScript.typeBread, UnityEngine.Random.Range(1, 4), counterMachineScript);
        }
    }

    [Header("IngredientsRecipeTransform")]
    public Transform IngredientsRecipeTransform;
    public void setUI_IngredientsRecipe()
    {
        foreach (Transform item in IngredientsRecipeTransform)
        {
            IngredientsRecipe ingredientsRecipe = getIngredientsRecipe(StaticKeyEnum.getEnumValue<StaticKeyEnum.TypeIngredient>(item.name));
            item.GetChild(1).GetChild(0).GetComponent<Text>().text = ingredientsRecipe.countIngredient.ToString();
        }
    }

    public IngredientsRecipe getIngredientsRecipe(StaticKeyEnum.TypeIngredient typeIngredient)
    {
        foreach (IngredientsRecipe item in ingredientsRecipesCurrent)
        {
            if (item.typeIngredient == typeIngredient)
                return item;
        }
        return null;
    }

    public bool addIngredientsRecipe(StaticKeyEnum.TypeIngredient typeIngredient, int value)
    {
        foreach (IngredientsRecipe item in ingredientsRecipesCurrent)
        {
            if (item.typeIngredient == typeIngredient)
            {
                item.countIngredient += value;
                setUI_IngredientsRecipe();
                return true;
            }
        }
        return false;
    }

    public bool useIngredientsRecipe(StaticKeyEnum.TypeIngredient typeIngredient, int value)
    {
        foreach (IngredientsRecipe item in ingredientsRecipesCurrent)
        {
            if (item.typeIngredient == typeIngredient && item.countIngredient >= value)
            {
                item.countIngredient -= value;
                setUI_IngredientsRecipe();
                return true;
            }
        }
        return false;
    }

    //--------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (Time.timeScale < 0.05f)
            {
                Time.timeScale = 1f;
            }
            else if (Time.timeScale > 0.95f)
            {
                Time.timeScale = 0f;
            }
        }

        if (Input.GetKeyUp(KeyCode.C)) SaveGame.instance.changeCash(1000, "");
    }
    //----------------------------------------------------------------------------------------------------------------------------------

    // ----------------------------------------------------------------------------------------------------------------------------------
    // set effect
    public Transform effectPool;
    public Transform setEffect(Transform pool, string nameEffect, Vector3 pos)
    {
        Transform effect = null;
        foreach (Transform t in pool)
        {
            if (t.name == nameEffect)
            {
                effect = t;
                break;
            }
        }

        if (effect != null)
        {
            PoolingScript poolingScript = effect.GetComponent<PoolingScript>();
            if (poolingScript != null)
            {
                GameObject g = poolingScript.getPoolGameObject();
                g.transform.position = pos;
                g.SetActive(true);
                return g.transform;
            }
            else
            {
                foreach (Transform t in effect)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        t.position = pos;
                        t.gameObject.SetActive(true);
                        return t;
                    }
                }
            }
            return null;
        }
        else return null;
    }

    public Transform setEffect(Transform pool, string nameEffect)
    {
        Transform effect = null;
        foreach (Transform t in pool)
        {
            if (t.name == nameEffect)
            {
                effect = t;
                break;
            }
        }

        if (effect != null)
        {
            PoolingScript poolingScript = effect.GetComponent<PoolingScript>();
            if (poolingScript != null)
            {
                GameObject g = poolingScript.getPoolGameObject();
                g.SetActive(true);
                return g.transform;
            }
            else
            {
                foreach (Transform t in effect)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        t.gameObject.SetActive(true);
                        return t;
                    }
                }
            }
            return null;
        }
        else return null;
    }

    public Transform setEffect(Transform pool, Vector3 pos)
    {
        Transform effect = pool;

        if (effect != null)
        {
            PoolingScript poolingScript = effect.GetComponent<PoolingScript>();
            if (poolingScript != null)
            {
                GameObject g = poolingScript.getPoolGameObject();
                g.transform.position = pos;
                g.SetActive(true);
                return g.transform;
            }
            else
            {
                foreach (Transform t in effect)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        t.position = pos;
                        t.gameObject.SetActive(true);
                        return t;
                    }
                }
            }
            return null;
        }
        else return null;
    }

    public Transform setEffect(PoolingScript poolingScript, Vector3 pos)
    {
        if (poolingScript != null)
        {
            GameObject g = poolingScript.getPoolGameObject();
            g.transform.position = pos;
            g.SetActive(true);
            return g.transform;
        }
        else return null;
    }

    public Transform setEffectText(Transform pool, string value, Vector3 pos)
    {
        Transform effect = pool;

        if (effect != null)
        {
            PoolingScript poolingScript = effect.GetComponent<PoolingScript>();
            if (poolingScript != null)
            {
                GameObject g = poolingScript.getPoolGameObject();
                g.transform.position = new Vector3(pos.x, pos.y, g.transform.position.z);
                g.transform.GetChild(0).GetComponent<Text>().text = value;
                g.SetActive(true);
                return g.transform;
            }
            else
            {
                foreach (Transform t in effect)
                {
                    if (!t.gameObject.activeSelf)
                    {
                        t.position = new Vector3(pos.x, pos.y, t.position.z);
                        t.GetChild(0).GetComponent<Text>().text = value;
                        t.gameObject.SetActive(true);
                        return t;
                    }
                }
            }
            return null;
        }
        else return null;
    }
    //----------------------------------------------------------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------------------------------------------------
    // Cash effect
    public Transform cashFlyEffectTransform;

    public void cashFlyEffect(int cash, int countSeed, Transform startTransform, Vector3 offsetStart, Transform targetFly, Vector2 offsetCurve, Vector2 offsetX, Vector2 offsetY, Vector2 timeRange, Vector2 timeDelayFly, string source, float timeDelay = 0f)
    {
        StartCoroutine(cashFlyEffectIEnumerator(cash, countSeed, startTransform, offsetStart, targetFly, offsetCurve, offsetX, offsetY, timeRange, timeDelayFly, source, timeDelay));
    }

    IEnumerator cashFlyEffectIEnumerator(int cash, int countSeed, Transform startTransform, Vector3 offsetStart, Transform targetFly, Vector2 offsetCurve, Vector2 offsetX, Vector2 offsetY, Vector2 timeRange, Vector2 timeDelayFly, string source, float timeDelay)
    {
        yield return new WaitForFixedUpdate();
        int countObjectEffect = 0;
        List<Transform> listSeedTransform = new List<Transform>();

        PoolingScript poolingScript = cashFlyEffectTransform.GetComponent<PoolingScript>();
        if (poolingScript != null)
        {
            for (int i = 0; i < countSeed; i++)
            {
                GameObject g = poolingScript.getPoolGameObject(listSeedTransform);
                listSeedTransform.Add(g.transform);
                countObjectEffect++;
            }
        }
        else
        {
            foreach (Transform item in cashFlyEffectTransform)
            {
                if (!item.gameObject.activeSelf)
                {
                    listSeedTransform.Add(item);
                    countObjectEffect++;
                    if (countObjectEffect >= countSeed) break;
                }
            }
        }

        countObjectEffect = listSeedTransform.Count;
        if (countObjectEffect == 0)
        {
            SaveGame.instance.changeCash(cash, source);
            //UIController.instance.onSound(StaticVar.keySound.CollectCoin);
            yield break;
        }
        List<int> valueSeeds = UIController.instance.caculatorEffectCount(listSeedTransform.Count, cash);
        Vector3 startEffect = new Vector3(startTransform.position.x, startTransform.position.y, cashFlyEffectTransform.position.z) + offsetStart;
        Vector3 endEffect = new Vector3(targetFly.position.x, targetFly.position.y, cashFlyEffectTransform.position.z);

        for (int i = 0; i < countObjectEffect; i++)
        {
            GameObject G_Seed = listSeedTransform[i].gameObject;
            int valueSeed = valueSeeds[i];
            UnityEvent isDone = new UnityEvent();
            isDone.AddListener(() =>
            {
                G_Seed.SetActive(false);
                SaveGame.instance.changeCash(valueSeed, source);
                //UIController.instance.onSound(StaticVar.keySound.CollectCoin);
            });
            G_Seed.transform.position = startEffect + new Vector3(UnityEngine.Random.Range(offsetX.x, offsetX.y), UnityEngine.Random.Range(offsetY.x, offsetY.y), 0);
            G_Seed.SetActive(true);
            UIController.instance.CurveMove(G_Seed, endEffect, (UnityEngine.Random.Range(0, 100) % 2 == 0 ? 1 : -1) * UnityEngine.Random.Range(offsetCurve.x, offsetCurve.y),
            UnityEngine.Random.Range(timeRange.x, timeRange.y), "X", isDone, UnityEngine.Random.Range(timeDelayFly.x, timeDelayFly.y), true, false, false);
        }
        //UIController.instance.onSound(StaticVar.keySound.GetCash);
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------------------------------------------------------
    // main character upgrade
    public Transform upgradePanel;
    public int levelSpeedMove;
    public int levelCapacity;
    public int levelProfit;
    public Sprite buttonMaxLvSprite;
    public float profit;

    public void loadUpgradePanel()
    {
        levelSpeedMove = SaveGame.instance.getValueNode(StaticVar.keyDataSave.LevelSpeedMove.ToString(), 0);
        levelCapacity = SaveGame.instance.getValueNode(StaticVar.keyDataSave.LevelCapacity.ToString(), 0);
        levelProfit = SaveGame.instance.getValueNode(StaticVar.keyDataSave.LevelProfit.ToString(), 0);

        JoystickCharacterMapHouse.instance.SpeedMove = StaticKeyEnum.defaultCharacter_SpeedMove * (1f + 0.2f * levelSpeedMove);
        JoystickCharacterMapHouse.instance.ingredientStackScript.maxStackCount = StaticKeyEnum.defaultCharacter_Capacity + levelCapacity;
        profit = 1f + 0.5f * levelProfit;
        JoystickCharacterMapHouse.instance.ingredientStackScript.resetMaxText();

        for (int i = 1; i < 4; i++)
        {
            Transform t_Upgrade = upgradePanel.GetChild(0).GetChild(0).GetChild(i).GetChild(0);
            int levelTemp = 0;
            if (i == 1)
                levelTemp = levelSpeedMove;
            else if (i == 2)
                levelTemp = levelCapacity;
            else if (i == 3)
                levelTemp = levelProfit;
            if (levelTemp < 10)
            {
                t_Upgrade.GetChild(1).GetComponent<Text>().text = "Lv." + (levelTemp + 1);
                t_Upgrade.GetChild(4).GetChild(1).GetComponent<Text>().text = "" + ((levelTemp + 1) * 100);
            }
            else
            {
                t_Upgrade.GetChild(1).GetComponent<Text>().text = "Lv.max";
                t_Upgrade.GetChild(3).GetComponent<Button>().enabled = false;
                t_Upgrade.GetChild(3).GetComponent<Image>().sprite = buttonMaxLvSprite;
                t_Upgrade.GetChild(3).GetChild(1).GetComponent<Text>().text = "max";
                t_Upgrade.GetChild(4).GetComponent<Button>().enabled = false;
                t_Upgrade.GetChild(4).GetComponent<Image>().sprite = buttonMaxLvSprite;
                t_Upgrade.GetChild(4).GetChild(1).GetComponent<Text>().text = "max";
            }
        }
    }

    public void upgradeLevelSpeedMove(bool isAds)
    {
        levelSpeedMove = SaveGame.instance.getValueNode(StaticVar.keyDataSave.LevelSpeedMove.ToString(), 0);
        if (levelSpeedMove < 10)
        {
            UnityEvent isDone = new UnityEvent();
            isDone.AddListener(() =>
            {
                upgradePanel.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(5).gameObject.SetActive(false);
                upgradePanel.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(5).gameObject.SetActive(true);
                SaveGame.instance.setValueNode(StaticVar.keyDataSave.LevelSpeedMove.ToString(), 1, false);
                loadUpgradePanel();
            });
            if (isAds)
                isDone.Invoke();
            else
            {
                if (SaveGame.instance.checkCash((levelSpeedMove + 1) * 100))
                {
                    SaveGame.instance.changeCash(-(levelSpeedMove + 1) * 100);
                    isDone.Invoke();
                }
            }
        }
    }

    public void upgradeLevelCapacity(bool isAds)
    {
        levelCapacity = SaveGame.instance.getValueNode(StaticVar.keyDataSave.LevelCapacity.ToString(), 0);
        if (levelCapacity < 10)
        {
            UnityEvent isDone = new UnityEvent();
            isDone.AddListener(() =>
            {
                upgradePanel.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(5).gameObject.SetActive(false);
                upgradePanel.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(5).gameObject.SetActive(true);
                SaveGame.instance.setValueNode(StaticVar.keyDataSave.LevelCapacity.ToString(), 1, false);
                loadUpgradePanel();
            });
            if (isAds)
                isDone.Invoke();
            else
            {
                if (SaveGame.instance.checkCash((levelCapacity + 1) * 100))
                {
                    SaveGame.instance.changeCash(-(levelCapacity + 1) * 100);
                    isDone.Invoke();
                }
            }
        }
    }

    public void upgradeLevelProfit(bool isAds)
    {
        levelProfit = SaveGame.instance.getValueNode(StaticVar.keyDataSave.LevelProfit.ToString(), 0);
        if (levelProfit < 10)
        {
            UnityEvent isDone = new UnityEvent();
            isDone.AddListener(() =>
            {
                upgradePanel.GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetChild(5).gameObject.SetActive(false);
                upgradePanel.GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetChild(5).gameObject.SetActive(true);
                SaveGame.instance.setValueNode(StaticVar.keyDataSave.LevelProfit.ToString(), 1, false);
                loadUpgradePanel();
            });
            if (isAds)
                isDone.Invoke();
            else
            {
                if (SaveGame.instance.checkCash((levelProfit + 1) * 100))
                {
                    SaveGame.instance.changeCash(-(levelProfit + 1) * 100);
                    isDone.Invoke();
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------------------------------------------------------
    // Unlock process
    public List<UnlockProcess> unlockProcesses;

    public void unlockMachine()
    {
        if ((SaveGame.instance.getValueNode(StaticVar.keyDataSave.PhaseUnlock.ToString(), 0) >= 0))
            foreach (UnlockProcess unlockProcess in unlockProcesses)
            {
                foreach (MachineUnlockScript item in unlockProcess.machineUnlockScripts)
                {
                    if (item.getValueUnlock() > 0)
                    {
                        SaveGame.instance.setValueNode(StaticVar.keyDataSave.PhaseUnlock.ToString(), unlockProcess.phaseId, true);
                        goto EndGetPhase;
                    }
                }
            }
        EndGetPhase:

        foreach (UnlockProcess unlockProcess in unlockProcesses)
        {
            bool isShow = false;
            if (unlockProcess.phaseId == SaveGame.instance.getValueNode(StaticVar.keyDataSave.PhaseUnlock.ToString(), 0) && (SaveGame.instance.getValueNode(StaticVar.keyDataSave.PhaseUnlock.ToString(), 0) >= 0))
                isShow = true;
            foreach (MachineUnlockScript item in unlockProcess.machineUnlockScripts)
            {
                if (item.getValueUnlock() == 0)
                {
                    item.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    if (isShow)
                        item.transform.GetChild(0).gameObject.SetActive(true);
                    else
                        item.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }

        Transform tartgetHint = null;
        if ((SaveGame.instance.getValueNode(StaticVar.keyDataSave.PhaseUnlock.ToString(), 0) >= 0))
            foreach (UnlockProcess unlockProcess in unlockProcesses)
                if (unlockProcess.phaseId == SaveGame.instance.getValueNode(StaticVar.keyDataSave.PhaseUnlock.ToString(), 0))
                    foreach (MachineUnlockScript item in unlockProcess.machineUnlockScripts)
                    {
                        if (item.getValueUnlock() > 0 && item.isShowHintCharacter)
                        {
                            tartgetHint = item.transform;
                            goto EndHint;
                        }
                    }
                EndHint:
        JoystickCharacterMapHouse.instance.characterMap.targetHint = tartgetHint;
    }

    public MoneyStackScript moneyStackScriptStartPlay;
    public void initMoneyStart()
    {
        for (int i = 0; i < 20; i++)
        {
            moneyStackScriptStartPlay.addMoneyStack(20);
        }
        JoystickCharacterMapHouse.instance.characterMap.targetHint = moneyStackScriptStartPlay.transform;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------------------------------------------------------

    // Coin
    public void plusCoin(bool isPlus)
    {
        if (isPlus)
            SaveGame.instance.changeCash(1000);
        else
            SaveGame.instance.changeCash(-Mathf.Min(1000, SaveGame.instance.getCash()));
    }

    // Character
    public void changeSpeedMoveConfig(Slider slider)
    {
        Text valueText = slider.transform.parent.GetChild(1).GetComponent<Text>();
        float start = 1;
        float end = 30;
        float value = slider.value * (end - start) + start;
        valueText.text = value.ToString();
        JoystickCharacterMapHouse.instance.SpeedMove = value;
    }

    public void changeCapacityConfig(Slider slider)
    {
        Text valueText = slider.transform.parent.GetChild(1).GetComponent<Text>();
        int start = 1;
        int end = 20;
        int value = Mathf.RoundToInt(slider.value * (end - start)) + start;
        valueText.text = value.ToString();
        JoystickCharacterMapHouse.instance.ingredientStackScript.maxStackCount = value;
    }

    public void changeProfitConfig(Slider slider)
    {
        Text valueText = slider.transform.parent.GetChild(1).GetComponent<Text>();
        float start = 1;
        float end = 10;
        float value = slider.value * (end - start) + start;
        valueText.text = value.ToString();
        profit = value;
    }

    public void resetDefaultCharacterConfig()
    {
        loadUpgradePanel();
    }

    // Camera
    public Transform cam_1st, cam_3rd;
    public void enableCam_1st()
    {
        cam_1st.gameObject.SetActive(true);
        cam_3rd.gameObject.SetActive(false);
    }

    public void enableCam_3rd()
    {
        cam_1st.gameObject.SetActive(false);
        cam_3rd.gameObject.SetActive(true);
    }

    public void resetDefaultCam()
    {
        enableCam_3rd();
        cam_3rd.localEulerAngles = new Vector3(0, 45, 0);
        cam_3rd.GetChild(0).GetChild(0).localPosition = Vector3.zero;
    }

    public void setCam_3rd_far(Slider slider)
    {
        Text valueText = slider.transform.parent.GetChild(1).GetComponent<Text>();
        float start = -10;
        float end = 10;
        float value = slider.value * (end - start) + start;
        valueText.text = value.ToString();
        cam_3rd.GetChild(0).GetChild(0).localPosition = new Vector3(0, 0, value);
    }

    public void setCam_3rd_rotateHeight(Slider slider)
    {
        Text valueText = slider.transform.parent.GetChild(1).GetComponent<Text>();
        float start = -45;
        float end = 30;
        float value = slider.value * (end - start) + start;
        valueText.text = value.ToString();
        cam_3rd.localEulerAngles = new Vector3(value, cam_3rd.localEulerAngles.y, 0);
    }

    public void setCam_3rd_rotateCircle(Slider slider)
    {
        Text valueText = slider.transform.parent.GetChild(1).GetComponent<Text>();
        float start = 0;
        float end = 360;
        float value = slider.value * (end - start) + start;
        valueText.text = value.ToString();
        cam_3rd.localEulerAngles = new Vector3(cam_3rd.localEulerAngles.x, value, 0);
    }

    public float speedMachines = 1f;
    public void setSpeedMachines(Slider slider)
    {
        Text valueText = slider.transform.parent.GetChild(1).GetComponent<Text>();
        float start = 1f;
        float end = 10f;
        float value = slider.value * (end - start) + start;
        valueText.text = value.ToString();
        speedMachines = value;
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------
}

[Serializable]
public class UnlockProcess
{
    public int phaseId;
    public List<MachineUnlockScript> machineUnlockScripts;
}