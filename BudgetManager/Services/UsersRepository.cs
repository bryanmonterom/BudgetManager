using BudgetManager.Models;
using Dapper;
using System.Data.SqlClient;
using System.Security.Claims;

namespace BudgetManager.Services
{
    public interface IRepositoryUsers
    {
        Task<int> CreateUser(User user);
        Task<User> GetUserByEmail(string emailNormalized);
        int GetUserId();
    }
    public class UsersRepository : IRepositoryUsers
    {
        private readonly HttpContext httpContext;
      
        public int GetUserId()
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                throw new ApplicationException("The user is not authenticated");
            }
            else {
                var idClaim = httpContext.User.Claims.
                    Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;

            }
        }

        private readonly string connectionString;
        public UsersRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            httpContext = httpContextAccessor.HttpContext;

        }

        public async Task<int> CreateUser(User user)
        {

            using var connection = new SqlConnection(connectionString);

            var IdUser = await connection.QuerySingleAsync<int>(@"INSERT INTO [dbo].[Users]
                                                           ([Email]
                                                           ,[NormalizedEmail]
                                                           ,[PasswordHash])
                                                     VALUES
                                                           (@Email, @NormalizedEmail, @PasswordHash);
                                                select SCOPE_IDENTITY();
                                            ", new { user.Email, user.NormalizedEmail, user.PasswordHash });

            await connection.ExecuteAsync("CreateDataNewUser", new { IdUser }, commandType: System.Data.CommandType.StoredProcedure);
            return IdUser;
        }

        public async Task<User> GetUserByEmail(string emailNormalized)
        {

            using var connection = new SqlConnection(connectionString);

            return await connection.QuerySingleOrDefaultAsync<User>(@"SELECT * FROM Users  
                                                                where NormalizedEmail = @emailNormalized", new
                {
                    emailNormalized
                });
        }

    }
}
