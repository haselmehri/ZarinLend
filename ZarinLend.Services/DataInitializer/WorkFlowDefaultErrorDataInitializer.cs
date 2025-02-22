using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class WorkFlowDefaultErrorDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<WorkFlowDefaultError> workFlowDefaultErrorRepository;

        public WorkFlowDefaultErrorDataInitializer(IBaseRepository<WorkFlowDefaultError> workFlowDefaultErrorRepository)
        {
            this.workFlowDefaultErrorRepository = workFlowDefaultErrorRepository;
        }

        public int Order => 2;
        public void InitializeData()
        {
            if (!workFlowDefaultErrorRepository.TableNoTracking.Any(p => p.Id == 1))
            {
                workFlowDefaultErrorRepository.Add(new WorkFlowDefaultError
                {
                    Id = 1,
                    Name = "NationlCodeWrong",
                    ErrorField = "NationalCode",
                    Message = "کد ملی وارد شده نامعتبر است،یا با کد درج شده در تصویر کارت ملی مطابقت ندارد"
                });
            }
            if (!workFlowDefaultErrorRepository.TableNoTracking.Any(p => p.Id == 2))
            {
                workFlowDefaultErrorRepository.Add(new WorkFlowDefaultError
                {
                    Id = 2,
                    Name = "ConflictNationlCardInfoToRegisterIdenticalInfo",
                    ErrorField = "NationalCode,FName,LName",
                    Message = "اطلاعات درج شده در کارت ملی با اطلاعات هویتی وارد شده توسط شما مغایرت دارد"
                });
            }
            if (!workFlowDefaultErrorRepository.TableNoTracking.Any(p => p.Id == 3))
            {
                workFlowDefaultErrorRepository.Add(new WorkFlowDefaultError
                {
                    Id = 3,
                    Name = "ConflictBirthCertificateInfoToRegisterIdenticalInfo",
                    ErrorField = "FName,LName,BirthDate",
                    Message = "اطلاعات درج شده در شناسنامه با اطلاعات هویتی وارد شده توسط شما مغایرت دارد"
                });
            }
            if (!workFlowDefaultErrorRepository.TableNoTracking.Any(p => p.Id == 4))
            {
                workFlowDefaultErrorRepository.Add(new WorkFlowDefaultError
                {
                    Id = 4,
                    Name = "TheNationalCardImageIsIllegible",
                    Message = "عکس بارگذاری شده برای کارت ملی مبهم است،عکس جدید بارگذاری کنید"
                });
            }
            if (!workFlowDefaultErrorRepository.TableNoTracking.Any(p => p.Id == 5))
            {
                workFlowDefaultErrorRepository.Add(new WorkFlowDefaultError
                {
                    Id = 5,
                    Name = "TheBirthCertificateIsIllegible",
                    Message = "عکس بارگذاری شده برای شناسنامه مبهم است،عکس جدید بارگذاری کنید"
                });
            }
        }
    }
}