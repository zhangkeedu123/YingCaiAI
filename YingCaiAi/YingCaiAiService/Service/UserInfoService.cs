using Npgsql;
using YingCaiAiModel;
using YingCaiAiService.IService;
namespace YingCaiAiService.Service
{
    public class UserInfoService : BaseScopedService<User>, IUserInfoService
    {
        

        public UserInfoService(DapperHelper helper) : base(helper)
        {
           
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _dbHelper.QueryAsync<User>("SELECT * FROM users ORDER BY id");
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取所有用户失败", ex);
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                
                return await _dbHelper.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM users WHERE id = @Id", new { Id = id });
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"获取ID为{id}的用户失败", ex);
            }
        }

        public async Task<int> AddUserAsync(User user)
        {
            try
            {
                var sql = @"
                    INSERT INTO users (username, email, created_at)
                    VALUES (@Username, @Email, @CreatedAt)
                    RETURNING id";

                return await _dbHelper.ExecuteAsync(sql, user);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("添加用户失败", ex);
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var sql = @"
                    UPDATE users 
                    SET username = @Username, email = @Email
                    WHERE id = @Id";

                var affectedRows = await _dbHelper.ExecuteAsync(sql, user);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"更新用户{user.Id}失败", ex);
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                return true;//测试 要删的
                var affectedRows = await _dbHelper.ExecuteAsync(
                    "DELETE FROM users WHERE id = @Id", new { Id = id });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"删除用户{id}失败", ex);
            }
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string keyword)
        {
            try
            {
                return await _dbHelper.QueryAsync<User>(
                    "SELECT * FROM users WHERE username LIKE @Keyword OR email LIKE @Keyword",
                    new { Keyword = $"%{keyword}%" });
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"搜索用户'{keyword}'失败", ex);
            }
        }

  

        public void Dispose()
        {
            _dbHelper?.Dispose();
        }
    }

    public class UserServiceException : Exception
    {
        public UserServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
