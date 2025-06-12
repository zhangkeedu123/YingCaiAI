using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class UserMemo
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CusId { get; set; }
    }
}
