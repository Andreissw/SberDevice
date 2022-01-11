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

        public JsonResult FindSN(string SN)
        {
            SearchData searchData = new SearchData();

            searchData.Search = Identity(SN);

            if (searchData.Search == null) return Json("NoData", JsonRequestBehavior.AllowGet);

            var strresult = searchData.IsSingle();

            if (strresult == "null") return Json("NoSingle", JsonRequestBehavior.AllowGet);

            var result = searchData.Search.IsPacking();

            if (!result) return Json("NoPac", JsonRequestBehavior.AllowGet);

            return Json(strresult, JsonRequestBehavior.AllowGet);

        }


        public JsonResult SetReplace(string SN, string FirstSN, string SNReplace)
        {
            if (SN == FirstSN) return Json("Dup", JsonRequestBehavior.AllowGet);

            SearchData searchData = new SearchData();
            searchData.Search = Identity(SN);

            if (searchData.Search == null) return Json("null", JsonRequestBehavior.AllowGet);

            if (!searchData.Search.IsBunch(FirstSN)) return Json("fake", JsonRequestBehavior.AllowGet);

            searchData.RemovePac();
            searchData.AddPac(Identity(SNReplace));


            return Json(true,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPacking(string SN, string FirstSN)
        {
            if (SN == FirstSN)  return View(new List<string>());


            SearchData searchData = new SearchData();
            searchData.Search = Identity(SN);

            if (searchData.Search == null) return View(new List<string>());

            if (!searchData.Search.IsBunch(FirstSN)) return View(new List<string>());

            var Result = searchData.Search.GetInfoPacking();

            if (Result == null) return View(new List<string>());

            return View(Result);
        }

        ISearch Identity(string SN)
        {
            var _sndatas = Fas.Ct_FASSN_reg.Where(c => c.SN == SN).Select(c => new Datas { PCBID = Fas.Ct_PackingTable.Where(b=>b.SNID == c.ID).Select(x=>x.PCBID).FirstOrDefault()
                , SN = c.SN, ID = c.ID, LOTID = c.LOTID }).FirstOrDefault();

            if (_sndatas != null) return new ScanSN() { Datas = _sndatas };

            var _pcbid = SMD.LazerBase.Where(c => c.Content == SN).Select(c => c.IDLaser).FirstOrDefault();
            if (_pcbid == 0) return null;

            var _pcbiddatas = Fas.Ct_PackingTable.Where(c => c.PCBID == _pcbid).Select(c => new Datas { PCBID = (int)c.PCBID, ID = (int)c.SNID, LOTID = c.LOTID }).FirstOrDefault();
            if (_pcbiddatas == null) return null;

            return new ScanPCBID() { Datas = _pcbiddatas };

        }        

    }
}