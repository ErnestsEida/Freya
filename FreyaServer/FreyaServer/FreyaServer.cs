using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

struct WriteData
{
    public int id;
    public string data;

    public WriteData(int id, string data)
    {
        this.id = id;
        this.data = data;
    }
}

class ServerSettings
{
    public bool debug = true;
    public IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8888);

    public ServerSettings() { }
}

class FreyaServer
{
    private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>();
    private List<Thread> threads = new List<Thread>();
    private bool hostOpen = false;
    private BufferSerializer bufferSerializer = new BufferSerializer();

    public ServerSettings serverSettings = new ServerSettings();

    public FreyaServer(string ipAddress, int port)
    {
        this.listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        this.serverSettings.endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    }

    public FreyaServer(IPEndPoint endpoint)
    {
        this.listener = new TcpListener(endpoint);
        this.serverSettings.endpoint = endpoint;
    }

    // Buffer Methods
    public void AddReadCallback(int identifier, BufferSerializer.Callback callback)
    {
        this.bufferSerializer.AddCallback(identifier, callback);
    }

    public void WriteBuffer(int identifier, string data)
    {
        foreach (TcpClient client in clients) {
            NetworkStream stream = client.GetStream();
            this.bufferSerializer.WriteBuffer(stream, identifier, data);
        }
    }

    public void WriteBufferDirect(Socket socket, int identifier, string data) {
        this.bufferSerializer.WriteBuffer(new NetworkStream(socket), identifier, data);
    }

    public void SetBufferIdentifierSize(int sizeInBytes)
    {
        this.bufferSerializer.identifier_byte_size = sizeInBytes;
    }

    public void SetBufferDataSize(int sizeInBytes)
    {
        this.bufferSerializer.buffer_size = sizeInBytes;
    }
    ////////////////////////////////////////////////////////////

    public void Start()
    {
        try
        {
            listener.Start();
            this.hostOpen = true;

            Thread listenerThread = new Thread(new ThreadStart(ListenForConnections));
            listenerThread.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    public void Shutdown()
    {
        // Close client streams & connections
        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        foreach (TcpClient client in clients)
        {
            if (client.Available != 0) client.GetStream().Close();
            if (client != null) client.Close();
        }

        Debug(Debugger.Color.Info, "Server is shutting down...");
        listener.Stop();
    }

    private void Debug(Debugger.Color color, string message)
    {
        Debugger.Say(this.serverSettings.debug, color, message);
    }

    private void ListenForConnections()
    {
        Debug(Debugger.Color.Info, $"Server listening on {this.serverSettings.endpoint.ToString()}");
        try
        {
            while (this.hostOpen)
            {
                TcpClient client = listener.AcceptTcpClient();
                var clientThread = new Thread(() => HandleClient(client));
                clients.Add(client); // Add client to the list
                threads.Add(clientThread); // Add client thread to the list
                clientThread.Start();
            }
        }
        catch (SocketException ex) { }
    }

    private void RemoveClient(TcpClient client)
    {
        clients.Remove(client);
        threads.RemoveAll(t => t.Name == client.Client.RemoteEndPoint.ToString()); // Remove client thread
        client.Close();
    }

    private bool IsClientConnected(TcpClient client)
    {
        try
        {
            if (client.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] buffer = new byte[1];
                if (client.Client.Receive(buffer, SocketFlags.Peek) == 0)
                    return false;
            }

            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private void HandleClient(TcpClient client)
    {
        string clientName = $"Client({client.Client.RemoteEndPoint}) - {clients.Count}";
        Debug(Debugger.Color.Success, $"{clientName} Connected!");

        NetworkStream stream = client.GetStream();
        while (this.hostOpen && IsClientConnected(client))
        {
            if (client.Available > 0)
            {
                try
                {
                    this.bufferSerializer.ReadBuffer(stream);
                }
                catch (Exception e)
                {
                    Debug(Debugger.Color.Info, $"{clientName} disconnected!");
                    RemoveClient(client);
                    break;
                }
            }
        }
        Debug(Debugger.Color.Info, $"{clientName} disconnected!");
        RemoveClient(client);
    }
}
