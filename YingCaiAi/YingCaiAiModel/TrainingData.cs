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
        public string? Question { get; set; }
        public string? Answer { get; set; }
      
        public bool? ForLora { get; set; }
        public int? Status { get; set; }

        public string? StatusName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
