using Services.Dto;
using Services.Model.IranCreditScoring;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IIranCreditScoringResultRuleService
    {
        Task<IranCreditScoringResultRuleAddEditModel> Get(int id, CancellationToken cancellationToken);
        Task<PagingDto<IranCreditScoringResultRuleListModel>> GetList(PagingFilterDto filter, CancellationToken cancellationToken);
        Task Add(IranCreditScoringResultRuleAddEditModel model, CancellationToken cancellationToken);
        Task Edit(IranCreditScoringResultRuleAddEditModel model, CancellationToken cancellationToken);
        Task Delete(int id, CancellationToken cancellationToken);
    }
}