using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface IAudioRecordService : IScopedService
    {
        Task<BaseDataModel> GetAllAsync();
        Task<BaseDataModel> GetAllPageAsync(int pageIndex, AudioRecord ar);
        Task<BaseDataModel> UpdateAsync(AudioRecord audio);
        BaseDataModel DeleteAsync(int id);
    }
}
