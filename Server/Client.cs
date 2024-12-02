using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime DateConnect { get; set; }
        public bool IsBlackList { get; set; }

        public Client()
        {
            Random rnd = new Random();
            string Chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            this.Token = new string(Enumerable.Repeat(Chars, 15).Select(x => x[rnd.Next(Chars.Length)]).ToArray());
            DateConnect = DateTime.Now;
        }
    }
}
