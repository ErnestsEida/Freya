using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class FreyaClient
{
    private TcpClient client;
    private NetworkStream stream;
    private bool showDebug = true;
    private IPEndPoint endpoint;
    private bool isRunning = false;
    private BufferSerializer bufferSerializer = new BufferSerializer();

    public FreyaClient(IPEndPoint endpoint) {
        this.client = new TcpClient();
        this.endpoint = endpoint;
    }

    public FreyaClient(string address, int port) {
        this.client = new TcpClient();
        this.endpoint = new IPEndPoint(IPAddress.Parse(address), port);
    }

    // Buffer Methods
    public void AddReadCallback(int identifier, BufferSerializer.Callback callback)
    {
        this.bufferSerializer.AddCallback(identifier, callback);
    }

    public void WriteBuffer(int identifier, string data)
    {
        this.bufferSerializer.WriteBuffer(this.client.GetStream(), identifier, data);
    }

    public void SetBufferIdentefierSize(int size_in_bytes)
    {
        this.bufferSerializer.identifier_byte_size = size_in_bytes;
    }

    public void SetBufferDataSize(int size_in_bytes)
    {
        this.bufferSerializer.buffer_size = size_in_bytes;
    }
    ////////////////////////////////////////////////////////////

    public void SetDebug(bool debug_on) { 
        this.showDebug = debug_on;
    }

    private void Debug(Debugger.Color color, string msg) {
        Debugger.Say(showDebug, color, msg);
    }

    public void Connect()
    {
        try
        {
            this.client.Connect(endpoint);

            if (client.Connected)
            {
                this.isRunning = true;
                this.Debug(Debugger.Color.Success, $"Client connected to {this.client.Client.RemoteEndPoint.ToString()}");
                this.stream = client.GetStream();

                Thread connectionThread = new Thread(() => { HandleConnection(); });
                connectionThread.Start();
            }
            else
            {
                this.Debug(Debugger.Color.Error, "Client couldn't connect to the server!");
                throw new Exception("Client couldn't connect to the server!");
            }
        }
        catch (SocketException e) { 
            Debug(Debugger.Color.Error, e.ToString());
        }
    }

    public void Disconnect() {
        this.isRunning = false;
        if (client.Available != 0) this.stream.Close();
        if (client != null) this.client.Close();
    }

    private void HandleConnection()
    {
        try
        {
            while (isRunning) {
                if (this.client.Connected && this.client.Available > 0)
                {
                    this.bufferSerializer.ReadBuffer(this.stream);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }
        catch(Exception e)
        {
                Console.WriteLine("Error?? : " + e.ToString());
        }
    }
}