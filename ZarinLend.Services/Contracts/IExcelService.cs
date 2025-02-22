using Services.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IExcelService 
    {
        Task<MemoryStream> GenerateRequestFacilityDetail(RequestFacilityInfoModel requestFacilityInfo, SamatFacilityHeaderModel samatFacility,
            SamatBackChequeHeaderModel samatBackCheque,CancellationToken cancellationToken);
    }
}
