using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using SimpleJSON;

public static partial class StaticKeyEnum
{
    public static float defaultCharacter_SpeedMove = 6f;
    public static float defaultStaff_SpeedMove = 2.5f;
    public static int defaultCharacter_Capacity = 5;
    public static float defaultCharacter_Profit = 1f;


    public enum TypeObjectDecor
    {
        DoorArea, DoorExit, ObjectDecor, Wardrobe
    };

    public enum ObjectDecorId
    {
    };

    public enum TypeCamPos
    {
        Cam_Default
    };
    //--------------------------------------------------------------------------------------------------------------
    public enum TypeStaff
    {
        Cashier_Staff, Luggage_Staff, Ingredient_Staff
    }

    public enum TypeIngredient
    {
        BreadNormal_Done, BreadPlus_Done,
        Bread_Ingredient, Salad_Ingredient, Tomato_Ingredient, TomatoSauce_Ingredient, Sausage_Ingredient, Egg_Ingredient,
        None
    }

    public enum TypeMachine
    {
        Box_Ingredient, Bread_Machine, Counter_Machine, Shelve_Ingredient, Decor, Staff
    }

    //--------------------------------------------------------------------------------------------------------------

    // Animation ------------------
    public enum AnimKey
    {
        Idle, Idle_Carry, Walk, Walk_Carry, Run, Run_Carry, Work
    };

    public enum EventAnimationKey
    {
    };
    // Animation ------------------
    //------------------------------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------------------------------
    public static List<string> EnumToList<T>()
    {
        var array = (T[])(Enum.GetValues(typeof(T)).Cast<T>());
        List<string> lst = null;
        for (int i = 0; i < array.Length; i++)
        {
            if (lst == null)
                lst = new List<string>();
            string value = array[i].ToString();
            lst.Add(value);
        }
        return lst;
    }

    public static List<string> getListSave(string keySave)
    {
        List<string> resuilt = new List<string>();
        JSONNode dataSave = SaveGame.instance.getValueNode(keySave, new JSONObject());
        if (dataSave != null)
        {
            foreach (JSONNode item in dataSave)
                resuilt.Add(item);
        }
        return resuilt;
    }
    //---------------------------------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------------------------------
    public static T getEnumValue<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value);
    }
    //---------------------------------------------------------------------------------------------------------------------
}

[Serializable]
public class IngredientsRecipe
{
    public StaticKeyEnum.TypeIngredient typeIngredient;
    public int countIngredient;
}
