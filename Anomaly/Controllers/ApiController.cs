using Anomaly.Data.Entities;
using Anomaly.Data.Repositories;
using Anomaly.Models.Api.GameFile;
using Anomaly.Models.Api.GameFiles;
using Anomaly.Models.Auth;
using Anomaly.Services.FileHashers;
using Anomaly.Services.PasswordHasher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Anomaly.Controllers
{
    [Authorize]
    [Route("api")]
    public class ApiController(UsersRepository usersRepository) : Controller
    {
        private static readonly string GameFilesDirectory = $"{Environment.CurrentDirectory}\\wwwroot\\files\\game\\";

        private readonly UsersRepository _usersRepository = usersRepository;

        private static readonly Dictionary<string, string> _gameFiles = [];

        public static void CalculateGameFilesHashes(IFileHasher fileHasher)
        {
            var gameFilesDirectoryInfo = new DirectoryInfo(GameFilesDirectory);

            if (!gameFilesDirectoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(GameFilesDirectory);
            }

            var gameFilesDirectoryUri = new Uri(GameFilesDirectory);

            foreach (var fileInfo in gameFilesDirectoryInfo.GetFiles("*.*", searchOption: SearchOption.AllDirectories))
            {
                var fileFullnameUri = new Uri(fileInfo.FullName);

                var relativePath = Uri.UnescapeDataString(gameFilesDirectoryUri.MakeRelativeUri(fileFullnameUri).ToString().Replace('/', Path.DirectorySeparatorChar));

                var hash = GetFileHash(fileInfo, fileHasher);

                _gameFiles.Add(relativePath, hash);
            }
        }

        public static string GetAuthToken(UserEntity user)
        {
            var userId = user.Id.ToString();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
            };

            var jwt = new JwtSecurityToken(
                    claims: claims,
                    issuer: AuthOptions.AuthTokenIssuer,
                    audience: AuthOptions.AuthTokenAudience,
                    expires: DateTime.UtcNow + AuthOptions.AuthTokenLifetime,
                    signingCredentials: new SigningCredentials(AuthOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

        [HttpGet("validate")]
        public ActionResult Validate()
        {
            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> LoginAsync([FromBody] AuthRequest request, [FromServices] IPasswordHasher passwordHasher)
        {
            var user = await _usersRepository.GetUserByLoginAsync(request.Login);

            if (user is null)
            {
                return BadRequest(new { Field = "login" });
            }

            var verified = passwordHasher.Verify(request.Password, user.Password!);

            if (!verified)
            {
                return BadRequest(new { Field = "password" });
            }

            var token = GetAuthToken(user);

            var response = new AuthResponse
            {
                Token = token,
            };

            return Ok(response);
        }

        [HttpGet("game-files")]
        public ActionResult<GameFilesListResponse> GameFiles()
        {
            return Ok(new GameFilesListResponse { GameFiles = _gameFiles });
        }

        [HttpPost("game-file")]
        public ActionResult<GameFileResponse> GameFile([FromBody] GameFileRequest request)
        {
            var filePath = Path.Combine(GameFilesDirectory, request.FilePath);

            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                return BadRequest(request.FilePath);
            }

            var fileBytes = System.IO.File.ReadAllBytes(fileInfo.FullName);
            
            return Ok(new GameFileResponse { Content = fileBytes });
        }

        private static string GetFileHash(FileInfo fileInfo, IFileHasher fileHasher)
        {
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(fileInfo.FullName);
            }

            using var stream = new BufferedStream(fileInfo.OpenRead(), 1024 * 1024 * 16);

            var hash = fileHasher.Hash(stream);

            return hash;
        }
    }
}
