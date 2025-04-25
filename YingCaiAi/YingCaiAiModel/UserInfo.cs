using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string  Email { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// 是否可用  ture 是 false 否
        /// </summary>
        public bool IsActive { get; set; } = true;

        public int CreaterId { get; set; }
        public int UpdaterId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
      
        
        public DateTime LastLogin { get; set; }
        
    }
}
