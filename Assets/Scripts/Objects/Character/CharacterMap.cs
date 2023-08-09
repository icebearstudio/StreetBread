using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMap : MonoBehaviour
{
    public Transform moneyMovePointTransform;
    public Transform arrowHint;
    public CharacterAnimScript characterAnimScript;
    public IngredientStackScript ingredientStackScript;
    public Transform targetHint;

    void Start()
    {
    }

    void Update()
    {
        if (targetHint != null)
        {
            arrowHint.forward = targetHint.position - transform.position;
            if (!arrowHint.gameObject.activeSelf)
            {
                if (targetHint.GetComponent<MachineUnlockScript>() != null)
                {
                    if (targetHint.GetComponent<MachineUnlockScript>().getValueUnlock() <= SaveGame.instance.getCash())
                        arrowHint.gameObject.SetActive(true);
                    else
                        arrowHint.gameObject.SetActive(false);
                }
                else
                    arrowHint.gameObject.SetActive(true);
            }
        }
        else
            arrowHint.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "MoneyCollider")
        {

        }
        else if (collider.tag == "AvailableBuyCollider")
        {
            CounterMachineScript counterMachineScript = collider.GetComponentInParent<CounterMachineScript>();
            counterMachineScript.isAvailableBuy = true;
        }
        else if (collider.tag == "BoxSoundNoise" && isSoundNoise)
        {
            int k = NumberListController.instance.getNumberRandom(5);
            if (k == 0)
                UIController.instance.onSound(StaticVar.keySound.sfx_noise_1);
            else if (k == 1)
                UIController.instance.onSound(StaticVar.keySound.sfx_noise_2);
            else if (k == 2)
                UIController.instance.onSound(StaticVar.keySound.sfx_noise_3);
            else if (k == 3)
                UIController.instance.onSound(StaticVar.keySound.sfx_noise_4);
            else if (k == 4)
                UIController.instance.onSound(StaticVar.keySound.sfx_noise_5);
            StartCoroutine(cooldownSoundNoise());
        }
    }

    bool isSoundNoise = true;
    IEnumerator cooldownSoundNoise()
    {
        isSoundNoise = false;
        yield return new WaitForSeconds(5);
        isSoundNoise = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "AvailableBuyCollider")
        {
            CounterMachineScript counterMachineScript = collider.GetComponentInParent<CounterMachineScript>();
            counterMachineScript.isAvailableBuy = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "MoneyCollider")
        {

        }
        else if (collider.tag == "IngredientCollider")
        {
            if (ingredientStackScript.maxStackCount > ingredientStackScript.ingredientScriptsStackCurrent.Count && ingredientStackScript.isAvailableGetStack
            && (ingredientStackScript.ingredientScriptsStackCurrent.Count == 0 || (ingredientStackScript.ingredientScriptsStackCurrent.Count > 0 && (ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient != StaticKeyEnum.TypeIngredient.BreadNormal_Done && ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient != StaticKeyEnum.TypeIngredient.BreadPlus_Done))))
            {
                ShelveIngredientScript shelveIngredientScript = collider.GetComponentInParent<ShelveIngredientScript>();
                if (shelveIngredientScript.listIngredient.Count > 0)
                {
                    ingredientStackScript.cooldownGetStack();
                    ingredientStackScript.addIngredientStack(shelveIngredientScript.getIngredient());
                }
            }
        }
        else if (collider.tag == "Box_Ingredient")
        {
            if (ingredientStackScript.ingredientScriptsStackCurrent.Count > 0 && ingredientStackScript.isAvailableBox_Ingredient
            && (ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient != StaticKeyEnum.TypeIngredient.BreadNormal_Done && ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient != StaticKeyEnum.TypeIngredient.BreadPlus_Done))
            {
                BoxIngredientScript boxIngredientScript = collider.GetComponentInParent<BoxIngredientScript>();
                ingredientStackScript.cooldownBox_Ingredient();
                IngredientScript ingredientScript = ingredientStackScript.getIngredientStack();
                ingredientScript.transform.parent = GameplayController.instance.draftPos;
                boxIngredientScript.moveIngredientToBoxIngredient(ingredientScript, 3f, .3f, true);
            }
        }
        else if (collider.tag == "GetBreadCollider")
        {
            if (ingredientStackScript.maxStackCount > ingredientStackScript.ingredientScriptsStackCurrent.Count && ingredientStackScript.isAvailableGetStack
            && (ingredientStackScript.ingredientScriptsStackCurrent.Count == 0 || (ingredientStackScript.ingredientScriptsStackCurrent.Count > 0 && (ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient == StaticKeyEnum.TypeIngredient.BreadNormal_Done || ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient == StaticKeyEnum.TypeIngredient.BreadPlus_Done))))
            {
                BreadMachineScript breadMachineScript = collider.GetComponentInParent<BreadMachineScript>();
                if (breadMachineScript.breadStack.Count > 0)
                {
                    ingredientStackScript.cooldownGetStack();
                    ingredientStackScript.addIngredientStack(breadMachineScript.getIngredient());
                }
            }
        }
        else if (collider.tag == "GetBreadCountMachine")
        {
            if (ingredientStackScript.ingredientScriptsStackCurrent.Count > 0 && ingredientStackScript.isAvailableBox_Ingredient
            && (ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient == StaticKeyEnum.TypeIngredient.BreadNormal_Done || ingredientStackScript.ingredientScriptsStackCurrent[0].typeIngredient == StaticKeyEnum.TypeIngredient.BreadPlus_Done))
            {
                CounterMachineScript counterMachineScript = collider.GetComponentInParent<CounterMachineScript>();
                if (counterMachineScript.maxCountStack > counterMachineScript.breadStackCurrent.Count)
                {
                    IngredientScript ingredientScript = ingredientStackScript.getIngredientStack(counterMachineScript.typeBread);
                    if (ingredientScript != null)
                    {
                        ingredientStackScript.cooldownBox_Ingredient();
                        counterMachineScript.breadStackCurrent.Add(ingredientScript);
                        Transform parent = counterMachineScript.listStackElementTransform[counterMachineScript.breadStackCurrent.Count - 1];
                        ingredientScript.transform.parent = parent;
                        counterMachineScript.moveIngredientToCounterMachine(ingredientScript, parent, 3f, .3f, true);
                    }
                }
            }
        }
        else if (collider.tag == "MoneyStackCollider")
        {
            MoneyStackScript moneyStackScript = collider.GetComponentInParent<MoneyStackScript>();
            if (moneyStackScript.moneyStackCurrent.Count > 0 && moneyStackScript.isAvailableGetStack)
            {
                GameObject money = moneyStackScript.getMoneyStack();
                if (money != null)
                {
                    moneyStackScript.cooldownAvailableGetStack();
                    StartCoroutine(moveEarnMoney(money));
                }
                if (moneyStackScript.isStackStartPlay && moneyStackScript.moneyStackCurrent.Count == 0)
                {
                    SaveGame.instance.setKey(0, StaticVar.keySaveGame.IsStartPlay.ToString());
                    SaveGame.instance.setValueNode(StaticVar.keyDataSave.PhaseUnlock.ToString(), 0, true);
                    GameplayController.instance.moneyStackScriptStartPlay.gameObject.SetActive(false);
                    GameplayController.instance.unlockMachine();
                }
            }
        }
        else if (collider.tag == "ColliderUnlock")
        {
            MachineUnlockScript machineUnlockScript = collider.GetComponentInParent<MachineUnlockScript>();
            if (SaveGame.instance.getCash() > 0 && machineUnlockScript.getValueUnlock() > 0)
            {
                int k = Mathf.Min(SaveGame.instance.getCash(), Mathf.Min(machineUnlockScript.getValueUnlock(), UnityEngine.Random.Range(0, 1000) % 3 + 3));
                machineUnlockScript.changeValue(k);
            }
        }
    }

    IEnumerator moveEarnMoney(GameObject money)
    {
        yield return new WaitForFixedUpdate();
        if (money != null)
        {
            UnityEvent isDone = new UnityEvent();
            isDone.AddListener(() =>
            {
                UIController.instance.onSound(StaticVar.keySound.sfx_money);
                money.SetActive(false);
                money.transform.parent = GameplayController.instance.draftPos;
                SaveGame.instance.changeCash(int.Parse(money.name.Split("_")[1]));
                Destroy(money);
            });
            float timeMove = 0.25f;
            UIController.instance.CurveMove(money, moneyMovePointTransform, 5f, timeMove, "Y", isDone, 0, true, false, false);
        }
    }
}
