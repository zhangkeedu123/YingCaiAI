using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiService.Service
{
    public class DocumentsService : BaseScopedService<Documents>, IDocumentsService
    {
        public DocumentsService(DapperHelper helper) : base(helper)
        {

        }
        public async Task<BaseDataModel> AddListAsync(List<Documents> doc)
        {
            try
            {
                 string sql = @"INSERT INTO documents (filename, content, status,status_name, created_at)
                         VALUES (@Filename, @Content, @Status,@StatusName, @CreatedAt)";
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
                var data =  _dbHelper.ExecuteAsync("delete from Documents  where id=@Id", new { Id = id }).Result;
                return data > 0 ? BaseDataModel.Instance.OK("") : BaseDataModel.Instance.Error("");
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public BaseDataModel GetAllAsync()
        {
            try
            {
                var data = _dbHelper.QueryAsync<Documents>("SELECT * FROM Documents ORDER BY id").Result;
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public BaseDataModel GetAllPageAsync(int pageIndex,Documents documents )
        {
            try
            {
                string sql = "WHERE 1=1 ";
                
                if (documents.Status != null)
                {
                   
                    sql += " and  status = @Status ";
                }else if ( !string.IsNullOrWhiteSpace(documents.Content))
                {
                    sql +=$" and  content =%{documents.Content}%";
                }
                var parameters = new DynamicParameters();
                parameters.Add("Status", documents.Status ?? 0);
               
                var data = _dbHelper.QueryPagedAsync<Documents>($"SELECT id, filename, content,created_at,status_name,status FROM Documents\r\n   {sql}    ORDER BY id\r\n    LIMIT @Limit OFFSET @Offset; SELECT COUNT(1) FROM Documents {sql}", documents, pageIndex,20).Result;
              
                return BaseDataModel.Instance.OK(data.TotalCount.ToString(), data.Data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public Task<BaseDataModel> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseDataModel> SearchAsync(string keyword)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseDataModel> UpdateAsync(int id)
        {
            try
            {
                var data = await _dbHelper.ExecuteAsync("update Documents set status=1, status_name='已审核' where id=@Id", new { Id=id });
                return data > 0 ? BaseDataModel.Instance.OK("") : BaseDataModel.Instance.Error("");
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }
    }
}
