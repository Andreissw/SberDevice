using SberDevice.Classes.Replace.Class;
using SberDevice.Interface;
using SberDevice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SberDevice.Classes
{
    public class ScanSN : ISearch
    {
        FASEntities Fas = new FASEntities();
        SMDCOMPONETSEntities SMD = new SMDCOMPONETSEntities();

        public Datas Datas { get; set; }

        public bool IsPacking()
        {
            return Fas.Ct_PackingTable.Where(c => c.SNID == Datas.ID).Select(c => c.SNID == c.SNID).FirstOrDefault();           
        }

        public bool IsBunch(string FirstSN)
        {
            var _pcbid = Fas.Ct_PackingTable.Where(c => c.SNID == Datas.ID).Select(c => c.PCBID).FirstOrDefault();
            var _content = SMD.LazerBase.Where(c => c.IDLaser == _pcbid).Select(c => c.Content).FirstOrDefault();
            if (_content != FirstSN) return false;

            return true;
        }

        public InfoPacking GetInfoPacking()
        {
            //var pcbid = Fas.Ct_PackingTable.Where(c => c.SNID == Datas.ID).Select(c => c.PCBID).FirstOrDefault();
            var decode = SMD.LazerBase.Where(c => c.IDLaser == Datas.PCBID).Select(c => c.Content).FirstOrDefault();

            return Fas.Ct_PackingTable.Where(c => c.SNID == Datas.ID).Select(c => new InfoPacking()
            {
                Decode = decode,
                SN = Fas.Ct_FASSN_reg.Where(b => b.ID == c.SNID).Select(b => b.SN).FirstOrDefault(),
                OrderName = Fas.Contract_LOT.Where(b => b.ID == c.LOTID).Select(b => b.FullLOTCode).FirstOrDefault(),
                Liter = Fas.FAS_Liter.Where(b => b.ID == c.LiterID).Select(b => b.LiterName + c.LiterIndex).FirstOrDefault(),
                LineName = Fas.FAS_Liter.Where(b => b.ID == c.LiterID).Select(b => Fas.FAS_Lines.Where(x => x.LineID == b.LineID).Select(x => x.LineName).FirstOrDefault()).FirstOrDefault(),
                PalletNum = c.PalletNum,
                BoxNum = c.BoxNum,
                UnitNum = c.UnitNum,
                PackingDate = c.PackingDate,
                UserName = Fas.FAS_Users.Where(x => x.UserID == c.UserID).Select(x => x.UserName).FirstOrDefault(),
                Description = c.Descriptions,
                Mac = Fas.Depo_SN_MAC.Where(x => x.SN == Fas.Ct_FASSN_reg.Where(b => b.ID == c.SNID).Select(b => b.SN).FirstOrDefault()).Select(x => x.MAC1).FirstOrDefault(),
                LiterID = c.LiterID,
                LiterIndex = c.LiterIndex,
                LOTID = c.LOTID,
                PCBID = (int)c.PCBID,
                SNID = (int)c.SNID,               

            }).FirstOrDefault();
        }


    }
}