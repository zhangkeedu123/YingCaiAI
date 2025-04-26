using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface IKnowledgeBaseService: IScopedService
    {
        Task<IEnumerable<KnowledgeEntry>> GetAllAsync();
        Task<KnowledgeEntry> GetByIdAsync(int id);
        Task<int> AddAsync(KnowledgeEntry user);
        Task<bool> UpdateAsync(KnowledgeEntry user);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<KnowledgeEntry>> SearchAsync(string keyword);
    }
}
