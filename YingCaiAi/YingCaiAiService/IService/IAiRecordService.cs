using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface IAiRecordService : IScopedService
    {
        Task<BaseDataModel> GetAllAsync();
        Task<BaseDataModel> GetAllPageAsync(int pageIndex, AiRecord td);
        Task<BaseDataModel> AddAsync(AiRecord td);
        BaseDataModel DeleteAsync(int id);

    }
}
