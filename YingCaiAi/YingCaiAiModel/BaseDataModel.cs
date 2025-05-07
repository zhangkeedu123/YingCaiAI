using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class BaseDataModel
    {
        public BaseDataModel()
        {
        }
        public string Message { get; set; }

        public object Data { get; set; }

        public bool Status { get; set; }

        public static BaseDataModel Instance
        {
            get { return new BaseDataModel(); }
        }
        public BaseDataModel OK(string message = null, object data = null)
        {
            this.Status = true;
            this.Message = message;
            this.Data = data;
            return this;
        }
        public BaseDataModel OK()
        {
            this.Status = true;
            this.Message = "成功";
            return this;
        }
        public BaseDataModel Error(string message = null)
        {
            this.Status = false;
            this.Message = message;
            return this;
        }
    }
}
