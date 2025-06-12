using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface IUserMemoService : IScopedService
    {
        Task<UserMemo> GetByUserAsync(string user, int cusId = 0);
        Task<int> AddUserMemoAsync(UserMemo user);
    }
}
