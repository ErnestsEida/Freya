using System;

class Program
{
    static void Main()
    {
        string ipAddress = "127.0.0.1";
        int port = 8888;

        FreyaServer server = new FreyaServer(ipAddress, port);
        server.Start();

        server.AddReadCallback(0, (data, socket) => {
            Console.WriteLine("[0] Text Recieved: " + data + ", From: " + socket.RemoteEndPoint);
            server.WriteBuffer(0, "Message Recieved!");
        });

        server.AddReadCallback(1, (data, socket) => {
            Console.WriteLine("[1] Text Recieved: " + data + ", From: " + socket.RemoteEndPoint);
        });

        Console.ReadLine();
        server.Shutdown();
    }
}
