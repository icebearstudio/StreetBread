using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleJSON;
using UnityEngine.Events;
using System;

public class MachineUnlockScript : MonoBehaviour
{
    public string idMachineSave;
    public StaticKeyEnum.TypeMachine typeMachine;
    public bool isShowHintCharacter;
    public int costUnlock;
    public int currentValue;
    public string nameMachine;
    public GameObject machineGameObject;
    public Text nameText;
    public Text costText;
    public Image fillProcess;
    public Transform posCharacterUnlock;

    void Start()
    {
        init();
    }

    public void init()
    {
        gameObject.name = idMachineSave;
        currentValue = SaveGame.instance.getKey(idMachineSave, 0);
        if (currentValue == costUnlock)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            machineGameObject.gameObject.SetActive(true);
            if (machineGameObject.GetComponent<MachineScript>() != null)
                machineGameObject.GetComponent<MachineScript>().isUnlock = true;
        }
        else
        {
            nameText.text = nameMachine;
            costText.text = (costUnlock - currentValue).ToString();
            fillProcess.fillAmount = currentValue * 1f / costUnlock;
        }
    }

    public int getValueUnlock()
    {
        currentValue = SaveGame.instance.getKey(idMachineSave, 0);
        return costUnlock - currentValue;
    }

    public void changeValue(int k)
    {
        if (SaveGame.instance.checkCash(k) && SaveGame.instance.changeCash(-k))
        {
            SaveGame.instance.setKey(SaveGame.instance.getKey(idMachineSave, 0) + k, idMachineSave);
            currentValue = SaveGame.instance.getKey(idMachineSave, 0);
            costText.text = (costUnlock - currentValue).ToString();
            fillProcess.fillAmount = currentValue * 1f / costUnlock;
            if (costUnlock - currentValue == 0 && !machineGameObject.activeSelf)
            {
                machineGameObject.SetActive(true);
                if (machineGameObject.GetComponent<MachineScript>() != null)
                    machineGameObject.GetComponent<MachineScript>().isUnlock = true;
                transform.GetChild(0).gameObject.SetActive(false);
                GameplayController.instance.unlockMachine();
                Vector3 pos = new Vector3(posCharacterUnlock.position.x, JoystickCharacterMapHouse.instance.characterMap.transform.position.y, posCharacterUnlock.position.z);
                iTween.MoveTo(JoystickCharacterMapHouse.instance.characterMap.gameObject, pos, 0.2f, iTween.EaseType.easeOutSine);

                if (typeMachine == StaticKeyEnum.TypeMachine.Staff)
                {
                    machineGameObject.GetComponent<StaffScript>().setupAction();
                }
            }
        }
    }
}
