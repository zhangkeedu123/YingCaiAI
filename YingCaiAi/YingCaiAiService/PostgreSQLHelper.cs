using Npgsql;
using System.Data;

namespace YingCaiAiService
{
    public class PostgreSQLHelper : IDisposable
    {
        private readonly string _connectionString;
        private NpgsqlConnection _connection;

        public PostgreSQLHelper(string connectionString)
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
                // 这里可以记录日志或抛出异常
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

        /// <summary>
        /// 执行查询并返回单个值
        /// </summary>
        public async Task<object> ExecuteScalarAsync(string sql, params NpgsqlParameter[] parameters)
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
                    return await command.ExecuteScalarAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("执行查询并返回单个值时出错", ex);
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        public async Task<bool> ExecuteTransactionAsync(params (string sql, NpgsqlParameter[] parameters)[] commands)
        {
            await OpenConnectionAsync();
            using (var transaction = await _connection.BeginTransactionAsync())
            {
                try
                {
                    foreach (var (sql, parameters) in commands)
                    {
                        using (var command = new NpgsqlCommand(sql, _connection, transaction))
                        {
                            if (parameters != null && parameters.Length > 0)
                            {
                                command.Parameters.AddRange(parameters);
                            }
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        public async Task BulkInsertAsync(string tableName, DataTable dataTable)
        {
            try
            {
                await OpenConnectionAsync();
                using (var writer = await _connection.BeginBinaryImportAsync($"COPY {tableName} FROM STDIN (FORMAT BINARY)"))
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        await writer.StartRowAsync();
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            await writer.WriteAsync(row[i]);
                        }
                    }
                    await writer.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("批量插入数据时出错", ex);
            }
        }

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
