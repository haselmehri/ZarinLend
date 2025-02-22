using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class SalaryRangeDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<SalaryRange> salaryRangeRepository;

        public SalaryRangeDataInitializer(IBaseRepository<SalaryRange> salaryRangeRepository)
        {
            this.salaryRangeRepository = salaryRangeRepository;
        }

        public int Order => 1;
        public void InitializeData()
        {
            if (!salaryRangeRepository.TableNoTracking.Any(p => p.Id == 1))
            {
                salaryRangeRepository.Add(new SalaryRange
                {
                    Id = 1,
                    Title = "کمتر از 5 میلیون تومان"                    
                });
            }
            if (!salaryRangeRepository.TableNoTracking.Any(p => p.Id == 2))
            {
                salaryRangeRepository.Add(new SalaryRange
                {
                    Id = 2,
                    Title = "پنج تا ده میلیون تومان"
                });
            }
            if (!salaryRangeRepository.TableNoTracking.Any(p => p.Id == 3))
            {
                salaryRangeRepository.Add(new SalaryRange
                {
                    Id = 3,
                    Title = "ده تا پانزده میلیون تومان"
                });
            }
            if (!salaryRangeRepository.TableNoTracking.Any(p => p.Id == 4))
            {
                salaryRangeRepository.Add(new SalaryRange
                {
                    Id = 4,
                    Title = "پانزده تا بیست میلیون تومان"
                });
            }
            if (!salaryRangeRepository.TableNoTracking.Any(p => p.Id == 5))
            {
                salaryRangeRepository.Add(new SalaryRange
                {
                    Id = 5,
                    Title = "بیست تا بیت و پنج میلیون تومان"
                });
            }
            if (!salaryRangeRepository.TableNoTracking.Any(p => p.Id == 6))
            {
                salaryRangeRepository.Add(new SalaryRange
                {
                    Id = 6,
                    Title = "بیست و پنچ تا سی میلیون تومان"
                });
            }
            if (!salaryRangeRepository.TableNoTracking.Any(p => p.Id == 7))
            {
                salaryRangeRepository.Add(new SalaryRange
                {
                    Id = 7,
                    Title = "سی تا چهل میلیون تومان"
                });
            }
            if (!salaryRangeRepository.TableNoTracking.Any(p => p.Id == 8))
            {
                salaryRangeRepository.Add(new SalaryRange
                {
                    Id = 8,
                    Title = "چهل تا پنجاه میلیون تومان"
                });
            }
            if (!salaryRangeRepository.TableNoTracking.Any(p => p.Id == 9))
            {
                salaryRangeRepository.Add(new SalaryRange
                {
                    Id = 9,
                    Title = "بیشتر از پنجاه میلیون تومان"
                });
            }
        }
    }
}