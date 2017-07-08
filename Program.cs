using System;
using RabbitMQ.Client;

namespace dotnetcore_rmq_playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.VirtualHost = "/";
            factory.HostName = "localhost";

            using(var connection = factory.CreateConnection())
            {
                var exchangeName = "inbox-x";
                var queueName = "worker-q";
                var routingKey = "";

                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                    channel.QueueDeclare(queueName, false, false, false, null);
                    channel.QueueBind(queueName, exchangeName, routingKey, null);

                    var messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello, world!");
                    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

                    var result = channel.BasicGet(queueName, true);
                    if (result == null)
                    {
                        System.Console.WriteLine("Queue is empty");
                    }
                    else
                    {
                        var receviedMessageBodyBytes = result.Body;
                        Console.WriteLine(System.Text.Encoding.UTF8.GetString(receviedMessageBodyBytes));
                    }
                }
            }
        }
    }
}
