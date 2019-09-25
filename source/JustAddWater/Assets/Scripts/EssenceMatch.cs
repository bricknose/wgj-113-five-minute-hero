using System.Collections;
using System.Collections.Generic;
using JustAddWater.Logic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EssenceMatch", order = 2)]
public class EssenceMatch : ScriptableObject, IEssenceMatch
{
    [SerializeField]
    private Essence _primaryMatch = null;
    public IEssence PrimaryMatch
    {
        get => _primaryMatch;
        set => value = _primaryMatch;
    }

    [SerializeField]
    private Essence _secondaryMatch = null;
    public IEssence SecondaryMatch {
        get => _secondaryMatch;
        set => value = _secondaryMatch;
    }

    [SerializeField]
    private Essence _matchResult = null;
    public IEssence MatchResult {
        get => _matchResult;
        set => value = _matchResult;
    }
}
