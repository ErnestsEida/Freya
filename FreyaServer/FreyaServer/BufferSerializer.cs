using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

class BufferSerializer
{
    public delegate void Callback(string data, Socket socket);

    public int identifier_byte_size = 1;
    public int buffer_size = 512;
    private Dictionary<string, Callback> callbacks = new Dictionary<string, Callback>();

    public void ReadBuffer(NetworkStream stream)
    {
        stream.ReadTimeout = 10;
        byte[] buffer = new byte[buffer_size];
        byte[] identifier_buffer = new byte[identifier_byte_size];

        stream.Read(identifier_buffer, 0, identifier_byte_size);
        stream.Read(buffer, identifier_byte_size, buffer_size - identifier_byte_size);

        string data = Encoding.UTF8.GetString(buffer);
        string identifier = Encoding.UTF8.GetString(identifier_buffer);

        bool result = callbacks.TryGetValue(identifier, out Callback callback);
        if (result) callback.Invoke(data, stream.Socket);
        else Debugger.Say(true, Debugger.Color.Error, $"Callback not found for identifier - {identifier}");
    }

    public void WriteBuffer(NetworkStream stream, int identifier, string data)
    {
        string data_with_id = identifier.ToString() + data;
        byte[] data_buffer = Encoding.ASCII.GetBytes(data_with_id);
        stream.Write(data_buffer, 0, data_buffer.Length);
    }

    public void AddCallback(int identifier, Callback callback)
    {

        this.callbacks[identifier.ToString()] = callback;
    }
}
