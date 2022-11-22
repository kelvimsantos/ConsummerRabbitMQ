using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "filaTeste",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    //try
                    //{ //tratativa
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                        //Processou
                        channel.BasicAck(ea.DeliveryTag, false);
                    //}
                   // catch(Exception ex) 
                   // {//Nack-> caso ocorra um problema ele manda o item para a fila novamente
                        channel.BasicNack(ea.DeliveryTag, false, true);
                   // }
                };
                channel.BasicConsume(queue: "filaTeste",
                                     autoAck: true,  //como tem a tratativa pode ser falso
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
               
            }
        }
    }
}
