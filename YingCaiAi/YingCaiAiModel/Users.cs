using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{

    /// <summary>
    /// 系统用户表
    /// </summary>
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string? Email { get; set; }
        public string? Role { get; set; }

        /// <summary>
        /// 是否可用  ture 是 false 否
        /// </summary>
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }= DateTime.Now;
    }

}
