using ClientProject;
using Newtonsoft.Json;
using TChessP;

namespace ClientGUI
{
    public partial class Form1 : Form
    {
        Client client;
        Button[,] boardButtons = new Button[8, 8];

        public static string ltSqr = "beige";
        public static string dkSqr = "saddlebrown";
        public static string hiLtSqr = "moccasin";
        public static string hiDkSqr = "steelblue";
        public Form1()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(100, 100); 
            InitializeComponent();
            CreateChessBoard();

        }



        private void CreateChessBoard()
        {
            int tileSize = 50;
            int startX = 20;
            int startY = 10;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(tileSize, tileSize);
                    btn.Location = new Point(startX + col * tileSize, startY + row * tileSize);
                    btn.Tag = (row, col);
                    btn.Text = ""; // Start empty



                    btn.Click += SquareButton_Click;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.Black;
                    if ((row + col) % 2 == 0)
                        btn.BackColor = Color.FromName(ltSqr);
                    else
                        btn.BackColor = Color.FromName(dkSqr);

                    Controls.Add(btn);
                    boardButtons[row, col] = btn;
                }
            }
        }


        private void SquareButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                var click = new SquareClick { Row = coords.Item1, Column = coords.Item2 };
                var packet = new Packet
                {
                    ContentType = MessageType.SelectedSquare,
                    Payload = click.JsonSerialized()
                };
                client.SendMessage(packet);
            }
        }


        #region TCProject
        private void btnConnect_Click(object sender, EventArgs e)
        {
            client = new Client(txtHost.Text, int.Parse(txtPort.Text));
            client.RecievePacketMessageEvent += Client_RecievePacketMessageEvent;
            picState.BackColor = Color.CornflowerBlue;
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
            btnSendMessage.Enabled = true;
        }

        private void Client_RecievePacketMessageEvent(TChessP.Packet msg)
        {
            //For now we will assume a text message will come across the wire
            if (msg.ContentType == MessageType.Broadcast)
            {
                Invoke(() => lstMessage.Items.Add(msg.Payload));
            }

            if (msg.ContentType == MessageType.SelectedSquare)
            {
                var square = SquareClick.FromJson(msg.Payload);
                boardButtons[square.Row, square.Column].BackColor = Color.LightGreen;
            }
            else if (msg.ContentType == MessageType.ServerOnly)
            {
                // Check if this is a full board array or just a move
                if (msg.Payload.TrimStart().StartsWith("[["))
                {
                    string[,] board = JsonConvert.DeserializeObject<string[,]>(msg.Payload);

                    Invoke(() =>
                    {
                        for (int row = 0; row < 8; row++)
                        {
                            for (int col = 0; col < 8; col++)
                            {
                                boardButtons[row, col].Text = board[row, col] ?? "";
                            }
                        }
                    });
                }
                else
                {
                    var move = PieceMove.FromJson(msg.Payload);

                    Invoke(() =>
                    {
                        boardButtons[move.FromRow, move.FromCol].Text = "";
                        if (!string.IsNullOrWhiteSpace(move.Piece))
                            boardButtons[move.ToRow, move.ToCol].Text = move.Piece;
                    });
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.SendTextMessage(txtMessage.Text);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Close();
                client.RecievePacketMessageEvent -= Client_RecievePacketMessageEvent;
                client = null;
            }
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            btnSendMessage.Enabled = false;
            picState.BackColor = Color.DarkOrchid;
        }

        private void btnSendFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
                if (ofd.FileName != "")
                {
                    FileTransferObject obj = new FileTransferObject();
                    obj.FileBytes = File.ReadAllBytes(ofd.FileName);
                    //March 27 : add filename on sender side 
                    obj.FileName = ofd.SafeFileName;
                    //wrap the fto into a packet and send it
                    Packet msg = new();
                    msg.Payload = obj.JsonSerialized();
                    msg.ContentType = MessageType.File;
                    //send it
                    client.SendMessage(msg);
                }
        }

        private void btnStressTest_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 500; i++)
            {
                client.SendTextMessage("Message:" + i);

            }
        }
        #endregion
    }
}
