using BudgetManager.Models;
using Dapper;
using System.Data.SqlClient;

namespace BudgetManager.Services
{
    public interface IRepositoryAccounts
    {
        Task Create(Account account);
        Task Delete(int id);
        Task<IEnumerable<Account>> GetAll(int idUser);
        Task<Account> GetById(int id, int idUser);
        Task Update(AccountViewModel account);
    }
    public class AccountRepository : IRepositoryAccounts
    {
        private readonly string connectionString;

        public AccountRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Account account)
        {

            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO [dbo].[Accounts]
                                                       ([Name]
                                                       ,[IdAccountType]
                                                       ,[Balance]
                                                       ,[Description])
                                                         VALUES (@Name, @IdAccountType, @Balance, @Description)
		                                                      Select Scope_Identity();", account);

            account.Id = id;
        }

        public async Task<IEnumerable<Account>> GetAll(int idUser) { 
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Account>(@"select ac.Id, ac.Name, Balance, at.Name as 'AccountType'
                                                        from accounts ac
                                                        inner join AccountTypes at 
                                                        on at.Id = ac.IdAccountType
                                                        where at.IdUser = @IdUser
                                                        order by at.Position", new {idUser });
        }

        public async Task<Account> GetById(int id, int idUser) { 

            using var connection= new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>(@"select ac.Id, ac.Name, Balance,Description, ac.IdAccountType 
                                                                        from accounts ac
                                                                        inner join AccountTypes at 
                                                                        on at.Id = ac.IdAccountType
                                                                        where at.IdUser = @IdUser and ac.Id = @Id", new {id, idUser });
        
        }

        public async Task Update(AccountViewModel account) {
            using var connection = new SqlConnection(connectionString);
           await connection.ExecuteAsync(@"UPDATE [dbo].[Accounts]
                                       SET[Name] = @Name 
                                          ,[IdAccountType] = @IdAccountType
                                          ,[Balance] = @Balance
                                          ,[Description] = @Description
                                     WHERE Id = @Id", account);

        }

        public async Task Delete(int id) {
            using var connection = new SqlConnection(connectionString);
             await connection.ExecuteAsync("Delete from Accounts where Id=@Id", new { id });
        }

      
    }
}
