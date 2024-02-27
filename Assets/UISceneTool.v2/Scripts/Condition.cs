using System;
using UnityEngine;


[Serializable]
public struct Condition
{
    public enum Operator
    {
        [InspectorName("<")] Less,
        [InspectorName("<=")] LessEquals,
        [InspectorName("=")] Equally,
        [InspectorName(">=")] MoreEquals,
        [InspectorName(">")] More,
        [InspectorName("!=")] NotEqually
    }

    public GameObject target;
    public Component component;
    public string nameProperty;
    public int indexComponent;
    public int indexProperty;
    public Operator sign;
    public string requiredValue;

    public bool GetResult()
    {
        var properties = component.GetType().GetProperty(nameProperty);
        var valueProperty = properties.GetValue(component);

        var res = 0f;
        switch (valueProperty)
        {
            case bool property:
                res = property ? 1 : 0;
                break;
            default:
                res = (float)valueProperty;
                break;
        }

        if (requiredValue.Contains(" "))
        {
            var values = requiredValue.Trim().Split(" ");

            foreach (var value in values)
            {
                if (ResultCondition(res, int.Parse(value), sign)) return true;
            }

            return false;
        }

        return ResultCondition(res, int.Parse(requiredValue), sign);
    }

    private bool ResultCondition(float value1, float value2, Operator sign)
    {
        switch (sign)
        {
            case Operator.Less: return value1 < value2;
            case Operator.LessEquals: return value1 <= value2;
            case Operator.Equally: return (int)value1 == (int)value2;
            case Operator.MoreEquals: return value1 >= value2;
            case Operator.More: return value1 > value2;
            case Operator.NotEqually: return (int)value1 != (int)value2;
        }

        return false;
    }
}