namespace Organisation.SchedulingLayer
{
    public class ItSystemStatements
    {
        public const string CREATE_TABLE_MSSQL = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='itsystems' AND xtype='U') CREATE TABLE itsystems (
                id			BIGINT NOT NULL PRIMARY KEY IDENTITY(1, 1),
                timestamp		DATETIME2 NOT NULL DEFAULT GETDATE(),
                uuid			NVARCHAR(36) NOT NULL,
                shortkey		NVARCHAR(50),
                jump_url		NVARCHAR(36),
                operation		NVARCHAR(16) NOT NULL,

                CONSTRAINT it_operation_check CHECK (
                    operation IN('UPDATE', 'DELETE')
                )
            );";

        public static string CREATE_TABLE_SQLITE = @"CREATE TABLE IF NOT EXISTS itsystems (
                id			    INTEGER NOT NULL PRIMARY KEY,
                timestamp		DATETIME DEFAULT CURRENT_TIMESTAMP,
                uuid			NVARCHAR(36) NOT NULL,
                shortkey		NVARCHAR(50),
                jump_url		NVARCHAR(36),
                operation		NVARCHAR(16) NOT NULL);";

        public const string INSERT = @"
                insert into itsystems(
                        uuid, 
                        shortkey, 
                        jump_url,
                        operation)
                output INSERTED.ID
                values(
                        @uuid, 
                        @shortkey, 
                        @jump_url, 
                        @operation)";

        public const string SELECT_MSSQL = @"select top(1) * from itsystems order by timestamp";
        public const string SELECT_SQLITE = @"select * from itsystems order by timestamp LIMIT 1";

        public const string DELETE = @"DELETE FROM itsystems where it_system_uuid = @uuid";
    }
}
