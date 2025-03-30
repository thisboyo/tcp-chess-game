using TChessP;
using ServerProject;
using Newtonsoft.Json;

namespace ServerGUI
{
    public partial class Form1 : Form
    {
        Swerver server;
        public Form1()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(1100, 100);
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            server = new Swerver(10004);
            server.ServerTextMessageEvent += Server_ServerTextMessageEvent;
            server.Start();

        }
        private void Server_ServerTextMessageEvent(TChessP.Packet msg)
        {
            //challenge is to try and add connection and disconnection messagses to the server 
            //trace end to end, where does the connect message get broadcast
            //where does it get received

            try
            {
                if (msg.ContentType == TChessP.MessageType.Connected)
                {
                    this.Invoke((Delegate)(() => this.lstServerMessage.Items.Add(msg.Payload)));
                }
                else if (msg.ContentType == TChessP.MessageType.Disconnected)
                {
                    this.Invoke((Delegate)(() => this.lstServerMessage.Items.Add(msg.Payload)));
                }
                //process the packet first
                if (msg.ContentType == TChessP.MessageType.Broadcast)
                {
                    this.Invoke((Delegate)(() => this.lstServerMessage.Items.Add(msg.Payload)));
                }
                else if (msg.ContentType == TChessP.MessageType.File)
                {
                    // We have the packet deserialized, however, the FTO is still in json
                    // in the payload
                    FileTransferObject fto = JsonConvert.DeserializeObject<FileTransferObject>(msg.Payload);
                    //Now we have our bytes reconstructed on the other end of the stream
                    File.WriteAllBytes("D:/rhys/" + fto.FileName, fto.FileBytes);
                }
                else if (msg.ContentType == MessageType.ServerOnly) //Say Moved piece
                {
                    this.Invoke(() => lstBoardMessage.Items.Add(msg.Payload));
                }

            }
            catch
            {

            }
        }
    }
}
