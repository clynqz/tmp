using Anomaly.Data.Entities;
using Anomaly.Data.Repositories;
using Anomaly.Helpers;
using Anomaly.Middlewares;
using Anomaly.Models.Account;
using Anomaly.Services.PasswordHasher;
using CorsairMessengerServer.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static Anomaly.Data.Constraints.UserEntityConstraints;

namespace Anomaly.Controllers
{
    public class AccountController(UsersRepository usersRepository, IPasswordHasher passwordHasher) : Controller
    {
        private readonly UsersRepository _usersRepository = usersRepository;

        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public new IActionResult SignOut()
        {
            Response.Cookies.Delete(RequestHeadersComplementaryMiddleware.AuthorizationTokenCookieName);

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }

            LoginViewModel model;

            if (!Request.HasFormContentType)
            {
                model = new LoginViewModel
                {
                    IsFormDataCorrect = true,
                    PreviousLogin = string.Empty,
                    PreviousPassword = string.Empty,
                };

                return View(model);
            }

            var form = Request.Form;

            var login = form["login"].FirstOrDefault();
            var password = form["password"].FirstOrDefault();

            if (!IsLoginFormValid(login, password))
            {
                model = new LoginViewModel
                {
                    IsFormDataCorrect = false,
                    PreviousLogin = login!,
                    PreviousPassword = password!,
                };

                return View(model);
            }

            var user = await _usersRepository.GetUserByLoginAsync(login!);

            if (user is null)
            {
                model = new LoginViewModel
                {
                    IsFormDataCorrect = false,
                    PreviousLogin = login!,
                    PreviousPassword = password!,
                };

                return View(model);
            }
            else
            {
                var verified = _passwordHasher.Verify(password!, user!.Password!);

                if (!verified)
                {
                    model = new LoginViewModel
                    {
                        IsFormDataCorrect = false,
                        PreviousLogin = login!,
                        PreviousPassword = password!,
                    };

                    return View(model);
                }
            }

            var token = ApiController.GetAuthToken(user);

            AddAuthorizationCookie(token);

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> Register()
        {
            if (User.Identity is not null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }

            RegisterViewModel model;

            if (!Request.HasFormContentType)
            {
                model = new RegisterViewModel
                {
                    IsEmailExist = false,
                    IsNicknameTaken = false,
                    PreviousEmail = string.Empty,
                    PreviousLogin = string.Empty,
                    PreviousPassword = string.Empty,
                };

                return View(model);
            }

            var form = Request.Form;

            var email = form["email"].FirstOrDefault();
            var login = form["login"].FirstOrDefault();
            var password = form["password"].FirstOrDefault();

            if (!IsRegisterFormValid(email, login, password))
            {
                model = new RegisterViewModel
                {
                    IsEmailExist = false,
                    IsNicknameTaken = false,
                    PreviousEmail = string.Empty,
                    PreviousLogin = string.Empty,
                    PreviousPassword = string.Empty,
                };

                return View(model);
            }

            var isEmailExist = await _usersRepository.IsEmailExistAsync(email!);

            if (isEmailExist)
            {
                model = new RegisterViewModel
                {
                    IsEmailExist = true,
                    IsNicknameTaken = false,
                    PreviousEmail = email!,
                    PreviousLogin = login!,
                    PreviousPassword = password!,
                };

                return View(model);
            }

            var isNicknameExist = await _usersRepository.IsNicknameExistAsync(login!);

            if (isNicknameExist)
            {
                model = new RegisterViewModel
                {
                    IsEmailExist = false,
                    IsNicknameTaken = true,
                    PreviousEmail = email!,
                    PreviousLogin = login!,
                    PreviousPassword = password!,
                };

                return View(model);
            }

            var user = await CreateUserAsync(login!, email!, password!, _passwordHasher);

            var token = ApiController.GetAuthToken(user);

            AddAuthorizationCookie(token);

            return RedirectToAction("index", "home");
        }

        private async Task<UserEntity> CreateUserAsync(string nickname, string email, string password, IPasswordHasher passwordHasher)
        {
            var hashedPassword = passwordHasher.Hash(password);

            var user = new UserEntity { Nickname = nickname, Email = email, Password = hashedPassword };

            await _usersRepository.AddUserAsync(user);

            return user;
        }

        private static bool IsRegisterFormValid(string? email, string? login, string? password)
        {
            return !email.IsNullOrEmpty() && !login.IsNullOrEmpty() && !password.IsNullOrEmpty()
                && login!.Length.InRange(NICKNAME_MIN_LENGTH, NICKNAME_MAX_LENGTH) && password!.Length.InRange(PASSWORD_MIN_LENGTH, PASSWORD_MAX_LENGTH)
                && RegexHelper.IsEmail(email!) && RegexHelper.IsNickname(login!);
        }

        private static bool IsLoginFormValid(string? login, string? password)
        {
            return !login.IsNullOrEmpty() && !password.IsNullOrEmpty()
                && login!.Length.InRange(NICKNAME_MIN_LENGTH, NICKNAME_MAX_LENGTH) && password!.Length.InRange(PASSWORD_MIN_LENGTH, PASSWORD_MAX_LENGTH)
                && (RegexHelper.IsEmail(login!) || RegexHelper.IsNickname(login!));
        }

        private void AddAuthorizationCookie(string token)
        {
            HttpContext.Response.Cookies.Append(RequestHeadersComplementaryMiddleware.AuthorizationTokenCookieName, token, new CookieOptions
            {
                MaxAge = AuthOptions.AuthTokenLifetime,
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            });
        }
    }
}