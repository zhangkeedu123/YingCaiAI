using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace YingCaiAiWin.Helpers
{

    /// <summary>
    /// 文件相关处理
    /// </summary>
    public class FileHelper
    {
        public List<string> FileReadAll(string filepath)
        {
            string extension = Path.GetExtension(filepath).ToLower();
            List<string> textContent = new List<string>() ;

            switch (extension)
            {
                case ".txt":
                case ".doc":
                case ".docx":
                    textContent = SegmentText(filepath);
                    break;

                case ".xls":
                case ".xlsx":
                    textContent = ReadExcelFile(filepath);
                    break;

                default:
                    break;
            }
            return textContent;
        }



        public static string ReadWordFile(string filePath)
        {
            using var wordDoc = WordprocessingDocument.Open(filePath, false);
            var body = wordDoc.MainDocumentPart.Document.Body;
            return body.InnerText;
        }


        public static List<string> ReadExcelFile(string filePath)
        {
            var result = new List<string>();

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = null;

            if (filePath.EndsWith(".xlsx"))
                workbook = new XSSFWorkbook(fs);
            else if (filePath.EndsWith(".xls"))
                workbook = new HSSFWorkbook(fs);

            var sheet = workbook.GetSheetAt(0); // 默认只取第一个 Sheet

            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++) // 从第1行开始，跳过表头
            {
                var row = sheet.GetRow(rowIndex);
                if (row == null) continue;

                var titleCell = row.GetCell(0);  // 第1列，标题
                var contentCell = row.GetCell(1); // 第2列，内容

                if (titleCell == null || contentCell == null) continue;

                string title = titleCell.ToString().Trim();
                string content = contentCell.ToString().Trim();

                if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(content))
                    continue;

                string fullText = $"标题为 {title}，内容为 {content}";

                // 分段处理，每350字一段
                result.AddRange(SplitText(fullText));
            }

            return result;
        }

        private static List<string> SplitText(string text)
        {
            var segments = new List<string>();

            for (int i = 0; i < text.Length; i += 350)
            {
                var length = Math.Min(350, text.Length - i);
                segments.Add(text.Substring(i, length));
            }

            return segments;
        }


        /// <summary>
        /// 文本分段
        /// </summary>

        public static List<string> SegmentText(string text, int maxLength = 350)
        {
            var segments = new List<string>();
            text = ReadWordFile(text);
            // 先按段落（两个换行符）
            var paragraphs = Regex.Split(text, @"\r?\n\r?\n");

            foreach (var para in paragraphs)
            {
                if (para.Length <= maxLength)
                {
                    segments.Add(para.Trim());
                }
                else
                {
                    // 再按句号、感叹号、问号等
                    var sentences = Regex.Split(para, @"(?<=[。！？])");

                    foreach (var sentence in sentences)
                    {
                        if (string.IsNullOrWhiteSpace(sentence)) continue;

                        if (sentence.Length <= maxLength)
                        {
                            segments.Add(sentence.Trim());
                        }
                        else
                        {
                            // 还超长的话，强制按字符硬切
                            segments.AddRange(SplitText(sentence));
                        }
                    }
                }
            }

            return segments;
        }

      

    }

}
