namespace Ivtem.DatabaseTools.UnitTests;

public static class SuspiciousSelectTestCases
{
    public static readonly string[] SelectQueries =
    {
        // 1. Direct System Table Access
        /* 0 */  "SELECT * FROM sys.syslogins;",
        /* 1 */  "SELECT name FROM sys.databases;",
        /* 2 */  "SELECT * FROM INFORMATION_SCHEMA.TABLES;",
        /* 3 */  "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME LIKE '%password%';",

        // 2. Extended Stored Procedures and Remote Access
        /* 4 */  "SELECT * FROM OPENQUERY(MyLinkedServer, 'SELECT name FROM sys.databases');",
        /* 5 */  "SELECT * FROM OPENROWSET('SQLNCLI', 'Server=.;Trusted_Connection=yes;', 'SELECT name FROM sys.databases');",
        /* 6 */  "SELECT * FROM OPENDATASOURCE('SQLNCLI', 'Data Source=MyServer').MyDb.dbo.Users;",

        // 3. CTEs Referencing System Tables
        /* 7 */  "WITH logins AS (SELECT name FROM sys.syslogins) SELECT * FROM logins;",
        /* 8 */  "WITH metascan AS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS) SELECT * FROM metascan WHERE COLUMN_NAME = 'Password';",

        // 4. UNION-based Data Exfiltration
        /* 9 */  "SELECT name FROM sys.databases UNION ALL SELECT name FROM master.sys.tables;",
        /* 10 */ "SELECT 'safe' UNION SELECT name FROM sys.syslogins;",

        // 5. Joins on System Views
        /* 11 */ "SELECT u.name, r.name FROM sys.database_principals u JOIN sys.database_role_members r ON u.principal_id = r.member_principal_id;",
        /* 12 */ "SELECT a.name, b.name FROM sysobjects a JOIN syscolumns b ON a.id = b.id WHERE b.name LIKE '%pass%';",

        // 6. CROSS APPLY on Sensitive Sources
        /* 13 */ "SELECT * FROM sys.dm_exec_sessions CROSS APPLY sys.dm_exec_sql_text(most_recent_sql_handle);",
        /* 14 */ "SELECT * FROM OPENQUERY(MyLinkedServer, 'SELECT name FROM sys.tables') CROSS APPLY OPENROWSET('SQLNCLI', '...', 'SELECT ...');",

        // 7. SELECT Referencing EXEC (sp_executesql)
        /* 15 */ "WITH dynamic_exec AS (SELECT * FROM sys.objects WHERE OBJECT_ID = OBJECT_ID('sys.sp_executesql')) SELECT * FROM dynamic_exec;",

        // 8. Obfuscated or Aliased References
        /* 16 */ "SELECT * FROM [sys].[databases];",
        /* 17 */ "SELECT * FROM [INFORMATION_SCHEMA].[TABLES];",
        /* 18 */ "SELECT * FROM sys.[\"logins\"];", // brackets around dangerous name

        // 9. Legitimate Query + System Table Check
        /* 19 */ "SELECT u.Name, u.Email FROM Users u WHERE EXISTS (SELECT * FROM sys.syslogins WHERE name = SYSTEM_USER);",

        // 10 Security context exposure
        /* 20 */ "SELECT SUSER_NAME();",
        /* 21 */ "SELECT ORIGINAL_LOGIN();",
        /* 22 */ "SELECT IS_SRVROLEMEMBER('sysadmin');",
        /* 23 */ "SELECT IS_MEMBER('db_owner');",

        // 11 Metadata discovery
        /* 24 */ "SELECT OBJECT_NAME(object_id) FROM sys.objects;",
        /* 25 */ "SELECT COL_NAME(object_id, column_id) FROM sys.columns;",
        /* 26 */ "SELECT TYPE_NAME(system_type_id) FROM sys.columns;",
        /* 27 */ "SELECT DB_NAME();",
        /* 28 */ "SELECT SCHEMA_NAME(schema_id) FROM sys.tables;",

        // 12 Dynamic SQL helpers
        /* 29 */ "SELECT FORMATMESSAGE('Hello %s', SYSTEM_USER);",

        // 13 Server context / environment
        /* 30 */ "SELECT HOST_NAME();",
        /* 31 */ "SELECT APP_NAME();",

        // 14 Trace and internal use
        /* 32 */ "SELECT * FROM fn_trace_gettable('C:\\trace.trc', 1);"
    };

    public static IEnumerable<TestCaseData> All 
        => SelectQueries.Select((selectQuery, index) => new TestCaseData(selectQuery)
            .SetName($"Suspicious query number {index}"));
}