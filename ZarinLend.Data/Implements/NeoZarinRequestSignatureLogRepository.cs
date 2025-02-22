using Common;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class NeoZarinRequestSignatureLogRepository : BaseRepository<NeoZarinRequestSignatureLog>, INeoZarinRequestSignatureLogRepository, IScopedDependency
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<NeoZarinRequestSignatureLogRepository> logger;

        public NeoZarinRequestSignatureLogRepository(ApplicationDbContext dbContext, ILogger<NeoZarinRequestSignatureLogRepository> logger) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<long> AddLog(NeoZarinRequestSignatureLog neoZarinRequestSignatureLog)
        {
            try
            {
                using (var command = dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Insert Into NeoZarinRequestSignatureLogs (RequestFacilityId,CreatorId,Url,Mobile,TrackId) Values (@RequestFacilityId,@CreatorId,@Url,@Mobile,@TrackId);SELECT SCOPE_IDENTITY();";
                    var urlParam = command.CreateParameter();
                    urlParam.ParameterName = "@Url";
                    urlParam.Value = neoZarinRequestSignatureLog.Url;
                    command.Parameters.Add(urlParam);

                    var requestFacilityIdParam = command.CreateParameter();
                    requestFacilityIdParam.ParameterName = "@RequestFacilityId";
                    requestFacilityIdParam.Value = neoZarinRequestSignatureLog.RequestFacilityId;
                    command.Parameters.Add(requestFacilityIdParam);

                    var creatorParam = command.CreateParameter();
                    creatorParam.ParameterName = "@CreatorId";
                    creatorParam.Value = neoZarinRequestSignatureLog.CreatorId;
                    command.Parameters.Add(creatorParam);

                    var mobileParam = command.CreateParameter();
                    mobileParam.ParameterName = "@Mobile";
                    mobileParam.Value = neoZarinRequestSignatureLog.Mobile;
                    command.Parameters.Add(mobileParam);

                    var trackParam = command.CreateParameter();
                    trackParam.ParameterName = "@TrackId";
                    trackParam.Value = neoZarinRequestSignatureLog.TrackId;
                    command.Parameters.Add(trackParam);

                    await dbContext.Database.OpenConnectionAsync();
                    return Convert.ToInt64(await command.ExecuteScalarAsync());
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return -1;
            }
        }

        public async Task<int> UpdateLog(long id, string result,CancellationToken cancellationToken)
        {
            try
            {
                return await TableNoTracking.Where(p => p.Id == id)
                                            .ExecuteUpdateAsync(setter => setter.SetProperty(p => p.Result, result), cancellationToken);
                //using (var command = dbContext.Database.GetDbConnection().CreateCommand())
                //{
                //    command.CommandText = "Update NeoZarinRequestSignatureLogs Set Result=@Result,UpdateDate=@UpdateDate Where Id= @Id";

                //    var idParam = command.CreateParameter();
                //    idParam.ParameterName = "@Id";
                //    idParam.Value = id;
                //    command.Parameters.Add(idParam);

                //    var resultParam = command.CreateParameter();
                //    resultParam.ParameterName = "@Result";
                //    resultParam.Value = result;
                //    command.Parameters.Add(resultParam);

                //    var updateParam = command.CreateParameter();
                //    updateParam.ParameterName = "@UpdateDate";
                //    updateParam.Value = DateTime.Now;
                //    command.Parameters.Add(updateParam);

                //    await dbContext.Database.OpenConnectionAsync();
                //    return await command.ExecuteNonQueryAsync();
                //}
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return -1;
            }
        }
    }
}
