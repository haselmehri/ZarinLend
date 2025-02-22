using Common;
using Common.Exceptions;
using Common.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Mvc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WebFramework.StimulsoftHelper
{
    public enum ReportNameEnum
    {
        AyandehContract,
    }
    public class StimulsoftHelper : IStimulsoftHelper, ISingletonDependency
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly ILogger<StimulsoftHelper> logger;

        public StimulsoftHelper(IWebHostEnvironment webHostEnvironment, IRequestFacilityService requestFacilityService, ILogger<StimulsoftHelper> logger)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.requestFacilityService = requestFacilityService;
            this.logger = logger;
            //.Net Core
            //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkcgIvwL0jnpsDqRpWg5FI5kt2G7A0tYIcUygBh1sPs7rE7BAeUEkpkjUKhl6/j24S6yxsIWZIRjJksEoLVUjBueVKUbrngXOqKSPJ8HE3n1pShqAKcqrYW8MlF8pB4nnRnYzLWJ/P+/p8zFGywvfSWm7L6hGvJFWozdlx5wLTj4K5UuclS2XfPNkIDrt7BY5X2KVdt42NBLZbM5RdUB8iJFobpp0HzoKZI8TSn++9s0y2cM/uGn0zHRcz/b8P/PiiOJkRkm0XlFrXG19KuA6eBAUfWiHYAgTMZq2UCyOdCbDZEcF8SqCGjboFuTyI7OHTQ4PVFQY8uEmsqhes9jqiz7u7Ts7Ndy88rVAe10GiHrBdyAGf4AR4G9DFrA10fnTGIVLixX8GpNTGgsLFIOf+IQOUvdcV39PeCf2JA2vEhSqbiaiftgGwxxgbc8ENPXijj+wYztDzMBeTJUwZBheNLcD2Rqwrc//HYvbuG6aZSjPCA5DvD3QJMvdBdHM3HWvlyU0tN6xVAiECAvWQdSOks";
            Stimulsoft.Base.StiLicense.LoadFromFile("license.key");
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            Stimulsoft.Base.StiFontCollection.AddFontFile(Path.Combine(webHostEnvironment.WebRootPath, @$"fonts\Vazir\Vazir.ttf"));
            Stimulsoft.Base.StiFontCollection.AddFontFile(Path.Combine(webHostEnvironment.WebRootPath, @$"fonts\Vazir\Vazir-Light.ttf"));
            Stimulsoft.Base.StiFontCollection.AddFontFile(Path.Combine(webHostEnvironment.WebRootPath, @$"fonts\Vazir\Vazir-Thin.ttf"));
        }

        public StiReport GetReport(ReportNameEnum reportName)
        {
            var reportPath = string.Empty;
            switch (reportName)
            {
                case ReportNameEnum.AyandehContract:
                    reportPath = Path.Combine(webHostEnvironment.WebRootPath, $"Report\\{reportName}.mrt");
                    break;
                default:
                    break;
            }

            if (reportPath == string.Empty || !File.Exists(reportPath))
                throw new AppException("Report file not found!");

            StiReport report = new StiReport();
            report.Load(Path.Combine(webHostEnvironment.WebRootPath, $"Report\\{reportName}.mrt"));
            if (report == null)
                throw new AppException("Report file not found!");

            return report;
        }

        public StiNetCoreActionResult ExportToPdf(StiReport report)
        {
            var pdf = StiNetCoreReportResponse.ResponseAsPdf(report, new StiPdfExportSettings()
            {
                StandardPdfFonts = false,
            });
            #region Save Pdf on Disk

            //string path = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{Guid.NewGuid()}.pdf");
            ////System.IO.File.WriteAllBytes(path, pdf.Data);

            //using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            //{
            //    stream.Write(pdf.Data, 0, pdf.Data.Length);
            //}

            #endregion Save Pdf on Disk
            return pdf;
        }

        public StiNetCoreActionResult ExportToPdf(StiReport report, StiPdfExportSettings stiPdfExportSettings)
        {
            var pdf = StiNetCoreReportResponse.ResponseAsPdf(report, stiPdfExportSettings);

            return pdf;
        }

        public async Task<StiReport> GetContractReport(int requestFacilityId, CancellationToken cancellationToken)
        {
            // Create the report object
            StiReport report = GetReport(ReportNameEnum.AyandehContract);

            var contractInfo = await requestFacilityService.GetRequestFacilityInfoForContract(requestFacilityId, cancellationToken);
            report.Dictionary.Variables["CurrentDate"].Value = DateTime.Now.GregorianToShamsi();
            report.Dictionary.Variables["FName"].Value = contractInfo.FName;
            report.Dictionary.Variables["LName"].Value = contractInfo.LName;
            report.Dictionary.Variables["FatherName"].Value = contractInfo.FatherName;
            report.Dictionary.Variables["NationalCode"].Value = contractInfo.NationalCode;
            report.Dictionary.Variables["SSID"].Value = contractInfo.SSID;
            report.Dictionary.Variables["BirthCertificateSerial"].Value = contractInfo.IdentificationSerial;
            report.Dictionary.Variables["CityOfIssue"].Value = contractInfo.CityOfIssue;
            report.Dictionary.Variables["BirthDate"].Value = contractInfo.ShamsiBirthDate;
            report.Dictionary.Variables["Mobile"].Value = contractInfo.Mobile;
            report.Dictionary.Variables["PostalCode"].Value = contractInfo.PostalCode;
            report.Dictionary.Variables["PhoneNumber"].Value = contractInfo.PhoneNumber;
            report.Dictionary.Variables["Email"].Value = contractInfo.Email;
            report.Dictionary.Variables["FacilityNumber"].Value = contractInfo.FacilityNumber;
            report.Dictionary.Variables["Address"].Value = $"{contractInfo.AddressProvince}-{contractInfo.AddressCity}-{contractInfo.Address}";
            report.Dictionary.Variables["OrganizationName"].Value = contractInfo.OrganizationName;
            report.Dictionary.Variables["OrganizationBranch"].Value = contractInfo.OrganizationBranch;
            report.Dictionary.Variables["OrganizationCode"].Value = contractInfo.OrganizationCode;
            report.Dictionary.Variables["OrganizationAgent"].Value = contractInfo.OrganizationAgent;
            report.Dictionary.Variables["OrganizationAddress"].Value = contractInfo.OrganizationAddress;

            report.Dictionary.Variables["CustomerNumber"].Value = contractInfo.CustomerNumber;

            report.Dictionary.Variables["ContractSubject"].Value = contractInfo.ContractSubject;
            report.Dictionary.Variables["ContractPercentage"].Value = "23";
            report.Dictionary.Variables["RequestAmount"].Value = contractInfo.Amount.ToString("N0");
            report.Dictionary.Variables["RequestAmountText"].Value = NumberHelper.NumberToString(contractInfo.Amount);
            report.Dictionary.Variables["TotalInstallment"].Value =
                InstallmentCalculator.CalculateTotalInstallment(contractInfo.Amount, contractInfo.Month, contractInfo.FacilityInterest).ToString("N0");
            report.Dictionary.Variables["TotalInstallmentText"].Value =
                NumberHelper.NumberToString(Math.Round(InstallmentCalculator.CalculateTotalInstallment(contractInfo.Amount, contractInfo.Month, contractInfo.FacilityInterest)));

            report.Dictionary.Variables["FacilityInterest"].Value =
                (InstallmentCalculator.CalculateTotalInstallment(contractInfo.Amount, contractInfo.Month, contractInfo.FacilityInterest) - contractInfo.Amount).ToString("N0");
            report.Dictionary.Variables["FacilityInterestText"].Value =
                NumberHelper.NumberToString(Math.Round(InstallmentCalculator.CalculateTotalInstallment(contractInfo.Amount, contractInfo.Month, contractInfo.FacilityInterest) - contractInfo.Amount));
            report.Dictionary.Variables["Month"].Value = contractInfo.Month.ToString();
            report.Dictionary.Variables["SafteNumber"].Value = contractInfo.SafteNumber;
            report.Dictionary.Variables["AmountWarranty"].Value =
                InstallmentCalculator.CalculateChequeAmountWarranty(contractInfo.Amount, contractInfo.Month, contractInfo.FacilityInterest, contractInfo.WarantyPercentage).ToString("N0");

            // Load data from XML file for report template
            //if (!report.IsDocument)
            //{
            //    List<Person> persons = new List<Person>()
            //    {
            //        new Person(){FName="علی",LName="دایی", Age = 48},
            //        new Person(){FName="علی",LName="کریمی",Age = 42},
            //    };

            //    report.RegBusinessObject("Person", persons);
            //}

            return report;
        }
    }
}
