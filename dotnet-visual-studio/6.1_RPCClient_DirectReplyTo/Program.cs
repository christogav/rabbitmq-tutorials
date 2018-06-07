using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    public static void Main( string[] args )
    {
        while (true)
        {
            var rpcClient = new RPCClient();

            var n = args.Length > 0 ? args[0] : "30";
            Console.WriteLine(" [x] Requesting fib({0})", n);
            var response = rpcClient.Call(n);
            Console.WriteLine(" [.] Got '{0}'", response);
            rpcClient.Close();

            Console.WriteLine(" Press [enter] to exit, any other key to continue with a new request.");
            if (Console.ReadKey().Key == ConsoleKey.Enter) {
                break;
            }
        }
    }
}
