using QRCodeGenerator.Creator.Concretes;
//Connection initialize
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("amqps://vbqelytc:MbF26kun1-5hNwzQ9Q8OQ--HckTNQuZu@shark.rmq.cloudamqp.com/vbqelytc");

//Active connection and open channel
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

string QRCodeQueueName = "qr-code-queue";

channel.QueueDeclare(
    queue: QRCodeQueueName,
    durable: true,
    exclusive: false,
    autoDelete: false);

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: QRCodeQueueName,
    autoAck: true,
    consumer: consumer);

consumer.Received += (sender, eventArgs) =>
{
    
    string message = Encoding.UTF8.GetString(eventArgs.Body.Span);
    Console.WriteLine($"QR Code Generated for {message}");
    QRCodeGenerateService generateService = new();
    byte[]qrCode= generateService.GenerateQRCode(message);




    IBasicProperties properties = eventArgs.BasicProperties;
    IBasicProperties replyProperties = channel.CreateBasicProperties();
    replyProperties.CorrelationId = properties.CorrelationId;
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: properties.ReplyTo,
        basicProperties: replyProperties,
        body: qrCode);
    

};

Console.Read();

