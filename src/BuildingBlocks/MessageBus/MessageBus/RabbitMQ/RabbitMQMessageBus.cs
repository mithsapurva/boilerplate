
namespace MessageBus
{
    using Microsoft.Extensions.Configuration;
    using RabbitMQ.Client;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Common;

    /// <summary>
    /// Class for rabbit mq message bus
    /// </summary>
    public class RabbitMQMessageBus : IMessageBus
    {
        /// <summary>
        /// Create connection
        /// </summary>
        /// <returns>Instance for connection</returns>
        public IConnection CreateConnection()
        {
            IConfiguration configuration = GetConfiguration();
            IConnection connection = null;
            RabbitMQOptions rabbitMQOptions = configuration.GetOptions<RabbitMQOptions>("RabbitMQ");          
            
            List<string> nodeList = rabbitMQOptions.Url.Split(new string[] { "," }, StringSplitOptions.None).ToList();

            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                UserName = rabbitMQOptions.UserName,
                Password = rabbitMQOptions.Password,
                AutomaticRecoveryEnabled = true
            };
            AmqpTcpEndpoint nodeEndPoint = new AmqpTcpEndpoint();
            string nodeUri = string.Empty;

            //Create nodes when servers are available
            if (nodeList != null && nodeList.Any())
            {
                IList<AmqpTcpEndpoint> nodeEndPoints = new List<AmqpTcpEndpoint>();
                foreach (var nodeUrl in nodeList)
                {
                    nodeUri = string.Format(CultureInfo.InvariantCulture, "amqp://{0}:{1}@{2}", rabbitMQOptions.UserName, rabbitMQOptions.Password, nodeUrl);
                    nodeEndPoint = new AmqpTcpEndpoint(new Uri(nodeUri));
                    nodeEndPoints.Add(nodeEndPoint);
                }
                connection = connectionFactory.CreateConnection(nodeEndPoints);
            }
            return connection;
        }

        /// <summary>
        /// Method to declare exchange and queue
        /// </summary>
        /// <param name="queueConfig">Queue config</param>
        /// <param name="channel">Channel</param>
        public void DeclareExchangeAndQueue(QueueConfiguration queueConfig, IModel channel)
        {
            if (queueConfig != null && channel != null)
            {
                channel.ExchangeDeclare(queueConfig.ExchangeName, Enum.GetName(typeof(RabbitMQExchangeType), queueConfig.ExchangeType), true);
                channel.BasicQos(0, queueConfig.ChannelPrefetchCount, false);
                channel.QueueDeclare(queueConfig.QueueName, true, false, false, null);
                channel.QueueBind(queueConfig.QueueName, queueConfig.ExchangeName, queueConfig.RoutingKey);
            }
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            return builder.Build();
        }
    }
}
