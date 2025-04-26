using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiService.Service
{
    public class KnowledgeBaseService : BaseScopedService<KnowledgeEntry>, IKnowledgeBaseService
    {
        public KnowledgeBaseService(DapperHelper helper) : base(helper)
        {

        }
        public Task<int> AddAsync(KnowledgeEntry user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<KnowledgeEntry>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<KnowledgeEntry> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<KnowledgeEntry>> SearchAsync(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(KnowledgeEntry user)
        {
            throw new NotImplementedException();
        }
    }
}
