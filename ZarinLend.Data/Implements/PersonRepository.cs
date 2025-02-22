using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class PersonRepository : BaseRepository<Person>, IPersonRepository, IScopedDependency
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<PersonRepository> logger;

        public PersonRepository(ApplicationDbContext dbContext,ILogger<PersonRepository> logger) : base(dbContext)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<DataTable> GetZarinpalTransactionInfo(string hashCardString,CancellationToken cancellationToken)
        {
            try
            {
                using (var command = dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Select * from Card_Hash Where Card_Hash=@Card_Hash";
                    var bodyParam = command.CreateParameter();
                    bodyParam.ParameterName = "@Card_Hash";
                    bodyParam.Value = hashCardString;
                    command.Parameters.Add(bodyParam);
                   
                    await dbContext.Database.OpenConnectionAsync(cancellationToken);
                    var reader = await command.ExecuteReaderAsync(cancellationToken);
                    if (reader.HasRows)
                    {
                        if (await reader.ReadAsync(cancellationToken))
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("CardPan", typeof(string));
                            dt.Columns.Add("CardHash", typeof(string));
                            dt.Columns.Add("NumnerOfTerminals", typeof(int));
                            dt.Columns.Add("NumberOfTransactions", typeof(int));
                            dt.Columns.Add("SumAmount", typeof(double));
                            var row = dt.NewRow();
                            row["CardPan"] = Convert.ToString(reader["card_pan"]);
                            row["CardHash"] = Convert.ToString(reader["card_hash"]);
                            row["NumnerOfTerminals"] = Convert.ToInt32(reader["No_of_Terminals"]);
                            row["NumberOfTransactions"] = Convert.ToInt32(reader["No_of_Transactions"]);
                            row["SumAmount"] = Convert.ToDouble(reader["SUM_Amount"]);

                            dt.Rows.Add(row);
                            return dt;
                        }
                    }

                    return null;
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return null;
            }
        }
    }
}
