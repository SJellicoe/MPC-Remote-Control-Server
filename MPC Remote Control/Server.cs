using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace MPC_Remote_Control
{
    class Server
    {
        private IPEndPoint serverEndPoint;
        private TcpListener listener;
        private bool done = false;
        Controller controller = new Controller();

        Server()
        {
            //Create EndPoint for the Server to listen on
            serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23621);
            //Initialize the TcpListener
            listener = new TcpListener(serverEndPoint);
        }

        public void Listen()
        {
            //Begin listening
            listener.Start();

            while (!done)
            {
                //Assign the accepted socket
                Socket client = listener.AcceptSocket();
                //Create thread for communication
                Thread communication = new Thread(() => Communication(client));
                //Start thread
                communication.Start();
            }

            //Stop Listening
            listener.Stop();
        }

        public void Communication(Socket client)
        {
            int bufferLength = 20;
            byte[] buffer = new byte[bufferLength];
            string message = string.Empty;
            ASCIIEncoding encoder = new ASCIIEncoding();
            int messageLength;

            bool exit = false;

            while (!exit)
            {
                try
                {
                    //blocking read will wait until a message is sent
                    client.Receive(buffer);
                    //convert the byte array we received to a string
                    message = encoder.GetString(buffer);

                    //loop to find first occurence of a null character
                    for (messageLength = 0; message[messageLength] != '\0'; messageLength++)
                    {
                    }

                    //remove the null characters from the message
                    message = message.Substring(0, messageLength - 1);

                    if (message.Equals("exit"))
                    {
                        exit = true;
                    }
                    else
                    {
                        controller.ParseMessage(message);
                    }

                }
                catch (Exception e)
                {
                    client.Close();
                    exit = true;
                }
            }
            if (client != null)
            {
                client.Close();
            }

        }

    }
}
