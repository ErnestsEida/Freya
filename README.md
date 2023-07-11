## Freya Networking System
**Freya Networking System** was made in mind to make it easier for C# developers to use its accessable TcpListener and TcpClient classes. ( you can think of it as a wrapper )

System works completely in the background using threads.
Once you start the server or connect to the server with client, both of them will listen for incoming data and allows you to write to the buffer at any time.

Data comes with a prepended identifier, so that both sides know what callback to Invoke upon data receive. Using the identifier, the according callback method is called to process the data.
___

### Server setup
If you want to start server, you have to create an instance of **FreyaServer** and better save it somewhere, where you can call methods from.
```csharp
private FreyaServer node = null;

public void RunServer() {
	node = new FreyaServer("127.0.0.1", 8888);
	node.Start();
}
```
___

### Client setup
Same story with client - don't hide it far away, because you use it for writing to buffer, or to add a new callback, or most important - to close connection.
```csharp
private FreyaClient client = null;

public void StartClient() {
	client = new FreyaClient("127.0.0.1", 8888);
	client.Connect();
}

public void CloseClient() {
	if (client != null) {
		client.Disconnect();
	}
}
```
___
### Buffer writing & reading callbacks
Certainly, most important part of a network connection is so that it can send and receive data.
#### Reading
Lets start with reading, because it's mostly the same on both parts of the network.
Once network detects that Client/Server has sent something and there is data to receive, it starts by splitting the **Identifier** from the start of the buffer and after that it read the reminding length of the buffer to the data variable.
Depending on the identifier, Freya System finds previously defined callback to be Invoked on such Identifier, and runs the callback method and passes sent data + sender Socket in parameters.
Example:
```csharp
class MyServer {
	private FreyaServer node = null;

	private void SetupCallbacks() {
		this.node.AddReadCallback(0, (data, socket) => {
			Console.WriteLine($"Client wrote: {data}");
		});
		...
	}

	public void Start() {
		this.node = new FreyaServer("your-ip-or-hostname", 8888) // (hostname,port)
		this.node.Start();
	}

	public void Stop() {
		if (this.node != null) {
			this.node.Shutdown();
		}
	}
}
```
Same process goes for client, you just pass method to be called on certain read identifier.
**[!]** Keep in mind, that by default "the identifier" size on buffer is 1 byte which gives us only 10 callback possiblities (0 - 9). You can change how much bytes it takes up on the buffer. Just remember that the buffer has its limits too.

#### Writing
Freya does writing pretty similar on both ends, but with little differences.
Client has 1 method available for writing:
```csharp
clientNode.WriteBuffer(int identifier, string data)
``` 
The identifier is prepended to the data and sent to the server.

Server on the other hand has 2 methods for writing:
```csharp
1. serverNode.WriteBuffer(int identifier, string data)
```
```csharp
2. serverNode.WriteBufferDirect(Socket socket, int identifier, string data)
```
1. This method, will send same message with same identifier, to every client that is currently connected to the serverNode.
2. Second method, allows server to send data to only one client by passing its socket directly in arguments of method.

Example of direct writing on server would be that it responds to the sender of the message only:
```csharp
serverNode.AddReadCallback(0, (data, socket) => {
	Console.WriteLine($"Client said: {data})");
	serverNode.WriteBufferDirect(socket, 0, "I recieved the message");
})
```
