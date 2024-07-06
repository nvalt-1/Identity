using System.Data.Common;
using System.Diagnostics;
using Identity.Services.Interfaces;

namespace Identity.Services;

public class SqlLiteDatabaseService : IDatabaseService
{
    private readonly DbConnection _connection;

    public SqlLiteDatabaseService(DbConnection connection)
    {
        Debug.Assert(connection != null);
        _connection = connection;
        _connection.Open();
    }

    public Task<int> ExecuteProcedure(string storedProcedure, IDictionary<string, object>? parameters = null)
    {
        throw new NotImplementedException("SQLite does not support stored procedures");
    }

    public Task<IList<IDictionary<string, string>>> QueryProcedure(string storedProcedure, IDictionary<string, object>? parameters = null)
    {
        throw new NotImplementedException("SQLite does not support stored procedures");
    }

    public async Task<int> ExecuteCommand(string commandText, IDictionary<string, object>? parameters = null)
    {
        var command = _connection.CreateCommand();
        command.CommandText = commandText;

        if (parameters != null)
        {
            foreach (var (parameterName, value) in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = value;

                command.Parameters.Add(parameter);
            }
        }

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected;
    }

    public async Task<IList<IDictionary<string, string>>> QueryCommand(string commandText, IDictionary<string, object>? parameters = null)
    {
        var command = _connection.CreateCommand();
        command.CommandText = commandText;

        if (parameters != null)
        {
            foreach (var (parameterName, value) in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = value;

                command.Parameters.Add(parameter);
            }
        }

        var rows = new List<IDictionary<string, string>>();
        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, string>();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader[i].ToString() ?? "";
                    row.Add(columnName, value);
                }

                rows.Add(row);
            }
        }

        return rows;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
