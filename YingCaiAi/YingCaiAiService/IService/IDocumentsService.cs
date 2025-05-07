using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface IDocumentsService : IScopedService
    {
        BaseDataModel GetAllAsync(); 
        BaseDataModel GetAllPageAsync(int pageIndex, Documents documents);
        Task<BaseDataModel> GetByIdAsync(int id);
        Task<BaseDataModel> AddListAsync(List<Documents> doc);
        Task<BaseDataModel> UpdateAsync(int  id);
       BaseDataModel DeleteAsync(int id);
        Task<BaseDataModel> SearchAsync(string keyword);
    }
}
