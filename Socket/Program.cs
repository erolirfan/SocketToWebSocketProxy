using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
public class SynchronousSocketListener
{

    // Incoming data from the client.
    public static string data = null;

    public static void StartListening()
    {
        // Data buffer for incoming data.
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.
        // Dns.GetHostName returns the name of the 
        // host running the application.
        IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP socket.
        Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and 
        // listen for incoming connections.
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.
                Socket handler = listener.Accept();
                data = null;

                // Show the data on the console.
                Console.WriteLine("Text received : {0}", data);
                byte[] msg = null;
                // Echo the data back to the client.
                while (true)
                {
                    data = Guid.NewGuid().ToString();
                    msg = Encoding.ASCII.GetBytes(data);
                    handler.Send(msg);
                    Console.WriteLine("Text sent : {0} {1}", data, DateTime.Now.ToString());
                    
                    System.Threading.Thread.Sleep(1000);
                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.ReadLine();

    }

    public static int Main(String[] args)
    {
        StartListening();
        return 0;
    }
}