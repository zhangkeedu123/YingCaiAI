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
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using YingCaiAiModel;
using System.Net.Http.Headers;
using System.Net.Http;
using YingCaiAiWin.Models;

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


        public List<TrainingData> GetQAItems(string filePath)
        {

            var data = new List<TrainingData>();
            try
            {
                foreach (var line in File.ReadLines(filePath))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {

                        var person = JsonConvert.DeserializeObject<TrainingData>(line);
                        if (person != null)
                            data.Add(person);

                    }
                }
            }
            catch (Exception)
            {

                
            }
           
            return data;
           
          
           
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

        public async Task<string> UploadAndRecognizeAudio(string filePath)
        {
            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();

            // 添加文件部分
            var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/wav");

            // 这里的 "file" 要和 FastAPI 接口定义一致
            form.Add(fileContent, "file", Path.GetFileName(filePath));

            // 添加其他字段
            form.Add(new StringContent(AppUser.Instance.Id.ToString()), "user_id");
            form.Add(new StringContent(AppUser.Instance.Username), "user_name");

            // 发送 POST 请求
            var response = await client.PostAsync("http://113.105.116.171:8000/whisper/recognize/", form);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }

    }
    public class QAItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
