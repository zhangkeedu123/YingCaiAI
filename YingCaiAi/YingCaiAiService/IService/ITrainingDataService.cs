using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface ITrainingDataService : IScopedService
    {
        Task<BaseDataModel> GetAllAsync();
        Task<BaseDataModel> GetAllPageAsync(int pageIndex, TrainingData td);
        Task<BaseDataModel> AddListAsync(List<TrainingData> td);
        Task<BaseDataModel> UpdateAsync(int [] ids);
        BaseDataModel DeleteAsync(int [] id);


        //大屏相关


        Task<BaseDataModel> GetBigDataSum();

        Task<BaseDataModel> GetBigDataToDo();
        Task<BaseDataModel> GetAiSumAsync();
     }
}
