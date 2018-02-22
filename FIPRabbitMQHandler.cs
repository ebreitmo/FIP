using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FIPMessenger
{
    class FIPRabbitMQHandler
    {
       
        
        private IModel channel_;
        private IConnection connection_;


        public FIPRabbitMQHandler()
        {
            // Do initialisation of RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection_ = factory.CreateConnection();
            channel_ = connection_.CreateModel();

            // Declare queue
            channel_.QueueDeclare(
                    queue: "FIPqTest",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

        }
        // Create queue (queue parameters)


        // Create exchange (exchange parameters)

        // send message(to, message)
        public void sendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            channel_.BasicPublish(exchange: "",
                                 routingKey: "FIPqTest",
                                 basicProperties: null,
                                 body: body);
        }

        // get message(from, )
        public string receiveMessage()
        {
            var consumer = new EventingBasicConsumer(channel_);
            string message = "";
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel_.BasicConsume(queue: "FIPqTest",
                                 autoAck: true,
                                 consumer: consumer);
            return message;
        }
       

    }
}
