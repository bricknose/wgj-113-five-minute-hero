using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using JustAddWater.Logic;
using Xunit;

namespace LogicTests
{
    public class BrewLogicTests
    {
        [Theory]
        [MemberData(nameof(StirOffsetCombinations))]
        public void StirClockwise_Always_Rotates2x2Clockwise(int columnOffset, int rowOffset)
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Attack);
            var thirdEssence = GetEssence(3, EssenceType.Attack);
            var fourthEssence = GetEssence(4, EssenceType.Attack);
            var essenceArray = new EssenceArray(5, 5);
            essenceArray.Set(columnOffset, rowOffset, firstEssence);
            essenceArray.Set(columnOffset + 1, rowOffset, secondEssence);
            essenceArray.Set(columnOffset + 1, rowOffset + 1, thirdEssence);
            essenceArray.Set(columnOffset, rowOffset + 1, fourthEssence);

            BrewLogic.StirClockwise(essenceArray, columnOffset, rowOffset);

            essenceArray.Get(columnOffset, rowOffset).Should().BeEquivalentTo(fourthEssence);
            essenceArray.Get(columnOffset, rowOffset + 1).Should().BeEquivalentTo(thirdEssence);
            essenceArray.Get(columnOffset + 1, rowOffset + 1).Should().BeEquivalentTo(secondEssence);
            essenceArray.Get(columnOffset + 1, rowOffset).Should().BeEquivalentTo(firstEssence);
        }

        [Theory]
        [MemberData(nameof(StirOffsetCombinations))]
        public void StirCounterclockwise_Always_Rotates2x2Counterclockwise(int columnOffset, int rowOffset)
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Attack);
            var thirdEssence = GetEssence(3, EssenceType.Attack);
            var fourthEssence = GetEssence(4, EssenceType.Attack);
            var essenceArray = new EssenceArray(5, 5);
            essenceArray.Set(columnOffset, rowOffset, firstEssence);
            essenceArray.Set(columnOffset + 1, rowOffset, secondEssence);
            essenceArray.Set(columnOffset + 1, rowOffset + 1, thirdEssence);
            essenceArray.Set(columnOffset, rowOffset + 1, fourthEssence);

            BrewLogic.StirCounterclockwise(essenceArray, columnOffset, rowOffset);

            essenceArray.Get(columnOffset, rowOffset).Should().BeEquivalentTo(secondEssence);
            essenceArray.Get(columnOffset + 1, rowOffset).Should().BeEquivalentTo(thirdEssence);
            essenceArray.Get(columnOffset + 1, rowOffset + 1).Should().BeEquivalentTo(fourthEssence);
            essenceArray.Get(columnOffset, rowOffset + 1).Should().BeEquivalentTo(firstEssence);
        }

        [Fact]
        public void Settle_WithEssenceInEmptyBrew_MovesEssencesToBottomAndReturnsSettleResult()
        {
            var essence = GetEssence(1, EssenceType.Attack);
            var essenceArray = new EssenceArray(1, 5);
            essenceArray.Set(0, 0, essence);

            var settleResult = BrewLogic.Settle(essenceArray);

            essenceArray.Get(0, 4).Should().BeEquivalentTo(essence);
            settleResult.Length.Should().Be(1);
            var settledResult = settleResult.First();
            settledResult.Essence.Should().BeEquivalentTo(essence);
            settledResult.ColumnIndex.Should().Be(0);
            settledResult.OldRowIndex.Should().Be(0);
            settledResult.NewRowIndex.Should().Be(4);
        }

        [Fact]
        public void Settle_WithEssenceAtBottomOfBrew_KeepsEssenceLocationAndReturnsEmptySettleResults()
        {
            var essence = GetEssence(1, EssenceType.Attack);
            var essenceArray = new EssenceArray(1, 5);
            essenceArray.Set(0, 4, essence);

            var settleResult = BrewLogic.Settle(essenceArray);

            essenceArray.Get(0, 4).Should().BeEquivalentTo(essence);
            settleResult.Length.Should().Be(0);
        }

        [Fact]
        public void Settle_WithMultipleEssences_MovesEssencesDownAndReturnsSettleResultInBottomToTopOrder()
        {
            var anExistingEssence = GetEssence(99, EssenceType.Attack);
            var anotherExistingEssence = GetEssence(999, EssenceType.Attack);
            var essence = GetEssence(1, EssenceType.Attack);
            var essenceArray = new EssenceArray(1, 5);
            essenceArray.Set(0, 4, anExistingEssence);
            essenceArray.Set(0, 2, anotherExistingEssence);
            essenceArray.Set(0, 0, essence);

            var settleResult = BrewLogic.Settle(essenceArray);

            essenceArray.Get(0, 2).Should().BeEquivalentTo(essence);
            settleResult.Length.Should().Be(2);
            settleResult.First().Essence.Should().BeEquivalentTo(anotherExistingEssence);
            settleResult.First().OldRowIndex.Should().Be(2);
            settleResult.First().NewRowIndex.Should().Be(3);
            settleResult.Last().Essence.Should().BeEquivalentTo(essence);
            settleResult.Last().OldRowIndex.Should().Be(0);
            settleResult.Last().NewRowIndex.Should().Be(2);
        }

        [Fact]
        public void ResolveMatches_WithNoMatchMatches_DoesNotChangeElementsAndReturnsNoResults()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Defense);
            var matchMap = new Dictionary<IEssence, IEssenceMatch>
            {
                { firstEssence, GetEssenceMatch(null, firstEssence) }
            };
            var essenceArray = new EssenceArray(3, new []
            {
                firstEssence, secondEssence, firstEssence,
                secondEssence, firstEssence, secondEssence,
                firstEssence, secondEssence, firstEssence
            });

            var result = BrewLogic.ResolveMatches(essenceArray, matchMap);

            ((IEssence[]) essenceArray).Should().ContainInOrder
            (
                firstEssence, secondEssence, firstEssence, secondEssence, firstEssence, secondEssence, firstEssence, secondEssence, firstEssence
            );
            result.Length.Should().Be(0);
        }

        [Fact]
        public void ResolveMatches_WithSingleMatch_FillsInMatchAndReturnsMatchResult()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Defense);
            var matchMap = new Dictionary<IEssence, IEssenceMatch>
            {
                { firstEssence, GetEssenceMatch(secondEssence, firstEssence) }
            };
            var essenceArray = new EssenceArray(3, new[]
            {
                firstEssence, firstEssence, firstEssence,
                null, null, null,
                null, null, null
            });

            var result = BrewLogic.ResolveMatches(essenceArray, matchMap);

            ((IEssence[])essenceArray).Should().ContainInOrder
            (
                null, secondEssence, null,
                null, null, null,
                null, null, null
            );
            result.Length.Should().Be(1);
        }

        [Fact]
        public void ResolveMatches_WithMultipleMatches_FillsInMultipleMatchesAndReturnsMatchResults()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Defense);
            var matchMap = new Dictionary<IEssence, IEssenceMatch>
            {
                { firstEssence, GetEssenceMatch(secondEssence, firstEssence) },
                { secondEssence, GetEssenceMatch(firstEssence, secondEssence) }
            };
            var essenceArray = new EssenceArray(3, new[]
            {
                firstEssence, firstEssence, firstEssence,
                secondEssence, null, null,
                secondEssence, null, null,
                secondEssence, null, null
            });

            var result = BrewLogic.ResolveMatches(essenceArray, matchMap);

            ((IEssence[])essenceArray).Should().ContainInOrder
            (
                null, secondEssence, null,
                null, null, null,
                firstEssence, null, null,
                null, null, null
            );
            result.Length.Should().Be(2);
        }


        [Fact]
        public void ResolveMatches_WithCompetingMatches_FillsInBestSingleMatchAndReturnsMatchResult()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Defense);
            var matchMap = new Dictionary<IEssence, IEssenceMatch>
            {
                { firstEssence, GetEssenceMatch(secondEssence, firstEssence, secondEssence) },
                { secondEssence, GetEssenceMatch(firstEssence, secondEssence, firstEssence) }
            };
            var essenceArray = new EssenceArray(3, new[]
            {
                firstEssence, firstEssence, firstEssence,
                secondEssence, null, null,
                secondEssence, null, null
            });

            var result = BrewLogic.ResolveMatches(essenceArray, matchMap);

            ((IEssence[])essenceArray).Should().ContainInOrder
            (
                null, firstEssence, firstEssence,
                firstEssence, null, null,
                null, null, null
            );
            result.Length.Should().Be(1);
        }

        #region Internal Tests

        [Fact]
        public void PickBestMatch_WithNullSecondMatch_ReturnsFirstMatch()
        {
            var firstMatch = A.Fake<IEssenceMatch>();
            IEssenceMatch secondMatch = null;

            var result = BrewLogic.PickBestMatch(firstMatch, secondMatch);

            result.Should().BeEquivalentTo(firstMatch);
        }

        [Fact]
        public void PickBestMatch_WithNullFirstMatch_ReturnsSecondMatch()
        {
            IEssenceMatch firstMatch = null;
            var secondMatch = A.Fake<IEssenceMatch>(); 

            var result = BrewLogic.PickBestMatch(firstMatch, secondMatch);

            result.Should().BeEquivalentTo(secondMatch);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public void PickBestMatch_WithNoNullMatches_ReturnsMatchWithHighestValue(int firstValue, int secondValue)
        {
            var firstMatch = A.Fake<IEssenceMatch>();
            var secondMatch = A.Fake<IEssenceMatch>();
            var expectedMatch = firstValue > secondValue ? firstMatch : secondMatch;
            A.CallTo(() => firstMatch.PrimaryMatch.Value)
                .Returns(firstValue);
            A.CallTo(() => secondMatch.PrimaryMatch.Value)
                .Returns(secondValue);

            var result = BrewLogic.PickBestMatch(firstMatch, secondMatch);

            result.Should().BeEquivalentTo(expectedMatch);
        }

        [Fact]
        public void FindBestMatch_WithNulls_ReturnsNull()
        {
            var essences = new IEssence[3];
            var essenceMap = new Dictionary<IEssence, IEssenceMatch>();

            var result = BrewLogic.FindBestMatch(essences, essenceMap);

            result.Should().BeNull();
        }

        [Fact]
        public void FindBestMatch_WithNoNullsAndNoMatch_ReturnsNull()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Attack);
            var firstMatch = GetEssenceMatch(firstEssence, firstEssence);
            var secondMatch = GetEssenceMatch(secondEssence, secondEssence);
            var essences = new[] { firstEssence, firstEssence, secondEssence };
            var essenceMap = new Dictionary<IEssence, IEssenceMatch>
            {
                {firstEssence, firstMatch},
                { secondEssence, secondMatch }
            };

            var result = BrewLogic.FindBestMatch(essences, essenceMap);

            result.Should().BeNull();
        }

        [Fact]
        public void FindBestMatch_WithPrimaryMatch_ReturnsMatch()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var firstMatch = GetEssenceMatch(firstEssence, firstEssence);
            var essences = new[] { firstEssence, firstEssence, firstEssence };
            var essenceMap = new Dictionary<IEssence, IEssenceMatch>
            {
                {firstEssence, firstMatch}
            };

            var result = BrewLogic.FindBestMatch(essences, essenceMap);

            result.Should().BeEquivalentTo(firstMatch);
        }

        [Fact]
        public void FindBestMatch_WithSecondaryMatch_ReturnsMatch()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(1, EssenceType.Attack);
            var firstMatch = GetEssenceMatch(firstEssence, firstEssence, secondEssence);
            var essences = new[] { firstEssence, secondEssence, firstEssence };
            var essenceMap = new Dictionary<IEssence, IEssenceMatch>
            {
                {firstEssence, firstMatch}
            };

            var result = BrewLogic.FindBestMatch(essences, essenceMap);

            result.Should().BeEquivalentTo(firstMatch);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(-2)]
        public void FindBestMatch_WithMultipleMatches_ReturnsHighestPowerMatch(int secondEssenceValue)
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(secondEssenceValue, EssenceType.Attack);
            var firstMatch = GetEssenceMatch(firstEssence, firstEssence, secondEssence);
            var secondMatch = GetEssenceMatch(secondEssence, secondEssence, firstEssence);
            var essences =  new[] { firstEssence, secondEssence, firstEssence };
            var essenceMap = new Dictionary<IEssence, IEssenceMatch>
            {
                {firstEssence, firstMatch},
                {secondEssence, secondMatch}
            };

            var result = BrewLogic.FindBestMatch(essences, essenceMap);

            result.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetEssencesForColumn_Always_ReturnsArrayOfEssencesInColumn()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Attack);
            var essenceArray = new EssenceArray(3, new []
            {
                null, firstEssence, null,
                null, null, null,
                null, secondEssence, null
            });

            var result = BrewLogic.GetEsencesForColumn(essenceArray, 1, 0, 2);

            result.Length.Should().Be(3);
            result.Should().ContainInOrder(firstEssence, null, secondEssence);
        }

        [Fact]
        public void GetEssencesForRow_Always_ReturnsArrayOfEssencesInRow()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Attack);
            var essenceArray = new EssenceArray(3, new[]
            {
                null, null, null,
                firstEssence, null, secondEssence,
                null, null, null
            });

            var result = BrewLogic.GetEssencesForRow(essenceArray, 0, 2, 1);

            result.Length.Should().Be(3);
            result.Should().ContainInOrder(firstEssence, null, secondEssence);
        }

        [Fact]
        public void GetColumnRange_Always_ReturnsTuplesForCellsInRange()
        {
            (int columnIndex, int rowIndex)[] expected = { (1, 1), (2, 1), (3, 1) };

            var result = BrewLogic.GetColumnRange(1, 3, 1);

            result.Length.Should().Be(3);
            result.Should().ContainInOrder(expected);
        }

        [Fact]
        public void GetRowRange_Always_ReturnsTuplesForCellsInRange()
        {
            (int columnIndex, int rowIndex)[] expected = { (1, 1), (1, 2), (1, 3) };

            var result = BrewLogic.GetRowRange(1, 1, 3);

            result.Length.Should().Be(3);
            result.Should().ContainInOrder(expected);
        }

        [Fact]
        public void PerformMatchReplacement_Always_FillsCenterWithMatchAndRestWithNulls()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Attack);
            var essenceMatch = GetEssenceMatch(secondEssence, firstEssence);
            var essences = new[]
            {
                firstEssence, null, null,
                null, firstEssence, null,
                null, null, firstEssence
            };
            var essenceArray = new EssenceArray(3, essences);
            (int columnIndex, int rowIndex)[] coordinates = { (0, 0), (1, 1), (2, 2) };

            BrewLogic.PerformMatchReplacement(essenceArray, coordinates, essenceMatch);

            essences.Should().ContainInOrder(new []
            {
                null, null, null,
                null, secondEssence, null,
                null, null, null
            });
        }

        [Fact]
        public void PerformMatchReplacement_Always_ReturnsValidMatchResult()
        {
            var firstEssence = GetEssence(1, EssenceType.Attack);
            var secondEssence = GetEssence(2, EssenceType.Attack);
            var essenceMatch = GetEssenceMatch(secondEssence, firstEssence);
            var essences = new[]
            {
                firstEssence, null, null,
                null, firstEssence, null,
                null, null, secondEssence
            };
            var essenceArray = new EssenceArray(3, essences);
            (int columnIndex, int rowIndex)[] coordinates = { (0, 0), (1, 1), (2, 2) };

            var matchResult = BrewLogic.PerformMatchReplacement(essenceArray, coordinates, essenceMatch);

            matchResult.Essences.Length.Should().Be(2);
            matchResult.Essences.Should().Contain(new[] { firstEssence, secondEssence });
            matchResult.MatchType.Should().BeEquivalentTo(essenceMatch);
            matchResult.ColumnStartIndex.Should().Be(0);
            matchResult.ColumnEndIndex.Should().Be(2);
            matchResult.RowStartIndex.Should().Be(0);
            matchResult.RowEndIndex.Should().Be(2);
            matchResult.MatchCenterColumnIndex.Should().Be(1);
            matchResult.MatchCenterRowIndex.Should().Be(1);
        }

        #endregion

        #region Helpers

        public static IEnumerable<object[]> StirOffsetCombinations => GetOffsetParameterPair(3, 3);

        private static IEnumerable<object[]> GetOffsetParameterPair(int rows, int columns)
        {
            for (var rowOffset = 0; rowOffset < rows; rowOffset++)
            {
                for (var columnOffset = 0; columnOffset < columns; columnOffset++)
                {
                    yield return new object[] { columnOffset, rowOffset };
                }
            }
        }

        private IEssence GetEssence(int value, EssenceType essenceType)
        {
            var fakeEssence = A.Fake<IEssence>();

            A.CallTo(() => fakeEssence.Value).Returns(value);
            A.CallTo(() => fakeEssence.Type).Returns(essenceType);
            A.CallTo(() => fakeEssence.ToString()).Returns(fakeEssence.GetEssenceString());

            return fakeEssence;
        }

        private IEssenceMatch GetEssenceMatch(IEssence matchResult, IEssence primaryEssence, IEssence secondaryEssence = null)
        {
            var fakeEssenceMatch = A.Fake<IEssenceMatch>();

            A.CallTo(() => fakeEssenceMatch.PrimaryMatch).Returns(primaryEssence);
            A.CallTo(() => fakeEssenceMatch.SecondaryMatch).Returns(secondaryEssence);
            A.CallTo(() => fakeEssenceMatch.MatchResult).Returns(matchResult);

            return fakeEssenceMatch;
        }

        #endregion
    }
}
