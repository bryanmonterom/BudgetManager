using BudgetManager.Models;
using Dapper;
using System.Data.SqlClient;

namespace BudgetManager.Services
{
    public interface IRepositoryCategories
    {
        Task<int> Count(int idUser);
        Task Create(Category category);
        Task Delete(int id);
        Task<IEnumerable<Category>> GetAll(int idUser, PaginationViewModel pagination);
        Task<IEnumerable<Category>> GetAll(int idUser, OperationType transactionTypeId);
        Task<Category> GetById(int id, int idUser);
        Task Update(Category category);
    }
    public class CategoriesRepository : IRepositoryCategories
    {
        private readonly string connectionString;
        public CategoriesRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Create(Category category)
        {

            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO [dbo].[Categories]
                                                                                   ([Name]
                                                                                   ,[IdTransactionType]
                                                                                   ,[IdUser])
                                                                             VALUES
           (@Name, @IdTransactionType, @IdUser); SELECT SCOPE_IDENTITY();", new { IdUser = category.IdUser, Name = category.Name, IdTransactionType = (int)category.IdTransactionType });
            category.Id = id;
        }

        public async Task<IEnumerable<Category>> GetAll(int idUser, PaginationViewModel pagination)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Category>(@$"Select * 
                                                            from Categories 
                                                            where IdUser = @IdUser
                                                                order by name
                                                            OFFSET {pagination.RecordsToIgnore} ROWS
                                                             FETCH NEXT {pagination.RecordsPerPage} rows only
                                                      

                                                            ", new { idUser });

        }

        public async Task<IEnumerable<Category>> GetAll(int idUser, OperationType transactionTypeId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Category>(@"Select * from Categories 
                                                            where IdUser = @IdUser and
                                                        IdTransactionType = @transactionTypeId", new { idUser, transactionTypeId });

        }

        public async Task<int> Count(int idUser)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(*) 
                                                FROM CATEGORIES WHERE IdUser = @idUser", new { idUser });
        }
        public async Task<Category> GetById(int id, int idUser)
        {

            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Category>(@"SELECT 
                                                        * FROM Categories
                                                         WHERE ID = @Id and IdUser = @IdUser", new { id, idUser });
        }

        public async Task Update(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE CATEGORIES 
                                            SET NAME=@NAME, 
                                            IdTransactionType=@IdTransactionType, 
                                            IdUser =@IdUser where Id = @Id", new { Id = category.Id, Name = category.Name, IdUser = category.IdUser, IdTransactionType = (int)category.IdTransactionType });

        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE 
                                        FROM CATEGORIES 
                                        WHERE ID=@Id", new { id });
        }



    }
}
