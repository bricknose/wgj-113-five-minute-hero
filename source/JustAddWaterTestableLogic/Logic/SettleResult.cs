namespace JustAddWater.Logic
{
    public struct SettleResult
    {
        public int OldRowIndex;
        public int NewRowIndex;
        public int ColumnIndex;
        public IEssence Essence;

        public override string ToString()
        {
            return $"{Essence.GetEssenceDebugString()}:[{OldRowIndex},{ColumnIndex}]->[{NewRowIndex},{ColumnIndex}]";
        }
    }
}
