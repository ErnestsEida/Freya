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

    private void Announcment(string msg) {
        Console.ForegroundColor = ConsoleColor.Magenta;
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
        this.listener = null;
    }
    
    public void Host() {
        try
        {
            // Setup
            IPAddress ip = IPAddress.Parse(address);
            this.listener = new TcpListener(ip, this.port);

            // Starting
            this.listener.Start();
            this.Announcment($"Server is listening on {this.address}:{this.port}...");

            // Connection Loop
            while (true)
            {
                Socket newConnection = this.listener.AcceptSocket();
                Console.WriteLine("Connection accepted!");
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
}
