using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Application.Interfaces.Stream;
using RabbitMQ.Client;

namespace Backend.Application.Services.Stream
{
    public class CameraRabbitService : ICameraRabbitService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public CameraRabbitService()
        {

            var factory = new ConnectionFactory()
            {
                HostName = "localhost", // RabbitMQ server address
                Port = 5672,            // Default port
                UserName = "guest",     // Default username
                Password = "guest"      // Default password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

        }



        public async Task SendImagesAsBase64Async(string queueName, string base64Image)
        {
            _channel.QueueDeclare(queue: queueName,
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);
            var body = Encoding.UTF8.GetBytes(base64Image);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);

        }

    }
}