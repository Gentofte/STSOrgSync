namespace Organisation.SchedulingLayer
{
    public class UserStatements
    {
        public const string CREATE_TABLE_MSSQL = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='users' and xtype='U') CREATE TABLE users (
	            id			BIGINT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	            timestamp		DATETIME2 NOT NULL DEFAULT GETDATE(),
	            user_uuid		NVARCHAR(36) NOT NULL,
	            user_shortkey		NVARCHAR(50),
	            user_id			NVARCHAR(64),
	            user_phone_uuid		NVARCHAR(36),
	            user_phone_shortkey	NVARCHAR(50),
	            user_phone_value	NVARCHAR(64),
	            user_email_uuid		NVARCHAR(36),
	            user_email_shortkey	NVARCHAR(50),
	            user_email_value	NVARCHAR(64),
	            user_location_uuid	NVARCHAR(36),
	            user_location_shortkey	NVARCHAR(50),
	            user_location_value	NVARCHAR(64),
	            position_uuid		NVARCHAR(36),
	            position_shortkey	NVARCHAR(50),
	            position_name		NVARCHAR(64),
	            position_orgunit_uuid	NVARCHAR(36),
	            person_uuid		NVARCHAR(36),
	            person_shortkey		NVARCHAR(50),
	            person_name		NVARCHAR(64),
	            person_cpr		NVARCHAR(11),
	            operation		NVARCHAR(16) NOT NULL

	            CONSTRAINT user_operation_check CHECK (
		            operation IN('UPDATE', 'DELETE')
	            ),
	            CONSTRAINT user_delete_check CHECK (
		            operation = 'DELETE' OR (
			            user_id IS NOT NULL AND
			            position_name IS NOT NULL AND
			            position_orgunit_uuid IS NOT NULL AND
			            person_name IS NOT NULL
		            )
	            ))";

        public const string CREATE_TABLE_SQLITE = @"CREATE TABLE IF NOT EXISTS users (
	            id			INTEGER PRIMARY KEY,
	            timestamp		DATETIME DEFAULT CURRENT_TIMESTAMP,
	            user_uuid		NVARCHAR(36) NOT NULL,
	            user_shortkey		NVARCHAR(50),
	            user_id			NVARCHAR(64),
	            user_phone_uuid		NVARCHAR(36),
	            user_phone_shortkey	NVARCHAR(50),
	            user_phone_value	NVARCHAR(64),
	            user_email_uuid		NVARCHAR(36),
	            user_email_shortkey	NVARCHAR(50),
	            user_email_value	NVARCHAR(64),
	            user_location_uuid	NVARCHAR(36),
	            user_location_shortkey	NVARCHAR(50),
	            user_location_value	NVARCHAR(64),
	            position_uuid		NVARCHAR(36),
	            position_shortkey	NVARCHAR(50),
	            position_name		NVARCHAR(64),
	            position_orgunit_uuid	NVARCHAR(36),
	            person_uuid		NVARCHAR(36),
	            person_shortkey		NVARCHAR(50),
	            person_name		NVARCHAR(64),
	            person_cpr		NVARCHAR(11),
	            operation		NVARCHAR(16) NOT NULL)";

        public const string INSERT = @"insert into users (user_uuid,user_shortkey,user_id,user_phone_uuid,user_phone_shortkey,user_phone_value,user_email_uuid,user_email_shortkey,user_email_value, user_location_uuid, user_location_shortkey, user_location_value, position_uuid, position_shortkey, position_name, position_orgunit_uuid, person_uuid, person_shortkey, person_name,person_cpr,operation) values(@user_uuid, @user_shortkey, @user_id, @user_phone_uuid, @user_phone_shortkey, @user_phone_value, @user_email_uuid, @user_email_shortkey, @user_email_value, @user_location_uuid, @user_location_shortkey, @user_location_value, @position_uuid, @position_shortkey, @position_name, @position_orgunit_uuid, @person_uuid, @person_shortkey,@person_name, @person_cpr,@operation)";

        public const string SELECT_MSSQL = @"select top(1) * from users order by timestamp";
        public const string SELECT_SQLITE = @"select * from users order by timestamp LIMIT 1";

        public const string DELETE = @"DELETE FROM users where user_uuid = @uuid";
    }
}
