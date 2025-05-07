
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace YingCaiAiService
{
    public class DapperHelper 
    {
        private readonly string _connectionString= "Host=113.105.116.171;Port=5432;Database=yingcaiai;Username=yingcai;Password=123456zk;Timeout=15;CommandTimeout=30;Keepalive=1;";
        

        public DapperHelper()
        {
           
        }

        #region Dapper 方法

        /// <summary>
        /// 查询单个实体
        /// </summary>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
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
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"查询实体列表时出错: {ex.Message}", ex);
            }
        }

        public async Task<int> InsertDocumentsAsync(string sql,object docs)
        {
            

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                await connection.ExecuteAsync(sql, docs, transaction);
                await transaction.CommitAsync();
                return 1;
            }
            catch
            {
                await transaction.RollbackAsync();
                return 0;
            }
        }

        /// <summary>
        /// 执行SQL命令（增删改）
        /// </summary>
        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return await connection.ExecuteAsync(sql, parameters);
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
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(
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
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return await connection.QueryAsync(sql, map, parameters, splitOn: splitOn);
            }
            catch (Exception ex)
            {
                throw new Exception($"查询多映射结果时出错: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<(List<T> Data, int TotalCount)> QueryPagedAsync<T>(
            string sql,
            object parameters = null,
            int pageNumber = 1,
            int Offset = 20)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // 添加分页参数
                var dynamicParameters = new DynamicParameters(parameters);
                dynamicParameters.Add("Limit", 20);
                dynamicParameters.Add("Offset", (pageNumber - 1) * Offset);

                // 假设SQL中已经包含分页逻辑（如PostgreSQL的LIMIT和OFFSET）
                using (var multi = await connection.QueryMultipleAsync(sql, dynamicParameters))
                {
                    var data = await multi.ReadAsync<T>();
                    var totalCount = await multi.ReadSingleAsync<int>();
                    return (data.ToList(), totalCount);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"分页查询时出错: {ex.Message}", ex);
            }
        }

        #endregion

      
     
    }
}
