﻿using System;
using System.Net;
using System.Net.Sockets;


public class FreyaClient {
    public TcpClient node;
    private string address;
    private int port;

    private void Announcment(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    private void ExceptionError(Exception msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    public FreyaClient(string address, int port) {
        this.address = address;
        this.port = port;
    }

    public void Connect() {
        try
        {
            this.node = new TcpClient(this.address, this.port);
        }
        catch (Exception e)
        {
            this.ExceptionError(e);
        } 
        finally
        {

        }
    }

    public void Disconnect() {
        this.node.Close();
        this.node = null;
    }
}
