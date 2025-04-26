using System.Collections.Generic;
using YingCaiAiModel;

namespace YingCaiAiService.IService
{
    public interface IPermissionService
    {
        /// <summary>
        /// 检查当前用户是否拥有指定权限
        /// </summary>
        /// <param name="permissionCode">权限代码</param>
        /// <returns>是否拥有权限</returns>
        bool HasPermission(string permissionCode);

        /// <summary>
        /// 获取当前用户的所有权限代码
        /// </summary>
        /// <returns>权限代码列表</returns>
        List<string> GetUserPermissions();

        /// <summary>
        /// 加载当前用户的权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        void LoadUserPermissions(int userId);
    }
}