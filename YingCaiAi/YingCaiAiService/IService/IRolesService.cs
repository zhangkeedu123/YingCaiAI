using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface   IRolesService: IScopedService
    {
        BaseDataModel GetPermAsync(int parentId);
        Task<BaseDataModel> GetRoleAsync();
        BaseDataModel AddRoleAsync(Role role);
        BaseDataModel UpdateRoleAsync(Role role);
        BaseDataModel DeleteRoleAsync(int id);
    }
}
