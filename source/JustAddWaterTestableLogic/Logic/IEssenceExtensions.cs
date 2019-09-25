namespace JustAddWater.Logic
{
    public static class IEssenceExtensions
    {
        public static string GetEssenceString(this IEssence essence)
        {
            return $"{GetEnumString(essence.Type)}:{GetValueString(essence.Value)}";
        }

        private static string GetEnumString(EssenceType type)
        {
            switch (type)
            {
                case EssenceType.Health:
                    return "HP";
                case EssenceType.Attack:
                    return "AK";
                case EssenceType.Defense:
                    return "DF";
                case EssenceType.Magic:
                    return "MG";
                case EssenceType.Resistance:
                    return "RS";
                case EssenceType.Wild:
                    return "WD";
                default:
                    return "??";
            }
        }

        private static string GetValueString(int value)
        {
            return value >= 0 ? $"+{value}" : value.ToString();
        }

        public static string GetEssenceDebugString(this IEssence essence)
        {
            return essence == null ? $"[  :  ]" : $"[{essence.GetEssenceString()}]";
        }
    }
}
