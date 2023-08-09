using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerScript : CharacterAnimScript
{
    public StaticKeyEnum.TypeIngredient typeBreadBuy;
    public int countBuy;
    public CounterMachineScript machineTarget;
    float speedMove = 2f;
    public IngredientStackScript ingredientStackScript;
    public Transform orderPos;

    new void Start()
    {
        base.Start();
    }

    public void setupTargetBuy(StaticKeyEnum.TypeIngredient typeBreadBuy, int countBuy, CounterMachineScript machineTarget)
    {
        this.typeBreadBuy = typeBreadBuy;
        this.countBuy = countBuy;
        this.machineTarget = machineTarget;
        Transform posMove = machineTarget.addCustomerWait(this);
        StartCoroutine(moveToMachine(posMove));
    }

    int id;
    public IEnumerator moveToMachine(Transform posMove)
    {
        yield return new WaitForFixedUpdate();
        float timeMove = (posMove.position - transform.position).magnitude / speedMove;
        //CurveMove(gameObject, posMove, 0, timeMove, "X", isDone, 0, true, false, false);
        iTween.MoveTo(gameObject, posMove, timeMove, iTween.EaseType.linear);
        setAnim(StaticKeyEnum.AnimKey.Walk.ToString(), true, 0, 0.15f);
        if ((posMove.position - transform.position).normalized != Vector3.zero)
            transform.forward = -(posMove.position - transform.position).normalized;
        yield return new WaitForSeconds(timeMove);
        yield return new WaitForFixedUpdate();
        callBuy();
    }

    void callBuy()
    {
        setAnim(StaticKeyEnum.AnimKey.Idle.ToString(), true, 0, 0.15f);
        transform.forward = -Vector3.forward;
        if (Vector3.Distance(transform.position, machineTarget.customerStackWaitTransform[0].position) < 0.1f)
        {
            machineTarget.customerWait = this;
            setOrderPos();
            orderPos.gameObject.SetActive(true);
        }
    }

    void setOrderPos()
    {
        if (typeBreadBuy == StaticKeyEnum.TypeIngredient.BreadNormal_Done)
        {
            orderPos.GetChild(1).GetChild(0).gameObject.SetActive(true);
            orderPos.GetChild(1).GetChild(1).gameObject.SetActive(false);
        }
        else if (typeBreadBuy == StaticKeyEnum.TypeIngredient.BreadPlus_Done)
        {
            orderPos.GetChild(1).GetChild(0).gameObject.SetActive(false);
            orderPos.GetChild(1).GetChild(1).gameObject.SetActive(true);
        }
        orderPos.GetChild(2).GetComponent<SpriteRenderer>().sprite = PrefabsList.instance.numberSprite[countBuy];
    }

    public IEnumerator moveToEnd()
    {
        orderPos.gameObject.SetActive(false);
        yield return new WaitForFixedUpdate();
        Transform posMove = GameplayController.instance.pointSpawnCustomer;
        float timeMove = (posMove.position - transform.position).magnitude / speedMove;
        //CurveMove(gameObject, posMove, 0, timeMove, "X", isDone, 0, true, false, false);
        iTween.MoveTo(gameObject, posMove, timeMove, iTween.EaseType.linear);
        setAnim(StaticKeyEnum.AnimKey.Walk_Carry.ToString(), true, 0, 0.15f);
        if ((posMove.position - transform.position).normalized != Vector3.zero)
            transform.forward = -(posMove.position - transform.position).normalized;
        yield return new WaitForSeconds(timeMove);
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
}
