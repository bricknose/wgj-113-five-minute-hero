namespace JustAddWater.Logic
{
    public static class HelperExtensions
    {
        public static int[] To(this int start, int end)
        {
            var returnRange = new int[end - start + 1];
            for (var i = 0; i < returnRange.Length; i++)
            {
                returnRange[i] = start + i;
            }

            return returnRange;
        }
    }
}
