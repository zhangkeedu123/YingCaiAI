using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public string  Email { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// 是否可用  ture 是 false 否
        /// </summary>
        public bool IsActive { get; set; } = true;

        public int CreaterId { get; set; }
        public int UpdaterId { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; } = DateTime.Now;
      
        
        public DateTime LastLoginTime { get; set; }

        public string Token { get; set; }

        [NotMapped]
        public List<Role> Roles { get; set; }

    }
}
