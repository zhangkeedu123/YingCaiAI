using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface ICustomerService : IScopedService
    {
        Task<BaseDataModel> AddListAsync(List<Customer> cus);
        Task<BaseDataModel> GetAllAsync();
        Task<BaseDataModel> AddAsync(Customer cus);
        Task<BaseDataModel> UpdateAsync(Customer cus);
        BaseDataModel DeleteAsync(int id);
        BaseDataModel GetAllPageAsync(int pageIndex, Customer cus);
    }
}
