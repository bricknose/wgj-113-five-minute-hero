using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine;
using Assets.Scripts;

public class Brew : MonoBehaviour
{
    public EssenceMatch[] Matches;

    [SerializeField]
    private Essence[] _contents;

    private Dictionary<Essence, EssenceMatch> _matchMap;

    public Brew()
    {
        _contents = new Essence[25];
    }

    // Start is called before the first frame update
    void Start()
    {
        _matchMap = Matches.ToDictionary(k => k.PrimaryMatch);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StirClockwise(int rowIndex, int columnIndex)
    {
        var startingEssence = GetEssence(rowIndex, columnIndex);
        SetEssence(rowIndex, columnIndex, GetEssence(rowIndex + 1, columnIndex));
        SetEssence(rowIndex + 1, columnIndex, GetEssence(rowIndex + 1, columnIndex + 1));
        SetEssence(rowIndex + 1, columnIndex + 1, GetEssence(rowIndex, columnIndex + 1));
        SetEssence(rowIndex, columnIndex + 1, startingEssence);
    }

    public void StirCounterclockwise(int rowIndex, int columnIndex)
    {
        var startingEssence = GetEssence(rowIndex, columnIndex);
        SetEssence(rowIndex, columnIndex, GetEssence(rowIndex, columnIndex + 1));
        SetEssence(rowIndex, columnIndex + 1, GetEssence(rowIndex + 1, columnIndex + 1));
        SetEssence(rowIndex + 1, columnIndex + 1, GetEssence(rowIndex + 1, columnIndex));
        SetEssence(rowIndex + 1, columnIndex, startingEssence);
    }

    public SettleResult[] Settle()
    {
        var settleList = new List<SettleResult>();

        for (var rowIndex = 3; rowIndex >= 0; rowIndex--)
        {
            for (var columnIndex = 0; columnIndex < 5; columnIndex++)
            {
                var thisEssence = GetEssence(rowIndex, columnIndex);
                if (!thisEssence)
                    continue;

                var settledRows = SettleEssence(rowIndex, columnIndex);
                if (settledRows > 0)
                {
                    var newRowIndex = rowIndex + settledRows;
                    settleList.Add(new SettleResult
                    {
                        Essence = thisEssence,
                        NewRowIndex = newRowIndex,
                        OldRowIndex = rowIndex,
                        ColumnIndex = columnIndex
                    });
                }
            }
        }

        return settleList.ToArray();
    }

    public MatchResult[] ResolveMatches()
    {
        var results = new List<MatchResult>();

        for (var rowIndex = 0; rowIndex < 3; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < 3; columnIndex++)
            {
                var bestHorizontalMatch = FindBestHorizontalMatch(rowIndex, columnIndex, columnIndex + 2);
                var bestVerticalMatch = FindBestVerticalMatch(rowIndex, rowIndex + 2, columnIndex);
                var bestMatch = FindBestMatch(bestHorizontalMatch, bestVerticalMatch);
                if (bestMatch)
                {
                    var resolvedMatch = bestMatch == bestHorizontalMatch
                        ? ResolveMatch(rowIndex, rowIndex, columnIndex, columnIndex + 2, bestMatch)
                        : ResolveMatch(rowIndex, rowIndex + 2, columnIndex, columnIndex, bestMatch);

                    results.Add(resolvedMatch);
                }
            }
        }

        return results.ToArray();
    }

    private EssenceMatch FindBestVerticalMatch(int rowStartIndex, int rowEndIndex, int columnIndex)
    {
        var rowRange = rowStartIndex.To(rowEndIndex);
        var possibleMatches = rowRange
            .Select(thisRowIndex => GetEssence(thisRowIndex, columnIndex))
            .Where(essence => essence && _matchMap.ContainsKey(essence))
            .Select(essence => _matchMap[essence])
            .Distinct()
            .OrderByDescending(essence => Math.Abs(essence.PrimaryMatch.Value));

        foreach (var possibleMatch in possibleMatches)
        {
            var isViableMatch = rowRange
                .Select(thisRowIndex => GetEssence(thisRowIndex, columnIndex))
                .All(thisEssence => thisEssence &&
                                    (thisEssence.Type == possibleMatch.PrimaryMatch.Type ||
                                    thisEssence.Type == possibleMatch.SecondaryMatch.Type));
            if (isViableMatch)
            {
                return possibleMatch;
            }
        }

        return null;
    }

    private EssenceMatch FindBestHorizontalMatch(int rowIndex, int columnStartIndex, int columnEndIndex)
    {
        var columnRange = columnStartIndex.To(columnEndIndex);
        var possibleMatches = columnRange
            .Select(thisColumnIndex => GetEssence(rowIndex, thisColumnIndex))
            .Where(essence => essence && _matchMap.ContainsKey(essence))
            .Select(essence => _matchMap[essence])
            .Distinct()
            .OrderByDescending(essence => Math.Abs(essence.PrimaryMatch.Value));

        foreach (var possibleMatch in possibleMatches)
        {
            var isViableMatch = columnRange
                .Select(thisColumnIndex => GetEssence(rowIndex, thisColumnIndex))
                .All(thisEssence => thisEssence &&
                                    (thisEssence.Type == possibleMatch.PrimaryMatch.Type ||
                                     thisEssence.Type == possibleMatch.SecondaryMatch.Type));
            if (isViableMatch)
            {
                return possibleMatch;
            }
        }

        return null;
    }

    private EssenceMatch FindBestMatch(EssenceMatch firstMatch, EssenceMatch secondMatch)
    {
        if (!firstMatch)
            return secondMatch;

        if (!secondMatch)
            return firstMatch;

        return Math.Abs(firstMatch.PrimaryMatch.Value) > Math.Abs(secondMatch.PrimaryMatch.Value) ? firstMatch : secondMatch;
    }

    private MatchResult ResolveMatch(int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex,
        EssenceMatch match)
    {
        var matchedEssences = new List<Essence>();

        for (var rowIndex = rowStartIndex; rowIndex > rowEndIndex; rowIndex++)
        {
            for (var columnIndex = columnStartIndex; columnIndex > columnEndIndex; columnIndex++)
            {
                matchedEssences.Add(GetEssence(rowIndex, columnIndex));
                SetEssence(rowIndex, columnIndex, null);
            }
        }

        var matchResult = new MatchResult
        {
            RowStartIndex = rowStartIndex,
            RowEndIndex = rowEndIndex,
            ColumnStartIndex = columnStartIndex,
            ColumnEndIndex = columnEndIndex,
            MatchType = match,
            Essences = matchedEssences.ToArray()
        };

        SetEssence(matchResult.MatchCenterRowIndex, matchResult.MatchCenterColumnIndex, match.MatchResult);

        return matchResult;
    }

    private int SettleEssence(int rowIndex, int columnIndex)
    {
        var rowsToSettle = 0;

        for (var currentRowIndex = 4; currentRowIndex > rowIndex; currentRowIndex--)
        {
            if (GetEssence(currentRowIndex, columnIndex) != null)
            {
                rowsToSettle = 0;
                continue;
            }

            rowsToSettle++;
        }

        if (rowsToSettle > 0)
        {
            SetEssence(rowIndex + rowsToSettle, columnIndex, GetEssence(rowIndex, columnIndex));
            SetEssence(rowIndex, columnIndex, null);
        }

        return rowsToSettle;
    }

    private Essence GetEssence(int rowIndex, int columnIndex)
    {
        return _contents[rowIndex * 5 + columnIndex];
    }

    private void SetEssence(int rowIndex, int columnIndex, Essence essence)
    {
        _contents[rowIndex * 5 + columnIndex] = essence;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder(Environment.NewLine);
        for (var row = 0; row < 5; row++)
        {
            for (var column = 0; column < 5; column++)
            {
                var essence = GetEssence(row, column);
                stringBuilder.Append(GetEssenceString(essence));
            }

            stringBuilder.Append(Environment.NewLine);
        }

        return stringBuilder.ToString();
    }

    private static string GetEssenceString(Essence essence)
    {
        return essence == null ? $"[  :  ]" : $"[{essence}]";
    }

    public struct SettleResult
    {
        public int OldRowIndex;
        public int NewRowIndex;
        public int ColumnIndex;
        public Essence Essence;

        public override string ToString()
        {
            return $"{GetEssenceString(Essence)}:[{OldRowIndex},{ColumnIndex}]->[{NewRowIndex},{ColumnIndex}]";
        }
    }

    public struct MatchResult
    {
        public int RowStartIndex;
        public int RowEndIndex;
        public int ColumnStartIndex;
        public int ColumnEndIndex;
        public Essence[] Essences;
        public EssenceMatch MatchType;
        public int MatchCenterRowIndex => (RowStartIndex + RowEndIndex) / 2;
        public int MatchCenterColumnIndex => (ColumnStartIndex + ColumnEndIndex) / 2;

        public override string ToString()
        {
            return $"[{RowStartIndex},{ColumnStartIndex}]-[{RowEndIndex},{ColumnEndIndex}]:{string.Join("+", Essences.Select(e => e.ToString()))}={MatchType.MatchResult}";
        }
    }
}
