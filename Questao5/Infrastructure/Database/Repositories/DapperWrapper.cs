using Dapper;
using System.Data;

namespace Questao5.Infrastructure.Database.Repositories
{
    public class DapperWrapper : IDapperWrapper
    {
        private readonly IDbConnection _dbConnection;

        public DapperWrapper(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> ExecuteAsync(string sql, object param = null)
        {
            return await _dbConnection.ExecuteAsync(sql, param);
        }

        public async Task<List<T>> QueryAsync<T>(string sql, object param = null)
        {
            var result = await _dbConnection.QueryAsync<T>(sql, param);
            return result.ToList();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, param);
        }
    }
}