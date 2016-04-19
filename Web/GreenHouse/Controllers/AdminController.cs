using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GreenHouse.Helpers;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;
using GreenHouse.ViewModels;

namespace GreenHouse.Controllers
{
    [Authorize]    
    public class AdminController : Controller
    {
        private readonly IRepository<User> userRepository;        

        public AdminController(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        public ActionResult Index()
        {            
            return View();
        }

        public async Task<ActionResult> GetUsers()
        {
            var users = await userRepository.GetAllAsync();

            var model = users.Select(x => new UserViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Login = x.Login,
                Email = x.Email,
                RegistrationDate = x.RegistrationDate,
                IsAdmin = x.IsAdmin,
                Phone = x.Phone
            });
            return View(model);
        }

        public ActionResult AddUser()
        {           
            return View("EditUser",new UserViewModel());
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {           
            var user = userRepository.GetAll().FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                Response.StatusCode = 400;
                return Content("User not found");
            }

            var model = new UserViewModel
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Login = user.Login,
                            Phone = user.Phone,
                            Email = user.Email
                        };
            return View(model);

        }

        [HttpPost]
        public async Task<ActionResult> AddUser(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.Message = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).Aggregate((x, y) => string.Format("{0}; {1}", x, y));
                return View("EditUser",model);
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password","Password is required.");
                return View("EditUser",model);
            }
            if (await IsUserExist(model.Login))
            {
                ModelState.AddModelError("Login", "This login is used");
                return View("EditUser", model);
            }
            var user = new User
                {
                    FirstName = model.FirstName, 
                    LastName = model.FirstName,
                    Login = model.Login,
                    Email = model.Email,
                    Phone = model.Phone,
                    RegistrationDate = DateTime.Now,
                    PasswordHash = HashHelper.GetMd5Hash(model.Password)
                };
                
                userRepository.Create(user);
                ViewBag.Message = "User was successful created.";
                model.Id = user.Id;
                return View("EditUser",model);
            
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(UserViewModel model)
        {
            
            if(model == null || model.Id<=0)
            {
                Response.StatusCode = 400;
                return Content("Bad request");
            }

            var user = userRepository.GetAll().FirstOrDefault(x => x.Id == model.Id);
            if (user == null)
            {
                Response.StatusCode = 400;
                return Content("User not found");
            }

            var userWithThisLogin = await userRepository.GetAsync(x => x.Login == model.Login && x.Id != user.Id);
            if (userWithThisLogin.Any())
            {
                ModelState.AddModelError("Login", "This login is used");
                return View("EditUser", model);
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = HashHelper.GetMd5Hash(model.Password);
            }


            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Login = model.Login;
            user.Phone = model.Phone;
            user.Email = model.Email;

            userRepository.Update(user);
            ViewBag.Message = "User was successful updated.";

            return View(model);
        }

        private async Task<bool> IsUserExist(string login)
        {
            var users = await userRepository.GetAsync(x=>x.Login == login);
            return users.Any();
        }

        
    }
}
