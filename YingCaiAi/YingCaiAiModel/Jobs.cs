using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class Jobs
    {
        public int Id { get; set; }

        /// <summary>
        /// 客户id
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// 职位名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 薪水
        /// </summary>
        public string Salary { get; set; }

        public int PeopleNo { get; set; }

        /// <summary>
        /// 职位介绍
        /// </summary>
        public string  Intro { get; set; }
    }
}
