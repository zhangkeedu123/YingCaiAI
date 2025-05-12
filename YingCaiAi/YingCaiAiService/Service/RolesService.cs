using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiService.Service
{
    public class RolesService : BaseScopedService<Role>, IRolesService
    {
        public RolesService(DapperHelper helper) : base(helper)
        {

        }


        public BaseDataModel GetPermAsync(int parentId)
        {
            try
            {
                var  data=  _dbHelper.QueryAsync<Permission>("SELECT * FROM permissions ", new {   }).Result;
                return BaseDataModel.Instance.OK("",data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"获取失败", ex);
            }
        }

        public BaseDataModel GetRoleAsync()
        {
            try
            {
                var data =  _dbHelper.QueryAsync<Role>("SELECT * FROM roles order by id ", new {  }).Result;
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException($"获取失败", ex);
            }
        }

        public BaseDataModel AddRoleAsync(Role role)
        {
            try
            {
                var sql = "";
                if (role.Id!=null&&role.Id>0)
                {
                    sql = @"Update roles set per_ids=@PerIds where id=@Id ";
                }
                else
                {
                    sql = @"
                    INSERT INTO roles (name, description)
                    VALUES (@Name, @Description)";
                   
                }
                int n = _dbHelper.ExecuteAsync(sql, role).Result;
                return n > 0 ? BaseDataModel.Instance.OK() : BaseDataModel.Instance.Error();
            }
            catch (Exception ex)
            {
                return BaseDataModel.Instance.Error("创建异常");
            }
        }

        public BaseDataModel UpdateRoleAsync(Role role)
        {
            try
            {
                var sql = @" update roles set name=@Name, per_ids=@PerId, description=@Description where id=@Id ";
                int n = _dbHelper.ExecuteAsync(sql, role).Result;
                return n > 0 ? BaseDataModel.Instance.OK() : BaseDataModel.Instance.Error();

             
            }
            catch (Exception ex)
            {
                return BaseDataModel.Instance.Error("更新异常");
            }
        }

        public BaseDataModel DeleteRoleAsync(int id)
        {
            try
            {
                var affectedRows = _dbHelper.ExecuteAsync(
                    "DELETE FROM roles WHERE id = @Id", new { Id = id }).Result;
                return affectedRows > 0 ? BaseDataModel.Instance.OK() : BaseDataModel.Instance.Error();
            }
            catch (Exception ex)
            {
                return BaseDataModel.Instance.Error("删除异常");
            }
        }



    }
}
