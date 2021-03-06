﻿using AlphaCinemaData.Context;
using AlphaCinemaData.Models;
using AlphaCinemaData.Models.Associative;
using AlphaCinemaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlphaCinemaTests.AlphaCinemaServicesTests.ProjectionServiceTests
{
    [TestClass]
    public class GetTopProjections_Should
    {
        // Create a fresh service provider, and therefore a fresh InMemory database instance.
        private ServiceProvider serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
        private Projection testProjectionOne = new Projection() { MovieId = 1, OpenHourId = 1 };
        private Projection testProjectionTwo = new Projection() { MovieId = 2, OpenHourId = 1 };
        private Projection testProjectionThree = new Projection() { MovieId = 3, CityId = 1, OpenHourId = 1 };
        private Movie testMovieOne = new Movie() { Name = "TestMovieOne" };
        private Movie testMovieTwo = new Movie() { Name = "TestMovieTwo" };
        private Movie testMovieThree = new Movie() { Name = "TestMovieThree" };
        private City testCity = new City() { Name = "TestCityName" };
        private OpenHour testOpenHour = new OpenHour() { Minutes = 100, Hours = 2 };

        private int projectionCount = 1;

        [TestMethod]
        public async Task ReturnCollectionOfProjections_WhenCountIsValid()
        {
            //Arrange
            // Create a new options instance telling the context to use an InMemory database and the new service provider.
            var contextOptions = new DbContextOptionsBuilder<AlphaCinemaContext>()
                .UseInMemoryDatabase(databaseName: "ReturnCollectionOfProjections_WhenCountIsValid")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            var listOfProjections = new List<Projection>() { testProjectionOne, testProjectionTwo, testProjectionThree };
            var listOfMovies = new List<Movie>() { testMovieOne, testMovieTwo, testMovieThree };

            //Act and Assert
            using (var actContext = new AlphaCinemaContext(contextOptions))
            {
                await actContext.AddRangeAsync(listOfProjections);
                await actContext.AddRangeAsync(listOfMovies);
                await actContext.AddAsync(testOpenHour);
                await actContext.AddAsync(testCity);
                await actContext.SaveChangesAsync();
            }

            //Assert
            using (var assertContext = new AlphaCinemaContext(contextOptions))
            {
                var command = new ProjectionService(assertContext);
                var result = await command.GetTopProjections(projectionCount);
                Assert.AreEqual(testProjectionOne.Id, result.First().Id);
                //We are returning the FirstProjection and we want the top 1
            }
        }
    }
}
