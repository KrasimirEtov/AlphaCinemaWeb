﻿using AlphaCinemaData.Context;
using AlphaCinemaData.Models;
using AlphaCinemaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaCinemaTests.AlphaCinemaServicesTests.UserServiceTests
{
	[TestClass]
	public class GetUser_Should
	{
		private DbContextOptions<AlphaCinemaContext> contextOptions;
		private Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
		private User user;

		[TestMethod]
		public async Task ReturnUserIfUserIdIsFound()
		{
			contextOptions = new DbContextOptionsBuilder<AlphaCinemaContext>()
			.UseInMemoryDatabase(databaseName: "ReturnUserIfUserIdIsFound")
				.Options;
			// Arrange
			string userId = "myId";

			user = new User()
			{
				Id = userId,
				FirstName = "Krasimir",
				LastName = "Etov",
				Age = 21,
			};

			using (var actContext = new AlphaCinemaContext(contextOptions))
			{
				await actContext.Users.AddAsync(user);
				await actContext.SaveChangesAsync();
			}

			// Act && Assert
			using (var assertContext = new AlphaCinemaContext(contextOptions))
			{
				var userService = new UserService(serviceProviderMock.Object, assertContext);

				var result = await userService.GetUser(userId);
				Assert.IsNotNull(result);
				Assert.AreEqual(result.Id, userId);
			}
		}

		[TestMethod]
		public async Task ReturnNullIfUserIdIsNotFound()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<AlphaCinemaContext>()
			.UseInMemoryDatabase(databaseName: "ReturnNullIfUserIdIsNotFound")
				.Options;
			
			// Act && Assert
			using (var assertContext = new AlphaCinemaContext(contextOptions))
			{
				var userService = new UserService(serviceProviderMock.Object, assertContext);
				var result = await userService.GetUser("no such id");

				Assert.IsNull(result);
			}
		}
	}
}
