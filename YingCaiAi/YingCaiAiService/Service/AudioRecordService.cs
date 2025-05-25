using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiService.Service
{
    public class AudioRecordService : BaseScopedService<AudioRecord>, IAudioRecordService
    {
        public AudioRecordService(DapperHelper helper) : base(helper)
        {

        }
    

        public BaseDataModel DeleteAsync(int id)
        {
            try
            {
                var data = _dbHelper.ExecuteAsync("delete from audio_record  where id=@Id", new { Id = id }).Result;
                return data > 0 ? BaseDataModel.Instance.OK("") : BaseDataModel.Instance.Error("");
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        /// <summary>
        /// 查询已审核数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserServiceException"></exception>
        public async Task<BaseDataModel> GetAllAsync()
        {
            try
            {
                var data = await _dbHelper.QueryAsync<AudioRecord>("SELECT * FROM audio_record where status=2 ORDER BY id");
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public async Task<BaseDataModel> GetAllPageAsync(int pageIndex, AudioRecord td)
        {
            try
            {
                string sql = " WHERE 1=1 ";
                var parameters = new DynamicParameters();
                if (td.UserId != null && td.UserId != 0)
                {

                    sql += " and  user_id =@UserId ";
                    parameters.Add("UserId", td.UserId);
                }
                if (!string.IsNullOrWhiteSpace(td.ViolationTag))
                {
                    sql += $" and  violation_tag LIKE @ViolationTag  ";
                    parameters.Add("ViolationTag", $"%{td.ViolationTag}%");
                }

                var data = await _dbHelper.QueryPagedAsync<AudioRecord>($"SELECT * FROM audio_record\r\n   {sql}    ORDER BY id desc  \r\n    LIMIT @Limit OFFSET @Offset; SELECT COUNT(1) FROM audio_record {sql}", parameters, pageIndex, 20);

                return BaseDataModel.Instance.OK(data.TotalCount.ToString(), data.Data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }



        public async Task<BaseDataModel> UpdateAsync(AudioRecord audio)
        {
            try
            {
                var data = await _dbHelper.ExecuteAsync($"UPDATE audio_record\r\n\tSET  sentiment=@Sentiment, emotion=@Emotion, intent=@Intent, violation_tag=@ViolationTag where id=@Id ", audio);
                return data > 0 ? BaseDataModel.Instance.OK("") : BaseDataModel.Instance.Error("");
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }
    }
}
