using Dapper;
using RabbitMQ.EventBus.AspNetCore.Modules;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace RabbitMQ.EventBus.AspNetCore.DapperEventLog
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitMQEventBusLogModuleHandler : IModuleHandle
    {
        private static RabbitMQEventBusLogModuleHandler _moduleHandler;
        private string _connectionString { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static RabbitMQEventBusLogModuleHandler Handle(string connectionString, string suffix = null)
        {
            if (_moduleHandler == null)
            {
                using (var db = new SqlConnection(connectionString))
                {
                    SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder(connectionString);
                    var oldDb = stringBuilder.InitialCatalog;
                    var newDb = oldDb + "." + suffix;
                    stringBuilder.InitialCatalog = "master";
                    db.ConnectionString = stringBuilder.ConnectionString;
                    db.Open();
                    db.Execute(@"
If(db_id(N'" + newDb + @"') IS NULL)
 CREATE DATABASE[" + newDb + @"]
");
                    db.ChangeDatabase(newDb);
                    string initDb = @"
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF OBJECT_ID(N'Publish', N'U') IS NULL
    CREATE TABLE[dbo].[Publish] (
        [Id][uniqueidentifier] NOT NULL,
        [Endpoint] [nvarchar] (max) NOT NULL,
		[Exchange] [nvarchar] (max) NOT NULL,
		[Queue] [nvarchar] (max) NOT NULL,
		[RoutingKey] [nvarchar] (max) NOT NULL,
		[ExchangeType] [nvarchar] (max) NOT NULL,
		[ClientProvidedName] [nvarchar] (max) NOT NULL,
		[Message] [nvarchar] (max) NOT NULL,
		[Success] [bit] NOT NULL,
        [CreationTime] [datetimeoffset] (7) NOT NULL
	) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]
IF OBJECT_ID(N'Receive', N'U') IS NULL
    CREATE TABLE[dbo].[Receive](
        [Id][uniqueidentifier] NOT NULL,
        [Endpoint] [nvarchar] (max) NOT NULL,
		[Exchange] [nvarchar] (max) NOT NULL,
		[Queue] [nvarchar] (max) NOT NULL,
		[RoutingKey] [nvarchar] (max) NOT NULL,
		[ExchangeType] [nvarchar] (max) NOT NULL,
		[ClientProvidedName] [nvarchar] (max) NOT NULL,
		[Message] [nvarchar] (max) NOT NULL,
		[Success] [bit] NOT NULL,
        [CreationTime] [datetimeoffset] (7) NOT NULL
	) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]
";
                    db.Execute(initDb);
                    stringBuilder.InitialCatalog = newDb;
                    connectionString = stringBuilder.ConnectionString;
                }
                _moduleHandler = new RabbitMQEventBusLogModuleHandler(connectionString);
            }
            return _moduleHandler;
        }
        private RabbitMQEventBusLogModuleHandler(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task PublishEvent(EventBusArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var db = new SqlConnection(_connectionString))
                    {
                        db.BulkInsert<Publish>(new Publish
                        {
                            ClientProvidedName = e.ClientProvidedName,
                            Endpoint = e.Endpoint,
                            Exchange = e.Exchange,
                            ExchangeType = e.ExchangeType,
                            Id = Guid.NewGuid(),
                            Message = e.Message,
                            Queue = e.Queue,
                            RoutingKey = e.RoutingKey,
                            Success = e.Success
                        });
                    }
                }
                catch
                {
                }
                return Task.CompletedTask;
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task SubscribeEvent(EventBusArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var db = new SqlConnection(_connectionString))
                    {
                        db.BulkInsert<Receive>(new Receive
                        {
                            ClientProvidedName = e.ClientProvidedName,
                            Endpoint = e.Endpoint,
                            Exchange = e.Exchange,
                            ExchangeType = e.ExchangeType,
                            Id = Guid.NewGuid(),
                            Message = e.Message,
                            Queue = e.Queue,
                            RoutingKey = e.RoutingKey,
                            Success = e.Success
                        });
                    }
                }
                catch
                {
                }
                return Task.CompletedTask;
            });
        }
    }
}

