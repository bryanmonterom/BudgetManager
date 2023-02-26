using BudgetManager.Models;
using Dapper;
using System.Data.SqlClient;

namespace BudgetManager.Services
{
    public interface IRepositoryTransactions
    {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<IEnumerable<Transaction>> GetByAccountId(ParametersTransactionsByAccount model);
        Task<Transaction> GetById(int id, int idUser);
        Task<IEnumerable<Transaction>> GetByIdUser(ParametersTransactionsByUsers model);
        Task<IEnumerable<ResultsGetByMonth>> GetByMonths(int idUser, int year);
        Task<IEnumerable<ResultsGetByWeek>> GetByWeek(ParametersTransactionsByUsers model);
        Task Update(Transaction transaction, decimal previousAmount, int previousAccount);
    }
    public class TransactionsRepository : IRepositoryTransactions
    {
        private readonly string connectionString = "";
        public TransactionsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Transaction transaction)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"Transactions_Insert", new
            {
                transaction.IdUser,
                transaction.TransactionDate,
                transaction.Amount,
                transaction.IdCategory,
                transaction.IdAccount,
                transaction.Notes
            }, commandType: System.Data.CommandType.StoredProcedure);

            transaction.Id = id;
        }

        public async Task Update(Transaction transaction, decimal previousAmount, int previousIdAccount)
        {

            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Transactions_Update", new
            {
                transaction.Id,
                transaction.TransactionDate,
                transaction.Notes,
                transaction.Amount,
                transaction.IdAccount,
                transaction.IdCategory,
                previousIdAccount,
                previousAmount
            }, commandType: System.Data.CommandType.StoredProcedure);

        }

        public async Task<Transaction> GetById(int id, int idUser)
        {

            using var connection = new SqlConnection(connectionString);

            return await connection.
                        QueryFirstOrDefaultAsync<Transaction>(@"SELECT t.*, cat.IdTransactionType 
                                                        FROM Transactions t
                                                        INNER JOIN Categories cat 
                                                        on cat.Id = t.IdCategory
                                                        where t.Id = @Id and t.IdUser = @IdUser", new { id, idUser });

        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Delete_Transactions", new { id }, commandType: System.Data.CommandType.StoredProcedure);


        }

        public async Task<IEnumerable<Transaction>> GetByAccountId(ParametersTransactionsByAccount model)
        {

            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transaction>(@" SELECT t.Id, t.TransactionDate, 
                                            C.Name as Category, 
                                            a.Name as Account,
                                            t.Amount,
                                            c.IdTransactionType 
                                            FROM Transactions T
                                            INNER JOIN Categories c on
                                            c.Id  = t.IdCategory
                                            INNER JOIN Accounts a 
                                            on a.Id = t.IdAccount
                                            where t.IdAccount = @IdAccount and t.IdUser = @IdUser
                                            and t.TransactionDate between @DateStart and @DateEnd", new { model.IdUser, model.IdAccount, model.DateStart, model.DateEnd });
        }

        public async Task<IEnumerable<Transaction>> GetByIdUser(ParametersTransactionsByUsers model)
        {

            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transaction>(@" SELECT t.Id, t.TransactionDate, 
                                            C.Name as Category, 
                                            a.Name as Accounts,
                                            t.Amount,Notes,
                                            c.IdTransactionType 
                                            FROM Transactions T
                                            INNER JOIN Categories c on
                                            c.Id  = t.IdCategory
                                            INNER JOIN Accounts a 
                                            on a.Id = t.IdAccount
                                            where t.IdUser = @IdUser
                                            and t.TransactionDate between @DateStart and @DateEnd
                                                ORDER  BY t.TransactionDate desc", new { model.IdUser, model.DateStart, model.DateEnd });
        }

        public async Task<IEnumerable<ResultsGetByWeek>> GetByWeek(ParametersTransactionsByUsers model)
        {

            using var connections = new SqlConnection(connectionString);

            return await connections.QueryAsync<ResultsGetByWeek>(@"SELECT Datediff(d, @DateStart, TransactionDate)/7+1 as Week, 
                                sum(Amount) as Amount,
                                IdTransactionType from
                                TRANSACTIONS t
                                inner join Categories cat on cat.Id = t.IdCategory
                                where TransactionDate between @DateStart and @DateEnd and t.IdUser = @IdUser
                                GROUP BY Datediff(d, @DateStart, TransactionDate)/7+1 , cat.IdTransactionType", new { model.IdUser, model.DateStart, model.DateEnd });

        }

        public async Task<IEnumerable<ResultsGetByMonth>> GetByMonths(int idUser, int year)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultsGetByMonth>(@"

                                                                    select Month(TransactionDate) as Month, 
                                                                    SUm(Amount) as Amount, cat.IdTransactionType
                                                                    from
                                                                    TRANSACTIONS t
                                                                    inner join Categories cat on cat.Id = t.IdCategory
                                                                    where t.IdUser = @IdUser and Year(t.TransactionDate) = @Year
                                                                    group by month(TransactionDate),cat.IdTransactionType", new
                                                                                {
                                                                                    idUser,
                                                                                    year
                                                                                });

        }


    }
}
