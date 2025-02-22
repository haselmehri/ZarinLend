using Common;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class FinotechLogRepository : BaseRepository<FinotechLog>, IFinotechLogRepository, IScopedDependency
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<FinotechLogRepository> logger;

        public FinotechLogRepository(ApplicationDbContext dbContext, ILogger<FinotechLogRepository> logger) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<long> AddLog(FinotechLog finotechLog)
        {
            try
            {
                using (var command = dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Insert Into FinotechLogs (RequestFacilityId,Curl,OpratorId,ServiceName,Url,Body,MethodType,TrackId) Values (@RequestFacilityId,@Curl,@OpratorId,@ServiceName,@Url,@Body,@MethodType,@TrackId);SELECT SCOPE_IDENTITY();";
                    var urlParam = command.CreateParameter();
                    urlParam.ParameterName = "@Url";
                    urlParam.Value = finotechLog.Url;
                    command.Parameters.Add(urlParam);

                    var requestFacilityIdParam = command.CreateParameter();
                    requestFacilityIdParam.ParameterName = "@RequestFacilityId";
                    if (finotechLog.RequestFacilityId.HasValue)
                        requestFacilityIdParam.Value = finotechLog.RequestFacilityId;
                    else
                        requestFacilityIdParam.Value = DBNull.Value;

                    command.Parameters.Add(requestFacilityIdParam);

                    var curlParam = command.CreateParameter();
                    curlParam.ParameterName = "@Curl";
                    if (!string.IsNullOrEmpty(finotechLog.Curl))
                        curlParam.Value = finotechLog.Curl;
                    else
                        curlParam.Value = DBNull.Value;

                    command.Parameters.Add(curlParam);

                    var creatorParam = command.CreateParameter();
                    creatorParam.ParameterName = "@OpratorId";
                    if (finotechLog.OpratorId.HasValue)
                        creatorParam.Value = finotechLog.OpratorId;
                    else
                        creatorParam.Value = DBNull.Value;

                    command.Parameters.Add(creatorParam);

                    var serviceNameParam = command.CreateParameter();
                    serviceNameParam.ParameterName = "@ServiceName";
                    serviceNameParam.Value = finotechLog.ServiceName;
                    command.Parameters.Add(serviceNameParam);

                    var bodyParam = command.CreateParameter();
                    bodyParam.ParameterName = "@Body";
                    if (!string.IsNullOrEmpty(finotechLog.Body))
                        bodyParam.Value = finotechLog.Body;
                    else
                        bodyParam.Value = DBNull.Value;

                    command.Parameters.Add(bodyParam);

                    var methodTypeParam = command.CreateParameter();
                    methodTypeParam.ParameterName = "@MethodType";
                    methodTypeParam.Value = (int)finotechLog.MethodType;
                    command.Parameters.Add(methodTypeParam);

                    var trackParam = command.CreateParameter();
                    trackParam.ParameterName = "@TrackId";
                    trackParam.Value = finotechLog.TrackId;
                    command.Parameters.Add(trackParam);

                    await dbContext.Database.OpenConnectionAsync();
                    return Convert.ToInt64(await command.ExecuteScalarAsync());
                }
                #region old code
                //var finotechLog = new FinotechLog()
                //{
                //    Body = body,
                //    Url = url,
                //    MethodType = methodType,
                //    TrackId = trackId
                //};
                //await finotechLogRepository.AddAsync(finotechLog, default);

                //return finotechLog.Id;
                #endregion
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return -1;
            }
        }

        public async Task<long> UpdateLog(long id, string result)
        {
            try
            {
                using (var command = dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Update FinotechLogs Set Result=@Result,UpdateDate=@UpdateDate Where Id= @Id";

                    var idParam = command.CreateParameter();
                    idParam.ParameterName = "@Id";
                    idParam.Value = id;
                    command.Parameters.Add(idParam);

                    var resultParam = command.CreateParameter();
                    resultParam.ParameterName = "@Result";
                    resultParam.Value = result;
                    command.Parameters.Add(resultParam);

                    var updateParam = command.CreateParameter();
                    updateParam.ParameterName = "@UpdateDate";
                    updateParam.Value = DateTime.Now;
                    command.Parameters.Add(updateParam);

                    await dbContext.Database.OpenConnectionAsync();
                    return await command.ExecuteNonQueryAsync();
                }
                //var finotechLog = await finotechLogRepository.GetByIdAsync(default, id);
                //if (finotechLog != null)
                //{
                //    finotechLog.Result = result;
                //    await finotechLogRepository.UpdateAsync(finotechLog, default);
                //}

                //return finotechLog.Id;
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return -1;
            }
        }
    }
}
