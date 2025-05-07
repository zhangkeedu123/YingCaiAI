using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace YingCaiAiWin.Helpers
{
    public class VectorizationScheduler
    {
        private readonly DispatcherTimer _timer;
        private readonly HttpClient _httpClient;

        public VectorizationScheduler()
        {
            _httpClient = new HttpClient();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(1); // 每 5 分钟执行一次
            _timer.Tick += async (s, e) => await CallVectorizeApiAsync();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private async Task CallVectorizeApiAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://113.105.116.171:8000/milvus/approved_documents");
                response.EnsureSuccessStatusCode();
                Console.WriteLine("向量化任务触发成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine("调用失败: " + ex.Message);
            }
        }
    }
}
