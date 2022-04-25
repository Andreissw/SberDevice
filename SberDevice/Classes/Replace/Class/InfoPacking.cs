using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SberDevice.Classes.Replace.Class
{
    public class InfoPacking
    {
        public string Decode { get; set; }
        public string SN { get; set; }
        public string Mac { get; set; }
        public string Liter { get; set; }
        public short PalletNum { get;set; }
        public short BoxNum { get; set; }
        public short UnitNum { get; set; }
        public DateTime PackingDate { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string LineName { get; set; }
        public string OrderName { get; set; }
        public int LOTID { get; set; }
        public int SNID { get; set; }
        public int? PCBID { get; set; }
        public int LiterID { get; set; }
        public int LiterIndex { get; set; }

        public int pcbid { get; set; }
        



    }
}