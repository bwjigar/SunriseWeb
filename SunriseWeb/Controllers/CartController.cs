using Lib.Models;
using SunriseWeb.Data;
using SunriseWeb.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SunriseWeb.Controllers
{
    [AuthorizeActionFilterAttribute]
    public class CartController : BaseController
    {
        // GET: Cart
        API _api = new API();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetCartStoneList(ViewCartRequest req)
        {
            var input = new
            {
                RefNo1 = req.RefNo1,
                RefNo = "",
                OfferTrans="",
                Location = req.Location,
                Shape = req.Shape,
                Color = req.Color,
                Polish = req.Polish,
                Pointer = req.Pointer,
                Lab = req.Lab,
                Fls = req.Fls,
                Clarity = req.Clarity,
                Cut = req.Cut,
                Symm = req.Symm,
                PageNo = req.PageNo,
                OrderBy = req.OrderBy,
                FromDate = req.FromDate,
                ToDate = req.ToDate,
                Status = req.Status,
                CompanyName = req.CompanyName,
                PageSize = req.PageSize,
                SubUser = req.SubUser
            };
            string inputJson = (new JavaScriptSerializer()).Serialize(input);
            string _response = _api.CallAPI(Constants.ViewCart, inputJson);
            ServiceResponse<ViewCartResponse> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<ViewCartResponse>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
    }
}