using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.EventBus.AspNetCore.DapperEventLog
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class EventBusBody
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Exchange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RoutingKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExchangeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClientProvidedName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;
    }
}

