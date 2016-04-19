using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GreenHouse.Helpers;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;
using GreenHouse.ViewModels;

namespace GreenHouse.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository<User> userRepository;

        public AccountController(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }
        //
        // GET: /Account/

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginUserViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var passwordHash = HashHelper.GetMd5Hash(model.Password);
            var user = (await userRepository.GetAsync(x=>x.Login == model.Login && x.PasswordHash == passwordHash)).FirstOrDefault();
            if(user==null)
            {
                await Task.Delay(1000);
                ModelState.AddModelError("Password", "Wrong login or password");
                return View(model);
            }

            FormsAuthentication.SetAuthCookie(model.Login, true);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }           

        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account");
        }

    }
}
