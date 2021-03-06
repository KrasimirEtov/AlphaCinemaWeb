﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AlphaCinemaData.Models;

namespace AlphaCinemaServices.Contracts
{
    public interface ICityService
    {
		Task<ICollection<City>> GetCities();

        Task<City> GetCity(string cityName);

        Task<City> GetCity(int cityId);

        Task<City> AddCity(string cityName);

        Task DeleteCity(string cityName);

        Task<string> GetCityName(int cityId);

        Task UpdateName(int cityId, string newName);

	}
}