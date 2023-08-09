using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CounterMachineScript : MachineScript
{
    public StaticKeyEnum.TypeIngredient typeBread;
    public Transform parentStackTransform;
    public List<Transform> listStackElementTransform;
    public float distanceElement;
    public List<IngredientScript> breadStackCurrent = new List<IngredientScript>();
    public MoneyStackScript moneyStackScript;
    public bool isAvailableBuy, isRentStaffCashier;
    public Transform posSellingAvailable;
    public StaffScript staffSellingScript;
    [Header("Customer")]
    public CustomerScript customerWait;
    public List<Transform> customerStackWaitTransform;
    public List<CustomerScript> customerStackWaitScript;

    void Start()
    {
    }

    void Update()
    {
        checkBuyBread();
    }

    void checkBuyBread()
    {
        if (customerWait != null && (isAvailableBuy || isRentStaffCashier) && breadStackCurrent.Count >= customerWait.countBuy && !isBuying)
        {
            StartCoroutine(buyBread());
        }
    }

    bool isBuying;
    IEnumerator buyBread()
    {
        isBuying = true;
        if (isRentStaffCashier)
            staffSellingScript.setAnim(StaticKeyEnum.AnimKey.Work.ToString(), true, 0, 0.1f);
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);
        if (isAvailableBuy || isRentStaffCashier)
        {
            customerWait.setAnim(StaticKeyEnum.AnimKey.Idle_Carry.ToString(), true, 0.1f, 0.1f);
            for (int i = 0; i < customerWait.countBuy; i++)
            {
                customerWait.ingredientStackScript.addIngredientStack(getIngredient());
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f * customerWait.countBuy + 0.2f);
            UIController.instance.onSound(StaticVar.keySound.sfx_order);
            List<int> valueCoin = StaticVar.splitCoinValue(Mathf.RoundToInt(customerWait.countBuy * 10 * GameplayController.instance.profit), 5);
            for (int i = 0; i < valueCoin.Count; i++)
            {
                moneyStackScript.addMoneyStack(valueCoin[i]);
                yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(0.1f);
            customerWait.StartCoroutine(customerWait.moveToEnd());
            yield return new WaitForSeconds(0.5f);
            nextCustomerWaitBuy();
        }
        if (isRentStaffCashier)
            staffSellingScript.setAnim(StaticKeyEnum.AnimKey.Idle.ToString(), true, 0, 0.1f);
        isBuying = false;
    }

    public void nextCustomerWaitBuy()
    {
        customerStackWaitScript.Remove(customerWait);
        customerWait = null;
        for (int i = 0; i < customerStackWaitScript.Count; i++)
        {
            customerStackWaitScript[i].StartCoroutine(customerStackWaitScript[i].moveToMachine(customerStackWaitTransform[i]));
        }
    }

    public IngredientScript getIngredient()
    {
        if (breadStackCurrent.Count > 0)
        {
            IngredientScript ingredientScript = breadStackCurrent[breadStackCurrent.Count - 1];
            breadStackCurrent.Remove(ingredientScript);
            if (breadStackCurrent.Count < maxCountStack)
                maxTextPos.gameObject.SetActive(false);
            else maxTextPos.gameObject.SetActive(true);
            return ingredientScript;
        }
        else return null;
    }

    public Transform addCustomerWait(CustomerScript customerScript)
    {
        customerStackWaitScript.Add(customerScript);
        return customerStackWaitTransform[customerStackWaitScript.Count - 1];
    }

    public Transform getPosWaitLine()
    {
        if (customerStackWaitScript.Count < customerStackWaitTransform.Count)
            return customerStackWaitTransform[customerStackWaitScript.Count];
        else return null;
    }

    [ContextMenu("createListStackElementTransform")]
    public void createListStackElementTransform()
    {
        listStackElementTransform = new List<Transform>();
        int childCount = parentStackTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform element = parentStackTransform.GetChild(i);
            element.name = "Element_" + i;
            element.localPosition = new Vector3(0, i * distanceElement, 0);
            listStackElementTransform.Add(element);
        }
    }

    public void moveIngredientToCounterMachine(IngredientScript ingredientScript, Transform stackPos, float offset, float time, bool isSoundFx)
    {
        UnityEvent isDone = new UnityEvent();
        isDone.AddListener(() =>
        {
            if (isSoundFx)
                UIController.instance.onSound(StaticVar.keySound.sfx_drop);
            ingredientScript.transform.localPosition = Vector3.zero;
            ingredientScript.transform.localEulerAngles = Vector3.zero;
            if (breadStackCurrent.Count < maxCountStack)
                maxTextPos.gameObject.SetActive(false);
            else maxTextPos.gameObject.SetActive(true);
        });
        CurveMove(ingredientScript.gameObject, stackPos, offset, time, "Y", isDone, 0, true, false, false);
    }
}
