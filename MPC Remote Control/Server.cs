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
        private Controller controller = new Controller();

        public Server()
        {
            int index = 0;
            bool isValid = false;
            IPAddress myIP = null;
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> iPv4Addresses = new List<IPAddress>();

            for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
            {
                if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    iPv4Addresses.Add(ipHostInfo.AddressList[i]);
                }
            }

            Console.WriteLine("Pick the IP from the list of your IPv4 addresses that corresponds to your wifi/ethernet adapter.");

            for (int i = 0; i < iPv4Addresses.Count; i++)
            {
                Console.WriteLine((i + 1).ToString() + ": " + iPv4Addresses[i].ToString());
            }

            while (!isValid)
            {
                Int32.TryParse(Console.ReadLine(), out index);
                if (index > 0 && index <= iPv4Addresses.Count)
                {
                    isValid = true;
                }
                else
                {
                    Console.WriteLine(Constants.INVALID_IP_CHOICE_ERROR);
                }
            }

            myIP = iPv4Addresses[index - 1];

            //Create EndPoint for the Server to listen on
            serverEndPoint = new IPEndPoint(myIP, 23621);
            //Initialize the TcpListener
            listener = new TcpListener(serverEndPoint);
            Console.Clear();
            //Display the IP address to connect to
            Console.WriteLine("Connect to this IP address: " + serverEndPoint.Address.ToString());
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
                Console.WriteLine("You've Connected!");
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

                    if (message.Equals("Exit"))
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
