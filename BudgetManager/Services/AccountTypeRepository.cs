using BudgetManager.Models;
using Dapper;
using System.Data.SqlClient;

namespace BudgetManager.Services
{

    public interface IRepositoryAccountTypes
    {
        Task Delete(int id);
        Task<bool> Exists(string name, int idUser, int id=0);
        Task<IEnumerable<AccountType>> Get(int IdUser);
        Task<AccountType> GetById(int id, int idUser);
        Task Insert(AccountType accountType);
        Task Sort(IEnumerable<AccountType> accountTypes);
        Task Update(AccountType accountType);
    }
    public class AccountTypeRepository : IRepositoryAccountTypes
    {
        private readonly string connectionString;
        public AccountTypeRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Insert(AccountType accountType)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(@"AccountType_Insert", new {Name=accountType.Name, IdUser=accountType.IdUser }, commandType: System.Data.CommandType.StoredProcedure);
                accountType.Id = id;
            }

        }

        public async Task<bool> Exists(string name, int idUser, int id=0)
        {

            using (var connection = new SqlConnection(connectionString))
            {
                var exists = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM AccountTypes 
                                                                            where Name = @Name and IdUser = @IdUser and Id <>@id;",
                                                                            new { name, idUser, id });
                return exists == 1;
            }
        }

        public async Task<IEnumerable<AccountType>> Get(int IdUser)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return await connection.QueryAsync<AccountType>(@"SELECT ID, NAME, Position FROM AccountTypes
                                                                where IdUser = @IdUser order by Position asc", new { IdUser });
            }
        }

        public async Task Update(AccountType accountType)
        {

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE AccountTypes
                                            SET NAME = @NAME
                                            WHERE ID = @ID",  accountType);
        }

        public async Task<AccountType> GetById(int id, int idUser) { 
        
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<AccountType>(@"SELECT Id, Name, Position, IdUser 
                                                                            FROM AccountTypes
                                                                            where Id = @Id and IdUser = @IdUser", new {id, idUser});
        }

        public async Task Delete(int id) {
            using var connection =new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM AccountTypes
                                            where Id = @Id", new { id});

        }

        public async Task Sort(IEnumerable<AccountType> accountTypes) {

            var query = @"UPDATE AccountTypes
                        SET Position =@Position
                        WHERE ID = @ID ";

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, accountTypes);
        
        }
    }
}
