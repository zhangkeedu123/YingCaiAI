using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface IUsersService: IScopedService 
    {
      
        Task<Users> GetUserByIdAsync(int id);
        BaseDataModel GetAllPageAsync(int pageIndex, Users user);
        BaseDataModel AddUserAsync(Users user);
        BaseDataModel UpdateUserAsync(Users user);
        BaseDataModel DeleteUserAsync(int id);
        Task<IEnumerable<Users>> SearchUsersAsync(string keyword);
    }
}
