namespace Questao5.Infrastructure.Database.Repositories
{
    public interface IDapperWrapper
    {
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null);

        Task<List<T>> QueryAsync<T>(string sql, object param = null);

        Task<int> ExecuteAsync(string sql, object param = null);
    }
}