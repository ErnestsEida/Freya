using System;

class Program
{
    static void Main()
    {
        string ipAddress = "127.0.0.1";
        int port = 8888;

        FreyaClient client = new FreyaClient(ipAddress, port);
        client.Connect();

        string input;
        input = Console.ReadLine();
        client.WriteBuffer(0, input);

        input = Console.ReadLine();
        client.WriteBuffer(1, input);

        client.Disconnect();
    }
}
