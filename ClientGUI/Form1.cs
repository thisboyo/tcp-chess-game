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
        public static bool player;

        Dictionary<string, Image> pieceImages = new Dictionary<string, Image>();

        public Form1()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(100, 100);
            InitializeComponent();
            LoadPieceImages();
        }



        private void CreateChessBoard()
        {
            int tileSize = 50;
            int startX = 20;
            int startY = 10;

            for (int displayRow = 0; displayRow < 8; displayRow++)
            {
                for (int displayCol = 0; displayCol < 8; displayCol++)
                {
                    int logicalRow = player ? displayRow : 7 - displayRow;
                    int logicalCol = player ? displayCol : 7 - displayCol;

                    Button btn = new Button();
                    btn.Size = new Size(tileSize, tileSize);
                    btn.Location = new Point(startX + displayCol * tileSize, startY + displayRow * tileSize);
                    btn.Tag = (logicalRow, logicalCol); // always logical coords for gameplay

                    btn.Click += SquareButton_Click;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.Black;

                    if ((logicalRow + logicalCol) % 2 == 0)
                        btn.BackColor = Color.FromName(ltSqr);
                    else
                        btn.BackColor = Color.FromName(dkSqr);

                    Controls.Add(btn);
                    boardButtons[logicalRow, logicalCol] = btn;
                }
            }
        }

        private void LoadPieceImages()
        {
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            for (int i = 1; i <= 6; i++)
            {
                pieceImages[i.ToString()] = Image.FromFile($"Images/{i}.png");
                pieceImages[(i + 10).ToString()] = Image.FromFile($"Images/{i + 10}.png");
            }
        }
        private Image ResizeImage(Image img, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, width, height);
            }
            return bmp;
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
            if (msg.Payload == "White player has connected")
            {
                player = true;
                Invoke(CreateChessBoard);
            }
            else if (msg.Payload == "Black player has connected")
            {
                player = false;
                Invoke(CreateChessBoard);
            }
                if (msg.ContentType == MessageType.Broadcast)
            {
                Invoke(() => lstMessage.Items.Add(msg.Payload));
            }

            if (msg.ContentType == MessageType.SelectedSquare) //Not Being Called right now
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
                                string piece = board[row, col];
                                boardButtons[row, col].Text = "";
                                boardButtons[row, col].Image = null;

                                if (!string.IsNullOrWhiteSpace(piece) && pieceImages.ContainsKey(piece))
                                {
                                    boardButtons[row, col].Image = ResizeImage(pieceImages[piece], boardButtons[row, col].Width - 10, boardButtons[row, col].Height - 10);
                                    boardButtons[row, col].ImageAlign = ContentAlignment.MiddleCenter;
                                }
                            }
                        }
                    });
                }
                else
                {
                    var move = PieceMove.FromJson(msg.Payload);

                    Invoke(() =>
                    {
                        // Clear source square
                        boardButtons[move.FromRow, move.FromCol].Text = "";
                        boardButtons[move.FromRow, move.FromCol].Image = null;

                        // Set destination square image
                        if (!string.IsNullOrWhiteSpace(move.Piece) && pieceImages.ContainsKey(move.Piece))
                        {
                            boardButtons[move.ToRow, move.ToCol].Text = "";
                            boardButtons[move.ToRow, move.ToCol].Image = ResizeImage(pieceImages[move.Piece],
                             boardButtons[move.ToRow, move.ToCol].Width - 10, boardButtons[move.ToRow, move.ToCol].Height - 10);
                            boardButtons[move.ToRow, move.ToCol].ImageAlign = ContentAlignment.MiddleCenter;
                        }
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



        private void btnStressTest_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 500; i++)
            {
                client.SendTextMessage("Message:" + i);

            }
        }
        #endregion

        public string[,] FlipBoard(string[,] board)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            string[,] flipped = new string[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    flipped[rows - 1 - i, cols - 1 - j] = board[i, j];
                }
            }

            return flipped;
        }

    }
}
