using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class AiRecord
    {
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }

        public int? Times { get; set; }

        public string? CreatedUser { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
