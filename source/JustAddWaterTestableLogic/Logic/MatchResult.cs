using System.Linq;

namespace JustAddWater.Logic
{
    public struct MatchResult
    {
        public int RowStartIndex;
        public int RowEndIndex;
        public int ColumnStartIndex;
        public int ColumnEndIndex;
        public IEssence[] Essences;
        public IEssenceMatch MatchType;
        public int MatchCenterRowIndex => (RowStartIndex + RowEndIndex) / 2;
        public int MatchCenterColumnIndex => (ColumnStartIndex + ColumnEndIndex) / 2;

        public override string ToString()
        {
            return $"[{RowStartIndex},{ColumnStartIndex}]-[{RowEndIndex},{ColumnEndIndex}]:{string.Join("+", Essences.Select(e => e.ToString()))}={MatchType.MatchResult}";
        }
    }
}
