using Services.Dto;
using Services.Model.GlobalSetting;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IGlobalSettingService
    {
        Task<GlobalSettingViewModel?> GetActiveGlobalSetting(CancellationToken cancellationToken);
        Task<PagingDto<GlobalSettingViewModel>> GetGlobalSettingList(PagingFilterDto filter, CancellationToken cancellationToken);
        Task Add(GlobalSettingModel globalSettingModel, CancellationToken cancellationToken);
    }
}