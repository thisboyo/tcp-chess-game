using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using TChessP;

namespace ClientProject
{
    public class Client
    {
        private TcpClient client;
        //Used to recieve packets from the server
        public delegate void RecievePacketMessage(Packet mag);
        public event RecievePacketMessage RecievePacketMessageEvent;
        //Make a constructor to specify an ip address and a port
        public Client(string host, int port)
        {
            this.client = new TcpClient(host, port);
            //start recieving messages
            Task.Run(Recieve);
        }
        public async Task<string> Recieve()
        {
            string message = "";
            //set up a newtork stream
            NetworkStream stream = this.client.GetStream();
            while (true)
            {
                byte[] buffer = new byte[4096];
                //How many bytes did we read

                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                var stringMsg = Encoding.UTF8.GetString(buffer);
                message += stringMsg;
                string[] packets = message.Split("!!!EOM!!!");
                message = "";
                foreach (var packet in packets)
                {
                    if (packet.Trim() == "")
                        continue;
                    try
                    {
                        var msg = JsonConvert.DeserializeObject<Packet>(packet);


                        if (RecievePacketMessageEvent != null && msg is not null)
                        {
                            RecievePacketMessageEvent(msg);
                        }
                    }
                    catch
                    {
                        message += stringMsg;
                    }
                }
            }
        }
        public void Close()
        {
            this.client.Close();
        }
        public async Task SendTextMessage(string msg)
        {
            Packet packet = new();
            packet.ContentType = MessageType.Broadcast;
            packet.Payload = msg;
            await SendMessage(packet);

        }
        public async Task SendMessage(Packet mag)
        {
            NetworkStream stream = client.GetStream();

            var tmp = JsonConvert.SerializeObject(mag);
            tmp = tmp + "!!!EOM!!!";
            byte[] buffer = Encoding.UTF8.GetBytes(tmp);

            await stream.WriteAsync(buffer, 0, tmp.Length);
        }
    }
}
