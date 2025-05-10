using Dapper;
using Npgsql;
using YingCaiAiModel;
using YingCaiAiService.IService;
namespace YingCaiAiService.Service
{
    public class UsersService : BaseScopedService<Users>, IUsersService
    {
        

        public UsersService(DapperHelper helper) : base(helper)
        {
           
        }

      
        public BaseDataModel GetAllPageAsync(int pageIndex, Users user)
        {
            try
            {
                string sql = "WHERE 1=1 ";
                var parameters = new DynamicParameters();
                if (user.Role != null)
                {

                    sql += " and  role = @Role ";
                    parameters.Add("Role", user.Role ?? 0);
                }
                if (!string.IsNullOrWhiteSpace(user.UserName))
                {
                    sql += $" and  username like @UserName ";
                    parameters.Add("UserName", $"%{user.UserName}%"  );
                }
                var data = _dbHelper.QueryPagedAsync<Users>($"SELECT id, username, password_hash,created_at,role,role_name,is_active FROM users\r\n   {sql}    ORDER BY id\r\n    LIMIT @Limit OFFSET @Offset; SELECT COUNT(1) FROM users {sql}", parameters, pageIndex, 20).Result;

                return BaseDataModel.Instance.OK(data.TotalCount.ToString(), data.Data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public async Task<Users> GetUserByIdAsync(int id)
        {
            try
            {
                
                return await _dbHelper.QueryFirstOrDefaultAsync<Users>(
                    "SELECT * FROM users WHERE id = @Id", new { Id = id });
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"获取ID为{id}的用户失败", ex);
            }
        }

        public BaseDataModel AddUserAsync(Users user)
        {
            try
            {
                var sql = @"
                    INSERT INTO users (username, password_hash,role,role_name,is_active, created_at)
                    VALUES (@UserName, @PasswordHash, @Role,@RoleName,@IsActive,@CreatedAt) ";
               
                int n =  _dbHelper.ExecuteAsync(sql, user).Result;
                return n>0? BaseDataModel.Instance.OK():BaseDataModel.Instance.Error();
            }
            catch (Exception ex)
            {
                return BaseDataModel.Instance.Error("创建异常");
            }
        }

        public BaseDataModel UpdateUserAsync(Users user)
        {
            try
            {
                string psd = "";
                if (!string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                     psd = " password_hash = @PasswordHash, ";
                }
                
                var sql = @$"
                    UPDATE users 
                    SET username = @UserName,{psd}
                    role=@Role,role_name=@RoleName,is_active=@IsActive
                    WHERE id = @Id";
                
                var affectedRows =  _dbHelper.ExecuteAsync(sql, user).Result;
                return affectedRows > 0 ? BaseDataModel.Instance.OK() : BaseDataModel.Instance.Error();
            }
            catch (Exception ex)
            {
                return BaseDataModel.Instance.Error("更新异常");
            }
        }

        public BaseDataModel DeleteUserAsync(int id)
        {
            try
            {
                var affectedRows =  _dbHelper.ExecuteAsync(
                    "DELETE FROM users WHERE id = @Id", new { Id = id }).Result;
                return affectedRows > 0 ? BaseDataModel.Instance.OK() : BaseDataModel.Instance.Error();
            }
            catch (Exception ex)
            {
                return BaseDataModel.Instance.Error("删除异常");
            }
        }

        public async Task<IEnumerable<Users>> SearchUsersAsync(string keyword)
        {
            try
            {
                return await _dbHelper.QueryAsync<Users>(
                    "SELECT * FROM users WHERE username LIKE @Keyword OR email LIKE @Keyword",
                    new { Keyword = $"%{keyword}%" });
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"搜索用户'{keyword}'失败", ex);
            }
        }

  

      
    }

    
}
