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
    public class AiRecordService : BaseScopedService<AiRecord>, IAiRecordService
    {
        public AiRecordService(DapperHelper helper) : base(helper)
        {

        }
        public async Task<BaseDataModel> AddAsync(AiRecord doc)
        {
            try
            {
                string sql = @"INSERT INTO ai_record( question, answer, times, created_user, created_at)
                         VALUES (@Question, @Answer, @Times,@CreatedUser, @CreatedAt)";
                var data = await _dbHelper.ExecuteAsync(sql, doc);
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public BaseDataModel DeleteAsync(int id)
        {
            try
            {
                var data = _dbHelper.ExecuteAsync("delete from ai_record  where id=@Id", new { Id = id }).Result;
                return data > 0 ? BaseDataModel.Instance.OK("") : BaseDataModel.Instance.Error("");
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public async Task<BaseDataModel> GetAllAsync()
        {
            try
            {
                var data = await _dbHelper.QueryAsync<AiRecord>("SELECT * FROM ai_record ORDER BY id");
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public async Task<BaseDataModel> GetAllPageAsync(int pageIndex, AiRecord td)
        {
            try
            {
                string sql = " WHERE 1=1 ";
                var parameters = new DynamicParameters();
                if (!string.IsNullOrWhiteSpace(td.CreatedUser) )
                {

                    sql += " and  created_user =@CreatedUser ";
                    parameters.Add("CreatedUser", td.CreatedUser);
                }
                if (!string.IsNullOrWhiteSpace(td.Question))
                {
                    sql += $" and ( question LIKE @Question or answer LIKE @Question   )";
                    parameters.Add("Question", $"%{td.Question}%");
                }

                var data = await _dbHelper.QueryPagedAsync<AiRecord>($"SELECT * FROM ai_record\r\n   {sql}    ORDER BY id desc  \r\n    LIMIT @Limit OFFSET @Offset; SELECT COUNT(1) FROM ai_record {sql}", parameters, pageIndex, 20);

                return BaseDataModel.Instance.OK(data.TotalCount.ToString(), data.Data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }
 
      
    }
}
