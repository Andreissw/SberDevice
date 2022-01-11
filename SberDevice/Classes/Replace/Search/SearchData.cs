using SberDevice.Classes.Replace.Class;
using SberDevice.Interface;
using SberDevice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SberDevice
{
  
    public class SearchData
    {      
        public ISearch Search { get; set; }
        public string Description { get; set; }

        public int UserID { get; set; }
        InfoPacking infoPacking { get; set; }

        

        public string IsSingle()
        {
            var fas = new FASEntities();
            var result = fas.Contract_LOT.Where(c => c.ID == Search.Datas.LOTID).Select(c => c.SingleSN).FirstOrDefault();
            
            if (result == null)  return "null";
            return result.ToString();           
        }

        public void RemovePac()
        {
            var fas = new FASEntities();
            var result = fas.Ct_PackingTable.Where(c => c.SNID == Search.Datas.ID).FirstOrDefault();

            infoPacking = Search.GetInfoPacking();
            SetLog(1,3);

            fas.Ct_PackingTable.Remove(result);
            fas.SaveChanges();
        }

        public void AddPac(ISearch data)
        {            
            var fas = new FASEntities();
            Ct_PackingTable ct_PackingTable = new Ct_PackingTable()
            { 
                PCBID = data.Datas.PCBID,
                SNID = data.Datas.ID,
                LOTID = data.Datas.LOTID,
                LiterID = (byte)infoPacking.LiterID,
                LiterIndex = (short)infoPacking.LiterIndex,
                BoxNum = infoPacking.BoxNum,
                PalletNum = infoPacking.PalletNum,
                UnitNum = infoPacking.UnitNum,
                PackingDate = infoPacking.PackingDate,
                Descriptions = "Замена упаковки: " + Description,
                UserID = (short)UserID,                
            };

            fas.Ct_PackingTable.Add(ct_PackingTable);
            fas.SaveChanges();

            SetLog(6,2);

        }

        void SetLog(int stepid, int TestResult)
        {
            var fas = new FASEntities();
            Ct_OperLog ct_OperLog = new Ct_OperLog()
            {
                LOTID = infoPacking.LOTID,
                StepID = (short)stepid,
                TestResultID = (byte)TestResult,
                Descriptions = "Замена упаковки: " + Description,
                PCBID = infoPacking.PCBID,
                SNID = infoPacking.SNID,
                StepByID = (short)UserID,
                StepDate = DateTime.UtcNow.AddHours(2),
            };

            fas.Ct_OperLog.Add(ct_OperLog);
            fas.SaveChanges();
        }


    }
}