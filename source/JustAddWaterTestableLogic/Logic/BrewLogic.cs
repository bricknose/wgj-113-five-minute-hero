using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LogicTests")]
namespace JustAddWater.Logic
{
    public static class BrewLogic
    {
        public static void StirClockwise(EssenceArray essences, int columnIndex, int rowIndex)
        {
            var startingEssence = essences.Get(columnIndex, rowIndex);
            essences.Set(columnIndex, rowIndex, essences.Get(columnIndex, rowIndex + 1));
            essences.Set(columnIndex, rowIndex + 1, essences.Get(columnIndex + 1, rowIndex + 1));
            essences.Set(columnIndex + 1, rowIndex + 1, essences.Get(columnIndex + 1, rowIndex));
            essences.Set(columnIndex + 1, rowIndex, startingEssence);
        }

        public static void StirCounterclockwise(EssenceArray essences, int columnIndex, int rowIndex)
        {
            var startingEssence = essences.Get(columnIndex, rowIndex);
            essences.Set(columnIndex, rowIndex, essences.Get(columnIndex + 1, rowIndex));
            essences.Set(columnIndex + 1, rowIndex, essences.Get(columnIndex + 1, rowIndex + 1));
            essences.Set(columnIndex + 1, rowIndex + 1, essences.Get(columnIndex, rowIndex + 1));
            essences.Set(columnIndex, rowIndex + 1, startingEssence);
        }

        public static SettleResult[] Settle(EssenceArray essences)
        {
            var settleList = new List<SettleResult>();

            for (var rowIndex = essences.Rows - 1; rowIndex >= 0; rowIndex--)
            {
                for (var columnIndex = 0; columnIndex < essences.Columns; columnIndex++)
                {
                    var thisEssence = essences.Get(columnIndex, rowIndex);
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

        public static MatchResult[] ResolveMatches(EssenceArray essences, Dictionary<IEssence, IEssenceMatch> matchMap)
        {
            var results = new List<MatchResult>();

            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < 3; columnIndex++)
                {
                    var columnEndIndex = columnIndex + 2;
                    var rowEndIndex = rowIndex + 2;

                    var columnRange = GetColumnEssences(essences, columnIndex, columnEndIndex, rowIndex);
                    var rowRange = GetRowEssences(essences, columnIndex, rowIndex, rowEndIndex);

                    var bestHorizontalMatch = FindBestMatch(columnRange, matchMap);
                    var bestVerticalMatch = FindBestMatch(rowRange, matchMap);

                    var bestMatch = PickBestMatch(bestHorizontalMatch, bestVerticalMatch);

                    if (bestMatch == null)
                    {
                        continue;
                    }

                    var bestMatchRange = 
                        bestMatch == bestHorizontalMatch 
                            ? GetColumnRange(columnIndex, columnEndIndex, rowIndex) 
                            : GetRowRange(columnIndex, rowIndex, rowEndIndex);

                    var matchResult = PerformMatchReplacement(essences, bestMatchRange, bestMatch);

                    results.Add(matchResult);
                }
            }

            return results.ToArray();
        }

        internal static IEssence[] GetColumnEssences(EssenceArray essences, int columnIndex, int rowStartIndex, int rowEndIndex)
        {
            var rowRange = rowStartIndex.To(rowEndIndex);
            return rowRange
                .Select(thisRowIndex => essences.Get(columnIndex, thisRowIndex))
                .ToArray();
        }

        internal static IEssence[] GetRowEssences(EssenceArray essences, int columnStartIndex, int columnEndIndex, int rowIndex)
        {
            var columnRange = columnStartIndex.To(columnEndIndex);
            return columnRange
                .Select(thisColumnIndex => essences.Get(thisColumnIndex, rowIndex))
                .ToArray();
        }

        internal static IEssenceMatch FindBestMatch(IEssence[] essences, Dictionary<IEssence, IEssenceMatch> matchMap)
        {
            var possibleMatches = essences
                .Where(essence => essence != null && matchMap.ContainsKey(essence))
                .Select(essence => matchMap[essence])
                .Distinct()
                .OrderByDescending(essenceMatch => Math.Abs(essenceMatch.PrimaryMatch.Value));

            foreach (var possibleMatch in possibleMatches)
            {
                var isViableMatch = essences
                    .All(thisEssence => thisEssence != null &&
                                        (thisEssence == possibleMatch.PrimaryMatch ||
                                         thisEssence == possibleMatch.SecondaryMatch));
                if (isViableMatch)
                {
                    return possibleMatch;
                }
            }

            return null;
        }

        internal static IEssenceMatch PickBestMatch(IEssenceMatch firstMatch, IEssenceMatch secondMatch)
        {
            if (firstMatch == null)
                return secondMatch;

            if (secondMatch == null)
                return firstMatch;

            return Math.Abs(firstMatch.PrimaryMatch.Value) > Math.Abs(secondMatch.PrimaryMatch.Value) ? firstMatch : secondMatch;
        }

        internal static (int columnIndex, int rowIndex)[] GetColumnRange(int columnStartIndex, int columnEndIndex, int rowIndex)
        {
            return columnStartIndex.To(columnEndIndex)
                .Select(thisColumnIndex => (thisColumnIndex, rowIndex))
                .ToArray();
        }

        internal static (int columnIndex, int rowIndex)[] GetRowRange(int columnIndex, int rowStartIndex, int rowEndIndex)
        {
            return rowStartIndex.To(rowEndIndex)
                .Select(thisRowIndex => (columnIndex, thisRowIndex))
                .ToArray();
        }

        internal static MatchResult PerformMatchReplacement(EssenceArray essences, (int columnIndex, int rowIndex)[] coordinates, IEssenceMatch match)
        {
            var matchedEssences = new List<IEssence>();

            foreach (var coordinate in coordinates)
            {
                matchedEssences.Add(essences.Get(coordinate.columnIndex, coordinate.rowIndex));
                essences.Set(coordinate.columnIndex, coordinate.rowIndex, null);
            }

            var matchResult = new MatchResult
            {
                RowStartIndex = coordinates.Min(coordinate => coordinate.rowIndex),
                RowEndIndex = coordinates.Max(coordinate => coordinate.rowIndex),
                ColumnStartIndex = coordinates.Min(coordinate => coordinate.columnIndex),
                ColumnEndIndex = coordinates.Max(coordinate => coordinate.columnIndex),
                MatchType = match,
                Essences = matchedEssences.Distinct().ToArray()
            };

            essences.Set(matchResult.MatchCenterColumnIndex, matchResult.MatchCenterRowIndex, match.MatchResult);

            return matchResult;
        }

        private static int SettleEssence(EssenceArray essences, int rowIndex, int columnIndex)
        {
            var rowsToSettle = 0;

            for (var currentRowIndex = 4; currentRowIndex > rowIndex; currentRowIndex--)
            {
                if (essences.Get(columnIndex, currentRowIndex) != null)
                {
                    rowsToSettle = 0;
                    continue;
                }

                rowsToSettle++;
            }

            if (rowsToSettle > 0)
            {
                essences.Set(columnIndex, rowIndex + rowsToSettle, essences.Get(columnIndex, rowIndex));
                essences.Set(columnIndex, rowIndex, null);
            }

            return rowsToSettle;
        }

        //public static IEssence GetEssence(IEssence[] essences, int columns, int columnIndex, int rowIndex)
        //{
        //    return essences[rowIndex * columns + columnIndex];
        //}

        //public static void SetEssence(IEssence[] essences, int columns, int columnIndex, int rowIndex, IEssence essence)
        //{
        //    essences[rowIndex * columns + columnIndex] = essence;
        //}
    }
}
