using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HeroEssence", order = 1)]
public class HeroEssence : ScriptableObject
{
    public int Value;
    public EssenceType Type;

    public enum EssenceType
    {
        Health,
        Attack,
        Defense,
        Magic,
        Resistance,
        Wild
    }

    public override string ToString()
    {
        return $"{GetEnumString(Type)}:{GetValueString(Value)}";
    }

    private static string GetEnumString(EssenceType type)
    {
        switch (type)
        {
            case EssenceType.Health:
                return "HP";
            case EssenceType.Attack:
                return "AK";
            case EssenceType.Defense:
                return "DF";
            case EssenceType.Magic:
                return "MG";
            case EssenceType.Resistance:
                return "RS";
            case EssenceType.Wild:
                return "WD";
            default:
                return "??";
        }
    }

    private static string GetValueString(int value)
    {
        return value >= 0 ? $"+{value}" : value.ToString();
    }
}
