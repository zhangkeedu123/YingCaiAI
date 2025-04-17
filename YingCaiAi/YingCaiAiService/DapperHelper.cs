
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace YingCaiAiService
{
    public class DapperHelper : IDisposable
    {
        private readonly string _connectionString;
        private NpgsqlConnection _connection;

        public DapperHelper(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new NpgsqlConnection(_connectionString);
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        private async Task OpenConnectionAsync()
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }
        }

        #region Dapper 方法

        /// <summary>
        /// 查询单个实体
        /// </summary>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null)
        {
            try
            {
                await OpenConnectionAsync();
                return await _connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"查询单个实体时出错: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 查询实体列表
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            try
            {
                await OpenConnectionAsync();
                return await _connection.QueryAsync<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"查询实体列表时出错: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 执行SQL命令（增删改）
        /// </summary>
        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            try
            {
                await OpenConnectionAsync();
                return await _connection.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"执行SQL命令时出错: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object parameters = null)
        {
            try
            {
                await OpenConnectionAsync();
                return await _connection.QueryAsync<T>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception($"执行存储过程时出错: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 查询多映射结果（一对多关系）
        /// </summary>
        public async Task<IEnumerable<TReturn>> QueryMultiMappingAsync<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object parameters = null,
            string splitOn = "Id")
        {
            try
            {
                await OpenConnectionAsync();
                return await _connection.QueryAsync(sql, map, parameters, splitOn: splitOn);
            }
            catch (Exception ex)
            {
                throw new Exception($"查询多映射结果时出错: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<(IEnumerable<T> Data, int TotalCount)> QueryPagedAsync<T>(
            string sql,
            object parameters = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                await OpenConnectionAsync();

                // 添加分页参数
                var dynamicParameters = new DynamicParameters(parameters);
                dynamicParameters.Add("PageNumber", pageNumber);
                dynamicParameters.Add("PageSize", pageSize);

                // 假设SQL中已经包含分页逻辑（如PostgreSQL的LIMIT和OFFSET）
                using (var multi = await _connection.QueryMultipleAsync(sql, dynamicParameters))
                {
                    var data = await multi.ReadAsync<T>();
                    var totalCount = await multi.ReadSingleAsync<int>();
                    return (data, totalCount);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"分页查询时出错: {ex.Message}", ex);
            }
        }

        #endregion

        #region 原有方法（保持兼容）

        /// <summary>
        /// 执行非查询SQL语句（增删改）
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string sql, params NpgsqlParameter[] parameters)
        {
            try
            {
                await OpenConnectionAsync();
                using (var command = new NpgsqlCommand(sql, _connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    return await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("执行非查询SQL语句时出错", ex);
            }
        }

        /// <summary>
        /// 执行查询并返回DataTable
        /// </summary>
        public async Task<DataTable> ExecuteDataTableAsync(string sql, params NpgsqlParameter[] parameters)
        {
            try
            {
                await OpenConnectionAsync();
                using (var command = new NpgsqlCommand(sql, _connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("执行查询并返回DataTable时出错", ex);
            }
        }

        // 其他原有方法...

        #endregion

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
