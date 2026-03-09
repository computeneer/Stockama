using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Stockama.Helper;

namespace Stockama.Core.Queue;

public sealed class RabbitMqQueueTransportManager : IQueueTransportManager, IDisposable
{
   private readonly ILogger<RabbitMqQueueTransportManager> _logger;
   private readonly object _syncRoot = new();
   private readonly ConnectionFactory _connectionFactory;

   private IConnection? _connection;
   private IModel? _channel;
   private bool _disposed;

   public RabbitMqQueueTransportManager(ILogger<RabbitMqQueueTransportManager> logger)
   {
      _logger = logger;
      _connectionFactory = new ConnectionFactory
      {
         HostName = EnvironmentVariables.RabbitMqHost,
         Port = int.Parse(EnvironmentVariables.RabbitMqPort),
         UserName = EnvironmentVariables.RabbitMqUser,
         Password = EnvironmentVariables.RabbitMqPassword,
         VirtualHost = EnvironmentVariables.RabbitMqVHost,
         DispatchConsumersAsync = true
      };
   }

   public Task PublishAsync(string queueName, string payload, CancellationToken cancellationToken = default)
   {
      EnsureConnected();
      var channel = _channel!;

      var body = Encoding.UTF8.GetBytes(payload);

      channel.QueueDeclare(
         queue: queueName,
         durable: true,
         exclusive: false,
         autoDelete: false,
         arguments: null);

      var properties = channel.CreateBasicProperties();
      properties.Persistent = true;

      channel.BasicPublish(
         exchange: string.Empty,
         routingKey: queueName,
         basicProperties: properties,
         body: body);

      _logger.LogInformation("RabbitMQ message published. Queue: {QueueName}", queueName);

      return Task.CompletedTask;
   }

   private void EnsureConnected()
   {
      if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
      {
         return;
      }

      lock (_syncRoot)
      {
         if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
         {
            return;
         }

         _channel?.Dispose();
         _connection?.Dispose();

         _connection = _connectionFactory.CreateConnection();
         _channel = _connection.CreateModel();
      }
   }

   public void Dispose()
   {
      if (_disposed)
      {
         return;
      }

      _channel?.Dispose();
      _connection?.Dispose();
      _disposed = true;
   }
}
