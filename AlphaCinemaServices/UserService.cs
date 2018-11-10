﻿using AlphaCinemaData.Context;
using AlphaCinemaData.Models;
using AlphaCinemaServices.Contracts;
using AlphaCinemaWeb.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaCinemaServices
{
	public class UserService : IUserService
	{
		private readonly IServiceProvider serviceProvider;
		private readonly AlphaCinemaContext context;

		public UserService(IServiceProvider serviceProvider, AlphaCinemaContext context)
		{
			this.serviceProvider = serviceProvider;
			this.context = context;
		}

		public async Task<ICollection<User>> GetAllUsers()
		{
			var users = await this.context.Users
				.ToListAsync();
			return users;
		}

		public async Task<User> GetUser(string userId)
		{
			var user = await this.context.Users
				.Where(u => u.Id == userId)
				.FirstOrDefaultAsync();
			return user;
		}

		public async Task SetRole(string userId, string roleName)
		{
			var user = await GetUser(userId);
			// check if user is alredady added in role
			await this.serviceProvider.GetRequiredService<UserManager<User>>().AddToRoleAsync(user, roleName);
		}

		public async Task RemoveRole(string userId, string roleName)
		{
			var user = await GetUser(userId);
			await this.serviceProvider.GetRequiredService<UserManager<User>>().RemoveFromRoleAsync(user, roleName);
		}

		public async Task<bool> IsUserAdmin(string userId, string roleName)
		{
			var result = await this.serviceProvider.GetRequiredService<UserManager<User>>()
				.IsInRoleAsync(GetUser(userId).Result, roleName);
			return result;
		}

		public async Task Modify(string userId)
		{
			var user = await GetUser(userId);

			if (user == null || user.IsDeleted)
			{
				throw new EntityDoesntExistException($"\nUser is not present in the database.");
			}
			user.ModifiedOn = DateTime.Now;

			this.context.Users.Update(user);
			await this.context.SaveChangesAsync();
		}
	}
}
