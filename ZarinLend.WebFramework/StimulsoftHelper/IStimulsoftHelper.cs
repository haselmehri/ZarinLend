using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Stimulsoft.Report.Mvc;

namespace WebFramework.StimulsoftHelper
{
    public interface IStimulsoftHelper
    {
        StiReport GetReport(ReportNameEnum reportName);
        StiNetCoreActionResult ExportToPdf(StiReport report);
        StiNetCoreActionResult ExportToPdf(StiReport report, StiPdfExportSettings stiPdfExportSettings);
        Task<StiReport> GetContractReport(int requestFacilityId, CancellationToken cancellationToken);
    }
}
