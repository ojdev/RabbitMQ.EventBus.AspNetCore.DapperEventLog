using RabbitMQ.EventBus.AspNetCore.DapperEventLog;
using RabbitMQ.EventBus.AspNetCore.Modules;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RabbitMQEventBusModuleOptionExtensions
    {
        /// <summary>
        /// Butterfly已停止维护
        /// </summary>
        /// <param name="build"></param>
        /// <param name="tracer"></param>
        /// <returns></returns>
        public static RabbitMQEventBusModuleOption AddDapperEventLog(this RabbitMQEventBusModuleOption build, string connectionString, string databaseSuffix)
        {
            build.AddModule(RabbitMQEventBusLogModuleHandler.Handle(connectionString, databaseSuffix));
            return build;
        }
    }
}

