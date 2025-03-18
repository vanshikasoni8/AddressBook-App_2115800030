using Microsoft.Extensions.Hosting;
using ModelLayer.DTO;
using ModelLayer.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IConnection _rabbitMqConnection;

    public RabbitMqConsumer(IConnection rabbitMqConnection)
    {
        _rabbitMqConnection = rabbitMqConnection;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _rabbitMqConnection.CreateModel();
        channel.ExchangeDeclare("events", ExchangeType.Topic);
        var queueName = channel.QueueDeclare().QueueName;

        // Bind queues to listen to specific events
        channel.QueueBind(queueName, "events", "user.registered");
        channel.QueueBind(queueName, "events", "contact.added");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;

            if (routingKey == "user.registered")
            {
                var user = JsonSerializer.Deserialize<UserEntity>(message);
                Console.WriteLine($"📩 Sending email to {user.Email}");
            }
            else if (routingKey == "contact.added")
            {
                var contact = JsonSerializer.Deserialize<AddressBookEntity>(message);
                Console.WriteLine($"📖 Contact {contact.Name} added!");
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}