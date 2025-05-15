using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Cryptography;

namespace YingCaiAiWin.Helpers
{

    /// <summary>
    /// 文件相关处理
    /// </summary>
    public class FileHelper
    {
        public List<QAItem> FileReadAll(string filepath,int flag=1)
        {
            string extension = Path.GetExtension(filepath).ToLower();
            List<QAItem> textContent = new List<QAItem>() ;

            switch (extension)
            {
                case ".txt":
                case ".doc":
                case ".docx":
                    if (flag == 1)
                    {
                        textContent = SegmentText(filepath);
                    }
                    else
                    {
                        textContent = GetQAItems(filepath);
                    }
                    
                    break;

                default:
                    break;
            }
            return textContent;
        }



        public  string ReadWordFile(string filePath)
        {
            using var wordDoc = WordprocessingDocument.Open(filePath, false);
            var body = wordDoc.MainDocumentPart.Document.Body;
            return  string.Join("\n", body.Descendants<Paragraph>().Select(p => p.InnerText));
        }


        /// <summary>
        /// 文本分段
        /// </summary>

        public  List<QAItem> SegmentText(string filepath)
        {
            var qaList = new List<QAItem>();
            var fullText = ReadWordFile(filepath);
            // 每组问答由 #### 分隔
            var groups = fullText.Split("####", StringSplitOptions.RemoveEmptyEntries);

            foreach (var group in groups)
            {
                var lines = group.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(l => l.Trim())
                                 .ToList();

                var questionLine = lines.FirstOrDefault(l => l.StartsWith("标题："));
                var answerLine = lines.FirstOrDefault(l => l.StartsWith("内容："));

                if (!string.IsNullOrWhiteSpace(questionLine) && !string.IsNullOrWhiteSpace(answerLine))
                {
                    qaList.Add(new QAItem
                    {
                        Question = questionLine.Replace("标题：", "").Trim(),
                        Answer = answerLine.Replace("内容：", "").Trim()
                    });
                }
            }
            return qaList;
        }


        public List<QAItem> GetQAItems(string filePath)
        {
            var qaList = new List<QAItem>();
            var fullText = ReadWordFile(filePath);
            // 每组问答由 #### 分隔
            var groups = fullText.Split("####", StringSplitOptions.RemoveEmptyEntries);

            foreach (var group in groups)
            {
                var lines = group.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(l => l.Trim())
                                 .ToList();

                var questionLine = lines.FirstOrDefault(l => l.StartsWith("问："));
                var answerLine = lines.FirstOrDefault(l => l.StartsWith("答："));

                if (!string.IsNullOrWhiteSpace(questionLine) && !string.IsNullOrWhiteSpace(answerLine))
                {
                    qaList.Add(new QAItem
                    {
                        Question = questionLine.Replace("问：", "").Trim(),
                        Answer = answerLine.Replace("答：", "").Trim()
                    });
                }
            }
            return qaList;
        }

        public  string ToMD5(string input)
        {
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2")); // 转为16进制字符串
            }
            return sb.ToString();
        }
    }
    public class QAItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
