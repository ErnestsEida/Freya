using System;

class Program
{
    static void Main()
    {
        string ipAddress = "127.0.0.1";
        int port = 8888;

        FreyaServer server = new FreyaServer(ipAddress, port);
        server.Start();

        server.AddReadCallback(0, (data) => {
            Console.WriteLine("[0] Text Recieved: " + data);
        });

        server.AddReadCallback(1, (data) => {
            Console.WriteLine("[1] Text Recieved: " + data);
        });

        Console.ReadLine();
        server.Shutdown();
    }
}
