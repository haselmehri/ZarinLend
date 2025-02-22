using Common.Utilities;
using Microsoft.Extensions.Logging;
using System;

namespace Services.Model.NeginHub
{
    public class GetCivilRegistryDataResultModel : NeginHubBaseResult
    {
        private readonly ILogger<GetCivilRegistryDataResultModel> logger;

        public GetCivilRegistryDataResultModel()
        {

        }
        public GetCivilRegistryDataResultModel(ILogger<GetCivilRegistryDataResultModel> logger)
        {
            this.logger = logger;
        }
        public string NationalCode { get; set; }
        public string BirthDate { get; set; }
        public string? GregorianBirthDate
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(BirthDate))
                        return DateTimeHelper.ShamsiToGregorian(Convert.ToInt64(BirthDate).ToString("####/##/##")).ToString("yyyy/MM/dd");
                }
                catch (Exception exp)
                {
                    logger.LogError(exp, NationalCode, BirthDate);
                }


                return null;
            }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Gender { get; set; }
        public string IdentityId { get; set; }
        public string? IdentificationSerial { get; set; }
        public string? IdentificationSeri { get; set; }
        public string? PlaceOfBirth { get; set; }
        public bool IsAlive { get; set; }
        public string ErrorMessage { get; set; }
    }
}
