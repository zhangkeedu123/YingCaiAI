using System.Collections.Generic;
using YingCaiAiModel;
using YingCaiAiService.IService;
using Dapper;
using Npgsql;
using System.Linq;

namespace YingCaiAiService.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly string _connectionString;
        private List<string> _userPermissions = new List<string>();

        public PermissionService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool HasPermission(string permissionCode)
        {
            if (string.IsNullOrEmpty(permissionCode))
                return true;

            return _userPermissions.Contains(permissionCode);
        }

        public List<string> GetUserPermissions()
        {
            return _userPermissions;
        }

        public void LoadUserPermissions(int userId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // 获取用户角色
                var userRoles = connection.Query<int>(
                    "SELECT role_id FROM user_role WHERE user_id = @UserId",
                    new { UserId = userId }).ToList();

                if (userRoles.Any())
                {
                    // 获取角色权限
                    var permissionCodes = connection.Query<string>(
                        @"SELECT p.code 
                          FROM permission p
                          JOIN role_permission rp ON p.id = rp.permission_id
                          WHERE rp.role_id IN @RoleIds AND p.is_enabled = true",
                        new { RoleIds = userRoles }).ToList();

                    _userPermissions = permissionCodes;
                }
            }
        }
    }
}