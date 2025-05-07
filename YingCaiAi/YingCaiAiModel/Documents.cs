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
    public class Documents
    {
        public int Id { get; set; }
        public string Filename { get; set; } = "";
        public string Content { get; set; } = "";
        public int? Status { get; set; } 

        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
