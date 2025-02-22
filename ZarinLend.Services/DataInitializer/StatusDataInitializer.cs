using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class StatusDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<Status> statusRepository;

        public StatusDataInitializer(IBaseRepository<Status> _statusepository)
        {
            this.statusRepository = _statusepository;
        }

        public int Order => 4;
        public void InitializeData()
        {
            if (!statusRepository.TableNoTracking.Any(p => p.Id == (short)StatusEnum.ReturnToCorrection))
            {
                statusRepository.Add(new Status
                {
                    Id = (short)StatusEnum.ReturnToCorrection,
                    Name = "ReturnToCorrection",
                    Description = "برگشت جهت اصلاح"
                });
            }
            if (!statusRepository.TableNoTracking.Any(p => p.Id == (short)StatusEnum.Approved))
            {
                statusRepository.Add(new Status
                {
                    Id = (short)StatusEnum.Approved,
                    Name = "Approved",
                    Description = "تایید شده",
                });
            }
            if (!statusRepository.TableNoTracking.Any(p => p.Id == (short)StatusEnum.Rejected))
            {
                statusRepository.Add(new Status
                {
                    Id = (short)StatusEnum.Rejected,
                    Name = "Rejected",
                    Description = "رد شده"
                });
            }
        }
    }
}