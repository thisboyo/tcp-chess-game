using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TChessP
{
    public class FileTransferObject
    {
        //You can add other things in here, like filename
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }

        public string JsonSerialized()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
