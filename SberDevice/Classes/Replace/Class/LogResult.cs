using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SberDevice.Classes.Replace.Class
{
    public class LogResult
    {
        public string Decode { get; set; }
        public string OrderName { get; set; }
        public string Step { get; set; }
        public string Result { get; set; }

        public DateTime Date { get; set; }

        public string User { get; set; }
        public string Description { get; set; }

        public string SN { get; set; }

        public int? pcbid { get; set; }
    }
}