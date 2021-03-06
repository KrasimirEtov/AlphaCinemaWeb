﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AlphaCinema.Models;
using AlphaCinema.Models.AccountViewModels;
using AlphaCinemaData.Models;
using System;

namespace AlphaCinema.Controllers
{
	[Authorize]
	[Route("[controller]/[action]")]
	public class AccountController : Controller
	{
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;

		public AccountController(
			UserManager<User> userManager,
			SignInManager<User> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		[TempData]
		public string ErrorMessage { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl = null)
		{

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if (ModelState.IsValid)
			{
				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
				var result = await this.signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					return RedirectToLocal(returnUrl);
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View(model);
				}
			}
			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if (ModelState.IsValid)
			{
				// TODO - CHANGE WITH Normal User
				var user = new User
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					Age = model.Age,
					// Maybe initialize watched movies collection?
					UserName = model.Email,
					Email = model.Email,
					CreatedOn = DateTime.UtcNow
			};

				var result = await this.userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await this.signInManager.SignInAsync(user, isPersistent: false);

					return RedirectToLocal(returnUrl);
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout(string returnUrl = null)
		{
			await this.signInManager.SignOutAsync();
            ViewData["ReturnUrl"] = returnUrl;
            return RedirectToLocal(returnUrl);
        }

		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				this.ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (this.Url.IsLocalUrl(returnUrl))
			{
				return this.Redirect(returnUrl);
			}
			else
			{
				return this.RedirectToAction(nameof(HomeController.Index), "Home");
			}
		}
	}
}
