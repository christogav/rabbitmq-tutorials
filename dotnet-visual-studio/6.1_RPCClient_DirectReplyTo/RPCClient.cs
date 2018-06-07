using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class RPCClient
{
    private IConnection connection;
    private IModel channel;
    private string replyQueueName;
    private QueueingBasicConsumer consumer;

    public RPCClient()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare("Test", "direct");
        replyQueueName = "amq.rabbitmq.reply-to";
        consumer = new QueueingBasicConsumer(channel);
        channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);
    }

    public string Call(string message)
    {
        var corrId = Guid.NewGuid().ToString();
        var props = channel.CreateBasicProperties();
        props.ReplyTo = replyQueueName;
        props.CorrelationId = corrId;

        var messageBytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "Test", routingKey: "", basicProperties: props, body: messageBytes);

        BasicDeliverEventArgs args;
        if (consumer.Queue.Dequeue(5000, out args))
            return Encoding.UTF8.GetString(args.Body);

        return string.Empty;
    }

    public void Close()
    {
        connection.Close();
    }
}
