using System.Data.Common;

namespace Identity.Services.Interfaces;

public interface IDatabaseService : IDisposable
{
    /// <summary>
    /// Executes a stored procedure. Returns the number of rows affected.
    /// </summary>
    /// <param name="storedProcedure">Name of the stored procedure</param>
    /// <param name="parameters">Optional dictionary containing parameters for the stored procedure</param>
    /// <returns>Returns the number of rows affected.</returns>
    /// <exception cref="DbException">An error occurred while executing the command</exception>
    public Task<int> ExecuteProcedure(string storedProcedure, IDictionary<string, object>? parameters = null);

    /// <summary>
    /// Executes a stored procedure and returns the resulting rows as a list.
    /// </summary>
    /// <param name="storedProcedure">Name of the stored procedure</param>
    /// <param name="parameters">Optional dictionary containing parameters for the stored procedure</param>
    /// <returns>List of rows represented as a dictionary of column names to string values</returns>
    /// <exception cref="DbException">An error occurred while executing the command</exception>
    public Task<IList<IDictionary<string, string>>> QueryProcedure(string storedProcedure, IDictionary<string, object>? parameters = null);

    /// <summary>
    /// Executes the passed command. Returns the number of rows affected.
    /// </summary>
    /// <param name="commandText">SQL command</param>
    /// <param name="parameters">Optional dictionary containing parameters for the command</param>
    /// <returns>Returns the number of rows affected.</returns>
    /// <exception cref="DbException">An error occurred while executing the command</exception>
    public Task<int> ExecuteCommand(string commandText, IDictionary<string, object>? parameters = null);

    /// <summary>
    /// Executes the passed command and returns the resulting rows as a list.
    /// </summary>
    /// <param name="commandText">SQL command</param>
    /// <param name="parameters">Optional dictionary containing parameters for the command</param>
    /// <returns>List of rows represented as a dictionary of column names to string values</returns>
    /// <exception cref="DbException">An error occurred while executing the command</exception>
    /// <exception cref="ArgumentException">An invalid CommandBehavior value</exception>
    public Task<IList<IDictionary<string, string>>> QueryCommand(string commandText, IDictionary<string, object>? parameters = null);
}
