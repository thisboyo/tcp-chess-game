using Newtonsoft.Json;

namespace TChessP
{
    public enum MessageType
    {
        Broadcast,
        ServerOnly,
        File,
        Connected,
        Disconnected,
        SelectedSquare,
    }
    public class Packet
    {
        public MessageType ContentType { get; set; }
        //We Will serialize types of objects into json
        //Send em in a wrapped packet
        //MessageTpe will help us to decode on the other side
        public string? Payload { get; set; }

    }

    public class SquareClick
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public string JsonSerialized() => JsonConvert.SerializeObject(this);

        public static SquareClick FromJson(string json) =>
            JsonConvert.DeserializeObject<SquareClick>(json);
    }
    public class PieceMove
    {
        public int FromRow { get; set; }
        public int FromCol { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }
        public string Piece { get; set; }

        public string JsonSerialized() => JsonConvert.SerializeObject(this);
        public static PieceMove FromJson(string json) => JsonConvert.DeserializeObject<PieceMove>(json);
    }

}
