using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class CountryDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<Country> countryRepository;

        public CountryDataInitializer(IBaseRepository<Country> _countryRepository)
        {
            this.countryRepository = _countryRepository;
        }

        public int Order => 0;

        public void InitializeData()
        {
            if (!countryRepository.TableNoTracking.Any(p => p.Name == "ایران"))
            {
                countryRepository.Add(new Country
                {
                    Id = 1,
                    Name = "ایران",
                });
            }
        }
    }
}