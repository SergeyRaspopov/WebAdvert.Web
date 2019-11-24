using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private SignInManager<CognitoUser> _signInManager;
        private UserManager<CognitoUser> _userManager;
        private CognitoUserPool _cognitoUserPool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool cognitoUserPool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _cognitoUserPool = cognitoUserPool;
        }

        [HttpGet]
        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _cognitoUserPool.GetUser(model.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Password);
                if (createdUser.Succeeded)
                    return RedirectToAction(nameof(Confirm), new { model.Email }) ;
                else
                    createdUser.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(string email)
        {
            var model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null) {
                    ModelState.AddModelError("NotFound", "User is not found");
                }

                var cognitoUserManager = _userManager as CognitoUserManager<CognitoUser>;
                var result = await cognitoUserManager.ConfirmSignUpAsync(user, model.Code, true);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
                else
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
            }
            return View(model);
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
                else
                    ModelState.AddModelError("LoginFailed", "Could not login");
            }   
                
            return View(model);
        }

        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var cognitoUserManager = _userManager as CognitoUserManager<CognitoUser>;
                var cognitoSignInManager = _signInManager as CognitoSignInManager<CognitoUser>;
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await cognitoUserManager.ResetPasswordAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("ResetPassword", new { model.Email });
                    else
                        result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ResetPassword(string email)
        {
            var model = new ResetPasswordModel() { Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    if (result.Succeeded)
                        return RedirectToAction("Login");
                    else
                        result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                }
                else
                    ModelState.AddModelError("NotFound", "User not found");
            }

            return View(model);
        }
    }
}
