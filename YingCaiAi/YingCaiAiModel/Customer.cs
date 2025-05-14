using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public  class Customer
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }

        /// <summary>
        /// 公司性质
        /// </summary>
        public string CoProperty { get; set; }

        /// <summary>
        /// 公司规模
        /// </summary>
        public string CoSize { get; set; }


        /// <summary>
        /// 联系人
        /// </summary>
        public string Contacts { get; set; }

        /// <summary>
        /// 电话手机
        /// </summary>

        public string Phone { get; set; }

        /// <summary>
        /// 职位标题
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// 薪资
        /// </summary>
        public string Salary { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>

        public string Intro { get; set; }

        /// <summary>
        /// 备注
        /// </summary>

        public string Remark { get; set; }

        /// <summary>
        /// 是否标记 0未联系 ，1已联系，2 联系不上，
        /// </summary>

        public int? Status { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
