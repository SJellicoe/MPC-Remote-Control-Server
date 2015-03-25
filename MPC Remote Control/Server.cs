using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MPC_Remote_Control
{
    class Server
    {
        private IPEndPoint serverEndPoint;
        private TcpListener listener;
        private Controller controller = new Controller();

        /**************************************************
         * Function: Server()
         * Description: Constructor to set up the ip for the server to listen on
         * ************************************************/
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

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://glacial-fjord-1021.herokuapp.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string internalIP = myIP.ToString();
                string apiQuery = "register?internalIP=" + internalIP;

                HttpResponseMessage response = client.GetAsync(apiQuery).Result;
            }
        }

        /**************************************************
         * Function: Listen
         * Description: Sets up a listener to accept connections and loops infinitely.
         *              On accept, it spawns a thread to deal with the communication
         * ************************************************/
        public void Listen()
        {
            //Begin listening
            listener.Start();

            while (true)
            {
                //Assign the accepted socket
                Socket client = listener.AcceptSocket();
                //Create thread for communication
                Thread communication = new Thread(() => Communication(client));
                //Start thread
                communication.Start();
            }

        }

        /**************************************************
         * Function: Communication
         * Input: client - Socket for the client that connected to the server
         * Description: Reads the message sent by the client and calls the parser on the controller
         * ************************************************/
        public void Communication(Socket client)
        {
            int bufferLength = 20;
            byte[] buffer = new byte[bufferLength];
            string message = string.Empty;
            ASCIIEncoding encoder = new ASCIIEncoding();
            int messageLength;

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
                message = message.Substring(0, messageLength);

                controller.ParseMessage(message);
            }
            catch (Exception)
            {
                client.Close();
            }
            
            if (client != null)
            {
                client.Close();
            }
        }

    }
}
