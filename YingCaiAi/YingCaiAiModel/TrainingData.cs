using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class TrainingData
    {
        public int Id { get; set; }
        public string? System { get; set; }
        public string? Instruction { get; set; }
        public string? Output { get; set; }

        public string? Input { get; set; }

        public int? Status { get; set; }

        public string? StatusName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime EditAt { get; set; }

        public bool IsSelected { get; set; } = false;
    }
}
