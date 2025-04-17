using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiService.IService;

namespace YingCaiAiService.Service
{
    public abstract class BaseService<T> : IBaseService, IDisposable where T : class
    {
        protected readonly DapperHelper _dbHelper;

        protected BaseService(DapperHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // 通用CRUD方法可以放在这里
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbHelper.QueryAsync<T>($"SELECT * FROM {typeof(T).Name}s");
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<T>(
                $"SELECT * FROM {typeof(T).Name}s WHERE Id = @Id", new { Id = id });
        }

        // 其他通用方法...

        public void Dispose()
        {
            _dbHelper?.Dispose();
        }
    }

    /// <summary>
    /// 单例模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSingletonService<T> : ISingletonService
    {
        protected readonly DapperHelper _dbHelper;

        protected BaseSingletonService(DapperHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // 共享的单例服务方法...
    }

    /// <summary>
    /// 作用域模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseScopedService<T> : IScopedService
    {
        protected readonly DapperHelper _dbHelper;

        protected BaseScopedService(DapperHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // 共享的作用域服务方法...
    }

    /// <summary>
    /// 瞬时模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseTransientService<T> : ITransientService
    {
        protected readonly DapperHelper _dbHelper;

        protected BaseTransientService(DapperHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // 共享的瞬时服务方法...
    }
}
