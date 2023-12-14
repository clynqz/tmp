using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Anomaly
{
    public static class AuthOptions
    {
        public const string AuthTokenIssuer = "Anomaly";

        public const string AuthTokenAudience = "Anomaly";

        public static readonly TimeSpan AuthTokenLifetime = TimeSpan.FromDays(1);

        public static string? SecurityKey { get; private set; }

        public static SymmetricSecurityKey SymmetricSecurityKey
        {
            get
            {
                ArgumentNullException.ThrowIfNull(SecurityKey);

                return new(Encoding.UTF8.GetBytes(SecurityKey));
            }
        }

        public static void Initialize(string securityKey)
        {
            ArgumentNullException.ThrowIfNull(securityKey);

            SecurityKey = securityKey;
        }
    }
}
