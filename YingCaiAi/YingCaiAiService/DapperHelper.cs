
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
        /// �����ݿ�����
        /// </summary>
        private async Task OpenConnectionAsync()
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }
        }

        #region Dapper ����

        /// <summary>
        /// ��ѯ����ʵ��
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
                await OpenConnectionAsync();
                return await _connection.QueryAsync<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"��ѯʵ���б�ʱ����: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ִ��SQL�����ɾ�ģ�
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
                await OpenConnectionAsync();
                return await _connection.QueryAsync<T>(
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
                await OpenConnectionAsync();
                return await _connection.QueryAsync(sql, map, parameters, splitOn: splitOn);
            }
            catch (Exception ex)
            {
                throw new Exception($"��ѯ��ӳ����ʱ����: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ��ҳ��ѯ
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

                // ��ӷ�ҳ����
                var dynamicParameters = new DynamicParameters(parameters);
                dynamicParameters.Add("PageNumber", pageNumber);
                dynamicParameters.Add("PageSize", pageSize);

                // ����SQL���Ѿ�������ҳ�߼�����PostgreSQL��LIMIT��OFFSET��
                using (var multi = await _connection.QueryMultipleAsync(sql, dynamicParameters))
                {
                    var data = await multi.ReadAsync<T>();
                    var totalCount = await multi.ReadSingleAsync<int>();
                    return (data, totalCount);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"��ҳ��ѯʱ����: {ex.Message}", ex);
            }
        }

        #endregion

        #region ԭ�з��������ּ��ݣ�

        /// <summary>
        /// ִ�зǲ�ѯSQL��䣨��ɾ�ģ�
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
                throw new Exception("ִ�зǲ�ѯSQL���ʱ����", ex);
            }
        }

        /// <summary>
        /// ִ�в�ѯ������DataTable
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
                throw new Exception("ִ�в�ѯ������DataTableʱ����", ex);
            }
        }

        // ����ԭ�з���...

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
