using QRCodeGenerator.App.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace QRCodeGenerator.App.Concretes
{
    public class MessageService : IMessageService
    {
        public async Task<byte[]> SendMessage(string message)
        {
            ConnectionFactory factory = new();
            factory.Uri = new("amqps://vbqelytc:MbF26kun1-5hNwzQ9Q8OQ--HckTNQuZu@shark.rmq.cloudamqp.com/vbqelytc");

            //Active connection and open channel
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();
            byte[] responseQrCodeBytes = null;

            string qrQueueName = "qr-code-queue";

            channel.QueueDeclare(
                queue: qrQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            string replyQueueName = channel.QueueDeclare().QueueName;

            string correlationId = Guid.NewGuid().ToString();


            IBasicProperties properties = channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = replyQueueName;

            byte[]requestMessage = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: qrQueueName,
                basicProperties: properties,
                body: requestMessage);
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();
            EventingBasicConsumer consumer =new(channel);

            
            consumer.Received += (sender, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    responseQrCodeBytes = ea.Body.ToArray();
                    tcs.SetResult(responseQrCodeBytes);
                }
            };
            channel.BasicConsume(
                queue: replyQueueName,
                autoAck: true,
                consumer: consumer);

            await Task.Run(() => tcs.Task);
            return responseQrCodeBytes;
        }
    }
}
