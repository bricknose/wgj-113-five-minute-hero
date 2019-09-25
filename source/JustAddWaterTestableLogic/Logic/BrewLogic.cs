using System;
using System.Collections.Generic;
using System.Linq;

namespace JustAddWater.Logic
{
    public static class BrewLogic
    {
        public static void StirClockwise(IEssence[] essences, int rowIndex, int columnIndex)
        {
            var startingEssence = GetEssence(essences, rowIndex, columnIndex);
            SetEssence(essences, rowIndex, columnIndex, GetEssence(essences, rowIndex + 1, columnIndex));
            SetEssence(essences, rowIndex + 1, columnIndex, GetEssence(essences, rowIndex + 1, columnIndex + 1));
            SetEssence(essences, rowIndex + 1, columnIndex + 1, GetEssence(essences, rowIndex, columnIndex + 1));
            SetEssence(essences, rowIndex, columnIndex + 1, startingEssence);
        }

        public static void StirCounterclockwise(IEssence[] essences, int rowIndex, int columnIndex)
        {
            var startingEssence = GetEssence(essences, rowIndex, columnIndex);
            SetEssence(essences, rowIndex, columnIndex, GetEssence(essences, rowIndex, columnIndex + 1));
            SetEssence(essences, rowIndex, columnIndex + 1, GetEssence(essences, rowIndex + 1, columnIndex + 1));
            SetEssence(essences, rowIndex + 1, columnIndex + 1, GetEssence(essences, rowIndex + 1, columnIndex));
            SetEssence(essences, rowIndex + 1, columnIndex, startingEssence);
        }

        public static SettleResult[] Settle(IEssence[] essences)
        {
            var settleList = new List<SettleResult>();

            for (var rowIndex = 3; rowIndex >= 0; rowIndex--)
            {
                for (var columnIndex = 0; columnIndex < 5; columnIndex++)
                {
                    var thisEssence = GetEssence(essences, rowIndex, columnIndex);
                    if (thisEssence == null)
                        continue;

                    var settledRows = SettleEssence(essences, rowIndex, columnIndex);
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

        public static MatchResult[] ResolveMatches(IEssence[] essences, Dictionary<IEssence, IEssenceMatch> matchMap)
        {
            var results = new List<MatchResult>();

            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < 3; columnIndex++)
                {
                    var bestHorizontalMatch = FindBestHorizontalMatch(essences, matchMap, rowIndex, columnIndex, columnIndex + 2);
                    var bestVerticalMatch = FindBestVerticalMatch(essences, matchMap, rowIndex, rowIndex + 2, columnIndex);
                    var bestMatch = FindBestMatch(bestHorizontalMatch, bestVerticalMatch);
                    if (bestMatch != null)
                    {
                        var resolvedMatch = bestMatch == bestHorizontalMatch
                            ? ResolveMatch(essences, rowIndex, rowIndex, columnIndex, columnIndex + 2, bestMatch)
                            : ResolveMatch(essences, rowIndex, rowIndex + 2, columnIndex, columnIndex, bestMatch);

                        results.Add(resolvedMatch);
                    }
                }
            }

            return results.ToArray();
        }

        private static IEssenceMatch FindBestVerticalMatch(IEssence[] essences, Dictionary<IEssence, IEssenceMatch> matchMap, int rowStartIndex, int rowEndIndex, int columnIndex)
        {
            var rowRange = rowStartIndex.To(rowEndIndex);
            var possibleMatches = rowRange
                .Select(thisRowIndex => GetEssence(essences, thisRowIndex, columnIndex))
                .Where(essence => essence != null && matchMap.ContainsKey(essence))
                .Select(essence => matchMap[essence])
                .Distinct()
                .OrderByDescending(essence => Math.Abs(essence.PrimaryMatch.Value));

            foreach (var possibleMatch in possibleMatches)
            {
                var isViableMatch = rowRange
                    .Select(thisRowIndex => GetEssence(essences, thisRowIndex, columnIndex))
                    .All(thisEssence => thisEssence != null &&
                                        (thisEssence.Type == possibleMatch.PrimaryMatch.Type ||
                                        thisEssence.Type == possibleMatch.SecondaryMatch.Type));
                if (isViableMatch)
                {
                    return possibleMatch;
                }
            }

            return null;
        }

        private static IEssenceMatch FindBestHorizontalMatch(IEssence[] essences, Dictionary<IEssence, IEssenceMatch> matchMap, int rowIndex, int columnStartIndex, int columnEndIndex)
        {
            var columnRange = columnStartIndex.To(columnEndIndex);
            var possibleMatches = columnRange
                .Select(thisColumnIndex => GetEssence(essences, rowIndex, thisColumnIndex))
                .Where(essence => essence != null && matchMap.ContainsKey(essence))
                .Select(essence => matchMap[essence])
                .Distinct()
                .OrderByDescending(essence => Math.Abs(essence.PrimaryMatch.Value));

            foreach (var possibleMatch in possibleMatches)
            {
                var isViableMatch = columnRange
                    .Select(thisColumnIndex => GetEssence(essences, rowIndex, thisColumnIndex))
                    .All(thisEssence => thisEssence != null &&
                                        (thisEssence.Type == possibleMatch.PrimaryMatch.Type ||
                                         thisEssence.Type == possibleMatch.SecondaryMatch.Type));
                if (isViableMatch)
                {
                    return possibleMatch;
                }
            }

            return null;
        }

        private static IEssenceMatch FindBestMatch(IEssenceMatch firstMatch, IEssenceMatch secondMatch)
        {
            if (firstMatch == null)
                return secondMatch;

            if (secondMatch == null)
                return firstMatch;

            return Math.Abs(firstMatch.PrimaryMatch.Value) > Math.Abs(secondMatch.PrimaryMatch.Value) ? firstMatch : secondMatch;
        }

        private static MatchResult ResolveMatch(IEssence[] essences, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex,
            IEssenceMatch match)
        {
            var matchedEssences = new List<IEssence>();

            for (var rowIndex = rowStartIndex; rowIndex > rowEndIndex; rowIndex++)
            {
                for (var columnIndex = columnStartIndex; columnIndex > columnEndIndex; columnIndex++)
                {
                    matchedEssences.Add(GetEssence(essences, rowIndex, columnIndex));
                    SetEssence(essences, rowIndex, columnIndex, null);
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

            SetEssence(essences, matchResult.MatchCenterRowIndex, matchResult.MatchCenterColumnIndex, match.MatchResult);

            return matchResult;
        }

        private static int SettleEssence(IEssence[] essences, int rowIndex, int columnIndex)
        {
            var rowsToSettle = 0;

            for (var currentRowIndex = 4; currentRowIndex > rowIndex; currentRowIndex--)
            {
                if (GetEssence(essences, currentRowIndex, columnIndex) != null)
                {
                    rowsToSettle = 0;
                    continue;
                }

                rowsToSettle++;
            }

            if (rowsToSettle > 0)
            {
                SetEssence(essences, rowIndex + rowsToSettle, columnIndex, GetEssence(essences, rowIndex, columnIndex));
                SetEssence(essences, rowIndex, columnIndex, null);
            }

            return rowsToSettle;
        }

        public static IEssence GetEssence(IEssence[] essences, int rowIndex, int columnIndex)
        {
            return essences[rowIndex * 5 + columnIndex];
        }

        public static void SetEssence(IEssence[] essences, int rowIndex, int columnIndex, IEssence essence)
        {
            essences[rowIndex * 5 + columnIndex] = essence;
        }
    }
}
