namespace JustAddWater.Logic
{
    public interface IEssenceMatch
    {
        IEssence PrimaryMatch { get; set; }
        IEssence SecondaryMatch { get; set; }
        IEssence MatchResult { get; set; }
    }
}
