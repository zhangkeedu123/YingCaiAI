using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    /// <summary>
    /// 知识库
    /// </summary>
    public class KnowledgeEntry
    {
        public int Id { get; set; }
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public string? Source { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
