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
    public class TrainingDataService : BaseScopedService<TrainingData>, ITrainingDataService
    {
        public TrainingDataService(DapperHelper helper) : base(helper)
        {

        }
        public async Task<BaseDataModel> AddListAsync(List<TrainingData> doc)
        {
            try
            {
                string sql = @"INSERT INTO training_data (question, answer, for_lora,status,status_name,created_at)
                         VALUES (@Question, @Answer, @ForLora,@Status,@StatusName, @CreatedAt)";
                var data = await _dbHelper.InsertDocumentsAsync(sql, doc);
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
                var data = _dbHelper.ExecuteAsync("delete from training_data  where id=@Id", new { Id = id }).Result;
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
                var data =await _dbHelper.QueryAsync<TrainingData>("SELECT * FROM training_data ORDER BY id");
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public async Task<BaseDataModel> GetAllPageAsync(int pageIndex, TrainingData td)
        {
            try
            {
                string sql = " WHERE 1=1 ";
                var parameters = new DynamicParameters();
                if (td.Status != null&&td.Status!=0)
                {
                   
                  sql += " and  status =@Status ";
                    parameters.Add("Status", td.Status);
                }
                if (!string.IsNullOrWhiteSpace(td.Question))
                {
                    sql += $" and ( question LIKE @Question or answer LIKE @Question   )";
                    parameters.Add("Question", $"%{td.Question}%");
                }

                var data =await _dbHelper.QueryPagedAsync<TrainingData>($"SELECT * FROM training_data\r\n   {sql}    ORDER BY id desc  \r\n    LIMIT @Limit OFFSET @Offset; SELECT COUNT(1) FROM training_data {sql}", parameters, pageIndex, 20);

                return BaseDataModel.Instance.OK(data.TotalCount.ToString(), data.Data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

     

        public async Task<BaseDataModel> UpdateAsync(int id)
        {
            try
            {
                var data = await _dbHelper.ExecuteAsync("update training_data set status=2, status_name='已审核' where id=@Id", new { Id = id });
                return data > 0 ? BaseDataModel.Instance.OK("") : BaseDataModel.Instance.Error("");
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }
    }
}
