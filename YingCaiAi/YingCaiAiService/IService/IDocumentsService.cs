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
        Task<List<Documents>> GetAllSystemAsync(string fileName = ""); 
        BaseDataModel GetAllPageAsync(int pageIndex, Documents documents);
        Task<Documents> GetByIdAsync(int id);
        Task<BaseDataModel> AddListAsync(List<Documents> doc);
        Task<BaseDataModel> UpdateAsync(Documents dc);
        Task<BaseDataModel> UpdateAsync(int id);
        BaseDataModel DeleteAsync(int id);
        Task<BaseDataModel> SearchAsync(string keyword);

        Task<Documents> GetByFileNameAsync(string fileName);

        BaseDataModel GetSystemPageAsync(int pageIndex, Documents documents);
    }
}
