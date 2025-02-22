using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.Mvc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;
using WebFramework.StimulsoftHelper;

namespace Web.UI.Controllers
{
    public class ReportController : BaseMvcController
    {
        private readonly ILogger<ReportController> logger;
        private readonly IStimulsoftHelper stimulsoftHelper;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IRequestFacilityService requestFacilityService;

        public ReportController(ILogger<ReportController> logger, IStimulsoftHelper stimulsoftHelper, IWebHostEnvironment webHostEnvironment, IRequestFacilityService requestFacilityService)
        {
            this.logger = logger;
            this.stimulsoftHelper = stimulsoftHelper;
            this.webHostEnvironment = webHostEnvironment;
            this.requestFacilityService = requestFacilityService;


            //.Net Core
            //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkcgIvwL0jnpsDqRpWg5FI5kt2G7A0tYIcUygBh1sPs7rE7BAeUEkpkjUKhl6/j24S6yxsIWZIRjJksEoLVUjBueVKUbrngXOqKSPJ8HE3n1pShqAKcqrYW8MlF8pB4nnRnYzLWJ/P+/p8zFGywvfSWm7L6hGvJFWozdlx5wLTj4K5UuclS2XfPNkIDrt7BY5X2KVdt42NBLZbM5RdUB8iJFobpp0HzoKZI8TSn++9s0y2cM/uGn0zHRcz/b8P/PiiOJkRkm0XlFrXG19KuA6eBAUfWiHYAgTMZq2UCyOdCbDZEcF8SqCGjboFuTyI7OHTQ4PVFQY8uEmsqhes9jqiz7u7Ts7Ndy88rVAe10GiHrBdyAGf4AR4G9DFrA10fnTGIVLixX8GpNTGgsLFIOf+IQOUvdcV39PeCf2JA2vEhSqbiaiftgGwxxgbc8ENPXijj+wYztDzMBeTJUwZBheNLcD2Rqwrc//HYvbuG6aZSjPCA5DvD3QJMvdBdHM3HWvlyU0tN6xVAiECAvWQdSOks";
            //Stimulsoft.Base.StiLicense.LoadFromFile("license.key");
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            //Stimulsoft.Base.StiFontCollection.AddFontFile(Path.Combine(webHostEnvironment.WebRootPath, @$"fonts\Vazir\Vazir.ttf"));
            //Stimulsoft.Base.StiFontCollection.AddFontFile(Path.Combine(webHostEnvironment.WebRootPath, @$"fonts\Vazir\Vazir-Light.ttf" ));
            //Stimulsoft.Base.StiFontCollection.AddFontFile(Path.Combine(webHostEnvironment.WebRootPath, @$"fonts\Vazir\Vazir-Thin.ttf" ));
        }

        [CustomAuthorize()]
        public IActionResult ReportViewer()
        {
            return View();
        }

        [CustomAuthorize]
        [Route("[controller]/[action]/{requestFacilityId:int?}")]
        public async Task<IActionResult> GetReport(int? requestFacilityId, CancellationToken cancellationToken)
        {
            var report = await stimulsoftHelper.GetContractReport(requestFacilityId!.Value, cancellationToken);        

            return StiNetCoreViewer.GetReportResult(this, report);
        }

        public IActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }

        [Route("[controller]/[action]/{requestFacilityId:int?}")]
        public async Task<IActionResult> DownloadPDF(int? requestFacilityId,CancellationToken cancellationToken)
        {
            var report = await stimulsoftHelper.GetContractReport(requestFacilityId!.Value, cancellationToken);
            report.ReportName = "قرارداد تسهیلات";
            //var pdf = StiNetCoreReportResponse.ResponseAsPdf(report, new StiPdfExportSettings()
            //{
            //    StandardPdfFonts = false,                                  
            //});
            var pdfResponse = stimulsoftHelper.ExportToPdf(report, new StiPdfExportSettings()
            {
                StandardPdfFonts = false,                
            });
            #region Save Pdf on Disk

            //string path = Path.Combine(webHostEnvironment.WebRootPath, $"UploadFiles\\RequestFacilityContract\\{Guid.NewGuid()}.pdf");
            ////System.IO.File.WriteAllBytes(path, pdf.Data);

            //using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            //{
            //    stream.Write(pdfResponse.Data, 0, pdfResponse.Data.Length);
            //}

            #endregion Save Pdf on Disk
            return pdfResponse;
        }
    }
}