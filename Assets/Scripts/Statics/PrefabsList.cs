using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SimpleJSON;
using Sirenix.OdinInspector;

public class PrefabsList : MonoBehaviour
{
    public static PrefabsList instance;
    [Header("Money Prefabs")]
    public List<GameObject> moneyPrefabs;
    [Header("BreadDone Prefabs")]
    public List<GameObject> breadDonePrefabs;
    [Header("Ingredient Prefabs")]
    public List<GameObject> ingredientPrefabs;
    [Header("Customer Prefabs")]
    public List<GameObject> customerPrefabs;

    [Header("Number Sprite")]
    public List<Sprite> numberSprite;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
    }

    public GameObject getPrefabByName(List<GameObject> listPrefabs, string idPrefab)
    {
        foreach (GameObject item in listPrefabs)
        {
            if (idPrefab == item.name) return item;
        }
        return null;
    }

    public GameObject getIngredientPrefab(List<GameObject> listPrefabs, string id)
    {
        foreach (GameObject item in listPrefabs)
        {
            if (id == item.GetComponent<IngredientScript>().typeIngredient.ToString()) return item;
        }
        return null;
    }
}
