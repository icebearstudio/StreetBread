using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class HideIfAttribute : PropertyAttribute
{
    public ActionOnConditionFail Action { get; private set; }
    public ConditionOperator Operator { get; private set; }
    public string[] Conditions { get; private set; }

    public HideIfAttribute(ActionOnConditionFail action, ConditionOperator conditionOperator, params string[] conditions)
    {
        Action = action;
        Operator = conditionOperator;
        Conditions = conditions;
    }
}
#endif