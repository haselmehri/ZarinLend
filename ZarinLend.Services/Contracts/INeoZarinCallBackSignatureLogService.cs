using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface INeoZarinCallBackSignatureLogService
    {
        Task AddLog(NeoZarinCallBackSignatureLogModel model, CancellationToken cancellationToken);
    }
}