using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{


    /// <summary>
    /// 角色表
    /// </summary>
    public class Role
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PerIds { get; set; }
        public List<Permission> Permissions { get; set; } = new List<Permission>();
    }

}
