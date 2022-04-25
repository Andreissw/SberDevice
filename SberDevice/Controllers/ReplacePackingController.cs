using SberDevice.Classes;
using SberDevice.Classes.Replace.Class;
using SberDevice.Interface;
using SberDevice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SberDevice.Controllers
{
    public class ReplacePackingController : Controller
    {
        // GET: ReplacePacking

        FASEntities Fas = new FASEntities();
        SMDCOMPONETSEntities SMD = new SMDCOMPONETSEntities();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Checkpac(string SN)
        {
            SearchData searchData = new SearchData();

            searchData.Search = Identity(SN);

            if (searchData.Search == null) return Json("NoData", JsonRequestBehavior.AllowGet);
          
            var result = searchData.Search.IsPacking();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CheckSingle(string SNFirst, string SN)
        {
            SearchData searchData = new SearchData();
            searchData.Search = Identity(SNFirst);

            var CheckSN = Identity(SN);
            if (CheckSN == null) return Json("False", JsonRequestBehavior.AllowGet);

            if (CheckSN.IsPacking()) return Json("Pac", JsonRequestBehavior.AllowGet);

            var result = searchData.IsSingle();

            if (result == "False") return Json("SingleFalse", JsonRequestBehavior.AllowGet);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SetReplace(string SN, string FirstSN, string SNReplace)
        {
            if (SN == FirstSN) return Json("Dup", JsonRequestBehavior.AllowGet);

            SearchData searchData = new SearchData();

            searchData.Search = Identity(FirstSN);
            if (searchData.Search == null) return Json("null", JsonRequestBehavior.AllowGet);

            var resultSN = Identity(SN,(int)searchData.Search.Datas.LOTID);
            if (resultSN == null) return Json("null", JsonRequestBehavior.AllowGet);

            if (resultSN.Datas.LOTID != searchData.Search.Datas.LOTID) return Json("Lot", JsonRequestBehavior.AllowGet);

            var ResultReplaceSN = Identity(SNReplace);
            if (ResultReplaceSN.Datas.LOTID != resultSN.Datas.LOTID) return Json("Lot", JsonRequestBehavior.AllowGet);

            if (resultSN.IsPacking()) return Json("Pac", JsonRequestBehavior.AllowGet);

            searchData.RemovePac(Identity(SNReplace));
            searchData.Search.MergeData(resultSN);         
            searchData.AddPac();

            return Json(true,JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetSingleReplace(string SN, string SNReplace)
        {
            
            SearchData searchData = new SearchData();

            var DataReplaceSN = Identity(SNReplace);

            searchData.Search = Identity(SN,(int)DataReplaceSN.Datas.LOTID);
            if (searchData.Search == null) return Json("null", JsonRequestBehavior.AllowGet);    

            if (searchData.Search.IsPacking()) return Json("Pac", JsonRequestBehavior.AllowGet);

            searchData.RemovePac(DataReplaceSN);            
            searchData.AddPac();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetResult()
        {
            ResultReplace resultReplace = new ResultReplace();

            resultReplace.InfoPacking = Fas.Ct_PackingTable.OrderByDescending(c => c.ID).Where(c => c.Descriptions.StartsWith("Замена упаковки:")).Select(c => new InfoPacking()
            {
                PackingDate = c.PackingDate,
                BoxNum = c.BoxNum,
                PalletNum = c.PalletNum,
                UnitNum = c.UnitNum,
                PCBID = c.PCBID,
                OrderName = Fas.Contract_LOT.Where(b => b.ID == c.LOTID).Select(b => b.FullLOTCode).FirstOrDefault(),
                Description = c.Descriptions,
                Liter = Fas.FAS_Liter.Where(b=>b.ID == c.LiterID).Select(b=>b.LiterName + c.LiterIndex).FirstOrDefault(),
                Mac = Fas.Depo_SN_MAC.Where(x => x.SN == Fas.Ct_FASSN_reg.Where(b => b.ID == c.SNID).Select(b => b.SN).FirstOrDefault()).Select(x => x.MAC1).FirstOrDefault(),
                SN = Fas.Ct_FASSN_reg.Where(b => b.ID == c.SNID).Select(b => b.SN).FirstOrDefault(),
                UserName = Fas.FAS_Users.Where(x => x.UserID == c.UserID).Select(x => x.UserName).FirstOrDefault(),
                LineName = Fas.FAS_Liter.Where(b => b.ID == c.LiterID).Select(b => Fas.FAS_Lines.Where(x => x.LineID == b.LineID).Select(x => x.LineName).FirstOrDefault()).FirstOrDefault(),          


            }).FirstOrDefault();

            resultReplace.InfoPacking.Decode = SMD.LazerBase.Where(b=>b.IDLaser == resultReplace.InfoPacking.PCBID).Select(x => x.Content).FirstOrDefault();

            resultReplace.Results = Fas.Ct_OperLog.OrderByDescending(c => c.StepDate).Where(c => c.Descriptions.StartsWith("Замена упаковки:")).Select(c => new LogResult()
            {
                Date = c.StepDate,
                pcbid = c.PCBID,
                Description = c.Descriptions,
                OrderName = Fas.Contract_LOT.Where(b => b.ID == c.LOTID).Select(b => b.FullLOTCode).FirstOrDefault(),
                Result = Fas.Ct_TestResult.Where(b=>b.ID == c.TestResultID).Select(b=>b.Result).FirstOrDefault(),
                Step = Fas.Ct_StepScan.Where(b=>b.ID == c.StepID).Select(b=>b.StepName).FirstOrDefault(),
                SN = Fas.Ct_FASSN_reg.Where(b => b.ID == c.SNID).Select(b => b.SN).FirstOrDefault(),
                User = Fas.FAS_Users.Where(x => x.UserID == c.StepByID).Select(x => x.UserName).FirstOrDefault(),

            }).Take(2).ToList();

            foreach (var item in resultReplace.Results)   item.Decode = SMD.LazerBase.Where(b => b.IDLaser == item.pcbid).Select(x => x.Content).FirstOrDefault();
            

            return View(resultReplace);
        }

        public ActionResult GetPacking(string SN)
        {
            
            //if (SN == FirstSN)  return View(new List<string>());


            SearchData searchData = new SearchData();
            searchData.Search = Identity(SN);

            if (searchData.Search == null) return View(new List<string>());

            //if (!searchData.Search.IsBunch(FirstSN)) return View(new List<string>());

            var Result = searchData.Search.GetInfoPacking();

            if (Result == null) return View(new List<string>());

            return View(Result);
        }

        ISearch Identity(string SN)
        {
            if (SN == "") return null;

            var _sndatas = Fas.Ct_FASSN_reg.Where(c => c.SN == SN).Select(c => new Datas { PCBID = Fas.Ct_PackingTable.Where(b=>b.SNID == c.ID).Select(x=>x.PCBID).FirstOrDefault()
                , SN = c.SN, ID = c.ID, LOTID = c.LOTID }).FirstOrDefault();

            if (_sndatas != null) return new ScanSN() { Datas = _sndatas };

            var _pcbid = SMD.LazerBase.Where(c => c.Content == SN).Select(c => c.IDLaser).FirstOrDefault();
            if (_pcbid == 0) return null;

            var _pcbiddatas = Fas.Ct_OperLog.OrderByDescending(c=>c.StepDate).Where(c => c.PCBID == _pcbid & c.TestResultID == 2).Select(c => new Datas { PCBID = c.PCBID, ID = c.SNID, LOTID = c.LOTID }).FirstOrDefault();
            if (_pcbiddatas == null) return null;

            return new ScanPCBID() { Datas = _pcbiddatas };

        }

        ISearch Identity(string SN,int LOTID)
        {
            var _pcbid = SMD.LazerBase.Where(c => c.Content == SN).Select(c => c.IDLaser).FirstOrDefault();
            if (_pcbid != 0) {

                var _pcbiddatas = Fas.Ct_OperLog.OrderByDescending(c => c.StepDate).Where(c => c.PCBID == _pcbid & c.TestResultID == 2).Select(c => new Datas { PCBID = c.PCBID, ID = c.SNID, LOTID = c.LOTID }).FirstOrDefault();
                if (_pcbiddatas != null) return new ScanPCBID() { Datas = _pcbiddatas };
            }

            var _sndatas = Fas.Ct_FASSN_reg.Where(c => c.SN == SN).Select(c => new Datas
            {
                PCBID = Fas.Ct_PackingTable.Where(b => b.SNID == c.ID).Select(x => x.PCBID).FirstOrDefault()
                ,
                SN = c.SN,
                ID = c.ID,
                LOTID = c.LOTID

            }).FirstOrDefault();

            if (_sndatas == null) {

                Mask mask = new Mask(LOTID, SN);
                if (!mask.IsMask()) return null;
                _sndatas = AddFasReg(SN, LOTID);            
            }
           
            return new ScanSN() { Datas = _sndatas };
           

        }

        Datas AddFasReg(string SN, int LOTID)
        {
            Ct_FASSN_reg ct_FASSN_Reg = new Ct_FASSN_reg()
            {
                SN = SN,
                LOTID = LOTID,
                AppID = 11,
                LineID = 9,
                UserID = 49,
                RegDate = DateTime.UtcNow.AddHours(2),
            };

            Fas.Ct_FASSN_reg.Add(ct_FASSN_Reg);
            Fas.SaveChanges();

            return new Datas() { ID = ct_FASSN_Reg.ID, SN = SN, LOTID = LOTID };
        }

    }
}