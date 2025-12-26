using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace MilkAnalyzerTest.DataAccess
{
    public static class Database
    {
        public static string? ConnectionString { get; set; }

        public static async Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new InvalidOperationException("ConnectionString is not set.");

            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            return await cmd.ExecuteNonQueryAsync();
        }

        public static async Task<T?> ExecuteScalarAsync<T>(string sql, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new InvalidOperationException("ConnectionString is not set.");

            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value) return default;
            return (T)result;
        }

        public static async Task<List<T>> QueryAsync<T>(string sql, Func<SqlDataReader, T> projector, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new InvalidOperationException("ConnectionString is not set.");

            var list = new List<T>();
            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null && parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(projector(reader));
            }
            return list;
        }

        // Inserts into dbo.MilkTestResult (master) and dbo.MilkTestResultValues (detail).
        // values: enumerable of (keyName, value, type) where keyName maps to MilkAnalyzerParameters.KeyName
        // Parameter rows must already exist (no auto-creation here).
        // profileId must be provided by caller.
        public static async Task<int> InsertMilkTestResultAsync(string rawLine, IEnumerable<(string name, string value, string type)> values, int profileId)
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new InvalidOperationException("ConnectionString is not set.");

            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var tran = await conn.BeginTransactionAsync();
            try
            {
                int resultId;

                // Insert master record with provided ProfileId
                await using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = (SqlTransaction)tran;
                    cmd.CommandText = "INSERT INTO dbo.MilkTestResult (TimestampUtc, RawLine, ProfileId) VALUES (SYSUTCDATETIME(), @RawLine, @ProfileId); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    cmd.Parameters.Add(new SqlParameter("@RawLine", SqlDbType.NVarChar) { Value = (object?)rawLine ?? DBNull.Value });
                    cmd.Parameters.Add(new SqlParameter("@ProfileId", SqlDbType.Int) { Value = profileId });
                    var idObj = await cmd.ExecuteScalarAsync();
                    resultId = Convert.ToInt32(idObj, CultureInfo.InvariantCulture);
                }

                // Insert detail rows
                if (values != null)
                {
                    foreach (var v in values)
                    {
                        var keyName = v.name ?? string.Empty;
                        // lookup parameter id
                        int? paramId = null;
                        await using (var cmdLookup = conn.CreateCommand())
                        {
                            cmdLookup.Transaction = (SqlTransaction)tran;
                            cmdLookup.CommandText = "SELECT Id FROM dbo.MilkAnalyzerParameters WHERE KeyName = @KeyName";
                            cmdLookup.Parameters.Add(new SqlParameter("@KeyName", SqlDbType.NVarChar, 100) { Value = keyName });
                            var pidObj = await cmdLookup.ExecuteScalarAsync();
                            if (pidObj != null && pidObj != DBNull.Value) paramId = Convert.ToInt32(pidObj, CultureInfo.InvariantCulture);
                        }

                        if (!paramId.HasValue)
                        {
                            // Do not create parameter here. Expect seed data to exist.
                            throw new InvalidOperationException($"Parameter with KeyName '{keyName}' not found in MilkAnalyzerParameters. Seed parameters before inserting results.");
                        }

                        // parse numeric value
                        object dbValue = DBNull.Value;
                        if (!string.IsNullOrWhiteSpace(v.value))
                        {
                            if (double.TryParse(v.value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                            {
                                dbValue = d;
                            }
                            else
                            {
                                // try replace comma with dot
                                var alt = v.value.Replace(',', '.');
                                if (double.TryParse(alt, NumberStyles.Any, CultureInfo.InvariantCulture, out var d2)) dbValue = d2;
                            }
                        }

                        await using var cmdVal = conn.CreateCommand();
                        cmdVal.Transaction = (SqlTransaction)tran;
                        cmdVal.CommandText = "INSERT INTO dbo.MilkTestResultValues (ResultId, ParameterId, Value) VALUES (@ResultId, @ParameterId, @Value);";
                        cmdVal.Parameters.Add(new SqlParameter("@ResultId", SqlDbType.Int) { Value = resultId });
                        cmdVal.Parameters.Add(new SqlParameter("@ParameterId", SqlDbType.Int) { Value = paramId.Value });
                        cmdVal.Parameters.Add(new SqlParameter("@Value", SqlDbType.Float) { Value = dbValue });
                        await cmdVal.ExecuteNonQueryAsync();
                    }
                }

                await tran.CommitAsync();
                return resultId;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        // Idempotently inserts default analyzer parameters.
        public static async Task SeedAnalyzerParametersAsync()
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new InvalidOperationException("ConnectionString is not set.");

            await using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            await using var tran = await conn.BeginTransactionAsync();
            try
            {
                // Default parameters
                var parameters = new (string Name, string KeyName, string Unit, bool IsActive, int SortOrder, double DefaultValue)[]
                {
                    ("Dummy Fat", "dummy_fat", "g/dL", true, 1, 0.0),
                    ("Dummy Protein", "dummy_protein", "g/dL", true, 2, 0.0),
                    ("Dummy Lactose", "dummy_lactose", "g/dL", true, 3, 0.0),
                    ("Dummy SNF", "dummy_snf", "g/dL", true, 4, 0.0)
                };

                foreach (var p in parameters)
                {
                    var exists = await ExecuteScalarAsync<int?>("SELECT Id FROM dbo.MilkAnalyzerParameters WHERE KeyName = @KeyName",
                        new SqlParameter("@KeyName", p.KeyName));

                    if (!exists.HasValue)
                    {
                        // Insert new parameter
                        await ExecuteNonQueryAsync("INSERT INTO dbo.MilkAnalyzerParameters (Name, KeyName, Unit, IsActive, SortOrder) VALUES (@Name, @KeyName, @Unit, @IsActive, @SortOrder)",
                            new SqlParameter("@Name", p.Name),
                            new SqlParameter("@KeyName", p.KeyName),
                            new SqlParameter("@Unit", p.Unit),
                            new SqlParameter("@IsActive", p.IsActive),
                            new SqlParameter("@SortOrder", p.SortOrder));
                    }
                }

                await tran.CommitAsync();
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        // Return latest MilkTestResult Id for profile
        public static async Task<int?> GetLatestResultIdForProfileAsync(int profileId)
        {
            return await ExecuteScalarAsync<int?>("SELECT TOP 1 Id FROM dbo.MilkTestResult WHERE ProfileId = @ProfileId ORDER BY TimestampUtc DESC", new SqlParameter("@ProfileId", SqlDbType.Int) { Value = profileId });
        }

        // Return parameter names and values for a given result id
        public static async Task<List<(string ParameterName, double? Value)>> GetResultValuesByResultIdAsync(int resultId)
        {
            var list = new List<(string ParameterName, double? Value)>();
            var sql = @"SELECT p.Name, v.Value
                        FROM dbo.MilkTestResultValues v
                        INNER JOIN dbo.MilkAnalyzerParameters p ON v.ParameterId = p.Id
                        WHERE v.ResultId = @ResultId
                        ORDER BY p.SortOrder, p.Name";

            var rows = await QueryAsync(sql, reader =>
            {
                var name = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                double? value = null;
                if (!reader.IsDBNull(1)) value = reader.GetDouble(1);
                return (name, value);
            }, new SqlParameter("@ResultId", SqlDbType.Int) { Value = resultId });

            return rows;
        }
    }
}
