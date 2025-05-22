using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiWin.Services
{
    public class PaddleOcrService
    {
        private static Process _ocrProcess;
        private StreamWriter _ocrWriter;
        private StreamReader _ocrReader;

        public async Task<bool> StartAsync(string exePath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = Path.GetDirectoryName(exePath),
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,  // <-- 确保这里设置编码
            };

            _ocrProcess = new Process { StartInfo = processStartInfo };
            bool started = _ocrProcess.Start();

            _ocrWriter = _ocrProcess.StandardInput;
            _ocrReader = _ocrProcess.StandardOutput;

            // 等待启动完成标志
            string line;
            while ((line = await _ocrReader.ReadLineAsync()) != null)
            {
                if (line.Contains("OCR init completed."))
                    return true;
                if (_ocrProcess.HasExited)
                    return false;
            }

            return false;
        }

        public async Task<string> RunClipboardAsync()
        {
            if (_ocrWriter == null || _ocrReader == null)
                throw new InvalidOperationException("OCR process not initialized.");

            var cmd = "{\"image_path\": \"clipboard\"}\n";
            await _ocrWriter.WriteLineAsync(cmd);
            await _ocrWriter.FlushAsync();

            string result = await _ocrReader.ReadLineAsync();
            return result;
        }

        public void Stop()
        {
            if (_ocrProcess != null)
            {
                if (!_ocrProcess.HasExited)
                    _ocrProcess.Kill();
            }
            
        }
    }

}
