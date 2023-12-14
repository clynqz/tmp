using Anomaly.Helpers;
using System.ComponentModel.DataAnnotations;
using static Anomaly.Data.Constraints.UserEntityConstraints;

namespace Anomaly.Data.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }

        [MinLength(NICKNAME_MIN_LENGTH)]
        [MaxLength(NICKNAME_MAX_LENGTH)]
        [RegularExpression(RegexHelper.NicknameValidationRegex)]
        public string? Nickname { get; set; }

        [RegularExpression(RegexHelper.EmailValidationRegex)]
        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}
