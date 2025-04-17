using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface IUserInfoService: IScopedService 
    {
        Task<IEnumerable<UserInfo>> GetAllUsersAsync();
        Task<UserInfo> GetUserByIdAsync(int id);
        Task<int> AddUserAsync(UserInfo user);
        Task<bool> UpdateUserAsync(UserInfo user);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<UserInfo>> SearchUsersAsync(string keyword);
    }
}
