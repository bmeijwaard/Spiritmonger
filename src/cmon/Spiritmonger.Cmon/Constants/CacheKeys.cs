namespace Spiritmonger.Cmon.Constants
{
    public static class CacheKeys
    {
        public const string CARDS = "CARDS";
        public static string GetCardSearchCacheKey(dynamic request)
            => $"{CARDS}_{request.Page}_{request.PageLength}_{request.NamePart}";
    }
}
