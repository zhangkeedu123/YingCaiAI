using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiService.Service
{
    public class UserMemoService : BaseScopedService<UserMemo>, IUserMemoService
    {
        public UserMemoService(DapperHelper helper) : base(helper)
        {

        }
        public async Task<int> AddUserMemoAsync(UserMemo user)
        {
            try
            {
                var sql = "";
                if (user != null && user.Id > 0)
                {
                    sql = @$"
                    UPDATE user_memo 
                    SET content = @Content 
                    WHERE id = @Id";
                }
                else
                {
                     sql = @"
                    INSERT INTO user_memo (created_user, content,created_at,cus_id)
                    VALUES (@CreatedUser, @Content, @CreatedAt,@CusId) ";
                }
             

                int n =await _dbHelper.ExecuteAsync(sql, user);
                return n ;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<UserMemo> GetByUserAsync(string user, int cusId = 0)
        {
            try
            {
                if (cusId == 0) {
                    var data = await _dbHelper.QueryFirstOrDefaultAsync<UserMemo>(
                        "SELECT * FROM user_memo where created_user=@CreatedUser ", new { CreatedUser = user });
                    return data;
                }
                else
                {
                    var data = await _dbHelper.QueryFirstOrDefaultAsync<UserMemo>(
                    "SELECT * FROM user_memo where created_user=@CreatedUser and cus_id=@cusId", new { CreatedUser = user, cusId });
                    return data;
                }
               
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"获取用户失败", ex);
            }
        }
    }
}
