using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JustAddWater.Logic;
using Unity.Collections;
using UnityEngine;

public class Brew : MonoBehaviour
{
    public EssenceMatch[] Matches;

    public Essence[] Contents;

    public Dictionary<IEssence, IEssenceMatch> MatchMap { get; private set; }

    public Brew()
    {
        Contents = new Essence[25];
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Matches.Length > 0 && Matches.All(match => match?.PrimaryMatch != null))
        {
            MatchMap = Matches.ToDictionary(k => k.PrimaryMatch, v => v as IEssenceMatch);
        }
        else
        {
            Debug.LogWarning("Missing EssenceMatches or PrimaryMatches!");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder(Environment.NewLine);
        for (var row = 0; row < 5; row++)
        {
            for (var column = 0; column < 5; column++)
            {
                var essence = BrewLogic.GetEssence(Contents, row, column);
                stringBuilder.Append(essence.GetEssenceDebugString());
            }

            stringBuilder.Append(Environment.NewLine);
        }

        return stringBuilder.ToString();
    }
}
