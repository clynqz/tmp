using System.Text.RegularExpressions;

namespace Anomaly.Helpers
{
    public static partial class RegexHelper
    {
        public const string NicknameValidationRegex = "^[a-zA-Z0-9_]*$";

        public const string EmailValidationRegex = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";

        public static bool IsNickname(string input)
        {
            return NicknameRegex().IsMatch(input);
        }

        public static bool IsEmail(string input)
        {
            return EmailRegex().IsMatch(input);
        }

        [GeneratedRegex(NicknameValidationRegex)]
        private static partial Regex NicknameRegex();

        [GeneratedRegex(EmailValidationRegex)]
        private static partial Regex EmailRegex();
    }
}
