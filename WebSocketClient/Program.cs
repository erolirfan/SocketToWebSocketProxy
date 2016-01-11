﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Microsoft.Owin;

namespace WebSocketClient
{
    public class SynchronousSocketClient
    {

        public static void StartClient()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Send the data through the socket.
                    //int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    var context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                    WebApp.Start("http://localhost:8080");
                    int bytesRec = 0;
                    while ((bytesRec = sender.Receive(bytes)) > 0)
                    {
                        context.Clients.All.addMessage("test", Encoding.UTF8.GetString(bytes));
                    }

                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static int Main(String[] args)
        {

            StartClient();
            

            Console.ReadLine();
            return 0;
        }




        public class MyHub : Hub
        {
            public void Send(string name, string message)
            {
                Clients.All.addMessage(name, message);
            }
        }
    }
}