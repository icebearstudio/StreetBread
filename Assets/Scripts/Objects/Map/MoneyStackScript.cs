using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoneyStackScript : ToolMoveScript
{
    public GameObject moneyPrefab;
    public Transform parentStackTransform;
    public List<Transform> listStackElementTransform;
    public Vector3 distanceElement;
    public Vector2Int floorSize;
    public List<GameObject> moneyStackCurrent = new List<GameObject>();
    public bool isAvailableGetStack;
    public bool isStackStartPlay;

    void Start()
    {
        moneyPrefab = PrefabsList.instance.moneyPrefabs[0];
        isAvailableGetStack = true;
    }

    [ContextMenu("createListStackElementTransform")]
    public void createListStackElementTransform()
    {
        listStackElementTransform = new List<Transform>();
        int childCount = parentStackTransform.childCount;
        int sizeFloor = floorSize.x * floorSize.y;
        for (int i = 0; i < childCount; i++)
        {
            int k = i % sizeFloor;
            Transform element = parentStackTransform.GetChild(i);
            element.name = "Element_" + (k / sizeFloor) + "_" + (k % sizeFloor) + "_" + (i / sizeFloor);
            element.localPosition = new Vector3((k % floorSize.x) * distanceElement.x, (i / sizeFloor) * distanceElement.y, (k / floorSize.x) * distanceElement.z);
            listStackElementTransform.Add(element);
        }
    }

    public void cooldownAvailableGetStack()
    {
        StartCoroutine(cooldownAvailableGetStackIEnumerator());
    }

    IEnumerator cooldownAvailableGetStackIEnumerator()
    {
        isAvailableGetStack = false;
        yield return new WaitForSeconds(.05f);
        isAvailableGetStack = true;
    }

    public GameObject getMoneyStack()
    {
        if (moneyStackCurrent.Count > 0)
        {
            GameObject money = moneyStackCurrent[moneyStackCurrent.Count - 1];
            moneyStackCurrent.Remove(money);
            return money;
        }
        else return null;
    }

    public void addMoneyStack(int value)
    {
        GameObject money = Instantiate(moneyPrefab);
        moneyStackCurrent.Add(money);
        money.name = "Money_" + value.ToString();
        money.transform.parent = listStackElementTransform[moneyStackCurrent.Count - 1];
        money.transform.localPosition = Vector3.zero;
        money.transform.localEulerAngles = Vector3.zero;
    }

    public void addMoneyStack(GameObject money)
    {
        moneyStackCurrent.Add(money);
        money.transform.parent = listStackElementTransform[moneyStackCurrent.Count - 1];
        money.transform.localPosition = Vector3.zero;
        moveMoneyToStack(money.gameObject, listStackElementTransform[moneyStackCurrent.Count - 1], 5, .3f);
    }

    public void moveMoneyToStack(GameObject ingredientGameObject, Transform stackPos, float offset, float time)
    {
        UnityEvent isDone = new UnityEvent();
        isDone.AddListener(() =>
        {
            ingredientGameObject.transform.localPosition = Vector3.zero;
            ingredientGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        });
        CurveMove(ingredientGameObject, stackPos, offset, time, "Y", isDone, 0, true, false, false);
    }
}
