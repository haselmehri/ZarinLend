using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class BankDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<Bank> bankRepository;

        public BankDataInitializer(IBaseRepository<Bank> bankepository)
        {
            this.bankRepository = bankepository;
        }

        public int Order => 1;
        public void InitializeData()
        {
            if (!bankRepository.TableNoTracking.Any(p => p.Id == 1))
            {
                bankRepository.Add(new Bank
                {
                    Id = 1,
                    Name = "بانک آینده"                    
                });
            }
            if (!bankRepository.TableNoTracking.Any(p => p.Id == 2))
            {
                bankRepository.Add(new Bank
                {
                    Id = 2,
                    Name = "بانک تجارت"
                });
            }
        }
    }
}