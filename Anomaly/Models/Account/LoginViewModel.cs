namespace Anomaly.Models.Account
{
    public class LoginViewModel
    {
        public bool IsFormDataCorrect { get; set; }

        public required string PreviousLogin { get; set; }

        public required string PreviousPassword { get; set; }
    }
}
