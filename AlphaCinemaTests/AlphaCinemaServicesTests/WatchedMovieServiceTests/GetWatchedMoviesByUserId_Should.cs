﻿using AlphaCinemaData.Context;
using AlphaCinemaData.Models;
using AlphaCinemaData.Models.Associative;
using AlphaCinemaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlphaCinemaTests.AlphaCinemaServicesTests.WatchedMovieServiceTests
{
	[TestClass]
	public class GetWatchedMoviesByUserId_Should
	{
		private DbContextOptions<AlphaCinemaContext> contextOptions;

		[TestMethod]
		public async Task ReturnWatchedMoviesWhenUserIsValid()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<AlphaCinemaContext>()
			.UseInMemoryDatabase(databaseName: "ReturnWatchedMoviesWhenUserIsValid")
				.Options;

			var userId = "Mitio";

			var watchedMovies = new List<WatchedMovie>()
			{
				new WatchedMovie()
				{ UserId = userId,
					Projection = new Projection()
					{
					Movie = new Movie() {Name = "movie Name"},
					City = new City() {Name = "city name"},
					OpenHour = new OpenHour {Hours = 12}
					}
				},
				new WatchedMovie()
				{ UserId = userId,
					Projection = new Projection()
					{
					Movie = new Movie() {Name = "movie Name"},
					City = new City() {Name = "city name"},
					OpenHour = new OpenHour {Hours = 12}
					}
				},
				new WatchedMovie()
				{ UserId = userId,
					Projection = new Projection()
					{
					Movie = new Movie() {Name = "movie Name"},
					City = new City() {Name = "city name"},
					OpenHour = new OpenHour {Hours = 12}
					}
				},
			};

			using (var actContext = new AlphaCinemaContext(contextOptions))
			{
				foreach (var watchedMovie in watchedMovies)
				{
					await actContext.WatchedMovies.AddAsync(watchedMovie);
				}
				await actContext.SaveChangesAsync();
			}
			using (var assertContext = new AlphaCinemaContext(contextOptions))
			{
				var watchedMoviesService = new WatchedMoviesService(assertContext);
				var result = await watchedMoviesService.GetWatchedMoviesByUserId(userId);
				Assert.AreEqual(watchedMovies.Count, result.Count);
			}
		}
	}
}
