using System.Diagnostics;
using Identity.Models.Identity;
using Identity.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly IDatabaseService _db;
    private readonly ILogger _logger;

    public IdentityService(IDatabaseService databaseService, ILogger logger)
    {
        _db = databaseService;
        _logger = logger;
    }

    public async Task<ApplicationUser?> FindUserById(string userId)
    {
        Debug.Assert(!string.IsNullOrEmpty(userId));
        try
        {
            const string command =
                """
                    SELECT
                        U.ID,
                        U.USERNAME,
                        U.SECURITY_STAMP,
                        L.HASH AS PASSWORD_HASH
                    FROM USER U
                        LEFT OUTER JOIN LOGIN L 
                            ON L.USER_ID = U.ID AND L.TYPE_ID = 1 -- Password
                    WHERE U.ID = $userId;
                """;
            var parameters = new Dictionary<string, object>()
            {
                { "userId", userId }
            };
            var rows = await _db.QueryCommand(command, parameters);

            if (rows.Count == 0)
            {
                return null;
            }

            var row = rows.First();
            return UserFromRow(row);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<ApplicationUser?> FindActiveUserByUserName(string normalizedUserName)
    {
        Debug.Assert(!string.IsNullOrEmpty(normalizedUserName));
        try
        {
            const string command =
                """
                    SELECT
                        U.ID,
                        U.USERNAME,
                        U.SECURITY_STAMP,
                        L.HASH AS PASSWORD_HASH
                    FROM USER U
                        LEFT OUTER JOIN LOGIN L 
                            ON L.USER_ID = U.ID AND L.TYPE_ID = 1 -- Password
                    WHERE UPPER(U.USERNAME) = $username
                        AND U.IS_ARCHIVED = 0;
                """;
            var parameters = new Dictionary<string, object>()
            {
                { "username", normalizedUserName }
            };
            var rows = await _db.QueryCommand(command, parameters);

            if (rows.Count == 0)
            {
                return null;
            }

            var row = rows.First();
            return UserFromRow(row);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return null;
        }
    }

    public async Task<bool> ArchiveUser(string userId)
    {
        Debug.Assert(!string.IsNullOrEmpty(userId));
        try
        {
            const string command =
                """
                    UPDATE USER
                    SET IS_ARCHIVED = 1
                    WHERE ID = $id;
                """;
            var parameters = new Dictionary<string, object>()
            {
                { "id", userId }
            };

            var rowsAffected = await _db.ExecuteCommand(command, parameters);
            return (rowsAffected == 1);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> CreateUser(ApplicationUser applicationUser)
    {
        if (string.IsNullOrEmpty(applicationUser.UserName) || string.IsNullOrEmpty(applicationUser.PasswordHash))
        {
            return false;
        }

        #if DEBUG
        Debug.Assert(!string.IsNullOrEmpty(applicationUser.NormalizedUserName));
        var foundUser = await FindActiveUserByUserName(applicationUser.NormalizedUserName);
        Debug.Assert(foundUser == null);
        #endif

        try
        {
            const string command =
                """
                    BEGIN TRANSACTION;
                    
                    INSERT INTO USER (USERNAME, SECURITY_STAMP)
                    VALUES ($username, $securityStamp);
                    
                    INSERT INTO LOGIN (USER_ID, TYPE_ID, HASH)
                    VALUES (last_insert_rowid(), 1, $passwordHash);
                    
                    COMMIT;
                """;
            var parameters = new Dictionary<string, object>()
            {
                { "username", applicationUser.UserName },
                { "passwordHash", applicationUser.PasswordHash },
                { "securityStamp", applicationUser.SecurityStamp ?? "" }
            };

            await _db.ExecuteCommand(command, parameters);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateUser(ApplicationUser applicationUser)
    {
        if (string.IsNullOrEmpty(applicationUser.Id) || string.IsNullOrEmpty(applicationUser.UserName))
        {
            return false;
        }

        try
        {
            const string command =
                """
                    BEGIN TRANSACTION;

                    UPDATE USER
                    SET USERNAME = $username,
                        SECURITY_STAMP = $securityStamp,
                        UPDATED_AT = current_timestamp
                    WHERE ID = $userId;
                    
                    UPDATE LOGIN
                    SET HASH = $passwordHash,
                        UPDATED_AT = current_timestamp
                    WHERE USER_ID = $userId
                        AND TYPE_ID = 1;

                    COMMIT;
                """;
            var parameters = new Dictionary<string, object>()
            {
                { "username", applicationUser.UserName },
                { "userId", applicationUser.Id },
                { "securityStamp", applicationUser.SecurityStamp ?? "" },
                { "passwordHash", applicationUser.PasswordHash ?? "" }
            };

            await _db.ExecuteCommand(command, parameters);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return false;
        }
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    private static ApplicationUser UserFromRow(IDictionary<string, string> row)
    {
        return new ApplicationUser
        {
            Id = row["ID"],
            UserName = row["USERNAME"] == "" ? null : row["USERNAME"],
            NormalizedUserName = row["USERNAME"] == "" ? null : row["USERNAME"].ToUpper(),
            PasswordHash = row["PASSWORD_HASH"] == "" ? null : row["PASSWORD_HASH"],
            SecurityStamp = row["SECURITY_STAMP"]
        };
    }
}
