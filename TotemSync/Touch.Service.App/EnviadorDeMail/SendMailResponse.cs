using System;
using System.Collections.Generic;
using System.Text;

namespace EnviadorDeMail
{
    public class SendMailResponse
    {
        public bool Success { get; set; }
        public DateTime Time { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Result { get; set; }
    }
}
