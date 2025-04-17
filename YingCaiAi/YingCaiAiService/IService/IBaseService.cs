using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YingCaiAiService.Service;

namespace YingCaiAiService.IService
{
    public interface IBaseService
    {
        // 这是一个标记接口，用于标识服务类
    }
    // 单例生命周期
    public interface ISingletonService : IBaseService
    {
    
    }

    // 作用域生命周期
    public interface IScopedService : IBaseService
    {
    }

    // 瞬时生命周期
    public interface ITransientService : IBaseService
    {
    }
}
