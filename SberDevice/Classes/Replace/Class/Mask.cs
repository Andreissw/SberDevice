using SberDevice.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SberDevice.Classes.Replace.Class
{
    public class Mask
    {
        int LOTID { get; set; }
        string SN { get; set; }

        public Mask(int lotid, string sn)
        {
            LOTID = lotid;
            SN = sn;
        }

        public bool IsMask()
        {
            FASEntities fas = new FASEntities();

            var _masksdata = fas.Contract_LOT.Where(c => c.ID == LOTID).Select(c => c.FASNumberFormat2).FirstOrDefault();
            if (_masksdata == null) return false;

            var _masks = _masksdata.Split(';').ToList();

            foreach (var item in _masks) if (GetMask(item)) return true;
            return false;
        }

        bool GetMask(string _mask)
        {
            var settings = _mask.Substring(_mask.Length - 6);
            List<int> sets = new List<int>();

            for (int i = 0; i < 5; i += 2)
                sets.Add(Convert.ToInt32(settings.Substring(i, 2), 16));

            return substr(_mask) == substr(SN) ? true : false;

            string substr(string _sn)
            {
                try
                {
                    var o = _sn.Substring(0, sets[0]);
                    var p = _sn.Substring(sets[0] + sets[1], sets[2]);
                    return o + p;
                }
                catch (Exception)
                {

                    return "";
                }

            }

        }
    }
}