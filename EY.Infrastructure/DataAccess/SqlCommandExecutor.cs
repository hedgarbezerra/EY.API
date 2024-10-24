using Dapper;
using EY.Domain.Contracts;
using EY.Shared.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EY.Infrastructure.DataAccess
{
    [BindInterface(typeof(ISqlExecutor))]
    public class SqlCommandExecutor : ISqlExecutor
    {
        private readonly AppDbContext _dbContext;

        public SqlCommandExecutor(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Query<T>(FormattableString query)
        {
            using var connection = _dbContext.Database.GetDbConnection();
            connection.Open();
            var sql = query.Format;
            var parameters = GetParameters(query);

            var result = connection.Query<T>(sql, parameters).AsQueryable();

            return result;
        }

        public IQueryable<T> Query<T>(string query, object parameters)
        {
            using var connection = _dbContext.Database.GetDbConnection();
            connection.Open();
            var result = connection.Query<T>(query, parameters).AsQueryable();

            return result;
        }

        public int Execute(FormattableString query)
        {
           using var connection = _dbContext.Database.GetDbConnection();
            connection.Open();
            var sql = query.Format;
            var parameters = GetParameters(query);

            return connection.Execute(sql, parameters);
        }

        private object GetParameters(FormattableString query)
        {
            var parameters = new DynamicParameters();

            for (int i = 0; i < query.ArgumentCount; i++)
            {
                parameters.Add($"param{i}", query.GetArgument(i));
            }

            return parameters;
        }

    }
}
