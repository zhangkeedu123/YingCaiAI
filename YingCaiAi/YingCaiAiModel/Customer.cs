using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public  class Customer
    {
        public int Id { get; set; }
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

        public string Salary { get; set; }

        public string Intro { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>

        public bool Is_active { get; set; }
    }
}
