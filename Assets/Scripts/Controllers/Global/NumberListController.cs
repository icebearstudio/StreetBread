using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NumberListController : MonoBehaviour
{

    public static NumberListController instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        //setRandomNumber();
    }

    // Use this for initialization
    void Start()
    {
        //setRandomNumber();
        if (SceneManager.GetActiveScene().name != "Gameplay")
            Invoke("setRandomNumber", 1f);
        else
            setRandomNumber();
    }

    [HideInInspector]
    public List<int> randomNumber;
    public void setRandomNumber()
    {
        randomNumber = new List<int>();
        for (int i = 0; i < 50000; i++)
        {
            randomNumber.Add(i);
        }
        int n = randomNumber.Count - 1;
        while (n > 1)
        {
            int k = UnityEngine.Random.Range(0, randomNumber.Count);
            int temp = randomNumber[n];
            randomNumber[n] = randomNumber[k];
            randomNumber[k] = temp;
            n--;
        }
    }

    public int getRandomNumber(int[] probabilityArray, int k)
    {
        List<int> randomNumber = this.randomNumber;
        int i = randomNumber[UnityEngine.Random.Range(0, randomNumber.Count)] % k;
        List<int> intelligenceList = new List<int>();
        for (int j = 0; j < probabilityArray.Length; j++)
        {
            intelligenceList.Add(probabilityArray[j]);
        }
        int[] array = new int[intelligenceList.Count];
        for (int j = 0; j < intelligenceList.Count; j++)
        {
            if (j == 0)
            {
                array[0] = intelligenceList[0];
            }
            else
            {
                array[j] = intelligenceList[j] + array[j - 1];
            }
        }
        for (int j = 0; j < array.Length; j++)
        {
            if (i < array[j])
                return j;
        }
        return 0;
    }

    public int getRandomNumber(int[] probabilityArray)
    {
        int k = StaticVar.totalArrayValue(probabilityArray);
        List<int> randomNumber = this.randomNumber;
        int i = randomNumber[UnityEngine.Random.Range(0, randomNumber.Count)] % k;
        List<int> intelligenceList = new List<int>();
        for (int j = 0; j < probabilityArray.Length; j++)
        {
            intelligenceList.Add(probabilityArray[j]);
        }
        int[] array = new int[intelligenceList.Count];
        for (int j = 0; j < intelligenceList.Count; j++)
        {
            if (j == 0)
            {
                array[0] = intelligenceList[0];
            }
            else
            {
                array[j] = intelligenceList[j] + array[j - 1];
            }
        }
        for (int j = 0; j < array.Length; j++)
        {
            if (i < array[j])
                return j;
        }
        return 0;
    }

    public int getNumberRandom(int count)
    {
        if (count == 0) return 0;
        return UnityEngine.Random.Range(0, 100000) % count;
    }
}
