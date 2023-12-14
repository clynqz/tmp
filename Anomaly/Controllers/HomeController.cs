using Anomaly.Data.Repositories;
using Anomaly.Models.Home;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Anomaly.Controllers
{
    public class HomeController(UsersRepository usersRepository) : Controller
    {
        private readonly UsersRepository _usersRepository = usersRepository;

        public async Task<IActionResult> Index()
        {
            IndexViewModel model;
            var r = Request;
            if (User.Identity!.IsAuthenticated)
            {
                var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (idClaim is null || !int.TryParse(idClaim, out var id))
                {
                    model = new IndexViewModel
                    {
                        User = null,
                    };

                    return View(model);
                }

                var user = await _usersRepository.GetUserByIdAsync(id);

                if (user is null)
                {
                    model = new IndexViewModel
                    {
                        User = null,
                    };

                    return View(model);
                }

                model = new IndexViewModel
                {
                    User = user,
                };

                return View(model);
            }

            model = new IndexViewModel
            {
                User = null,
            };

            return View(model);
        }

        public FileResult Download()
        {
            return File("~\\files\\launcher.txt", "text/plain");
        }
    }
}
