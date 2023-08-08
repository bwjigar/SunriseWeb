using Lib.Models;
using OfficeOpenXml;
using SunriseWeb.Data;
using SunriseWeb.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Web.Hosting;
using SunriseWeb.Resources;

namespace SunriseWeb.Controllers
{
    [AuthorizeActionFilterAttribute]
    public class OfferController : BaseController
    {
        API _api = new API();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetSearchStock(SearchDiamondsRequest obj)
        {
            Session["OfferDiamondStock"] = obj;
            string inputJson = (new JavaScriptSerializer()).Serialize(obj);
            string _response = _api.CallAPI(Constants.GetSearchStock, inputJson);
            ServiceResponse<SearchDiamondsResponse> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<SearchDiamondsResponse>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetModifyStockParameter()
        {
            SearchDiamondsRequest obj = (SearchDiamondsRequest)Session["OfferDiamondStock"];
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOfferCriteria()
        {
            string _response = _api.CallAPI(Constants.GetOfferCriteria, string.Empty);
            ServiceResponse<OfferCriteria> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<OfferCriteria>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveOfferCriterias(string OfferPer)
        {
            var input = new
            {
                OfferPer = OfferPer,
            };
            string inputJson = (new JavaScriptSerializer()).Serialize(input);
            string _response = _api.CallAPI(Constants.SaveOfferCriteria, inputJson);
            ServiceResponse<OfferCriteria> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<OfferCriteria>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveOfferCriteria(SaveOfferCriteria_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string response = _api.CallAPI(Constants.SaveOfferTransactions_Web, inputJson);
            CommonResponse data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(response);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveOfferCriteria_1(string StoneID, string OfferDiscPer, string OfferValidity, string Comments)
        {
            var input = new
            {
                StoneID = StoneID,
                OfferDiscPer = OfferDiscPer,
                OfferValidity = OfferValidity,
                Comments = Comments
            };
            string inputJson = (new JavaScriptSerializer()).Serialize(input);
            string _response = _api.CallAPI(Constants.SaveOfferTransactions_1, inputJson);
            ServiceResponse<OfferCriteria> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<OfferCriteria>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OfferExcelDownloadBySearchObject(SearchDiamondsRequest obj)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(obj);
            string _response = _api.CallAPI(Constants.DownloadOfferExcel, inputJson);
            string _data = (new JavaScriptSerializer()).Deserialize<string>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadExcelforOffer()
        {
            HttpPostedFileBase file = Request.Files[0]; //Uploaded file
            int fileSize = file.ContentLength;
            string fileName = file.FileName;
            string mimeType = file.ContentType;
            System.IO.Stream fileContent = file.InputStream;
            //To save file, use SaveAs method
            string path = Server.MapPath("~/Upload/OfferExcel/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            file.SaveAs(Server.MapPath("~/Upload/OfferExcel/") + fileName); //File will be saved in application root

            var ep = new ExcelPackage(new FileInfo(Server.MapPath("~/Upload/OfferExcel/") + fileName));
            var ws = ep.Workbook.Worksheets["StoneSelection"];
            string Error_msg = string.Empty;
            int Error_count = 0;

            Error_msg = "<table border='1' style='font-size: 12px;width: 60%;'>";
            Error_msg += "<tbody>";
            Error_msg += "<tr>";
            Error_msg += "<td style='background-color: #003d66;color: white;padding: 3px;width: 6%;'><b>No.</b></td>";
            Error_msg += "<td style='background-color: #003d66;color: white;padding: 3px;width: 37%;'><b>Stock ID</b>";
            Error_msg += "<td style='background-color: #003d66;color: white;padding: 3px;width: 25%;'><b>Disc(%)</b></td>";
            Error_msg += "</td><td style='background-color: #003d66;color: white;padding: 3px;width: 47%;'><b>Offer Disc(%)</b></td>";
            Error_msg += "</tr>";

            List<SearchStone> lst = new List<SearchStone>();
            bool status = false;
            for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
            {
                SearchStone obj = new SearchStone();
                if (ws.Cells[rw, 4].Value != null && ws.Cells[rw, 4].Value.ToString() != "")
                {
                    obj = GetStoneDetailByStoneNo(ws.Cells[rw, 4].Value.ToString());
                    if (obj != null && obj.cur_rap_rate != 0)
                    {
                        decimal offerDisc = 0, offerAmt = 0, offerFinalDisc = 0, offerFinalAmt = 0, From_Disc = 0, From_Amt = 0, To_Disc = 0, To_Amt = 0;
                        int validDays = 1;
                        string remark = "";

                        //To_Amt = Convert.ToDecimal(obj.net_amount) * Convert.ToDecimal(0.95);
                        //To_Disc = (To_Amt * 100 / obj.rap_amount) - 100;
                        To_Amt = obj.Maximum_Offer_Amt;
                        To_Disc = obj.Maximum_Offer_Dis;

                        if (ws.Cells[rw, 18].Value != null && ws.Cells[rw, 18].Value.ToString() != "NaN")
                        {
                            From_Disc = obj.sales_disc_per;
                            From_Amt = obj.net_amount;

                            decimal excel_offer_disc = Convert.ToDecimal(ws.Cells[rw, 18].Value);
                            decimal min = Math.Min(From_Disc, To_Disc);
                            decimal max = Math.Max(From_Disc, To_Disc);

                            //if ((From_Disc > 0 && excel_offer_disc >= From_Disc && excel_offer_disc <= To_Disc) || (From_Disc < 0 && excel_offer_disc <= From_Disc && excel_offer_disc >= To_Disc))
                            if (excel_offer_disc >= min && excel_offer_disc <= max)
                            {
                                //if (offerAmt >= To_Amt)

                                decimal _min = Math.Min(((100 + excel_offer_disc) * obj.rap_amount / 100), To_Amt);
                                decimal _max = Math.Max(((100 + excel_offer_disc) * obj.rap_amount / 100), To_Amt);

                                if (_min <= _max)
                                {
                                    status = true;

                                    offerDisc = excel_offer_disc;
                                    offerAmt = (100 + offerDisc) * obj.rap_amount / 100;
                                    offerFinalAmt = offerAmt * Convert.ToDecimal(0.9912);
                                    offerFinalDisc = (offerFinalAmt * 100 / obj.rap_amount) - 100;

                                    if (ws.Cells[rw, 19].Value != null && ws.Cells[rw, 19].Value.ToString() != "NaN")
                                        validDays = Convert.ToInt32(ws.Cells[rw, 19].Value);
                                    else
                                        validDays = 1;

                                    if (ws.Cells[rw, 20].Value != null && ws.Cells[rw, 20].Value.ToString() != "NaN")
                                        remark = Convert.ToString(ws.Cells[rw, 20].Value);
                                    else
                                        remark = "";
                                }
                                else
                                {
                                    status = false;
                                }
                            }
                            else
                            {
                                status = false;
                            }
                            if (status == false)
                            {
                                Error_count = Error_count + 1;
                                Error_msg += "<tr>";
                                Error_msg += "<td><b>" + Error_count + "</b></td>";
                                Error_msg += "<td>" + ws.Cells[rw, 4].Value + "</td>";
                                Error_msg += "<td>" + Convert.ToDecimal(ws.Cells[rw, 15].Value) + "</td>";
                                Error_msg += "<td>" + Convert.ToDecimal(ws.Cells[rw, 18].Value) + "</td>";
                                Error_msg += "</tr>";
                            }

                            obj.offerDisc = offerDisc;
                            obj.offerAmt = offerAmt;
                            obj.validDays = validDays;
                            obj.Offer_Final_Discount = offerFinalDisc;
                            obj.Offer_Final_Amount = offerFinalAmt;
                            obj.Offer_Remark = remark;

                        }
                        //obj.Final_Disc = obj.Final_Disc;
                        //obj.Final_Value = obj.Final_Value;

                        obj.Maximum_Offer_Amt = To_Amt;
                        obj.Maximum_Offer_Dis = To_Disc;
                        lst.Add(obj);
                    }
                }
            }

            Error_msg += "</tbody>";
            Error_msg += "</table>";

            if (Error_count == 0)
                Error_msg = "";

            SearchStone obj2 = new SearchStone();
            obj2.sComments = Error_msg;
            lst.Add(obj2);

            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UploadExcelforOffer_1()
        {
            HttpPostedFileBase file = Request.Files[0]; //Uploaded file
            int fileSize = file.ContentLength;
            string fileName = file.FileName;
            string mimeType = file.ContentType;
            System.IO.Stream fileContent = file.InputStream;
            //To save file, use SaveAs method
            string path = Server.MapPath("~/Upload/OfferExcel/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            file.SaveAs(Server.MapPath("~/Upload/OfferExcel/") + fileName); //File will be saved in application root

            var ep = new ExcelPackage(new FileInfo(Server.MapPath("~/Upload/OfferExcel/") + fileName));
            var ws = ep.Workbook.Worksheets["StoneSelection"];
            string Error_msg = string.Empty;
            int Error_count = 0;

            Error_msg = "<table border='1' style='font-size: 12px;width: 60%;'>";
            Error_msg += "<tbody>";
            Error_msg += "<tr>";
            Error_msg += "<td style='background-color: #003d66;color: white;padding: 3px;width: 6%;'><b>No.</b></td>";
            Error_msg += "<td style='background-color: #003d66;color: white;padding: 3px;width: 37%;'><b>Stock ID</b>";
            Error_msg += "<td style='background-color: #003d66;color: white;padding: 3px;width: 25%;'><b>Disc(%)</b></td>";
            Error_msg += "</td><td style='background-color: #003d66;color: white;padding: 3px;width: 47%;'><b>Offer Disc(%)</b></td>";
            Error_msg += "</tr>";

            List<SearchStone> lst = new List<SearchStone>();
            for (int rw = 2; rw <= ws.Dimension.End.Row; rw++)
            {
                SearchStone obj = new SearchStone();
                if (ws.Cells[rw, 4].Value != null && ws.Cells[rw, 4].Value.ToString() != "")
                {
                    obj = GetStoneDetailByStoneNo(ws.Cells[rw, 4].Value.ToString());

                    decimal offerDisc = 0, offerAmt = 0;
                    int validDays = 1;

                    if (ws.Cells[rw, 18].Value != null && ws.Cells[rw, 18].Value.ToString() != "NaN")
                    {
                        decimal From_Disc = Convert.ToDecimal(ws.Cells[rw, 15].Value) - 5;
                        decimal To_Disc = Convert.ToDecimal(ws.Cells[rw, 15].Value) + 5;

                        if (Convert.ToDecimal(ws.Cells[rw, 18].Value) >= From_Disc && Convert.ToDecimal(ws.Cells[rw, 18].Value) <= To_Disc)
                        {
                            offerDisc = Convert.ToDecimal(ws.Cells[rw, 18].Value);


                            decimal rapaport = Convert.ToDecimal(obj.cur_rap_rate);
                            //  offerAmt = (rapaport + (rapaport * obj.offerDisc) / 100) * obj.cts;

                            decimal newRate;
                            if (offerDisc > 0)
                            {
                                newRate = rapaport - ((rapaport * ((-1) * offerDisc)) / 100);
                                offerAmt = newRate * obj.cts;
                            }
                            else
                            {
                                newRate = rapaport + ((rapaport * offerDisc) / 100);
                                offerAmt = newRate * obj.cts;
                            }


                            if (ws.Cells[rw, 19].Value != null && ws.Cells[rw, 19].Value.ToString() != "NaN")
                                validDays = Convert.ToInt32(ws.Cells[rw, 19].Value);
                            else
                                validDays = 1;
                        }
                        else
                        {
                            Error_count = Error_count + 1;
                            Error_msg += "<tr>";
                            Error_msg += "<td><b>" + Error_count + "</b></td>";
                            Error_msg += "<td>" + ws.Cells[rw, 4].Value + "</td>";
                            Error_msg += "<td>" + Convert.ToDecimal(ws.Cells[rw, 15].Value) + "</td>";
                            Error_msg += "<td>" + Convert.ToDecimal(ws.Cells[rw, 18].Value) + "</td>";
                            Error_msg += "</tr>";
                        }
                        obj.offerDisc = offerDisc;
                        obj.offerAmt = offerAmt;
                        obj.validDays = validDays;

                        //if (obj.offerDisc > 0)
                        //{
                        //    obj.offerDisc = obj.offerDisc * (-1);
                        //}
                    }

                    //if (ws.Cells[rw, 19].Value != null && ws.Cells[rw, 19].Value.ToString() != "NaN")
                    //{
                    //    obj.validDays = validDays;
                    //}
                    //else
                    //{
                    //    obj.validDays = 1;
                    //}
                    lst.Add(obj);
                }
            }

            Error_msg += "</tbody>";
            Error_msg += "</table>";

            if (Error_count == 0)
                Error_msg = "";

            SearchStone obj2 = new SearchStone();
            obj2.sComments = Error_msg;
            lst.Add(obj2);

            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OfferHistory()
        {
            return View();
        }
        public ActionResult OfferCriteria()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetOfferHistoryList(OfferHisRequest _obj)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(_obj);
            string _response = _api.CallAPI(Constants.GetOfferHistory, inputJson);
            ServiceResponse<OfferHisResponse> _data = (new JavaScriptSerializer()).Deserialize<ServiceResponse<OfferHisResponse>>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DownloadOfferHistoryList(OfferHisRequest _obj)
        {
            _obj.PageNo = "";
            string inputJson = (new JavaScriptSerializer()).Serialize(_obj);
            string _response = _api.CallAPI(Constants.DownloadOfferHistory, inputJson);
            CommonResponse _data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(_data, JsonRequestBehavior.AllowGet);
        }
        public SearchStone GetStoneDetailByStoneNo(string stoneNo)
        {
            var input = new
            {
                StoneID = stoneNo
            };
            string inputJson = (new JavaScriptSerializer()).Serialize(input);
            string _response = _api.CallAPI(Constants.GetSearchStockByStoneID, inputJson);
            SearchStone _data = (new JavaScriptSerializer()).Deserialize<SearchStone>(_response);
            return _data;
        }
        public JsonResult Offer_Delete(Offer_Delete_Req req)
        {
            string inputJson = (new JavaScriptSerializer()).Serialize(req);
            string _response = _api.CallAPI(Constants.Offer_Delete, inputJson);
            CommonResponse data = (new JavaScriptSerializer()).Deserialize<CommonResponse>(_response);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}