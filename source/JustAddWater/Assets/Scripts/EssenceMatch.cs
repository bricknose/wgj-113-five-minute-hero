using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EssenceMatch", order = 2)]
public class EssenceMatch : ScriptableObject
{
    public Essence PrimaryMatch;
    public Essence SecondaryMatch;
    public Essence MatchResult;
}
