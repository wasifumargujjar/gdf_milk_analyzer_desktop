namespace MilkAnalyzerTest.Services
{
    public static class TokenStore
    {
        private static string? _token;
        public static string? Token
        {
            get => _token;
            set => _token = value;
        }

        public static void Clear() => _token = null;
    }
}
