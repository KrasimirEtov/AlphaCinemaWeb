﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AlphaCinema.Models;
using AlphaCinema.Models.ManageViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using AlphaCinemaData.Models;

namespace AlphaCinema.Controllers
{
	[Authorize]
	[Route("[controller]/[action]")]
	public class ManageController : Controller
	{
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;
		//private readonly IUsersService usersService;


		public ManageController(
		  UserManager<User> userManager,
		  SignInManager<User> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		[TempData]
		public string StatusMessage { get; set; }

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var user = await this.userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{this.userManager.GetUserId(User)}'.");
			}

			var model = new IndexViewModel
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Age = user.Age,
				// Watched movies collection? 
				Username = user.UserName,
				//Email = user.Email,
				ImageUrl = user.AvatarImageName,
				StatusMessage = StatusMessage
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(IndexViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await this.userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{this.userManager.GetUserId(User)}'.");
			}
			// Ако искаме да ползваме това трябва да extend-нем user-manager, за да може да добавим нашата логика - first name, last name, age и други..


			StatusMessage = "Your profile has been updated";
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> ChangePassword()
		{
			var user = await this.userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{this.userManager.GetUserId(User)}'.");
			}

			var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await this.userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{this.userManager.GetUserId(User)}'.");
			}

			var changePasswordResult = await this.userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
			if (!changePasswordResult.Succeeded)
			{
				AddErrors(changePasswordResult);
				return View(model);
			}

			await this.signInManager.SignInAsync(user, isPersistent: false);
			StatusMessage = "Your password has been changed.";

			return RedirectToAction(nameof(ChangePassword));
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Avatar(IFormFile avatarImage)
		{
			if (avatarImage == null)
			{
				this.StatusMessage = "Error: Please provide an image";
				return this.RedirectToAction(nameof(Index));
			}

			if (!this.IsValidImage(avatarImage))
			{
				this.StatusMessage = "Error: Please provide a .jpg or .png file smaller than 1MB";
				return this.RedirectToAction(nameof(Index));
			}
			// TODO : FIX
			//await this.usersService.SaveAvatarImageAsync(
			//	this.GetUploadsRoot(),
			//	avatarImage.FileName,
			//	avatarImage.OpenReadStream(),
			//	this.User.GetId()
			//);

			this.StatusMessage = "Profile image updated";

			return this.RedirectToAction(nameof(Index));
		}

		private string GetUploadsRoot()
		{
			var environment = this.HttpContext.RequestServices
				.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;

			return Path.Combine(environment.WebRootPath, "images", "avatars");
		}

		private bool IsValidImage(IFormFile image)
		{
			string type = image.ContentType;
			if (type != "image/png" && type != "image/jpg" && type != "image/jpeg")
			{
				return false;
			}

			return image.Length <= 1024 * 1024;
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				this.ModelState.AddModelError(string.Empty, error.Description);
			}
		}
	}
}
