using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using TChessP;

namespace ServerProject
{

    public class Swerver
    {
        //A listener is what oopens up the ports to recieve client connections
        private TcpListener listener;
        //maintain a list of all tcpclients that connect
        List<TcpClient> clients = new List<TcpClient>();
        //Indicate if the server is running (listening)
        private bool running = false;
        //Some delegates that can be called and the GUI can later hhook into 
        public delegate void ServerTextMessage(Packet msg);
        public event ServerTextMessage? ServerTextMessageEvent;
        //Another delegate for just the server local messages
        public delegate void LocalMessage(string msg);
        public event LocalMessage? LocalMessageEvent;

        private string[,] board = new string[8, 8];
        private (int Row, int Col)? selectedPiece = null;
        private int connectedPlayers = 0;



        //When we construct the class we want to send ina port
        public Swerver(int port)
        {
            this.listener = new TcpListener(IPAddress.Any, port);
        }

        //start the server

        public async Task Start()
        {
            this.listener.Start();
            InitializeBoard();
            this.running = true;
            //Make a logging event locally
            if (LocalMessageEvent != null)
                LocalMessageEvent($"Server started on {this.listener.LocalEndpoint}");
            while (this.running)
            {
                TcpClient client = await this.listener.AcceptTcpClientAsync();
                clients.Add(client);
                connectedPlayers++;

                if (connectedPlayers > 2)
                {
                    client.Close();
                    connectedPlayers--;
                    return;
                }

                string role = connectedPlayers == 1 ? "White" : "Black";
                string roleTag = role.ToLower();

                string clientInfo = client.Client.RemoteEndPoint?.ToString();
                string serverSideMessage = $"Client has connected from {clientInfo} as ({roleTag}) player";

                // Server GUI logs IP + role
                ServerTextMessageEvent?.Invoke(new Packet
                {
                    ContentType = MessageType.Connected,
                    Payload = serverSideMessage
                });

                // Clients see just the player role
                var connectPacket = new Packet
                {
                    ContentType = MessageType.Connected,
                    Payload = $"{role} player has connected"
                };
                await BroadcastToAllClients(connectPacket);
                // Send boardstate to client
                var boardPacket = new Packet
                {
                    ContentType = MessageType.ServerOnly,
                    Payload = JsonConvert.SerializeObject(board)
                };
                await SendToClient(client, boardPacket);

                // Continue handling this client's communication
                Task.Run(() => HandleClient(client));
            }


        }


        private void InitializeBoard()
        {
            // 1 = pawn, 2 = knight, 3 = bishop, 4 = rook, 5 = queen, 6 = king
            // Add 10 to make them black: 11 = black pawn

            // White pieces row 6 n 7 are the 1st n second rank
            
            for (int i = 0; i < 8; i++)
            {
                board[6, i] = "1";
                board[7, i] = (i == 1 || i == 6) ? "2" : (i == 2 || i == 5) ? "3" : (i == 0 || i == 7) ? "4" : (i == 3) ? "5" : "6"; // i meant to make a loop for the pawns, hit tab and copilot just made this line for me instaead which perfectly set up my bank rank
            }

            // Black pieces 0 is 8 and 1 is 7
            for (int i = 0; i < 8; i++)
            {
                board[1, i] = "11";
                board[0, i] = (i == 1 || i == 6) ? "12" : (i == 2 || i == 5) ? "13" : (i == 0 || i == 7) ? "14" : (i == 3) ? "15" : "16"; // i meant to make a loop for the pawns, hit tab and copilot just made this line for me instaead which perfectly set up my bank rank
            }
        }


        //Handle each client
        public async Task HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead;

                string message = "";
                //This will loooop while bytes are being read from the stream
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    message += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    try
                    {
                        string[] packets = message.Split("!!!EOM!!!");
                        message = "";
                        foreach (string s in packets)
                        {
                            if (s.Trim() == "")
                                continue;
                            try
                            {
                                var tmpMsg = JsonConvert.DeserializeObject<Packet>(s);
                                if (tmpMsg.ContentType == MessageType.Broadcast)
                                {
                                    tmpMsg.Payload = client.Client.RemoteEndPoint + ": " + tmpMsg.Payload;
                                    //send to all clients, so that everyone can get a copy
                                    BroadcastToAllClients(tmpMsg);
                                    ServerTextMessageEvent?.Invoke(tmpMsg);

                                }
                                else if (tmpMsg.ContentType == MessageType.File)
                                {
                                    ServerTextMessageEvent?.Invoke(tmpMsg);
                                }




                                //Board Click Starts Here
                                else if (tmpMsg.ContentType == MessageType.SelectedSquare)
                                {
                                    var square = SquareClick.FromJson(tmpMsg.Payload);

                                    if (selectedPiece == null)
                                    {
                                        // First click: select a piece if there's one
                                        if (!string.IsNullOrEmpty(board[square.Row, square.Column]))
                                        {
                                            selectedPiece = (square.Row, square.Column);
                                        }
                                    }
                                    else
                                    {
                                        // Second click: move the piece
                                        var (srcRow, srcCol) = selectedPiece.Value;
                                        board[square.Row, square.Column] = board[srcRow, srcCol];
                                        board[srcRow, srcCol] = null;
                                        selectedPiece = null;

                                        // Create move update packet to broadcast
                                        var move = new PieceMove
                                        {
                                            FromRow = srcRow,
                                            FromCol = srcCol,
                                            ToRow = square.Row,
                                            ToCol = square.Column,
                                            Piece = board[square.Row, square.Column]
                                        };

                                        var movePacket = new Packet
                                        {
                                            ContentType = MessageType.ServerOnly,
                                            Payload = move.JsonSerialized()
                                        };

                                        ServerTextMessageEvent?.Invoke(movePacket);
                                        await BroadcastToAllClients(movePacket);
                                        string moveDescription = $"Moved {move.Piece} from ({srcRow},{srcCol}) to ({square.Row},{square.Column})";

                                        ServerTextMessageEvent?.Invoke(new Packet
                                        {
                                            ContentType = MessageType.ServerOnly,
                                            Payload = moveDescription
                                        });

                                    }
                                }

                            }



                            catch
                            {
                                //assume theres only part of a message and spool bytes back into message
                                message += s;
                            }
                        }
                        //Make the message turn back into a packet 280

                    }
                    catch
                    {

                    }

                }
                //Connection as turned on us, they lost connection, crashed, etc.
                Packet packet = new Packet();
                packet.ContentType = MessageType.Disconnected;
                packet.Payload = $"Client {client.Client.RemoteEndPoint} has disconnected";
                ServerTextMessageEvent?.Invoke(packet);
                BroadcastToAllClients(packet);
                //remove our client from the list of tcp clients
                clients.Remove(client);
            }
            catch
            {
                //Server side, no more connection
                Packet packet = new Packet();
                packet.ContentType = MessageType.Disconnected;
                packet.Payload = $"Client {client.Client.RemoteEndPoint} has disconnected";
                BroadcastToAllClients(packet);
                ServerTextMessageEvent?.Invoke(packet);
                //remove the client here too
                clients.Remove(client);
            }
        }

        //Get a method to broadbcast to all clients that are connected
        private async Task BroadcastToAllClients(Packet message)
        {
            var msg = JsonConvert.SerializeObject(message) + "!!!EOM!!!";
            byte[] broadcast = Encoding.UTF8.GetBytes(msg);
            //Iterate the list of cilents and blast the message to them
            foreach (var client in clients)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(broadcast, 0, broadcast.Length);

                }
                catch
                {
                    if (LocalMessageEvent != null)
                        LocalMessageEvent($"The client {client.Client}");
                }
            }
        }




        private async Task SendToClient(TcpClient client, Packet packet)
        {
            var msg = JsonConvert.SerializeObject(packet) + "!!!EOM!!!";
            var data = Encoding.UTF8.GetBytes(msg);
            await client.GetStream().WriteAsync(data, 0, data.Length);
        }

    }
}
