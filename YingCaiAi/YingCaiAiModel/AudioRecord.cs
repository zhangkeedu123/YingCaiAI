using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiModel
{
    public class AudioRecord
    {
        public int? Id {  get; set; }
        public string FileName { get; set; }

        public string FileUrl { get { return "http://113.105.116.171:8000/uploads/audio/" + FileName; } set{ } }

        /// <summary>
        /// 识别文本
        /// </summary>
        public string TranScript {  get; set; }

        /// <summary>
        /// 情感正面/反面
        /// </summary>
        public string Sentiment { get; set; }

        /// <summary>
        /// 情绪识别
        /// </summary>
        public string Emotion {  get; set; }

        /// <summary>
        /// 意图识别
        /// </summary>
        public string  Intent { get; set; }

        /// <summary>
        /// 违规词
        /// </summary>

        public string  ViolationTag { get; set; }

        public int? UserId  { get; set; }

        public  string  UserName { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
