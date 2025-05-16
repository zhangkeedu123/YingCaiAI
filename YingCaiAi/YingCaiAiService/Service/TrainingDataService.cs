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


        public async Task<BaseDataModel> GetBigDataSum()
        {
            try
            {
                var data = await _dbHelper.QueryAsync<BigDataSum>(@"WITH dates AS (
                                                        SELECT generate_series(
                                                            CURRENT_DATE - INTERVAL '6 days',
                                                            CURRENT_DATE,
                                                            INTERVAL '1 day'
                                                        )::date AS stat_date
                                                    ),
                                                    customer_cumulative AS (
                                                        SELECT
                                                            d.stat_date,
                                                            COUNT(c.id) AS customer_total
                                                        FROM dates d
                                                        LEFT JOIN public.customer c
                                                          ON c.created_at::date <= d.stat_date
                                                        GROUP BY d.stat_date
                                                    ),
                                                    document_cumulative AS (
                                                        SELECT
                                                            d.stat_date,
                                                            COUNT(doc.id) AS document_total
                                                        FROM dates d
                                                        LEFT JOIN public.documents doc
                                                          ON doc.created_at::date <= d.stat_date
                                                        GROUP BY d.stat_date
                                                    ),
                                                    traindata_cumulative AS (
                                                        SELECT
                                                            d.stat_date,
                                                            COUNT(td.id) AS train_total
                                                        FROM dates d
                                                        LEFT JOIN public.training_data td
                                                          ON td.created_at::date <= d.stat_date
                                                        GROUP BY d.stat_date
                                                    )
                                                    SELECT 
                                                        c.stat_date,
                                                        c.customer_total,
                                                        d.document_total,
	                                                    e.train_total
                                                    FROM customer_cumulative c
                                                    JOIN document_cumulative d ON c.stat_date = d.stat_date
                                                    JOIN traindata_cumulative e ON c.stat_date = e.stat_date
                                                    ORDER BY c.stat_date ;", new {  });
                return BaseDataModel.Instance.OK("",data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }


        public async Task<BaseDataModel> GetBigDataToDo()
        {
            try
            {
                var data = await _dbHelper.QueryAsync<BigDataSum>(@"SELECT
                              (SELECT COUNT(id) FROM customer WHERE status = 1) AS customer_total,
                              (SELECT COUNT(id) FROM documents WHERE status = 1) AS document_total,
                              (SELECT COUNT(id) FROM training_data WHERE status = 1) AS train_total,
                              (SELECT COUNT(id) FROM documents WHERE status = 2) AS documents_to_total,
                              (SELECT COUNT(id) FROM training_data WHERE status = 2) AS train_to_total;", new { });
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }
    }
}
