FreyaClient client = new FreyaClient("127.0.0.1", 8080);
client.Connect();

Console.Read();
client.Disconnect();