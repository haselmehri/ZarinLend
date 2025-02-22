using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class AyandehSignRequestSignatureLogRepository : BaseRepository<AyandehSignRequestSignatureLog>, IAyandehSignRequestSignatureLogRepository, IScopedDependency
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<AyandehSignRequestSignatureLogRepository> logger;

        public AyandehSignRequestSignatureLogRepository(ApplicationDbContext dbContext, ILogger<AyandehSignRequestSignatureLogRepository> logger) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<long> AddLog(AyandehSignRequestSignatureLog ayandehSignRequestSignatureLog)
        {
            try
            {
                using (var command = dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Insert Into AyandehSignRequestSignatureLogs (RequestFacilityId,Curl,CreatorId,Url,ServiceName,MethodType,Body) Values (@RequestFacilityId,@Curl,@CreatorId,@Url,@ServiceName,@MethodType,@Body);SELECT SCOPE_IDENTITY();";
                    var bodyParam = command.CreateParameter();
                    bodyParam.ParameterName = "@Body";
                    bodyParam.Value = ayandehSignRequestSignatureLog.Body;
                    command.Parameters.Add(bodyParam);

                    var curlParam = command.CreateParameter();
                    curlParam.ParameterName = "@Curl";
                    if (!string.IsNullOrEmpty(ayandehSignRequestSignatureLog.Curl))
                        curlParam.Value = ayandehSignRequestSignatureLog.Curl;
                    else
                        curlParam.Value = DBNull.Value;

                    command.Parameters.Add(curlParam);

                    var urlParam = command.CreateParameter();
                    urlParam.ParameterName = "@Url";
                    urlParam.Value = ayandehSignRequestSignatureLog.Url;
                    command.Parameters.Add(urlParam);

                    var requestFacilityIdParam = command.CreateParameter();
                    requestFacilityIdParam.ParameterName = "@RequestFacilityId";
                    requestFacilityIdParam.Value = ayandehSignRequestSignatureLog.RequestFacilityId;
                    command.Parameters.Add(requestFacilityIdParam);

                    var creatorParam = command.CreateParameter();
                    creatorParam.ParameterName = "@CreatorId";
                    if (!ayandehSignRequestSignatureLog.CreatorId.HasValue || ayandehSignRequestSignatureLog.CreatorId == default)
                        creatorParam.Value = DBNull.Value;
                    else
                        creatorParam.Value = ayandehSignRequestSignatureLog.CreatorId;
                    command.Parameters.Add(creatorParam);

                    var serviceNameParam = command.CreateParameter();
                    serviceNameParam.ParameterName = "@ServiceName";
                    serviceNameParam.Value = ayandehSignRequestSignatureLog.ServiceName;
                    command.Parameters.Add(serviceNameParam);

                    var methodTypeParam = command.CreateParameter();
                    methodTypeParam.ParameterName = "@MethodType";
                    methodTypeParam.Value = ayandehSignRequestSignatureLog.MethodType;
                    command.Parameters.Add(methodTypeParam);

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

        public async Task<int> UpdateLog(long id, string result, CancellationToken cancellationToken)
        {
            try
            {
                return await TableNoTracking.Where(p => p.Id == id)
                                            .ExecuteUpdateAsync(setter => setter.SetProperty(p => p.ResponseMessage, result)
                                                                                .SetProperty(p => p.Url, ""), cancellationToken);
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
