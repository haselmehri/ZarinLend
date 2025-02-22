using AutoMapper;
using Common;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.NeginHub;

namespace Services
{
    public class PersonService : IPersonService, IScopedDependency
    {
        private readonly ILogger<PersonService> logger;
        private readonly IUserRepository userRepository;
        private readonly IPersonRepository personRepository;
        private readonly IBaseRepository<PersonJobInfo> personJobInfoRepository;

        public PersonService(IMapper mapper,
                             ILogger<PersonService> logger,
                             IUserRepository userRepository,
                             IPersonRepository personRepository,
                             IBaseRepository<PersonJobInfo> personJobInfoRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.personRepository = personRepository;
            this.personJobInfoRepository = personJobInfoRepository;
        }

        public async Task<PersonZarinpalTransactionsInfoModel> GetZarinpalTransactionInfo(string hashCardNumber, CancellationToken cancellationToken)
        {
            var result = await personRepository.GetZarinpalTransactionInfo(hashCardNumber, cancellationToken);
            if (result != null && result.Rows.Count > 0)
            {
                return new PersonZarinpalTransactionsInfoModel()
                {
                    CardPan = Convert.ToString(result.Rows[0]["CardPan"]),
                    CardHash = Convert.ToString(result.Rows[0]["CardHash"]),
                    NumnerOfTerminals = Convert.ToInt32(result.Rows[0]["NumnerOfTerminals"]),
                    NumberOfTransactions = Convert.ToInt32(result.Rows[0]["NumberOfTransactions"]),
                    SumAmount = Convert.ToDouble(result.Rows[0]["SumAmount"]),
                };
            }
            return null;
        }

        public async Task<PersonModel?> GetPerson(int id, CancellationToken cancellationToken)
        {
            return (await personRepository.SelectByAsync(p => p.Id.Equals(id),
                p => new PersonModel()
                {
                    Id = p.Id,
                    FName = p.FName,
                    LName = p.LName,
                    Mobile = p.Mobile,
                    OrganizationId = p.OrganizationId,
                    OrganizationName = p.OrganizationId.HasValue ? p.Organization.Name : null
                }, cancellationToken))
                .FirstOrDefault();
        }

        public async Task<PersonCompleteInfoModel?> GetPersonInfo(int id, CancellationToken cancellationToken)
        {
            return (await personRepository.SelectByAsync(p => p.Id.Equals(id),
                p => new PersonCompleteInfoModel()
                {
                    FName = p.FName,
                    LName = p.LName,
                    FatherName = p.FatherName,
                    NationalCode = p.NationalCode,
                    SSID = p.SSID,
                    BirthDate = p.BirthDate,
                    UserType = p.OrganizationId.HasValue ? p.Organization.OrganizationType.Name : "حقیقی",
                    PostalCode = p.PostalCode,
                    Gender = p.Gender,
                    Nationality = p.Country.Name,
                    Address = p.Address,
                    Mobile = p.Mobile,
                    AddressProvince = p.City.Parent.Name,
                    AddressCity = p.City.Name,
                    PhoneNumber = p.User.PhoneNumber
                }, cancellationToken))
                .FirstOrDefault();
        }

        public async Task<bool> UpdateJobInfo(PersonJobEditModel model, CancellationToken cancellationToken = default)
        {
            var personId = await userRepository.TableNoTracking.Where(p => p.Id == model.UserId).Select(p => p.PersonId).FirstOrDefaultAsync(cancellationToken);
            var personJobInfo = (await personJobInfoRepository.SelectByAsync(p => p.PersonId == personId, cancellationToken,
                navigationPropertyPaths: p => p.Person))
                .FirstOrDefault();

            if (personJobInfo != default(PersonJobInfo))
            {
                personJobInfo.Address = model.Address;
                personJobInfo.PhoneNumber = model.PhoneNumber;
                personJobInfo.JobTitleId = model.JobTitleId;
                personJobInfo.SalaryRangeId = model.SalaryRangeId;
                personJobInfo.IsActive = true;

                await personJobInfoRepository.UpdateAsync(personJobInfo, cancellationToken, true);
            }
            else
            {
                await personJobInfoRepository.AddAsync(new PersonJobInfo()
                {
                    PersonId = personId,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    JobTitleId = model.JobTitleId,
                    SalaryRangeId = model.SalaryRangeId,
                    IsActive = true
                }, cancellationToken);
            }

            logger.LogInformation("Buyer User Updated ");
            return true;
        }

        public async Task<PersonJobViewModel?> GetJobInfo(int personId, CancellationToken cancellationToken = default)
        {
            return await personJobInfoRepository.TableNoTracking
                 .Where(p => p.PersonId == personId)
                 .Select(p => new PersonJobViewModel()
                 {
                     Address = p.Address,
                     PhoneNumber = p.PhoneNumber,
                     JobTitle = p.JobTitle.Title,
                     SalaryRange = p.SalaryRange.Title
                 })
                 .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ValidateValidatedAddressAsync(ValidateAddressInputModel model, CancellationToken cancellationToken = default)
        {
            var person = await personRepository.GetByIdAsync(cancellationToken, model.PersonId);

            person.IsAddressValidated = true;
            person.VerifiedAddress = model.VerifiedAddress;
            person.Address = model.VerifiedAddress;

            await personRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> SetSanaTrackingCodeAsync(string nationalCode, string trackingCode, CancellationToken cancellationToken)
        {
            var person = personRepository.GetByCondition(p => p.NationalCode == nationalCode, true);

            person.SanaTrackingId = trackingCode;

            await personRepository.UpdateAsync(person, cancellationToken);

            return true;
        }

        public async Task UpdateAsync(UpdatePersonInfoInputModel model, CancellationToken cancellationToken = default) 
        {
            var person = personRepository.GetByCondition(c => c.NationalCode == model.NationalCode);

            person.FName = model.FirstName;
            person.LName = model.LastName;
            person.FatherName = model.FatherName;
            person.Gender = model.Gender;
            person.PlaceOfBirth = model.PlaceOfBirth;
            person.SSID = model.SSID;

            await personRepository.UpdateAsync(person, cancellationToken);
        }
    }
}
