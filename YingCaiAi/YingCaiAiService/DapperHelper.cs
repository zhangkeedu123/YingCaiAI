
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

        #region Dapper ����

        /// <summary>
        /// ��ѯ����ʵ��
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
                throw new Exception($"��ѯ����ʵ��ʱ����: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ��ѯʵ���б�
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
                throw new Exception($"��ѯʵ���б�ʱ����: {ex.Message}", ex);
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
        /// ִ��SQL�����ɾ�ģ�
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
                throw new Exception($"ִ��SQL����ʱ����: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ִ�д洢����
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
                throw new Exception($"ִ�д洢����ʱ����: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ��ѯ��ӳ������һ�Զ��ϵ��
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
                throw new Exception($"��ѯ��ӳ����ʱ����: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ��ҳ��ѯ
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

                // ��ӷ�ҳ����
                var dynamicParameters = new DynamicParameters(parameters);
                dynamicParameters.Add("Limit", 20);
                dynamicParameters.Add("Offset", (pageNumber - 1) * Offset);

                // ����SQL���Ѿ�������ҳ�߼�����PostgreSQL��LIMIT��OFFSET��
                using (var multi = await connection.QueryMultipleAsync(sql, dynamicParameters))
                {
                    var data = await multi.ReadAsync<T>();
                    var totalCount = await multi.ReadSingleAsync<int>();
                    return (data.ToList(), totalCount);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"��ҳ��ѯʱ����: {ex.Message}", ex);
            }
        }

        #endregion

      
     
    }
}
