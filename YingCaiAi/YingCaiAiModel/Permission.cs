using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{

    /// <summary>
    /// 权限表
    /// </summary>
    public class Permission
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Icon { get; set; }
    }

}
