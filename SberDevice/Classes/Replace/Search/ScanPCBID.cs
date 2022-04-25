using SberDevice.Classes.Replace.Class;
using SberDevice.Interface;
using SberDevice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SberDevice.Classes
{
    public class ScanPCBID : ISearch
    {
        FASEntities Fas = new FASEntities();
        SMDCOMPONETSEntities SMD = new SMDCOMPONETSEntities();
        public Datas Datas { get; set; }

        public bool IsPacking()
        {
            return Fas.Ct_PackingTable.Where(c => c.PCBID == Datas.PCBID).Select(c => c.PCBID == c.PCBID).FirstOrDefault();
        }

        public bool IsBunch(string FirstSN)
        {
            var _sn = Fas.Ct_PackingTable.Where(c => c.PCBID == Datas.PCBID).Select(c => Fas.Ct_FASSN_reg.Where(b => b.ID == c.SNID).Select(b => b.SN).FirstOrDefault()).FirstOrDefault();
            if (_sn != FirstSN) return false;

            return true;
        }
        public void MergeData(ISearch search)
        {
            Datas.SN = search.Datas.SN;
            Datas.ID = search.Datas.ID;
        }

        public InfoPacking GetInfoPacking()
        {
            var decode = SMD.LazerBase.Where(c => c.IDLaser == Datas.PCBID).Select(c => c.Content).FirstOrDefault();

            return Fas.Ct_PackingTable.Where(c => c.PCBID == Datas.PCBID).Select(c => new InfoPacking()
            {
                Decode = decode,
                SN = Fas.Ct_FASSN_reg.Where(b=> b.ID == c.SNID).Select(b=>b.SN).FirstOrDefault(),
                OrderName = Fas.Contract_LOT.Where(b=>b.ID == c.LOTID).Select(b=>b.FullLOTCode).FirstOrDefault(),
                Liter = Fas.FAS_Liter.Where(b=>b.ID == c.LiterID).Select(b=> b.LiterName + c.LiterIndex).FirstOrDefault(),
                LineName = Fas.FAS_Liter.Where(b => b.ID == c.LiterID).Select(b => Fas.FAS_Lines.Where(x=>x.LineID == b.LineID).Select(x=>x.LineName).FirstOrDefault()).FirstOrDefault(),
                PalletNum = c.PalletNum,
                BoxNum = c.BoxNum,
                UnitNum = c.UnitNum,
                PackingDate = c.PackingDate,
                UserName = Fas.FAS_Users.Where(x=>x.UserID == c.UserID).Select(x=>x.UserName).FirstOrDefault(),
                Description = c.Descriptions,
                Mac = Fas.Depo_SN_MAC.Where(x=>x.SN == Fas.Ct_FASSN_reg.Where(b=>b.ID == c.SNID).Select(b=>b.SN).FirstOrDefault()).Select(x=>x.MAC1).FirstOrDefault(),
                LiterID = c.LiterID,
                LiterIndex = c.LiterIndex,
                LOTID = c.LOTID,
                PCBID = (int)c.PCBID,
                SNID = (int)c.SNID,

            }).FirstOrDefault();
        }

    }
}