using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string UserName { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string? Email { get; set; }
        public int? Role { get; set; }
        public string?  RoleName { get; set; }

        /// <summary>
        /// 是否可用  ture 是 false 否
        /// </summary>
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }= DateTime.Now;

        public string? PerIds { get; set; }

        [NotMapped]
        public int? IsActiveInt { get; set; }

        public List<Role> Roles { get; set; }
    }

}
