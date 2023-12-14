using Anomaly.Data.Constraints;

namespace Anomaly.Models.Account
{
    public class RegisterViewModel
    {
        public const string EmailIsNotExistMessageText = "email";

        public const string EmailExistMessageText = "email уже существует";

        public const string NicknameTakenMessageText = "логин занят";

        public static readonly string NicknameIsNotTakenMessageText
            = $"логин (от {UserEntityConstraints.NICKNAME_MIN_LENGTH} до {UserEntityConstraints.NICKNAME_MAX_LENGTH} симв.)";

        public bool IsEmailExist { get; set; }

        public bool IsNicknameTaken { get; set; }

        public required string PreviousEmail { get; set; }

        public required string PreviousLogin { get; set; }

        public required string PreviousPassword { get; set; }
    }
}
