using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;
using YingCaiAiService.IService;

namespace YingCaiAiService.Service
{
    public class CustomerService : BaseScopedService<Customer>, ICustomerService
    {
        public CustomerService(DapperHelper helper) : base(helper)
        {

        }
   
        public BaseDataModel DeleteAsync(int id)
        {
            try
            {
                var data = _dbHelper.ExecuteAsync("delete from customer  where id=@Id", new { Id = id }).Result;
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
                var data =await _dbHelper.QueryAsync<Customer>("SELECT * FROM customer  where status=0 and phone is null order by id desc ");
                return  BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public async Task<BaseDataModel> GetAllNameAsync()
        {
            try
            {
                var data = await _dbHelper.QueryAsync<Customer>("SELECT Name FROM customer  ");
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public async Task<BaseDataModel> GetByIdAsync(int id)
        {
            try
            {
                var data = await _dbHelper.QueryFirstOrDefaultAsync<Customer>($"SELECT * FROM customer  where id={id}  ");
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public BaseDataModel GetAllPageAsync(int pageIndex, Customer cus)
        {
            try
            {
                string sql = "WHERE 1=1 ";
                var parameters = new DynamicParameters();
                if (cus.Status != null && cus.Status != 0)
                {
                    if (cus.Status == 1)
                    {
                        sql += " and  status =0 ";
                    }
                    else if(cus.Status==2)
                    {
                        sql += " and  status =1 ";
                    }
                    else
                    {
                        sql += " and  status =2 ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(cus.Name))
                {
                    sql += $" and ( name LIKE @Name or job_title LIKE @JobTitle   )";
                    parameters.Add("Name", $"%{cus.Name}%");
                    parameters.Add("JobTitle", $"%{cus.Name}%");
                }

                var data = _dbHelper.QueryPagedAsync<Customer>($"SELECT *  FROM customer\r\n   {sql}    ORDER BY id desc  \r\n    LIMIT @Limit OFFSET @Offset; SELECT COUNT(1) FROM customer {sql}", parameters, pageIndex, 20).Result;

                return BaseDataModel.Instance.OK(data.TotalCount.ToString(), data.Data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }
        public async Task<BaseDataModel> AddListAsync(List<Customer> doc)
        {
            try
            {
                string sql = @"INSERT INTO public.customer(
	                        name, area, co_property, co_size, contacts, phone, job_title, salary,   status,status_name, created_at)
	                        VALUES (@Name, @Area, @CoProperty, @CoSize, @Contacts, @Phone, @JobTitle, @Salary,@Status,@StatusName, @CreatedAt);";
                var data = await _dbHelper.InsertDocumentsAsync(sql, doc);
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public async Task<BaseDataModel> AddAsync(Customer cus)
        {
            try
            {
                string sql = @"INSERT INTO public.customer(
	                        name, area, co_property, co_size, contacts, phone, job_title, salary,  status,status_name, created_at)
	                        VALUES (@Name, @Area, @CoProperty, @CoSize, @Contacts, @Phone, @JobTitle, @Salary, @Status,@StatusName, @CreatedAt) ";
                var data = await _dbHelper.ExecuteAsync(sql, cus);
                return BaseDataModel.Instance.OK("", data);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }

        public async Task<BaseDataModel> UpdateAsync(Customer customer)
        {
            try
            {
                var data = await _dbHelper.ExecuteAsync("update customer set status=@Status, status_name=@StatusName,phone=@Phone, remark=@Remark,intro=@Intro where id=@Id", customer);
                return data > 0 ? BaseDataModel.Instance.OK("") : BaseDataModel.Instance.Error("");
            }
            catch (Exception ex)
            {
                throw new UserServiceException("获取失败", ex);
            }
        }


    }
}
