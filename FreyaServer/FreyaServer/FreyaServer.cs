using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class FreyaServer
{
    private string address;
    private int port;
    private TcpListener listener;
    private List<TcpClient> client_list;
    private List<Thread> threads;

    private void Announcment(string msg) {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private void ExceptionError(Exception msg) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    public FreyaServer(string address, Int32 port) {
        this.address = address;
        this.port = port;
        this.listener = new TcpListener(IPAddress.Parse(address), this.port);
        this.client_list = new List<TcpClient>();
        this.threads = new List<Thread>();
    }

    private void HandleClientConnection() {
        TcpClient client = this.client_list.Last();
    }

    private void RealHost() {
        try
        {
            // Starting
            this.listener.Start();
            this.Announcment($"Server is listening on {this.address}:{this.port}...");

            // Connection Loop
            while (this.listener.Server.IsBound)
            {
                using TcpClient client = this.listener.AcceptTcpClient();
                this.client_list.Add(client);
                Thread t = new Thread(new ThreadStart(this.HandleClientConnection));
                this.threads.Add(t);
                t.Start();
            }
        }
        catch (SocketException e)
        {
            this.ExceptionError(e);
        }
        finally
        {
            this.listener.Stop();
        }
    }

    public void Host() {
        Thread t = new Thread(() => this.RealHost());
        t.Start();
    }

    public void Close() {
        this.listener.Stop();
    }
}
