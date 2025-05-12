using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class Permission 
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限代码
        /// </summary>
        public string Path { get; set; }

    /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        public int? ParentId { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }

        [NotMapped]
        /// <summary>
        /// 子权限
        /// </summary>
        public List<Permission> Children { get; set; }

    }

}
