using System.Collections;
using System.Collections.Generic;
using JustAddWater.Logic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HeroEssence", order = 1)]
public class Essence : ScriptableObject, IEssence
{
    [SerializeField]
    private int _value;
    public int Value
    {
        get => _value;
        set => _value = value;
    }

    [SerializeField]
    private EssenceType _type;
    public EssenceType Type {
        get => _type;
        set => _type = value;
    }
}
