using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActueelNS.Services.Models
{
    public class ShareSearch
    {

        public PlannerSearch PlannerSearch { get; set; }
        public List<ReisMogelijkheid> ReisMogelijkheden { get; set; }

        // Convert an object to a byte array
        public byte[] ObjectToByteArray()
        {
            string str = JsonConvert.SerializeObject(this);
            
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;

        }

        public static ShareSearch Desserialize(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            var str = new string(chars);

            return JsonConvert.DeserializeObject <ShareSearch>(str);

        }

    }
}
