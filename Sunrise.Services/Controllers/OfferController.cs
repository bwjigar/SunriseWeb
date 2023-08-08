using DAL;
using DocumentFormat.OpenXml.Spreadsheet;
using EpExcelExportLib;
using ExcelExportLib;
using Lib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Oracle.DataAccess.Client;
using Sunrise.Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Sunrise.Services.Controllers
{
    [Authorize]
    [RoutePrefix("api/Offer")]
    public class OfferController : ApiController
    {
        DataTableExcelExport ge;
        DataTableEpExcelExport ep_ge;
        UInt32 DiscNormalStyleindex;
        UInt32 CutNormalStyleindex;
        UInt32 NormalStyleindex;
        UInt32 PointerStyleindex;
        UInt32 PriceStyleindex;
        public String External_ImageURL = ConfigurationManager.AppSettings["External_ImageURL"];
        [HttpPost]
        public IHttpActionResult GetOfferCriteria()
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                DataTable dt = db.ExecuteSP("OfferCriteria_Select", para.ToArray(), false);
                List<Lib.Models.OfferCriteria> offerCriteria = new List<Lib.Models.OfferCriteria>();
                offerCriteria = DataTableExtension.ToList<Lib.Models.OfferCriteria>(dt);

                if (offerCriteria.Count > 0)
                {
                    return Ok(new ServiceResponse<Lib.Models.OfferCriteria>
                    {
                        Data = offerCriteria,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<Lib.Models.OfferCriteria>
                    {
                        Data = offerCriteria,
                        Message = "Something Went wrong.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Lib.Models.OfferCriteria>
                {
                    Data = new List<Lib.Models.OfferCriteria>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        private List<Lib.Models.OfferCriteria> GetOfferCriteriaNew()
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                DataTable dt = db.ExecuteSP("OfferCriteria_Select", para.ToArray(), false);
                List<Lib.Models.OfferCriteria> offerCriteria = new List<Lib.Models.OfferCriteria>();
                offerCriteria = DataTableExtension.ToList<Lib.Models.OfferCriteria>(dt);

                if (offerCriteria.Count > 0)
                {
                    return offerCriteria;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return null;
            }
        }

        [HttpPost]
        public IHttpActionResult SaveOfferCriteria([FromBody]JObject data)
        {
            OrderCriteria offerTransReq = new OrderCriteria();

            try
            {
                offerTransReq = JsonConvert.DeserializeObject<OrderCriteria>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok("Input Parameters are not in the proper format");
            }

            CommonResponse resp = new CommonResponse();
            try
            {

                //DataTable dt = DAL.OfferCriteria(Convert.ToInt32(userID), iOfferID);

                DAL.OfferCriteria objstock = new DAL.OfferCriteria();
                DataTable dt = objstock.OfferCriteria_UpdateOffer(Convert.ToDecimal(offerTransReq.OfferPer));

                return Ok(new ServiceResponse<CommonResponse>
                {
                    Data = new List<CommonResponse>(),
                    Message = "SUCCESS",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0",
                    Error = ex.StackTrace
                });
            }
        }
        [HttpPost]
        public IHttpActionResult SaveOfferTransactions([FromBody]JObject data)
        {
            DAL.Common.InsertErrorLog(null, "Offer Module in Application is Under Development Stage. Please use our website to Place Offer", Request);
            return Ok(new CommonResponse
            {
                Error = "",
                Message = "Offer Module in Application is Under Development Stage. Please use our website to Place Offer",
                Status = "0"
            });
        }
        [HttpPost]
        public IHttpActionResult SaveOfferTransactions_Web([FromBody]JObject data)
        {
            SaveOfferCriteria_Req Req = new SaveOfferCriteria_Req();

            try
            {
                Req = JsonConvert.DeserializeObject<SaveOfferCriteria_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Error = "",
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            CommonResponse resp = new CommonResponse();
            try
            {
                int userID = Req.UserID;
                int Entry_userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                Req.Entry_UserID = Entry_userID;

                DAL.Stock objstock = new DAL.Stock();
                Int32 iOfferID = objstock.GetNewOfferID();

                string _strSuccessref = "";
                string _strerrorref = "";
                string _strSameRef = "";
                string _strSameDisc = "";

                String CustomerName = GetPartyNameByUserId(Convert.ToInt32(userID));
                Int32 AssistByID = GetAssistByUserId(Convert.ToInt32(userID));
                Int64 FortunePartyCode = Get_FortunePartyCode_ByUserId(Convert.ToInt32(userID));

                Int32 Assistby = AssistByID;
                string Party = CustomerName;

                DAL.Usermas objUser = new DAL.Usermas();
                DataTable loUserMas = objUser.UserMas_SelectOne(Convert.ToInt64(userID));
                string _stremailid = "";
                if (String.IsNullOrEmpty(loUserMas.Rows[0]["sCompEmail"].ToString()))
                    _stremailid = loUserMas.Rows[0]["sEmail"].ToString();
                else
                    _stremailid = loUserMas.Rows[0]["sCompEmail"].ToString();

                string _strcompname = loUserMas.Rows[0]["sCompName"].ToString();

                //List<FortuneService.HoldStone> StoneList = new List<FortuneService.HoldStone>();
                FortuneService.ServiceSoapClient wbService = new FortuneService.ServiceSoapClient();

                for (int i = 0; i < Req.StoneList.Count(); i++)
                {
                    String sRefNo = Req.StoneList[i].StoneNo.Trim();
                    decimal Offer_Discount = Convert.ToDecimal(Req.StoneList[i].Offer_Discount);
                    decimal Offer_Amount = Convert.ToDecimal(Req.StoneList[i].Offer_Amount);
                    string Remark = Req.StoneList[i].Remark;
                    decimal Offer_Final_Discount = Convert.ToDecimal(Req.StoneList[i].Offer_Final_Discount);
                    decimal Offer_Final_Amount = Convert.ToDecimal(Req.StoneList[i].Offer_Final_Amount);
                    decimal Original_Discount = Convert.ToDecimal(Req.StoneList[i].Discount);
                    Decimal sValidity = 1;
                    if (Req.StoneList[i].Valid_Days > 0 && Convert.ToString(Req.StoneList[i].Valid_Days) != "")
                    {
                        sValidity = Convert.ToDecimal(Req.StoneList[i].Valid_Days.ToString());
                    }

                    //FortuneService.HoldStone hs = new FortuneService.HoldStone();
                    //hs.ref_no = sRefNo;
                    //hs.disc_per = Convert.ToDecimal(Offer_Discount);
                    //hs.rap_price = sValidity;
                    ////hs.offer_remarks = Remark;

                    //StoneList.Add(hs);

                    try
                    {
                        FortuneService.CommonResultResponse cResult;
                        //cResult = wbService.MakeOfferTrans(_stremailid, Convert.ToString(userID), Party, iOfferID.ToString(), StoneList.ToArray());
                        cResult = wbService.MakeOfferTrans_New(
                            _stremailid,
                            Convert.ToString(userID),
                            Party,
                            iOfferID.ToString(),
                            Offer_Discount,
                            Convert.ToInt16(sValidity),
                            sRefNo,
                            Remark,
                            "N",
                            Offer_Amount,
                            FortunePartyCode);
                    }
                    catch
                    {
                    }


                    DataTable dtStk = objstock.Stock_SelectOne(sRefNo, Convert.ToInt32(userID));
                    if (dtStk.Rows.Count > 0)
                    {
                        float? _dLength = null, _dWidth = null, _dDepth = null, _dDepthPer = null, _dTablePer = null, _dCrAng = null, _dCrHt = null, _dPavAng = null, _dcts = null, _dPavHt = null;
                        decimal? _dRepPrice = null, dRapAmount = null, dNetPrice = null, _dDisc = null;

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dRapAmount"].ToString()))
                            dRapAmount = Convert.ToDecimal(dtStk.Rows[0]["dRapAmount"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dNetPrice"].ToString()))
                            dNetPrice = Convert.ToDecimal(dtStk.Rows[0]["dNetPrice"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dcts"].ToString()))
                            _dcts = Convert.ToSingle(dtStk.Rows[0]["dcts"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dLength"].ToString()))
                            _dLength = Convert.ToSingle(dtStk.Rows[0]["dLength"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dWidth"].ToString()))
                            _dWidth = Convert.ToSingle(dtStk.Rows[0]["dWidth"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dRepPrice"].ToString()))
                            _dRepPrice = Convert.ToDecimal(dtStk.Rows[0]["dRepPrice"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dDepth"].ToString()))
                            _dDepth = Convert.ToSingle(dtStk.Rows[0]["dDepth"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dDepthPer"].ToString()))
                            _dDepthPer = Convert.ToSingle(dtStk.Rows[0]["dDepthPer"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dTablePer"].ToString()))
                            _dTablePer = Convert.ToSingle(dtStk.Rows[0]["dTablePer"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dCrAng"].ToString()))
                            _dCrAng = Convert.ToSingle(dtStk.Rows[0]["dCrAng"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dCrHt"].ToString()))
                            _dCrHt = Convert.ToSingle(dtStk.Rows[0]["dCrHt"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dPavAng"].ToString()))
                            _dPavAng = Convert.ToSingle(dtStk.Rows[0]["dPavAng"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dPavHt"].ToString()))
                            _dPavHt = Convert.ToSingle(dtStk.Rows[0]["dPavHt"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dDisc"].ToString()))
                            _dDisc = Convert.ToDecimal(dtStk.Rows[0]["dDisc"].ToString());


                        objstock.Offer_Insert(iOfferID, sRefNo, dtStk.Rows[0]["sshape"].ToString(), _dcts, dtStk.Rows[0]["scolor"].ToString(), dtStk.Rows[0]["sclarity"].ToString(), _dRepPrice, dtStk.Rows[0]["sCut"].ToString(), dtStk.Rows[0]["sPolish"].ToString(), dtStk.Rows[0]["sSymm"].ToString(), dtStk.Rows[0]["sFls"].ToString(),
                           _dLength, _dWidth, _dDepth, _dDepthPer, _dTablePer, _dCrAng, _dCrHt, _dPavAng, _dPavHt, dtStk.Rows[0]["sCertiNo"].ToString(),
                                        _dDisc, dtStk.Rows[0]["sLab"].ToString(), dtStk.Rows[0]["sStatus"].ToString(), Convert.ToBoolean(dtStk.Rows[0]["SOffer"].ToString()), Offer_Discount, Offer_Amount, Convert.ToInt32(sValidity), Remark, Offer_Final_Discount, Offer_Final_Amount, dtStk.Rows[0]["sPointer"].ToString(), null, null,
                                      dtStk.Rows[0]["sLuster"].ToString(), dtStk.Rows[0]["sInclusion"].ToString(), dtStk.Rows[0]["sTableNatts"].ToString(), dtStk.Rows[0]["sGirdleType"].ToString(), dtStk.Rows[0]["Location"].ToString(), dtStk.Rows[0]["sShade"].ToString(), dtStk.Rows[0]["sSymbol"].ToString(), Convert.ToInt32(userID), Convert.ToInt32(Entry_userID), dtStk.Rows[0]["sCrownNatts"].ToString(), dtStk.Rows[0]["sCrownInclusion"].ToString(), dRapAmount, dNetPrice);

                        _strSuccessref += "'" + sRefNo + "'" + ",";
                    }
                }


                if (_strerrorref != "" || _strSameRef != "" || _strSameDisc != "")
                {
                    string strMsg = "";
                    if (_strSuccessref.Length == 0)
                    {
                        strMsg = "Your offer was not uploaded due to out of discount Range. Kindly try again." + _strSameRef;
                    }
                    else
                    {
                        strMsg = "Your offer was partially uploaded. Below are the stones which are not uploaded." +
                            (_strerrorref.Length > 0 ? "\\n" + "Range of discount is not valid for the stones " + _strerrorref.TrimEnd(',') + ".Please change discount and reupload." : "") +
                            (_strSameDisc.Length > 0 ? "\\n" + "Offer already received for the stones " + _strSameDisc.TrimEnd(',') + ".Please change discount and reupload it." : "") +
                            (_strSameRef.Length > 0 ? "\\n" + "You have already upload discount for the stones " + _strSameRef.TrimEnd(',') + "." : "");
                    }

                    resp.Message = strMsg;
                }
                else
                {
                    resp.Message = "Offer placed successfully!";
                }

                String _strfileName = "";
                Random rnd = new Random();

                if (_strSuccessref.Length != 0)
                {
                    DataTable dt = objstock.Offer_Excel(Convert.ToInt32(userID), iOfferID);
                    _strfileName = HttpContext.Current.Server.MapPath("~/Temp/Excel/" + rnd.Next().ToString() + ".xlsx");

                    string _strcerti = HttpContext.Current.Server.MapPath("~/certi");

                    if (File.Exists(_strfileName) == true)
                    {
                        File.Delete(_strfileName);
                    }

                    float OfferPer = 0;
                    List<Lib.Models.OfferCriteria> offerperList = GetOfferCriteriaNew();
                    if (offerperList != null && offerperList.Count > 0)
                        OfferPer = offerperList[0].OfferPer;

                    EpExcelExport.excel_offer(dt, _strfileName, OfferPer);
                }

                MailMessage xloMail = new MailMessage();
                SmtpClient xloSmtp = new SmtpClient();

                DataTable loEmails = objUser.UserMas_SelectEmailByUserId_For_Offer(Convert.ToInt32(userID));

                string lsToMail = "";
                foreach (DataRow lrEmail in loEmails.Rows)
                    lsToMail += lrEmail["sEmail"].ToString() + ",";

                if (lsToMail.Length > 0)
                    lsToMail = lsToMail.Remove(lsToMail.Length - 1);

                string _strresult = "";
                if (lsToMail.Trim() != "")
                {
                    if (_strfileName != "")
                    {
                        _strresult = "Y";
                        xloMail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromEmail"], "Sunrise Diamonds");
                        xloMail.To.Add(lsToMail);

                        string userName = Convert.ToString((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserName").FirstOrDefault().Value);
                        xloMail.Subject = "SUNRISE DIAMONDS : Offer Acknowledgement for Offer ID " + iOfferID.ToString();

                        Database db1 = new Database();
                        System.Collections.Generic.List<System.Data.IDbDataParameter> para1;
                        para1 = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

                        para1.Clear();
                        para1.Add(db1.CreateParam("iiUserId", System.Data.DbType.String, System.Data.ParameterDirection.Input, Convert.ToInt32(userID)));
                        System.Data.DataTable dtUserDetail = db1.ExecuteSP("UserMas_SelectOne", para1.ToArray(), false);

                        StringBuilder loSb = new StringBuilder();
                        loSb.Append(EmailHeader());
                        loSb.Append(@"<table cellpadding=""0"" cellspacing=""0"" width=""100%"">");
                        if (dtUserDetail.Rows[0]["sCompName"] != null && dtUserDetail.Rows[0]["sCompName"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Company Name:</td><td>" + dtUserDetail.Rows[0]["sCompName"].ToString() + "(" + dtUserDetail.Rows[0]["sCompMobile"].ToString() + ")</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["sCompAddress"] != null && dtUserDetail.Rows[0]["sCompAddress"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Address:</td><td>" + dtUserDetail.Rows[0]["sCompAddress"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["AssistBy1"] != null && dtUserDetail.Rows[0]["AssistBy1"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Sales Person:</td><td>" + dtUserDetail.Rows[0]["AssistBy1"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["mob_AssistBy1"] != null && dtUserDetail.Rows[0]["mob_AssistBy1"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Mobile:</td><td>" + dtUserDetail.Rows[0]["mob_AssistBy1"].ToString() + "</td></tr>");
                            loSb.Append(@"<tr><td>Whatsapp No:</td><td>" + dtUserDetail.Rows[0]["mob_AssistBy1"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["sWeChatId1"] != null && dtUserDetail.Rows[0]["sWeChatId1"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>WeChat ID:</td><td>" + dtUserDetail.Rows[0]["sWeChatId1"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["Email_AssistBy1"] != null && dtUserDetail.Rows[0]["Email_AssistBy1"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Email:</td><td>" + dtUserDetail.Rows[0]["Email_AssistBy1"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["iUserType"] != null && dtUserDetail.Rows[0]["iUserType"].ToString() != "3")
                        {
                            string Fname = "", Lname = "";
                            if (dtUserDetail.Rows[0]["sFirstName"] != null && dtUserDetail.Rows[0]["sFirstName"].ToString() != "")
                            {
                                Fname = dtUserDetail.Rows[0]["sFirstName"].ToString();
                            }
                            if (dtUserDetail.Rows[0]["sLastName"] != null && dtUserDetail.Rows[0]["sLastName"].ToString() != "")
                            {
                                Lname = dtUserDetail.Rows[0]["sLastName"].ToString();
                            }
                            loSb.Append(@"<tr><td>Employee Name:</td><td>" + Fname + " " + Lname + "</td></tr>");
                        }
                        loSb.Append("</table>");
                        loSb.Append("<br/> <br/>");

                        Database db = new Database();
                        System.Collections.Generic.List<System.Data.IDbDataParameter> para;
                        para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

                        para.Add(db.CreateParam("iOfferId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, iOfferID));
                        para.Add(db.CreateParam("iUserId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, Convert.ToInt32(userID)));

                        System.Data.DataTable dtofferdetail = db.ExecuteSP("OfferDetail_SelectAllByOrderId_Email", para.ToArray(), false);

                        if (dtofferdetail != null && dtofferdetail.Rows.Count > 0)
                        {
                            loSb.Append("<div style='width: 100%;overflow-x:scroll!important;'>");
                            loSb.Append("<table border = '1' style='overflow-x:scroll !important; width:2000px !important;'>");

                            loSb.Append("<tr>");

                            string _strfont = "\"font-size: 12px; font-family: Tahoma;text-align:center; background-color: #83CAFF;\"";
                            foreach (DataColumn column in dtofferdetail.Columns)
                            {
                                loSb.Append("<th style = " + _strfont + ">");
                                loSb.Append(column.ColumnName);
                                loSb.Append("</th>");
                            }
                            loSb.Append("</tr>");

                            _strfont = "\"font-size: 10px; font-family: Tahoma;text-align:center; \"";

                            string certiNo = "";
                            foreach (DataRow row1 in dtofferdetail.Rows)
                            {
                                if (row1["Stock Id"].ToString() != "Total")
                                {
                                    loSb.Append("<tr>");
                                }
                                else
                                {
                                    loSb.Append("<tr style='background-color: #83CAFF;'>");
                                }
                                foreach (DataColumn column in dtofferdetail.Columns)
                                {
                                    string _strcheck = "";
                                    if (row1["Stock Id"].ToString() != "Total" && (column.ColumnName.ToString() == "Disc(%)" || column.ColumnName.ToString() == "Net Amt($)" || column.ColumnName.ToString() == "Offer Amt" || column.ColumnName.ToString() == "Offer Disc"))
                                    {
                                        string _strstyle = "\"font-size: 10px; font-family: Tahoma;text-align:center;color: red;\"";
                                        loSb.Append("<td style = " + _strstyle + ">");
                                    }
                                    else if (column.ColumnName.ToString() == "Cut" || column.ColumnName.ToString() == "Polish" || column.ColumnName.ToString() == "Symm")
                                    {
                                        loSb.Append("<td style = " + _strfont + ">");
                                        if (row1["Cut"].ToString() == "3EX" && row1["Polish"].ToString() == "EX" && row1["Symm"].ToString() == "EX")
                                        {
                                            loSb.Append("<b>" + row1[column.ColumnName] + "<b>");
                                            _strcheck = "Y";
                                        }
                                    }
                                    else
                                        loSb.Append("<td style = " + _strfont + ">");

                                    if (_strcheck != "Y")
                                    {
                                        if (column.ColumnName == "Image")
                                        {
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append("<a href=\"" + row1[column.ColumnName] + "\" target=\"_blank\">Image</a>");
                                            else
                                                loSb.Append("");
                                        }
                                        else if (column.ColumnName == "Video")
                                        {
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append("<a href=\"" + row1[column.ColumnName] + "\" target=\"_blank\">Video</a>");
                                            else
                                                loSb.Append("");
                                        }
                                        else if (column.ColumnName == "Dna")
                                        {
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append("<a href=\"" + row1[column.ColumnName] + "\" target=\"_blank\">Dna</a>");
                                            else
                                                loSb.Append("");
                                        }
                                        else if (column.ColumnName == "Lab")
                                        {
                                            if (row1["Certi No"] != null && row1["Certi No"].ToString() != "")
                                                certiNo = row1["Certi No"].ToString();
                                            else
                                                certiNo = "";
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append("<a href=\"http://www.gia.edu/cs/Satellite?pagename=GST%2FDispatcher&childpagename=GIA%2FPage%2FReportCheck&c=Page&cid=1355954554547&reportno=" + certiNo + "\" target=\"_blank\">"
                                                    + row1[column.ColumnName].ToString() + "</a>");
                                            else
                                                loSb.Append("");
                                        }
                                        else if (column.ColumnName == "Rap Amt($)" || column.ColumnName == "Cts"
                                             || column.ColumnName == "Rap Price($)" || column.ColumnName == "Disc(%)"
                                             || column.ColumnName == "Net Amt($)" || column.ColumnName == "Offer Disc(%)"
                                             || column.ColumnName == "Offer Amt($)" || column.ColumnName == "Offer Final Disc(%)"
                                             || column.ColumnName == "Offer Final Amt($)")
                                        {
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append(Convert.ToDecimal(row1[column.ColumnName]).ToString("#,##0.00"));
                                            else
                                                loSb.Append("");
                                        }
                                        else
                                            loSb.Append(row1[column.ColumnName]);
                                    }
                                    loSb.Append("</td>");

                                }
                                loSb.Append("</tr>");
                            }

                            loSb.Append("</table></div>");
                        }
                        loSb.Append(@"<p>Thank you for placing offer from our website www.sunrisediamonds.com.hk</p>");
                        loSb.Append(EmailSignature());


                        xloMail.Body = loSb.ToString();
                        xloMail.IsBodyHtml = true;
                        Attachment attachFile = new Attachment(_strfileName);
                        xloMail.Attachments.Add(attachFile);

                        try
                        {
                            System.Threading.Thread email = new System.Threading.Thread(delegate ()
                            {
                                xloSmtp.Send(xloMail);
                            }
                            );
                            email.IsBackground = true;
                            email.Start();
                        }
                        catch (Exception ex)
                        {
                            ex = null;
                        }
                    }
                }

                if (_strresult == "Y")
                    resp.Status = "1";
                else
                    resp.Status = "0";
                resp.Error = "";

                if (!string.IsNullOrEmpty(resp.Status))
                {
                    return Ok(resp);
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Error = "",
                        Message = "Something Went wrong.\nPlease try again later",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Error = ex.StackTrace,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult SaveOfferTransactions_1([FromBody]JObject data)
        {
            OfferTransactionsRequest offerTransReq = new OfferTransactionsRequest();

            try
            {
                offerTransReq = JsonConvert.DeserializeObject<OfferTransactionsRequest>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok("Input Parameters are not in the proper format");
            }

            CommonResponse resp = new CommonResponse();
            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                offerTransReq.UserID = userID;

                DAL.Stock objstock = new DAL.Stock();
                Int32 iOfferID = objstock.GetNewOfferID();

                string _strSuccessref = "";
                string _strerrorref = "";
                string _strSameRef = "";
                string _strSameDisc = "";

                String CustomerName = GetPartyNameByUserId(Convert.ToInt32(userID));
                Int32 AssistByID = GetAssistByUserId(Convert.ToInt32(userID));
                String[] sRefNoList = offerTransReq.StoneID.Split(',');
                String[] sDiscList = offerTransReq.OfferDiscPer.Split(',');
                String[] sValidityList = new String[] { };

                List<FortuneService.HoldStone> StoneList = new List<FortuneService.HoldStone>();
                FortuneService.ServiceSoapClient wbService = new FortuneService.ServiceSoapClient();

                if (offerTransReq.OfferValidity != null && offerTransReq.OfferValidity != "")
                {
                    sValidityList = offerTransReq.OfferValidity.Split(',');
                }

                for (int i = 0; i < sRefNoList.Length; i++)
                {
                    String sRefNo = sRefNoList[i].ToString().Trim();
                    if (sRefNo == "")
                        continue;

                    float SOffer = Convert.ToSingle(sDiscList[i].ToString());
                    Decimal sValidity = 1;
                    if (offerTransReq.OfferValidity != null && offerTransReq.OfferValidity != "")
                    {
                        sValidity = Convert.ToDecimal(sValidityList[i].ToString());
                    }

                    FortuneService.HoldStone hs = new FortuneService.HoldStone();
                    hs.ref_no = sRefNo;

                    hs.disc_per = Convert.ToDecimal(SOffer);
                    hs.rap_price = sValidity;

                    StoneList.Add(hs);

                    DataTable dtStk = objstock.Stock_SelectOne(sRefNo, Convert.ToInt32(userID));
                    if (dtStk.Rows.Count > 0)
                    {
                        float? _dLength = null, _dWidth = null, _dRepPrice = null, _dDepth = null, _dDepthPer = null, _dTablePer = null, _dCrAng = null, _dCrHt = null, _dPavAng = null, _dcts = null, _dDisc = null, _dPavHt = null;
                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dcts"].ToString()))
                            _dcts = Convert.ToSingle(dtStk.Rows[0]["dcts"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dLength"].ToString()))
                            _dLength = Convert.ToSingle(dtStk.Rows[0]["dLength"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dWidth"].ToString()))
                            _dWidth = Convert.ToSingle(dtStk.Rows[0]["dWidth"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dRepPrice"].ToString()))
                            _dRepPrice = Convert.ToSingle(dtStk.Rows[0]["dRepPrice"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dDepth"].ToString()))
                            _dDepth = Convert.ToSingle(dtStk.Rows[0]["dDepth"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dDepthPer"].ToString()))
                            _dDepthPer = Convert.ToSingle(dtStk.Rows[0]["dDepthPer"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dTablePer"].ToString()))
                            _dTablePer = Convert.ToSingle(dtStk.Rows[0]["dTablePer"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dCrAng"].ToString()))
                            _dCrAng = Convert.ToSingle(dtStk.Rows[0]["dCrAng"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dCrHt"].ToString()))
                            _dCrHt = Convert.ToSingle(dtStk.Rows[0]["dCrHt"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dPavAng"].ToString()))
                            _dPavAng = Convert.ToSingle(dtStk.Rows[0]["dPavAng"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dPavHt"].ToString()))
                            _dPavHt = Convert.ToSingle(dtStk.Rows[0]["dPavHt"].ToString());

                        if (!string.IsNullOrEmpty(dtStk.Rows[0]["dDisc"].ToString()))
                            _dDisc = Convert.ToSingle(dtStk.Rows[0]["dDisc"].ToString());


                        //objstock.Offer_Insert(iOfferID, sRefNo, dtStk.Rows[0]["sshape"].ToString(), _dcts, dtStk.Rows[0]["scolor"].ToString(), dtStk.Rows[0]["sclarity"].ToString(), _dRepPrice, dtStk.Rows[0]["sCut"].ToString(), dtStk.Rows[0]["sPolish"].ToString(), dtStk.Rows[0]["sSymm"].ToString(), dtStk.Rows[0]["sFls"].ToString(),
                        //   _dLength, _dWidth, _dDepth, _dDepthPer, _dTablePer, _dCrAng, _dCrHt, _dPavAng, _dPavHt, dtStk.Rows[0]["sCertiNo"].ToString(),
                        //                _dDisc, dtStk.Rows[0]["sLab"].ToString(), dtStk.Rows[0]["sStatus"].ToString(), SOffer, Convert.ToInt32(sValidity), dtStk.Rows[0]["sPointer"].ToString(), null, null,
                        //              dtStk.Rows[0]["sLuster"].ToString(), dtStk.Rows[0]["sInclusion"].ToString(), dtStk.Rows[0]["sTableNatts"].ToString(), dtStk.Rows[0]["sGirdleType"].ToString(), dtStk.Rows[0]["Location"].ToString(), dtStk.Rows[0]["sShade"].ToString(), dtStk.Rows[0]["sSymbol"].ToString(), Convert.ToInt32(userID), dtStk.Rows[0]["sCrownNatts"].ToString(), dtStk.Rows[0]["sCrownInclusion"].ToString());

                        _strSuccessref += "'" + sRefNo + "'" + ",";
                    }
                }

                Int32 Assistby = AssistByID;
                string Party = CustomerName;

                DAL.Usermas objUser = new DAL.Usermas();
                DataTable loUserMas = objUser.UserMas_SelectOne(Convert.ToInt64(userID));
                string _stremailid = "";
                if (String.IsNullOrEmpty(loUserMas.Rows[0]["sCompEmail"].ToString()))
                    _stremailid = loUserMas.Rows[0]["sEmail"].ToString();
                else
                    _stremailid = loUserMas.Rows[0]["sCompEmail"].ToString();

                string _strcompname = loUserMas.Rows[0]["sCompName"].ToString();

                try
                {
                    FortuneService.CommonResultResponse cResult;
                    cResult = wbService.MakeOfferTrans(_stremailid, Convert.ToString(userID), Party, iOfferID.ToString(), StoneList.ToArray());
                }
                catch
                {
                }

                if (_strerrorref != "" || _strSameRef != "" || _strSameDisc != "")
                {
                    string strMsg = "";
                    if (_strSuccessref.Length == 0)
                    {
                        strMsg = "Your offer was not uploaded due to out of discount Range. Kindly try again." + _strSameRef;
                    }
                    else
                    {
                        strMsg = "Your offer was partially uploaded. Below are the stones which are not uploaded." +
                            (_strerrorref.Length > 0 ? "\\n" + "Range of discount is not valid for the stones " + _strerrorref.TrimEnd(',') + ".Please change discount and reupload." : "") +
                            (_strSameDisc.Length > 0 ? "\\n" + "Offer already received for the stones " + _strSameDisc.TrimEnd(',') + ".Please change discount and reupload it." : "") +
                            (_strSameRef.Length > 0 ? "\\n" + "You have already upload discount for the stones " + _strSameRef.TrimEnd(',') + "." : "");
                    }

                    resp.Message = strMsg;
                }
                else
                {
                    resp.Message = "Offer placed successfully!";
                }

                String _strfileName = "";
                Random rnd = new Random();

                if (_strSuccessref.Length != 0)
                {
                    DataTable dt = objstock.Offer_Excel(Convert.ToInt32(userID), iOfferID);
                    _strfileName = HttpContext.Current.Server.MapPath("~/Temp/Excel/" + rnd.Next().ToString() + ".xlsx");

                    string _strcerti = HttpContext.Current.Server.MapPath("~/certi");

                    // If File Exists then Delete That File
                    if (File.Exists(_strfileName) == true)
                    {
                        File.Delete(_strfileName);
                    }

                    float OfferPer = 0;
                    List<Lib.Models.OfferCriteria> offerperList = GetOfferCriteriaNew();
                    if (offerperList != null && offerperList.Count > 0)
                        OfferPer = offerperList[0].OfferPer;

                    EpExcelExport.excel_offer(dt, _strfileName, OfferPer);
                }

                MailMessage xloMail = new MailMessage();
                SmtpClient xloSmtp = new SmtpClient();

                DataTable loEmails = objUser.UserMas_SelectEmailByUserId(Convert.ToInt32(userID));
                string lsToMail = _stremailid + ",";
                foreach (DataRow lrEmail in loEmails.Rows)
                    lsToMail += lrEmail["sEmail"].ToString() + ",";
                if (lsToMail.Length > 0)
                    lsToMail = lsToMail.Remove(lsToMail.Length - 1);

                string _strresult = "";
                if (_stremailid.Trim() != "")
                {
                    if (_strfileName != "")
                    {
                        _strresult = "Y";
                        xloMail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromEmail"], "Sunrise Diamonds");
                        //xloMail.To.Add(_stremailid);

                        xloMail.To.Add(lsToMail);
                        string userName = Convert.ToString((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserName").FirstOrDefault().Value);
                        //xloMail.Body = "Dear Valued Customer \nThank you very much making an offer. \nwe will update you soon. \nThanks and Regards \n\n\nSunrise Diamonds Team, \n www.sunrisediamonds.com.hk";
                        xloMail.Subject = "SUNRISE DIAMONDS : Offer Acknowledgement for Offer ID " + iOfferID.ToString();
                        //xloMail.Body = "Dear " + userName + ", " + _strcompname + " \n\n" +
                        //              "Thank you for placing an offer at Sunrise Diamonds." + "\n" +
                        //              "Your Offer ID is " + iOfferID + ". Kindly use this Offer ID for tracing the status of your offer." + "\n" +
                        //              "Please find the confidential details of your offer in the file enclosed with this email." + "\n" +
                        //              "You will be informed about the status of your offer via email or our online system. We, at Sunrise Diamonds value our relationship with you." + "\n\n" +
                        //              "Thanking you," + "\n" +
                        //              "The Sunrise Diamonds Team";
                        Database db1 = new Database();
                        System.Collections.Generic.List<System.Data.IDbDataParameter> para1;
                        para1 = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

                        para1.Clear();
                        para1.Add(db1.CreateParam("iiUserId", System.Data.DbType.String, System.Data.ParameterDirection.Input, Convert.ToInt32(userID)));
                        System.Data.DataTable dtUserDetail = db1.ExecuteSP("UserMas_SelectOne", para1.ToArray(), false);

                        StringBuilder loSb = new StringBuilder();
                        loSb.Append(EmailHeader());
                        loSb.Append(@"<table cellpadding=""0"" cellspacing=""0"" width=""100%"">");
                        if (dtUserDetail.Rows[0]["sCompName"] != null && dtUserDetail.Rows[0]["sCompName"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Company Name:</td><td>" + dtUserDetail.Rows[0]["sCompName"].ToString() + "(" + dtUserDetail.Rows[0]["sCompMobile"].ToString() + ")</td></tr>");
                            //  loSb.Append(@"<tr><td colspan=""2"">" + dtUserDetail.Rows[0]["sCompName"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["sCompAddress"] != null && dtUserDetail.Rows[0]["sCompAddress"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Address:</td><td>" + dtUserDetail.Rows[0]["sCompAddress"].ToString() + "</td></tr>");
                            //  loSb.Append(@"<tr><td colspan=""2"">" + dtUserDetail.Rows[0]["sCompName"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["AssistBy1"] != null && dtUserDetail.Rows[0]["AssistBy1"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Sales Person:</td><td>" + dtUserDetail.Rows[0]["AssistBy1"].ToString() + "</td></tr>");
                            //loSb.Append(@"<tr><td colspan=""2"">(Sales Person):" + dtUserDetail.Rows[0]["AssistBy1"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["mob_AssistBy1"] != null && dtUserDetail.Rows[0]["mob_AssistBy1"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Mobile:</td><td>" + dtUserDetail.Rows[0]["mob_AssistBy1"].ToString() + "</td></tr>");
                            loSb.Append(@"<tr><td>Whatsapp No:</td><td>" + dtUserDetail.Rows[0]["mob_AssistBy1"].ToString() + "</td></tr>");
                            //loSb.Append(@"<tr><td colspan=""2"">(Sales Person):" + dtUserDetail.Rows[0]["AssistBy1"].ToString() + "</td></tr>");
                        }
                        //loSb.Append(@"<tr><td colspan=""2"">" + fsMobile + "</td></tr>");
                        if (dtUserDetail.Rows[0]["sWeChatId1"] != null && dtUserDetail.Rows[0]["sWeChatId1"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>WeChat ID:</td><td>" + dtUserDetail.Rows[0]["sWeChatId1"].ToString() + "</td></tr>");
                            // loSb.Append(@"<tr><td colspan=""2"">" + dtUserDetail.Rows[0]["sWeChatId1"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["Email_AssistBy1"] != null && dtUserDetail.Rows[0]["Email_AssistBy1"].ToString() != "")
                        {
                            loSb.Append(@"<tr><td>Email:</td><td>" + dtUserDetail.Rows[0]["Email_AssistBy1"].ToString() + "</td></tr>");
                            // loSb.Append(@"<tr><td colspan=""2"">" + dtUserDetail.Rows[0]["sWeChatId1"].ToString() + "</td></tr>");
                        }
                        if (dtUserDetail.Rows[0]["iUserType"] != null && dtUserDetail.Rows[0]["iUserType"].ToString() != "3")
                        {
                            string Fname = "", Lname = "";
                            if (dtUserDetail.Rows[0]["sFirstName"] != null && dtUserDetail.Rows[0]["sFirstName"].ToString() != "")
                            {
                                Fname = dtUserDetail.Rows[0]["sFirstName"].ToString();
                            }
                            if (dtUserDetail.Rows[0]["sLastName"] != null && dtUserDetail.Rows[0]["sLastName"].ToString() != "")
                            {
                                Lname = dtUserDetail.Rows[0]["sLastName"].ToString();
                            }
                            loSb.Append(@"<tr><td>Employee Name:</td><td>" + Fname + " " + Lname + "</td></tr>");
                            // loSb.Append(@"<tr><td colspan=""2"">" + dtUserDetail.Rows[0]["sWeChatId1"].ToString() + "</td></tr>");
                        }
                        loSb.Append("</table>");
                        loSb.Append("<br/> <br/>");

                        Database db = new Database();
                        System.Collections.Generic.List<System.Data.IDbDataParameter> para;
                        para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

                        para.Add(db.CreateParam("iOfferId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, iOfferID));
                        para.Add(db.CreateParam("iUserId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, Convert.ToInt32(userID)));

                        System.Data.DataTable dtofferdetail = db.ExecuteSP("OfferDetail_SelectAllByOrderId_Email", para.ToArray(), false);

                        //Building an HTML string.

                        //Table start.
                        if (dtofferdetail != null && dtofferdetail.Rows.Count > 0)
                        {
                            loSb.Append("<div style='width: 100%;overflow-x:scroll!important;'>");
                            loSb.Append("<table border = '1' style='overflow-x:scroll !important; width:2000px !important;'>");

                            //Building the Header row.
                            loSb.Append("<tr>");

                            string _strfont = "\"font-size: 12px; font-family: Tahoma;text-align:center; background-color: #83CAFF;\"";
                            foreach (DataColumn column in dtofferdetail.Columns)
                            {
                                loSb.Append("<th style = " + _strfont + ">");
                                loSb.Append(column.ColumnName);
                                loSb.Append("</th>");
                            }
                            loSb.Append("</tr>");

                            _strfont = "\"font-size: 10px; font-family: Tahoma;text-align:center; \"";
                            //Building the Data rows.

                            string certiNo = "";
                            foreach (DataRow row1 in dtofferdetail.Rows)
                            {
                                if (row1["Stock Id"].ToString() != "Total")
                                {
                                    loSb.Append("<tr>");
                                }
                                else
                                {
                                    loSb.Append("<tr style='background-color: #83CAFF;'>");
                                }
                                foreach (DataColumn column in dtofferdetail.Columns)
                                {
                                    string _strcheck = "";
                                    if (row1["Stock Id"].ToString() != "Total" && (column.ColumnName.ToString() == "Disc(%)" || column.ColumnName.ToString() == "Net Amt($)" || column.ColumnName.ToString() == "Offer Amt" || column.ColumnName.ToString() == "Offer Disc"))
                                    {
                                        string _strstyle = "\"font-size: 10px; font-family: Tahoma;text-align:center;color: red;\"";
                                        loSb.Append("<td style = " + _strstyle + ">");
                                    }
                                    else if (column.ColumnName.ToString() == "Cut" || column.ColumnName.ToString() == "Polish" || column.ColumnName.ToString() == "Symm")
                                    {
                                        loSb.Append("<td style = " + _strfont + ">");
                                        if (row1["Cut"].ToString() == "3EX" && row1["Polish"].ToString() == "EX" && row1["Symm"].ToString() == "EX")
                                        {
                                            loSb.Append("<b>" + row1[column.ColumnName] + "<b>");
                                            _strcheck = "Y";
                                        }
                                    }
                                    else
                                        loSb.Append("<td style = " + _strfont + ">");

                                    if (_strcheck != "Y")
                                    {
                                        if (column.ColumnName == "Image")
                                        {
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append("<a href=\"" + row1[column.ColumnName] + "\" target=\"_blank\">Image</a>");
                                            else
                                                loSb.Append("");
                                        }
                                        else if (column.ColumnName == "Video")
                                        {
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append("<a href=\"" + row1[column.ColumnName] + "\" target=\"_blank\">Video</a>");
                                            else
                                                loSb.Append("");
                                        }
                                        else if (column.ColumnName == "Dna")
                                        {
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append("<a href=\"" + row1[column.ColumnName] + "\" target=\"_blank\">Dna</a>");
                                            else
                                                loSb.Append("");
                                        }
                                        else if (column.ColumnName == "Lab")
                                        {
                                            if (row1["Certi No"] != null && row1["Certi No"].ToString() != "")
                                                certiNo = row1["Certi No"].ToString();
                                            else
                                                certiNo = "";
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append("<a href=\"http://www.gia.edu/cs/Satellite?pagename=GST%2FDispatcher&childpagename=GIA%2FPage%2FReportCheck&c=Page&cid=1355954554547&reportno=" + certiNo + "\" target=\"_blank\">"
                                                    + row1[column.ColumnName].ToString() + "</a>");
                                            else
                                                loSb.Append("");
                                        }
                                        else if (column.ColumnName == "Rap Amt($)" || column.ColumnName == "Cts"
                                             || column.ColumnName == "Rap Price($)" || column.ColumnName == "Disc(%)"
                                             || column.ColumnName == "Net Amt($)" || column.ColumnName == "Offer Disc"
                                             || column.ColumnName == "Offer Amt")
                                        {
                                            if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                                loSb.Append(Convert.ToDecimal(row1[column.ColumnName]).ToString("#,##0.00"));
                                            else
                                                loSb.Append("");
                                        }
                                        else
                                            loSb.Append(row1[column.ColumnName]);
                                    }
                                    loSb.Append("</td>");

                                }
                                loSb.Append("</tr>");
                            }

                            //Table end.
                            loSb.Append("</table></div>");
                        }
                        loSb.Append(@"<p>Thank you for placing offer from our website www.sunrisediamonds.com.hk</p>");
                        //////
                        loSb.Append(EmailSignature());


                        xloMail.Body = loSb.ToString();
                        xloMail.IsBodyHtml = true;
                        Attachment attachFile = new Attachment(_strfileName);
                        xloMail.Attachments.Add(attachFile);

                        try
                        {
                            System.Threading.Thread email = new System.Threading.Thread(delegate ()
                            {
                                xloSmtp.Send(xloMail);
                            }
                            );
                            email.IsBackground = true;
                            email.Start();
                        }
                        catch (Exception ex)
                        {
                            ex = null;
                        }
                    }
                }

                if (_strresult == "Y")
                    resp.Status = "1";
                else
                    resp.Status = "0";
                resp.Error = "";

                //return resp;


                if (!string.IsNullOrEmpty(resp.Status))
                {
                    return Ok(resp);
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Message = "Something Went wrong.",
                        Status = "0",
                        Error = ""
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0",
                    Error = ex.StackTrace
                });
            }
        }
        [HttpPost]
        public IHttpActionResult DownloadOfferExcel([FromBody]JObject data)
        {
            SearchDiamondsRequest searchDiamondsRequest = new SearchDiamondsRequest();
            try
            {
                searchDiamondsRequest = JsonConvert.DeserializeObject<SearchDiamondsRequest>(data.ToString());
                searchDiamondsRequest.StoneStatus = "O";
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok("Input Parameters are not in the proper format");
            }

            try
            {
                DataTable dtData = new DataTable();
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                searchDiamondsRequest.UserID = userID;

                if (userID != 6492)// restrict for jbbrothers username
                {
                    dtData = new StockController().SearchStockInner(searchDiamondsRequest);
                    string fileName = "Offer Stone " + Lib.Models.Common.GetHKTime().ToString("ddMMyyyy-HHmmss") + ".xlsx";
                    string _path = ConfigurationManager.AppSettings["data"];
                    string _realpath = HostingEnvironment.MapPath("~/ExcelFile/");

                    dtData.DefaultView.RowFilter = "P_SEQ_NO IS NOT NULL";
                    dtData = dtData.DefaultView.ToTable();
                    dtData.Columns.Add("Offer_Disc");
                    dtData.Columns.Add("Valid_Days");

                    bool? fileCreated = CreateOfferExcel(dtData, _realpath, fileName);
                    string _str = string.Empty;
                    if (fileCreated != null)
                    {
                        if ((bool)fileCreated)
                        {
                            _str = _path + fileName;
                            return Ok(_str);
                        }
                    }
                }
                return Ok("No data found.");
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Lib.Models.OfferCriteria>
                {
                    Data = new List<Lib.Models.OfferCriteria>(),
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        [NonAction]
        private String GetPartyNameByUserId(Int32 UserId)
        {
            Database db = new Database();
            List<IDbDataParameter> para = new List<IDbDataParameter>
            {
                db.CreateParam("iUserId", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(UserId))
            };

            DataTable dt = db.ExecuteSP("UserMas_SelectByPara", para.ToArray(), false);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["sCompName"].ToString().Length > 0)
                    return dt.Rows[0]["sCompName"].ToString();
                else
                    return dt.Rows[0]["sFirstName"].ToString();
            }
            else
            {
                return "";
            }
        }
        [NonAction]
        private Int64 Get_FortunePartyCode_ByUserId(Int32 UserId)
        {
            Database db = new Database();
            List<IDbDataParameter> para = new List<IDbDataParameter>
            {
                db.CreateParam("iUserId", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(UserId))
            };

            DataTable dt = db.ExecuteSP("UserMas_SelectByPara", para.ToArray(), false);

            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToInt64(dt.Rows[0]["FortunePartyCode"] != null ? dt.Rows[0]["FortunePartyCode"] : 0);
            }
            else
            {
                return 0;
            }
        }

        [NonAction]
        private Int32 GetAssistByUserId(Int32 UserId)
        {
            Database db = new Database();
            List<IDbDataParameter> para = new List<IDbDataParameter>
            {
                db.CreateParam("p_for_userid", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(UserId))
            };

            DataTable dt = db.ExecuteSP("get_assist_by_emp", para.ToArray(), false);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["iEmpid"].ToString().Length > 0)
                    return Convert.ToInt32(dt.Rows[0]["iEmpid"]);
            }

            //User Id of Jigneshbhai (JIGIJIGA) // why jigneshbhai only in return question from divya rana 
            return 10;
        }

        //[NonAction]
        //private bool? CreateOfferExcel(DataTable ExportTable, string FolderPath, string FileName)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        DataTableExcelExport ge;
        //        if (ExportTable.Rows.Count == 0)
        //            return null;

        //        ge = new DataTableExcelExport(ExportTable, "StoneSelection", "StoneSelection");

        //        ge.BeforeCreateColumnEvent += BeforeCreateColumnEventHandler;
        //        ge.AfterCreateCellEvent += AfterCreateCellEventHandler;
        //        ge.FillingWorksheetEvent += this.FillingWorksheetEventHandler;
        //        //ge.AddHeaderEvent += this.AddHeaderEventHandler;

        //        string parentPath = FolderPath;
        //        string fileName = string.Empty;
        //        if (ConfigurationManager.AppSettings["ConnMode"] == "Oracle")
        //            parentPath = @"C:\inetpub\wwwroot\Temp\";

        //        fileName = parentPath + FileName;

        //        MemoryStream ms = new MemoryStream();
        //        ge.CreateExcel(ms);
        //        File.WriteAllBytes(fileName, ms.ToArray());

        //        //EpExcelExport.excel_offer(ExportTable,fileName);
        //        flag = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        flag = false;
        //        throw ex;
        //    }
        //    return flag;
        //}

        [NonAction]
        private bool? CreateOfferExcel(DataTable dtDiamonds, string FolderPath, string FileName)
        {
            bool flag = false;
            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header

                    p.Workbook.Properties.Author = "SUNRISE DIAMOND";
                    p.Workbook.Properties.Title = "SUNRISE DIAMOND PVT. LTD.";

                    //Create a sheet
                    p.Workbook.Worksheets.Add("DOSSIERS");

                    ExcelWorksheet worksheet = p.Workbook.Worksheets[1];
                    worksheet.Name = "StoneSelection";

                    worksheet.Row(1).Height = 40;
                    worksheet.Row(1).Style.WrapText = true;

                    worksheet.Cells[1, 1, 1, 42].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1, 1, 42].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells[1, 1, 1, 42].Style.Font.Size = 10;
                    worksheet.Cells[1, 1, 1, 42].Style.Font.Bold = true;
                    worksheet.Cells[1, 1, 1, 42].AutoFilter = true;
                    worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[1, 1, 1, 42].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#d3d3d3");
                    worksheet.Cells[1, 1, 1, 42].Style.Fill.BackgroundColor.SetColor(colFromHex);

                    #endregion

                    #region Header Name Declaration

                    worksheet.Cells[1, 1].Value = "DNA";
                    worksheet.Cells[1, 2].Value = "Location";
                    worksheet.Cells[1, 3].Value = "Status";
                    worksheet.Cells[1, 4].Value = "Stock ID";
                    worksheet.Cells[1, 5].Value = "Shape";
                    worksheet.Cells[1, 6].Value = "Pointer";
                    worksheet.Cells[1, 7].Value = "Lab";
                    worksheet.Cells[1, 8].Value = "Certi No.";
                    worksheet.Cells[1, 9].Value = "BGM";
                    worksheet.Cells[1, 10].Value = "Color";
                    worksheet.Cells[1, 11].Value = "Clarity";
                    worksheet.Cells[1, 12].Value = "Cts";
                    worksheet.Cells[1, 13].Value = "Rap Price($)";
                    worksheet.Cells[1, 14].Value = "Rap Amt($)";
                    worksheet.Cells[1, 15].Value = "Disc(%)";
                    worksheet.Cells[1, 16].Value = "Net Amt($)";
                    worksheet.Cells[1, 17].Value = "Price/Cts";
                    worksheet.Cells[1, 18].Value = "Offer Disc(%)";
                    worksheet.Cells[1, 19].Value = "Offer Valid Days";
                    worksheet.Cells[1, 20].Value = "Offer Remark";
                    worksheet.Cells[1, 21].Value = "Cut";
                    worksheet.Cells[1, 22].Value = "Polish";
                    worksheet.Cells[1, 23].Value = "Symm";
                    worksheet.Cells[1, 24].Value = "Fls";
                    worksheet.Cells[1, 25].Value = "Length";
                    worksheet.Cells[1, 26].Value = "Width";
                    worksheet.Cells[1, 27].Value = "Depth";
                    worksheet.Cells[1, 28].Value = "Depth(%)";
                    worksheet.Cells[1, 29].Value = "Table(%)";
                    worksheet.Cells[1, 30].Value = "Key To Symbol";
                    worksheet.Cells[1, 31].Value = "Table White";
                    worksheet.Cells[1, 32].Value = "Crown White";
                    worksheet.Cells[1, 33].Value = "Table Black";
                    worksheet.Cells[1, 34].Value = "Crown Black";
                    worksheet.Cells[1, 35].Value = "Cr Ang";
                    worksheet.Cells[1, 36].Value = "Cr Ht";
                    worksheet.Cells[1, 37].Value = "Pav Ang";
                    worksheet.Cells[1, 38].Value = "Pav Ht";
                    worksheet.Cells[1, 39].Value = "Girdle Type";
                    worksheet.Cells[1, 40].Value = "Laser Insc";
                    worksheet.Cells[1, 41].Value = "Image";
                    worksheet.Cells[1, 42].Value = "HD Movie";


                    ExcelStyle cellStyleHeader1 = worksheet.Cells[1, 1, 1, 42].Style;
                    cellStyleHeader1.Border.Left.Style = cellStyleHeader1.Border.Right.Style
                            = cellStyleHeader1.Border.Top.Style = cellStyleHeader1.Border.Bottom.Style
                            = ExcelBorderStyle.Medium;

                    #endregion

                    int inStartIndex = 2;
                    int inwrkrow = 2;
                    int inEndCounter = dtDiamonds.Rows.Count + inStartIndex;
                    int TotalRow = dtDiamonds.Rows.Count;

                    #region Set AutoFit and Decimal Number Format

                    worksheet.View.FreezePanes(2, 1);
                    worksheet.Cells[1, 1].AutoFitColumns(9.50);
                    worksheet.Cells[1, 2].AutoFitColumns(10);
                    worksheet.Cells[1, 3].AutoFitColumns(17);
                    worksheet.Cells[1, 4].AutoFitColumns(10);
                    worksheet.Cells[1, 5].AutoFitColumns(10);
                    worksheet.Cells[1, 6].AutoFitColumns(8.50);
                    worksheet.Cells[1, 7].AutoFitColumns(8.50);
                    worksheet.Cells[1, 8].AutoFitColumns(14);
                    worksheet.Cells[1, 9].AutoFitColumns(8.50);
                    worksheet.Cells[1, 10].AutoFitColumns(8.50);
                    worksheet.Cells[1, 11].AutoFitColumns(8.50);
                    worksheet.Cells[1, 12].AutoFitColumns(8.50);
                    worksheet.Cells[1, 13].AutoFitColumns(9.5);
                    worksheet.Cells[1, 14].AutoFitColumns(11);
                    worksheet.Cells[1, 15].AutoFitColumns(8.50);
                    worksheet.Cells[1, 16].AutoFitColumns(11);
                    worksheet.Cells[1, 17].AutoFitColumns(11);
                    worksheet.Cells[1, 18].AutoFitColumns(8.50);
                    worksheet.Cells[1, 19].AutoFitColumns(8.50);
                    worksheet.Cells[1, 20].AutoFitColumns(18);
                    worksheet.Cells[1, 21].AutoFitColumns(8.50);
                    worksheet.Cells[1, 22].AutoFitColumns(8.50);
                    worksheet.Cells[1, 23].AutoFitColumns(8.50);
                    worksheet.Cells[1, 24].AutoFitColumns(8.50);
                    worksheet.Cells[1, 25].AutoFitColumns(9.50);
                    worksheet.Cells[1, 26].AutoFitColumns(9.50);
                    worksheet.Cells[1, 27].AutoFitColumns(9.50);
                    worksheet.Cells[1, 28].AutoFitColumns(9.5);
                    worksheet.Cells[1, 29].AutoFitColumns(9.5);
                    worksheet.Cells[1, 30].AutoFitColumns(40);
                    worksheet.Cells[1, 31].AutoFitColumns(9);
                    worksheet.Cells[1, 32].AutoFitColumns(9);
                    worksheet.Cells[1, 33].AutoFitColumns(9);
                    worksheet.Cells[1, 34].AutoFitColumns(9);
                    worksheet.Cells[1, 35].AutoFitColumns(7.86);
                    worksheet.Cells[1, 36].AutoFitColumns(7.86);
                    worksheet.Cells[1, 37].AutoFitColumns(7.86);
                    worksheet.Cells[1, 38].AutoFitColumns(7.86);
                    worksheet.Cells[1, 39].AutoFitColumns(7.86);
                    worksheet.Cells[1, 40].AutoFitColumns(7.86);
                    worksheet.Cells[1, 41].AutoFitColumns(7.86);
                    worksheet.Cells[1, 42].AutoFitColumns(7.86);

                    //Set Cell Faoat value with Alignment
                    worksheet.Cells[inStartIndex, 1, inEndCounter, 42].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    #endregion

                    var namedStyle = p.Workbook.Styles.CreateNamedStyle("HyperLink");
                    namedStyle.Style.Font.UnderLine = true;
                    namedStyle.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    namedStyle.Style.Font.Size = 11;
                    namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    namedStyle.Style.Font.Name = "Calibri";
                    int i;
                    String values_1;
                    Int64 number_1;
                    bool success1;
                    var asTitleCase = Thread.CurrentThread.CurrentCulture.TextInfo;
                    string Image, Video, dna, hyprlink1, status, cut;
                    for (i = inStartIndex; i < inEndCounter; i++)
                    {
                        #region Assigns Value to Cell
                        dna = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["view_dna"]);

                        if (dna != "")
                        {
                            worksheet.Cells[inwrkrow, 1].Formula = "=HYPERLINK(\"" + dna + "\",\" DNA \")";
                            worksheet.Cells[inwrkrow, 1].Style.Font.UnderLine = true;
                            worksheet.Cells[inwrkrow, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        }

                        worksheet.Cells[inwrkrow, 2].Value = asTitleCase.ToTitleCase(Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["Location"]).ToLower());

                        status = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["status"]).ToLower();
                        if (status == "available offer")
                            status = "Offer";
                        else if (status == "buss. process")
                            status = "Busy";

                        worksheet.Cells[inwrkrow, 3].Value = asTitleCase.ToTitleCase(status);

                        values_1 = dtDiamonds.Rows[i - inStartIndex]["stone_ref_no"].ToString();
                        success1 = Int64.TryParse(values_1, out number_1);
                        if (success1)
                        {
                            worksheet.Cells[inwrkrow, 4].Value = Convert.ToInt64(dtDiamonds.Rows[i - inStartIndex]["stone_ref_no"]);
                        }
                        else
                        {
                            worksheet.Cells[inwrkrow, 4].Value = values_1;
                        }
                        worksheet.Cells[inwrkrow, 5].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["shape"]);
                        worksheet.Cells[inwrkrow, 6].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["pointer"]);
                        worksheet.Cells[inwrkrow, 7].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["lab"]);

                        values_1 = dtDiamonds.Rows[i - inStartIndex]["certi_no"].ToString();
                        success1 = Int64.TryParse(values_1, out number_1);
                        if (success1)
                        {
                            worksheet.Cells[inwrkrow, 8].Value = Convert.ToInt64(dtDiamonds.Rows[i - inStartIndex]["certi_no"]);
                        }
                        else
                        {
                            worksheet.Cells[inwrkrow, 8].Value = values_1;
                        }

                        worksheet.Cells[inwrkrow, 9].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["BGM"]);
                        worksheet.Cells[inwrkrow, 10].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["color"]);
                        worksheet.Cells[inwrkrow, 11].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["clarity"]);
                        worksheet.Cells[inwrkrow, 12].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["cts"]);

                        worksheet.Cells[inwrkrow, 13].Value = dtDiamonds.Rows[i - inStartIndex]["cur_rap_rate"].GetType().Name != "DBNull" ?
                               Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["cur_rap_rate"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 14].Value = dtDiamonds.Rows[i - inStartIndex]["rap_amount"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["rap_amount"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 15].Value = dtDiamonds.Rows[i - inStartIndex]["sales_disc_per"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["sales_disc_per"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 16].Value = dtDiamonds.Rows[i - inStartIndex]["net_amount"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["net_amount"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 17].Value = dtDiamonds.Rows[i - inStartIndex]["price_per_cts"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["price_per_cts"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 18].Value =
                            (
                            dtDiamonds.Rows[i - inStartIndex]["offerDisc"].ToString() != "" ?
                            Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["offerDisc"])
                            :
                            (dtDiamonds.Rows[i - inStartIndex]["Offer_Disc"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["Offer_Disc"]) : ((Double?)null))
                            );

                        worksheet.Cells[inwrkrow, 19].Value =
                            (
                            dtDiamonds.Rows[i - inStartIndex]["validDays"].ToString() != "" ?
                            Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["validDays"])
                            :
                            (dtDiamonds.Rows[i - inStartIndex]["Valid_Days"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["Valid_Days"]) : ((Double?)null))
                            );

                        worksheet.Cells[inwrkrow, 20].Value = dtDiamonds.Rows[i - inStartIndex]["Offer_Remark"].GetType().Name != "DBNull" ?
                                Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["Offer_Remark"]) : ((String)null);


                        cut = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["cut"]);
                        worksheet.Cells[inwrkrow, 21].Value = (cut == "FR" ? "F" : cut);
                        worksheet.Cells[inwrkrow, 22].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["polish"]);
                        worksheet.Cells[inwrkrow, 23].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["symm"]);

                        if (Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["cut"]) == "3EX")
                        {
                            worksheet.Cells[inwrkrow, 21].Style.Font.Bold = true;
                            worksheet.Cells[inwrkrow, 22].Style.Font.Bold = true;
                            worksheet.Cells[inwrkrow, 23].Style.Font.Bold = true;
                        }

                        worksheet.Cells[inwrkrow, 24].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["fls"]);
                        worksheet.Cells[inwrkrow, 25].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["length"]);
                        worksheet.Cells[inwrkrow, 26].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["width"]);
                        worksheet.Cells[inwrkrow, 27].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["depth"]);
                        worksheet.Cells[inwrkrow, 28].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["depth_per"]);
                        worksheet.Cells[inwrkrow, 29].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["table_per"]);
                        worksheet.Cells[inwrkrow, 30].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["symbol"]);
                        worksheet.Cells[inwrkrow, 31].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["inclusion"]);
                        worksheet.Cells[inwrkrow, 32].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["Crown_Inclusion"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["Crown_Inclusion"]);
                        worksheet.Cells[inwrkrow, 33].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["table_natts"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["table_natts"]);
                        worksheet.Cells[inwrkrow, 34].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["Crown_Natts"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["Crown_Natts"]);
                        worksheet.Cells[inwrkrow, 35].Value = dtDiamonds.Rows[i - inStartIndex]["crown_angle"] == null ? 0 : dtDiamonds.Rows[i - inStartIndex]["crown_angle"].ToString() == "" ? 0 : dtDiamonds.Rows[i - inStartIndex]["crown_angle"];
                        worksheet.Cells[inwrkrow, 36].Value = dtDiamonds.Rows[i - inStartIndex]["crown_height"] == null ? 0 : dtDiamonds.Rows[i - inStartIndex]["crown_height"].ToString() == "" ? 0 : dtDiamonds.Rows[i - inStartIndex]["crown_height"];
                        worksheet.Cells[inwrkrow, 37].Value = dtDiamonds.Rows[i - inStartIndex]["pav_angle"] == null ? 0 : dtDiamonds.Rows[i - inStartIndex]["pav_angle"].ToString() == "" ? 0 : dtDiamonds.Rows[i - inStartIndex]["pav_angle"];
                        worksheet.Cells[inwrkrow, 38].Value = dtDiamonds.Rows[i - inStartIndex]["pav_height"] == null ? 0 : dtDiamonds.Rows[i - inStartIndex]["pav_height"].ToString() == "" ? 0 : dtDiamonds.Rows[i - inStartIndex]["pav_height"];
                        worksheet.Cells[inwrkrow, 39].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["girdle_type"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["girdle_type"]);
                        worksheet.Cells[inwrkrow, 40].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["sInscription"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["sInscription"]);

                        if (dtDiamonds.Rows[i - inStartIndex]["image_url"] != null)
                        {
                            Image = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["image_url"]);
                            if (Image != "")
                            {
                                hyprlink1 = External_ImageURL + Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["certi_no"]) + "/PR.jpg";
                                worksheet.Cells[inwrkrow, 41].Formula = "=HYPERLINK(\"" + hyprlink1 + "\",\" Image \")";
                                worksheet.Cells[inwrkrow, 41].Style.Font.UnderLine = true;
                                worksheet.Cells[inwrkrow, 41].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                            }
                        }

                        if (dtDiamonds.Rows[i - inStartIndex]["movie_url"] != null)
                        {
                            Video = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["movie_url"]);
                            if (Video != "")
                            {
                                hyprlink1 = Video;
                                worksheet.Cells[inwrkrow, 42].Formula = "=HYPERLINK(\"" + hyprlink1 + "\",\" Video \")";
                                worksheet.Cells[inwrkrow, 42].Style.Font.UnderLine = true;
                                worksheet.Cells[inwrkrow, 42].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                            }
                        }

                        inwrkrow++;

                        #endregion
                    }

                    worksheet.Cells[2, 1, inwrkrow - 1, 42].Style.Font.Size = 9;

                    worksheet.Cells[1, 6, inwrkrow - 1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    System.Drawing.Color colFromHex_Pointer = System.Drawing.ColorTranslator.FromHtml("#c6e0b4");
                    worksheet.Cells[1, 6, inwrkrow - 1, 6].Style.Fill.BackgroundColor.SetColor(colFromHex_Pointer);

                    worksheet.Cells[1, 15, inwrkrow - 1, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    System.Drawing.Color colFromHex_Dis = System.Drawing.ColorTranslator.FromHtml("#ccffff");
                    worksheet.Cells[1, 15, inwrkrow - 1, 15].Style.Fill.BackgroundColor.SetColor(colFromHex_Dis);

                    worksheet.Cells[1, 16, inwrkrow - 1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 16, inwrkrow - 1, 16].Style.Fill.BackgroundColor.SetColor(colFromHex_Dis);

                    worksheet.Cells[1, 12, inwrkrow - 1, 17].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[1, 24, inwrkrow - 1, 28].Style.Numberformat.Format = "0.00";
                    worksheet.Cells[1, 34, inwrkrow - 1, 37].Style.Numberformat.Format = "0.00";

                    worksheet.Cells[1, 15, inwrkrow - 1, 16].Style.Font.Bold = true;
                    worksheet.Cells[2, 15, inwrkrow - 1, 16].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    worksheet.Cells[1, 15, inwrkrow - 1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 15, inwrkrow - 1, 16].Style.Fill.BackgroundColor.SetColor(colFromHex_Dis);


                    string parentPath = FolderPath;
                    string fileName = string.Empty;
                    if (ConfigurationManager.AppSettings["ConnMode"] == "Oracle")
                        parentPath = @"C:\inetpub\wwwroot\Temp\";

                    fileName = parentPath + FileName;
                    Byte[] bin = p.GetAsByteArray();

                    if (!Directory.Exists(parentPath))
                    {
                        Directory.CreateDirectory(parentPath);
                    }

                    System.IO.File.WriteAllBytes(fileName, bin);

                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                throw ex;
            }
            return flag;
        }
        [NonAction]
        private bool? CreateOfferExcel_1(DataTable dtDiamonds, string FolderPath, string FileName)
        {
            bool flag = false;
            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    #region Company Detail on Header

                    p.Workbook.Properties.Author = "SUNRISE DIAMOND";
                    p.Workbook.Properties.Title = "SUNRISE DIAMOND PVT. LTD.";

                    //Create a sheet
                    p.Workbook.Worksheets.Add("DOSSIERS");

                    ExcelWorksheet worksheet = p.Workbook.Worksheets[1];
                    worksheet.Name = "StoneSelection";

                    worksheet.Row(1).Height = 40;
                    worksheet.Row(1).Style.WrapText = true;

                    worksheet.Cells[1, 1, 1, 41].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1, 1, 41].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Cells[1, 1, 1, 41].Style.Font.Size = 10;
                    worksheet.Cells[1, 1, 1, 41].Style.Font.Bold = true;
                    worksheet.Cells[1, 1, 1, 41].AutoFilter = true;
                    worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[1, 1, 1, 41].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#d3d3d3");
                    worksheet.Cells[1, 1, 1, 41].Style.Fill.BackgroundColor.SetColor(colFromHex);

                    #endregion

                    #region Header Name Declaration

                    worksheet.Cells[1, 1].Value = "DNA";
                    worksheet.Cells[1, 2].Value = "Location";
                    worksheet.Cells[1, 3].Value = "Status";
                    worksheet.Cells[1, 4].Value = "Stock ID";
                    worksheet.Cells[1, 5].Value = "Shape";
                    worksheet.Cells[1, 6].Value = "Pointer";
                    worksheet.Cells[1, 7].Value = "Lab";
                    worksheet.Cells[1, 8].Value = "Certi No.";
                    worksheet.Cells[1, 9].Value = "BGM";
                    worksheet.Cells[1, 10].Value = "Color";
                    worksheet.Cells[1, 11].Value = "Clarity";
                    worksheet.Cells[1, 12].Value = "Cts";
                    worksheet.Cells[1, 13].Value = "Rap Price($)";
                    worksheet.Cells[1, 14].Value = "Rap Amt($)";
                    worksheet.Cells[1, 15].Value = "Disc(%)";
                    worksheet.Cells[1, 16].Value = "Net Amt($)";
                    worksheet.Cells[1, 17].Value = "Price/Cts";
                    worksheet.Cells[1, 18].Value = "Offer Disc(%)";
                    worksheet.Cells[1, 19].Value = "Valid Days";
                    worksheet.Cells[1, 20].Value = "Cut";
                    worksheet.Cells[1, 21].Value = "Polish";
                    worksheet.Cells[1, 22].Value = "Symm";
                    worksheet.Cells[1, 23].Value = "Fls";
                    worksheet.Cells[1, 24].Value = "Length";
                    worksheet.Cells[1, 25].Value = "Width";
                    worksheet.Cells[1, 26].Value = "Depth";
                    worksheet.Cells[1, 27].Value = "Depth(%)";
                    worksheet.Cells[1, 28].Value = "Table(%)";
                    worksheet.Cells[1, 29].Value = "Key To Symbol";
                    worksheet.Cells[1, 30].Value = "Table White";
                    worksheet.Cells[1, 31].Value = "Crown White";
                    worksheet.Cells[1, 32].Value = "Table Black";
                    worksheet.Cells[1, 33].Value = "Crown Black";
                    worksheet.Cells[1, 34].Value = "Cr Ang";
                    worksheet.Cells[1, 35].Value = "Cr Ht";
                    worksheet.Cells[1, 36].Value = "Pav Ang";
                    worksheet.Cells[1, 37].Value = "Pav Ht";
                    worksheet.Cells[1, 38].Value = "Girdle Type";
                    worksheet.Cells[1, 39].Value = "Laser Insc";
                    worksheet.Cells[1, 40].Value = "Image";
                    worksheet.Cells[1, 41].Value = "HD Movie";


                    ExcelStyle cellStyleHeader1 = worksheet.Cells[1, 1, 1, 41].Style;
                    cellStyleHeader1.Border.Left.Style = cellStyleHeader1.Border.Right.Style
                            = cellStyleHeader1.Border.Top.Style = cellStyleHeader1.Border.Bottom.Style
                            = ExcelBorderStyle.Medium;

                    #endregion

                    int inStartIndex = 2;
                    int inwrkrow = 2;
                    int inEndCounter = dtDiamonds.Rows.Count + inStartIndex;
                    int TotalRow = dtDiamonds.Rows.Count;

                    #region Set AutoFit and Decimal Number Format

                    worksheet.View.FreezePanes(2, 1);
                    worksheet.Cells[1, 1].AutoFitColumns(9.50);
                    worksheet.Cells[1, 2].AutoFitColumns(10);
                    worksheet.Cells[1, 3].AutoFitColumns(17);
                    worksheet.Cells[1, 4].AutoFitColumns(10);
                    worksheet.Cells[1, 5].AutoFitColumns(10);
                    worksheet.Cells[1, 6].AutoFitColumns(8.50);
                    worksheet.Cells[1, 7].AutoFitColumns(8.50);
                    worksheet.Cells[1, 8].AutoFitColumns(14);
                    worksheet.Cells[1, 9].AutoFitColumns(8.50);
                    worksheet.Cells[1, 10].AutoFitColumns(8.50);
                    worksheet.Cells[1, 11].AutoFitColumns(8.50);
                    worksheet.Cells[1, 12].AutoFitColumns(8.50);
                    worksheet.Cells[1, 13].AutoFitColumns(9.5);
                    worksheet.Cells[1, 14].AutoFitColumns(11);
                    worksheet.Cells[1, 15].AutoFitColumns(8.50);
                    worksheet.Cells[1, 16].AutoFitColumns(9.5);
                    worksheet.Cells[1, 17].AutoFitColumns(8.50);
                    worksheet.Cells[1, 18].AutoFitColumns(8.50);
                    worksheet.Cells[1, 19].AutoFitColumns(8.50);
                    worksheet.Cells[1, 20].AutoFitColumns(8.50);
                    worksheet.Cells[1, 21].AutoFitColumns(8.50);
                    worksheet.Cells[1, 22].AutoFitColumns(8.50);
                    worksheet.Cells[1, 23].AutoFitColumns(8.50);
                    worksheet.Cells[1, 24].AutoFitColumns(9.50);
                    worksheet.Cells[1, 25].AutoFitColumns(9.50);
                    worksheet.Cells[1, 26].AutoFitColumns(9.50);
                    worksheet.Cells[1, 27].AutoFitColumns(9.5);
                    worksheet.Cells[1, 28].AutoFitColumns(9.5);
                    worksheet.Cells[1, 29].AutoFitColumns(40);
                    worksheet.Cells[1, 30].AutoFitColumns(9);
                    worksheet.Cells[1, 31].AutoFitColumns(9);
                    worksheet.Cells[1, 32].AutoFitColumns(9);
                    worksheet.Cells[1, 33].AutoFitColumns(9);
                    worksheet.Cells[1, 34].AutoFitColumns(7.86);
                    worksheet.Cells[1, 35].AutoFitColumns(7.86);
                    worksheet.Cells[1, 36].AutoFitColumns(7.86);
                    worksheet.Cells[1, 37].AutoFitColumns(7.86);
                    worksheet.Cells[1, 38].AutoFitColumns(7.86);
                    worksheet.Cells[1, 39].AutoFitColumns(7.86);
                    worksheet.Cells[1, 40].AutoFitColumns(7.86);
                    worksheet.Cells[1, 41].AutoFitColumns(7.86);

                    //Set Cell Faoat value with Alignment
                    worksheet.Cells[inStartIndex, 1, inEndCounter, 41].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    #endregion

                    var namedStyle = p.Workbook.Styles.CreateNamedStyle("HyperLink");
                    namedStyle.Style.Font.UnderLine = true;
                    namedStyle.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    namedStyle.Style.Font.Size = 11;
                    namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    namedStyle.Style.Font.Name = "Calibri";
                    int i;
                    String values_1;
                    Int64 number_1;
                    bool success1;
                    var asTitleCase = Thread.CurrentThread.CurrentCulture.TextInfo;
                    string Image, Video, dna, hyprlink1, status, cut;
                    for (i = inStartIndex; i < inEndCounter; i++)
                    {
                        #region Assigns Value to Cell
                        dna = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["view_dna"]);

                        if (dna != "")
                        {
                            worksheet.Cells[inwrkrow, 1].Formula = "=HYPERLINK(\"" + dna + "\",\" DNA \")";
                            worksheet.Cells[inwrkrow, 1].Style.Font.UnderLine = true;
                            worksheet.Cells[inwrkrow, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        }

                        worksheet.Cells[inwrkrow, 2].Value = asTitleCase.ToTitleCase(Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["Location"]).ToLower());

                        status = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["status"]).ToLower();
                        if (status == "available offer")
                            status = "Offer";
                        else if (status == "buss. process")
                            status = "Busy";

                        worksheet.Cells[inwrkrow, 3].Value = asTitleCase.ToTitleCase(status);

                        values_1 = dtDiamonds.Rows[i - inStartIndex]["stone_ref_no"].ToString();
                        success1 = Int64.TryParse(values_1, out number_1);
                        if (success1)
                        {
                            worksheet.Cells[inwrkrow, 4].Value = Convert.ToInt64(dtDiamonds.Rows[i - inStartIndex]["stone_ref_no"]);
                        }
                        else
                        {
                            worksheet.Cells[inwrkrow, 4].Value = values_1;
                        }
                        worksheet.Cells[inwrkrow, 5].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["shape"]);
                        worksheet.Cells[inwrkrow, 6].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["pointer"]);
                        worksheet.Cells[inwrkrow, 7].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["lab"]);

                        values_1 = dtDiamonds.Rows[i - inStartIndex]["certi_no"].ToString();
                        success1 = Int64.TryParse(values_1, out number_1);
                        if (success1)
                        {
                            worksheet.Cells[inwrkrow, 8].Value = Convert.ToInt64(dtDiamonds.Rows[i - inStartIndex]["certi_no"]);
                        }
                        else
                        {
                            worksheet.Cells[inwrkrow, 8].Value = values_1;
                        }

                        worksheet.Cells[inwrkrow, 9].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["BGM"]);
                        worksheet.Cells[inwrkrow, 10].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["color"]);
                        worksheet.Cells[inwrkrow, 11].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["clarity"]);
                        worksheet.Cells[inwrkrow, 12].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["cts"]);

                        worksheet.Cells[inwrkrow, 13].Value = dtDiamonds.Rows[i - inStartIndex]["cur_rap_rate"].GetType().Name != "DBNull" ?
                               Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["cur_rap_rate"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 14].Value = dtDiamonds.Rows[i - inStartIndex]["rap_amount"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["rap_amount"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 15].Value = dtDiamonds.Rows[i - inStartIndex]["sales_disc_per"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["sales_disc_per"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 16].Value = dtDiamonds.Rows[i - inStartIndex]["net_amount"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["net_amount"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 17].Value = dtDiamonds.Rows[i - inStartIndex]["price_per_cts"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["price_per_cts"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 18].Value = dtDiamonds.Rows[i - inStartIndex]["Offer_Disc"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["Offer_Disc"]) : ((Double?)null);

                        worksheet.Cells[inwrkrow, 19].Value = dtDiamonds.Rows[i - inStartIndex]["Valid_Days"].GetType().Name != "DBNull" ?
                                Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["Valid_Days"]) : ((Double?)null);

                        cut = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["cut"]);
                        worksheet.Cells[inwrkrow, 20].Value = (cut == "FR" ? "F" : cut);
                        worksheet.Cells[inwrkrow, 21].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["polish"]);
                        worksheet.Cells[inwrkrow, 22].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["symm"]);

                        if (Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["cut"]) == "3EX")
                        {
                            worksheet.Cells[inwrkrow, 20].Style.Font.Bold = true;
                            worksheet.Cells[inwrkrow, 21].Style.Font.Bold = true;
                            worksheet.Cells[inwrkrow, 22].Style.Font.Bold = true;
                        }

                        worksheet.Cells[inwrkrow, 23].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["fls"]);
                        worksheet.Cells[inwrkrow, 24].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["length"]);
                        worksheet.Cells[inwrkrow, 25].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["width"]);
                        worksheet.Cells[inwrkrow, 26].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["depth"]);
                        worksheet.Cells[inwrkrow, 27].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["depth_per"]);
                        worksheet.Cells[inwrkrow, 28].Value = Convert.ToDouble(dtDiamonds.Rows[i - inStartIndex]["table_per"]);
                        worksheet.Cells[inwrkrow, 29].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["symbol"]);
                        worksheet.Cells[inwrkrow, 30].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["inclusion"]);
                        worksheet.Cells[inwrkrow, 31].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["Crown_Inclusion"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["Crown_Inclusion"]);
                        worksheet.Cells[inwrkrow, 32].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["table_natts"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["table_natts"]);
                        worksheet.Cells[inwrkrow, 33].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["Crown_Natts"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["Crown_Natts"]);
                        worksheet.Cells[inwrkrow, 34].Value = dtDiamonds.Rows[i - inStartIndex]["crown_angle"] == null ? 0 : dtDiamonds.Rows[i - inStartIndex]["crown_angle"].ToString() == "" ? 0 : dtDiamonds.Rows[i - inStartIndex]["crown_angle"];
                        worksheet.Cells[inwrkrow, 35].Value = dtDiamonds.Rows[i - inStartIndex]["crown_height"] == null ? 0 : dtDiamonds.Rows[i - inStartIndex]["crown_height"].ToString() == "" ? 0 : dtDiamonds.Rows[i - inStartIndex]["crown_height"];
                        worksheet.Cells[inwrkrow, 36].Value = dtDiamonds.Rows[i - inStartIndex]["pav_angle"] == null ? 0 : dtDiamonds.Rows[i - inStartIndex]["pav_angle"].ToString() == "" ? 0 : dtDiamonds.Rows[i - inStartIndex]["pav_angle"];
                        worksheet.Cells[inwrkrow, 37].Value = dtDiamonds.Rows[i - inStartIndex]["pav_height"] == null ? 0 : dtDiamonds.Rows[i - inStartIndex]["pav_height"].ToString() == "" ? 0 : dtDiamonds.Rows[i - inStartIndex]["pav_height"];
                        worksheet.Cells[inwrkrow, 38].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["girdle_type"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["girdle_type"]);
                        worksheet.Cells[inwrkrow, 39].Value = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["sInscription"] == null ? "" : dtDiamonds.Rows[i - inStartIndex]["sInscription"]);

                        if (dtDiamonds.Rows[i - inStartIndex]["image_url"] != null)
                        {
                            Image = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["image_url"]);
                            if (Image != "")
                            {
                                hyprlink1 = External_ImageURL + Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["certi_no"]) + "/PR.jpg";
                                worksheet.Cells[inwrkrow, 40].Formula = "=HYPERLINK(\"" + hyprlink1 + "\",\" Image \")";
                                worksheet.Cells[inwrkrow, 40].Style.Font.UnderLine = true;
                                worksheet.Cells[inwrkrow, 40].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                            }
                        }

                        if (dtDiamonds.Rows[i - inStartIndex]["movie_url"] != null)
                        {
                            Video = Convert.ToString(dtDiamonds.Rows[i - inStartIndex]["movie_url"]);
                            if (Video != "")
                            {
                                hyprlink1 = Video;
                                worksheet.Cells[inwrkrow, 41].Formula = "=HYPERLINK(\"" + hyprlink1 + "\",\" Video \")";
                                worksheet.Cells[inwrkrow, 41].Style.Font.UnderLine = true;
                                worksheet.Cells[inwrkrow, 41].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                            }
                        }

                        inwrkrow++;

                        #endregion
                    }

                    worksheet.Cells[2, 1, inwrkrow - 1, 41].Style.Font.Size = 9;

                    worksheet.Cells[1, 6, inwrkrow - 1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    System.Drawing.Color colFromHex_Pointer = System.Drawing.ColorTranslator.FromHtml("#c6e0b4");
                    worksheet.Cells[1, 6, inwrkrow - 1, 6].Style.Fill.BackgroundColor.SetColor(colFromHex_Pointer);

                    worksheet.Cells[1, 15, inwrkrow - 1, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    System.Drawing.Color colFromHex_Dis = System.Drawing.ColorTranslator.FromHtml("#ccffff");
                    worksheet.Cells[1, 15, inwrkrow - 1, 15].Style.Fill.BackgroundColor.SetColor(colFromHex_Dis);

                    worksheet.Cells[1, 16, inwrkrow - 1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 16, inwrkrow - 1, 16].Style.Fill.BackgroundColor.SetColor(colFromHex_Dis);

                    worksheet.Cells[1, 12, inwrkrow - 1, 17].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[1, 24, inwrkrow - 1, 28].Style.Numberformat.Format = "0.00";
                    worksheet.Cells[1, 34, inwrkrow - 1, 37].Style.Numberformat.Format = "0.00";

                    worksheet.Cells[1, 15, inwrkrow - 1, 16].Style.Font.Bold = true;
                    worksheet.Cells[2, 15, inwrkrow - 1, 16].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    worksheet.Cells[1, 15, inwrkrow - 1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 15, inwrkrow - 1, 16].Style.Fill.BackgroundColor.SetColor(colFromHex_Dis);


                    string parentPath = FolderPath;
                    string fileName = string.Empty;
                    if (ConfigurationManager.AppSettings["ConnMode"] == "Oracle")
                        parentPath = @"C:\inetpub\wwwroot\Temp\";

                    fileName = parentPath + FileName;
                    Byte[] bin = p.GetAsByteArray();

                    if (!Directory.Exists(parentPath))
                    {
                        Directory.CreateDirectory(parentPath);
                    }

                    System.IO.File.WriteAllBytes(fileName, bin);

                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                throw ex;
            }
            return flag;
        }

        [NonAction]
        private void AddHeaderEventHandler(object sender, ref AddHeaderEventArgs e)
        {
            ExcelExport ee = (ExcelExport)sender;

            ExcelCellFormat f = new ExcelCellFormat();
            f.isbold = true;
            f.fontsize = 11;

            UInt32 statusind = ee.AddStyle(f);

            ExcelCellFormat c = new ExcelCellFormat();
            c.isbold = true;
            c.fontsize = 24;
            c.forColorArgb = ExcelExport.GetHexValue(System.Drawing.Color.FromArgb(0, 112, 192).ToArgb());

            if (System.Configuration.ConfigurationManager.AppSettings["Location"] == "M")
            {
                ee.SetCellValue("C1", "SHAIRUGEMS DIAMONDS INVENTORY FOR THE DATE  " + Lib.Models.Common.GetHKTime().ToString("dd-MMM-yyyy"), ee.AddStyle(c));
            }
            else
            {
                ee.SetCellValue("C1", "SUNRISE DIAMONDS INVENTORY FOR THE DATE " + Lib.Models.Common.GetHKTime().Date.ToString("dd-MMM-yyyy"), ee.AddStyle(c));
                ee.SetCellValue("C2", "Note :", statusind);
                ee.SetCellValue("D2", "Promotion Stones have fix Cash Selling Price", 1);
                ee.SetCellValue("C3", "Status :", statusind);
                ee.SetCellValue("D3", "Promotion", 1);
                ee.SetCellValue("E3", "P", statusind);
                ee.SetCellValue("F3", "Buss. Proc", 1);
                ee.SetCellValue("G3", "B", statusind);

                ee.AddNewRow("A4");
                ee.AddNewRow("A5");
            }
        }

        [NonAction]
        private void BeforeCreateColumnEventHandler(object sender, ref ExcelHeader e)
        {
            switch (e.ColName.ToUpper())
            {
                case "SR":
                    e.visible = false;
                    e.ColInd = 1;
                    e.Caption = "Sr";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                case "VIEW_DNA":
                    e.ColInd = 1;
                    e.HyperlinkColName = "VIEW_DNA";
                    e.Caption = "DNA";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6.00;
                    break;
                case "IMAGE_URL":
                    e.ColInd = 41;
                    e.HyperlinkColName = "IMAGE_URL";
                    e.Caption = "Image";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    // e.SummText = "Total:";
                    // e.SummFunction = DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Custom;
                    break;
                case "MOVIE_URL":
                    e.ColInd = 42;
                    e.HyperlinkColName = "MOVIE_URL";
                    e.Caption = "HD Movie";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                case "LOCATION":
                    e.ColInd = 2;
                    e.Caption = "Location";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 10.00;
                    break;
                case "STATUS":
                    e.ColInd = 3;
                    e.Caption = "Status";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 15.00;
                    break;
                case "STONE_REF_NO":
                    e.ColInd = 4;
                    e.Caption = "Stock ID";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    //e.SummFunction = DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Count;
                    //e.NumFormat = "#,##0";
                    e.Width = 10.00;
                    break;
                case "LAB":
                    e.ColInd = 7;
                    e.HyperlinkColName = "VERIFY_CERTI_URL";
                    e.Caption = "Lab";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6.00;
                    break;
                case "SHAPE":
                    e.ColInd = 5;
                    e.Caption = "Shape";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                case "POINTER":
                    e.ColInd = 6;
                    e.Caption = "Pointer";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                case "CERTI_NO":
                    e.ColInd = 8;
                    e.Caption = "Certi No.";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 12.00;
                    break;
                case "SHADE":
                    e.ColInd = 9;
                    e.Caption = "Shade";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 8.30;
                    break;
                case "COLOR":
                    e.ColInd = 10;
                    e.Caption = "Color";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                case "CLARITY":
                    e.ColInd = 11;
                    e.Caption = "Clarity";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6.00;
                    break;
                case "CTS":
                    e.ColInd = 12;
                    e.Caption = "Cts";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.NumFormat = "#,##0.00";
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    //e.SummFunction = DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Sum;
                    e.Width = 6.00;
                    break;
                case "CUR_RAP_RATE":
                    e.ColInd = 13;
                    e.Caption = "Rap Price($)";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.NumFormat = "#,##0.00";
                    e.Width = 10;
                    break;
                case "RAP_AMOUNT":
                    e.ColInd = 14;
                    e.Caption = "Rap Amt($)";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    //e.SummFunction = DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Sum;
                    e.NumFormat = "#,##0.00";
                    e.Width = 10;
                    break;
                case "SALES_DISC_PER":
                    e.ColInd = 15;
                    e.Caption = "Disc(%)";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    //e.SummFunction = DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Custom;
                    //e.SummFormula = "(1- (" + ge.GetSummFormula("Net Amt($)", DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Sum) +
                    //                 "/" + ge.GetSummFormula("Rap Amt($)", DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Sum) + " ))*-100";
                    e.NumFormat = "#,##0.00";
                    e.Width = 6;
                    break;
                case "NET_AMOUNT":
                    e.ColInd = 16;
                    e.Caption = "Net Amt($)";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    //e.SummFunction = DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Sum;
                    e.NumFormat = "#,##0.00";
                    e.Width = 10;
                    break;
                case "PRICE_PER_CTS":
                    e.ColInd = 17;
                    e.Caption = "Price/Cts";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    //e.SummFunction = DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Sum;
                    e.NumFormat = "#,##0.00";
                    e.Width = 10;
                    break;
                case "OFFER_DISC":
                    e.ColInd = 18;
                    e.Caption = "Offer Disc(%)";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    //e.SummFunction = DocumentFormat.OpenXml.Spreadsheet.TotalsRowFunctionValues.Sum;
                    e.Width = 10;
                    break;
                case "VALID_DAYS":
                    e.ColInd = 19;
                    e.Caption = "Valid Days";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.NumFormat = "#,##0";
                    e.Width = 10;
                    break;
                case "CUT":
                    e.ColInd = 20;
                    e.Caption = "Cut";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "POLISH":
                    e.ColInd = 21;
                    e.Caption = "Polish";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "SYMM":
                    e.ColInd = 22;
                    e.Caption = "Symm";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "FLS":
                    e.ColInd = 23;
                    e.Caption = "Fls";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "MEASUREMENT":
                    if (System.Configuration.ConfigurationManager.AppSettings["Location"] == "M")
                        e.visible = false;
                    else
                        e.visible = false;
                    e.Caption = "Measurement";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                case "LENGTH":
                    e.ColInd = 24;
                    e.Caption = "Length";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.NumFormat = "#,##0.00";
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "WIDTH":
                    e.ColInd = 25;
                    e.Caption = "Width";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.NumFormat = "#,##0.00";
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "DEPTH":
                    e.ColInd = 26;
                    e.Caption = "Depth";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.NumFormat = "#,##0.00";
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "DEPTH_PER":
                    e.ColInd = 27;
                    e.Caption = "Depth(%)";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.NumFormat = "#,##0.00";
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "TABLE_PER":
                    e.ColInd = 28;
                    e.Caption = "Table(%)";
                    e.NumFormat = "#,##0.00";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "USER_COMMENTS":
                    e.visible = false;
                    e.Caption = "User Comments";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                case "SYMBOL":
                    e.ColInd = 29;
                    e.Caption = "Key To Symbol";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 30;
                    break;
                case "LUSTER":
                    e.ColInd = 30;
                    e.Caption = "Luster";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "INCLUSION":
                    e.ColInd = 31;
                    e.Caption = "Table White";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 10;
                    break;
                case "CROWN_INCLUSION":
                    e.ColInd = 32;
                    e.Caption = "Crown White";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 10;
                    break;
                case "CROWN_NATTS":
                    e.ColInd = 34;
                    e.Caption = "Crown Black";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 10;
                    break;
                case "TABLE_NATTS":
                    e.ColInd = 33;
                    e.Caption = "Table Black";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 10;
                    break;
                case "CROWN_ANGLE":
                    e.ColInd = 35;
                    e.Caption = "Cr Ang";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.NumFormat = "#,##0.00";
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "CROWN_HEIGHT":
                    e.ColInd = 36;
                    e.Caption = "Cr Ht";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.NumFormat = "#,##0.00";
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "PAV_ANGLE":
                    e.ColInd = 37;
                    e.Caption = "Pav Ang";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.NumFormat = "#,##0.00";
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "PAV_HEIGHT":
                    e.ColInd = 38;
                    e.Caption = "Pav Ht";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.Number;
                    e.NumFormat = "#,##0.00";
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    e.Width = 6;
                    break;
                case "GIRDLE":
                    e.visible = false;
                    e.Caption = "Gridle";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                case "GIRDLE_TYPE":
                    e.ColInd = 39;
                    e.Caption = "Gridle Type";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                case "SINSCRIPTION":
                    e.ColInd = 40;
                    e.Caption = "Laser Insc";
                    e.ColDataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                    break;
                default:
                    e.visible = false;
                    break;
            }
        }

        [NonAction]
        private void FillingWorksheetEventHandler(object sender, ref FillingWorksheetEventArgs e)
        {
            ExcelExport ee = (ExcelExport)sender;

            ExcelFormat format = new ExcelFormat();
            format = new ExcelFormat();
            format.forColorArgb = ExcelExport.GetHexValue(System.Drawing.Color.Red.ToArgb());
            format.isbold = true;
            format.fontsize = 9;
            format.fontname = "Calibri";
            PriceStyleindex = ee.AddStyle(format);

            format = new ExcelFormat();
            format.forColorArgb = ExcelExport.GetHexValue(System.Drawing.Color.Red.ToArgb());
            format.isbold = true;
            format.fontsize = 9;
            format.fontname = "Calibri";
            format.backgroundArgb = ExcelExport.GetHexValue(System.Drawing.Color.FromArgb(204, 255, 255).ToArgb());
            DiscNormalStyleindex = ee.AddStyle(format);

            format = new ExcelFormat();
            format.fontsize = 9;
            format.fontname = "Calibri";
            format.backgroundArgb = ExcelExport.GetHexValue(System.Drawing.Color.FromArgb(198, 224, 180).ToArgb());
            PointerStyleindex = ee.AddStyle(format);

            format = new ExcelFormat();
            format.fontsize = 9;
            format.fontname = "Calibri";
            format.isbold = true;
            CutNormalStyleindex = ee.AddStyle(format);

            format = new ExcelFormat();
            format.fontsize = 9;
            format.fontname = "Calibri";
            NormalStyleindex = ee.AddStyle(format);
        }

        [NonAction]
        private void AfterCreateCellEventHandler(object sender, ref ExcelCellFormat e)
        {
            //return;
            if (e.tableArea == TableArea.Header)
            {
                e.fontname = "Calibri";
                e.fontsize = 10;
                e.backgroundArgb = ExcelExport.GetHexValue(System.Drawing.Color.FromArgb(211, 211, 211).ToArgb());
                e.HorizontalAllign = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center;
                e.bottom = BorderStyleValues.Medium;
                e.top = BorderStyleValues.Medium;
                e.left = BorderStyleValues.Medium;
                e.right = BorderStyleValues.Medium;
                e.isbold = true;
                if (e.ColumnName == "Pointer")
                {
                    e.backgroundArgb = ExcelExport.GetHexValue(System.Drawing.Color.FromArgb(198, 224, 180).ToArgb());
                }
                if (e.ColumnName == "Disc(%)" || e.ColumnName == "Net Amt($)")
                {
                    e.backgroundArgb = ExcelExport.GetHexValue(System.Drawing.Color.FromArgb(204, 255, 255).ToArgb());
                }
            }
            else if (e.tableArea == TableArea.Detail)
            {

                switch (e.ColumnName)
                {
                    case "Pointer":
                        e.StyleInd = PointerStyleindex;
                        break;
                    case "DNA":
                        if (e.url.Length > 0)
                        {
                            e.Text = "DNA";
                        }
                        e.StyleInd = NormalStyleindex;
                        break;
                    case "Image":
                        if (e.url.Length > 0)
                        {
                            e.Text = "Image";
                        }
                        e.StyleInd = NormalStyleindex;
                        break;
                    case "HD Movie":
                        if (e.url.Length > 0)
                        {
                            e.Text = "HD Movie";
                        }
                        e.StyleInd = NormalStyleindex;
                        break;
                    case "Disc(%)":
                        e.StyleInd = DiscNormalStyleindex;
                        break;
                    case "Net Amt($)":
                        e.StyleInd = DiscNormalStyleindex;
                        break;
                    case "Price/Cts":
                        e.StyleInd = PriceStyleindex;
                        break;
                    case "Cut":
                        if (e.Text == "3EX")
                            e.StyleInd = CutNormalStyleindex;
                        break;
                    case "Polish":
                        if (((DataRow)e.GridRow).Table.Rows[e.RowInd - 2]["Cut"].ToString() == "3EX")
                            e.StyleInd = CutNormalStyleindex;
                        break;
                    case "Symm":
                        if (((DataRow)e.GridRow).Table.Rows[e.RowInd - 2]["Cut"].ToString() == "3EX")
                            e.StyleInd = CutNormalStyleindex;
                        break;
                    default:
                        e.StyleInd = NormalStyleindex;
                        break;
                }
            }

            /*
            //else if (e.tableArea == TableArea.Footer)
            //{
            //    e.backgroundArgb = ExcelExport.GetHexValue(System.Drawing.Color.FromArgb(131, 202, 255).ToArgb());
            //    e.isbold = true;
            //    e.ul = DocumentFormat.OpenXml.Spreadsheet.UnderlineValues.None;

            //    switch (e.ColumnName)
            //    {
            //        case "Disc %":
            //            e.forColorArgb = ExcelExport.GetHexValue(System.Drawing.Color.Red.ToArgb());
            //            break;
            //        default:
            //            break;
            //    }
            //}
            */
        }

        [HttpPost]
        public IHttpActionResult OfferHistoryDetail([FromBody]JObject data)
        {
            OfferHisRequest req = new OfferHisRequest();
            try
            {
                req = JsonConvert.DeserializeObject<OfferHisRequest>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<OfferHistoryResponse>
                {
                    Data = new List<OfferHistoryResponse>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (userID > 0)
                    para.Add(db.CreateParam("iUserId", DbType.String, ParameterDirection.Input, userID));
                else
                    para.Add(db.CreateParam("iUserId", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.FromDate))
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, req.FromDate));
                else
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.ToDate))
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, req.ToDate));
                else
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.RefNo))
                    para.Add(db.CreateParam("RefNo", DbType.String, ParameterDirection.Input, (req.RefNo)));
                else
                    para.Add(db.CreateParam("RefNo", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.Status))
                    para.Add(db.CreateParam("Status", DbType.String, ParameterDirection.Input, (req.Status)));
                else
                    para.Add(db.CreateParam("Status", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.CompanyName))
                    para.Add(db.CreateParam("CompanyName", DbType.String, ParameterDirection.Input, (req.CompanyName)));
                else
                    para.Add(db.CreateParam("CompanyName", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.UserName))
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, (req.UserName)));
                else
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.PageNo))
                    para.Add(db.CreateParam("iPgNo", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(req.PageNo)));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int32, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.PageSize))
                    para.Add(db.CreateParam("iPgSize", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(req.PageSize)));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int32, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Location))
                    para.Add(db.CreateParam("location", DbType.String, ParameterDirection.Input, req.Location.ToUpper()));
                else
                    para.Add(db.CreateParam("location", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Shape))
                    para.Add(db.CreateParam("shape", DbType.String, ParameterDirection.Input, req.Shape.ToUpper()));
                else
                    para.Add(db.CreateParam("shape", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Pointer))
                    para.Add(db.CreateParam("pointer", DbType.String, ParameterDirection.Input, req.Pointer.ToUpper()));
                else
                    para.Add(db.CreateParam("pointer", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Lab))
                    para.Add(db.CreateParam("lab", DbType.String, ParameterDirection.Input, req.Lab.ToUpper()));
                else
                    para.Add(db.CreateParam("lab", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Color))
                    para.Add(db.CreateParam("color", DbType.String, ParameterDirection.Input, req.Color.ToUpper()));
                else
                    para.Add(db.CreateParam("color", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Clarity))
                    para.Add(db.CreateParam("clarity", DbType.String, ParameterDirection.Input, req.Clarity.ToUpper()));
                else
                    para.Add(db.CreateParam("clarity", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Cut))
                    para.Add(db.CreateParam("cut", DbType.String, ParameterDirection.Input, req.Cut.ToUpper()));
                else
                    para.Add(db.CreateParam("cut", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Polish))
                    para.Add(db.CreateParam("polish", DbType.String, ParameterDirection.Input, req.Polish.ToUpper()));
                else
                    para.Add(db.CreateParam("polish", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Symm))
                    para.Add(db.CreateParam("symm", DbType.String, ParameterDirection.Input, req.Symm.ToUpper()));
                else
                    para.Add(db.CreateParam("symm", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Fls))
                    para.Add(db.CreateParam("fls", DbType.String, ParameterDirection.Input, req.Fls.ToUpper()));
                else
                    para.Add(db.CreateParam("fls", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.OrderBy))
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, req.OrderBy.ToUpper()));
                else
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                para.Add(db.CreateParam("p_for_FormName", DbType.String, ParameterDirection.Input, req.FormName));
                para.Add(db.CreateParam("p_for_ActivityType", DbType.String, ParameterDirection.Input, req.ActivityType));

                para.Add(db.CreateParam("Active", DbType.Boolean, ParameterDirection.Input, req.Active));
                para.Add(db.CreateParam("InActive", DbType.Boolean, ParameterDirection.Input, req.InActive));

                DataTable dtData = db.ExecuteSP("IPD_Get_Offer_History_Sunrise", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    dtData.DefaultView.RowFilter = "sRefNo IS NOT NULL";
                    dtData = dtData.DefaultView.ToTable();

                    DataRow[] dra = dtData.Select("iSr IS NULL");
                    OfferSummary searchSummary = new OfferSummary();

                    if (dra.Length > 0)
                    {
                        searchSummary.TOT_PAGE = Convert.ToInt32(dra[0]["total_page"]);
                        searchSummary.PAGE_SIZE = Convert.ToInt32(dra[0]["page_size"]);
                        searchSummary.TOT_PCS = Convert.ToInt32(dra[0]["sRefNo"]);
                        searchSummary.TOT_CTS = Convert.ToDouble(dra[0]["Cts"]);
                        searchSummary.TOT_RAP_AMOUNT = Convert.ToDouble(dra[0]["RapAmount"]);
                        searchSummary.TOT_NET_AMOUNT = Convert.ToDouble(dra[0]["NetAmount"]);
                        searchSummary.AVG_SALES_DISC_PER = Convert.ToDouble(dra[0]["sSupplDisc"]);
                    }

                    dtData.DefaultView.RowFilter = "iSr IS NOT NULL";
                    dtData = dtData.DefaultView.ToTable();
                    if (dtData != null && dtData.Rows.Count > 0)
                    {

                        List<OfferHistoryResponse> listOrder = new List<OfferHistoryResponse>();
                        listOrder = DataTableExtension.ToList<OfferHistoryResponse>(dtData);
                        List<OfferHisResponse> offerHisResponse = new List<OfferHisResponse>();

                        //offerHisResponse = DataTableExtension.ToList<offerHisResponse>(dtData);
                        if (listOrder.Count > 0)
                        { //List<string> lst = dtData.AsDataView().ToTable(true, "CompanyName").ToList();
                          //var a = (from r in dtData.AsEnumerable()
                          //  select r["CompanyName"]).Distinct().ToList();
                            offerHisResponse.Add(new OfferHisResponse()
                            {
                                DataList = listOrder,
                                DataSummary = searchSummary

                            });

                            return Ok(new ServiceResponse<OfferHisResponse>
                            {
                                Data = offerHisResponse,
                                Message = "SUCCESS",
                                Status = "1"
                            });
                        }
                        else
                        {
                            return Ok(new ServiceResponse<OfferHisResponse>
                            {
                                Data = offerHisResponse,
                                Message = "SUCCESS",
                                Status = "1"
                            });
                        }
                    }
                    else
                    {
                        return Ok(new ServiceResponse<OfferHisResponse>
                        {
                            Data = null,
                            Message = "No data found.",
                            Status = "1"
                        });
                    }
                }
                else
                {
                    return Ok(new ServiceResponse<OfferHisResponse>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "1"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<OfferHistoryResponse>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        //[HttpPost]
        //public IHttpActionResult DownloadOfferHistoryDetail([FromBody]JObject data)
        //{
        //    OfferHistoryRequest req = new OfferHistoryRequest();
        //    try
        //    {
        //        req = JsonConvert.DeserializeObject<OfferHistoryRequest>(data.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        DAL.Common.InsertErrorLog(ex, null, Request);
        //        return Ok(new ServiceResponse<OfferHistoryResponse>
        //        {
        //            Data = new List<OfferHistoryResponse>(),
        //            Message = "Input Parameters are not in the proper format",
        //            Status = "0"
        //        });
        //    }

        //    try
        //    {
        //        Database db = new Database(Request);
        //        List<IDbDataParameter> para = new List<IDbDataParameter>();

        //        if (!string.IsNullOrEmpty(req.FromDate))
        //            para.Add(db.CreateParam("frdate", DbType.Date, ParameterDirection.Input, Convert.ToDateTime(req.FromDate)));
        //        else
        //            para.Add(db.CreateParam("frdate", DbType.Date, ParameterDirection.Input, DBNull.Value));

        //        if (!string.IsNullOrEmpty(req.ToDate))
        //            para.Add(db.CreateParam("todate", DbType.Date, ParameterDirection.Input, Convert.ToDateTime(req.ToDate)));
        //        else
        //            para.Add(db.CreateParam("todate", DbType.Date, ParameterDirection.Input, DBNull.Value));

        //        if (!string.IsNullOrEmpty(req.OfferId))
        //            para.Add(db.CreateParam("iOfferid", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(req.OfferId)));
        //        else
        //            para.Add(db.CreateParam("iOfferid", DbType.Int32, ParameterDirection.Input, DBNull.Value));

        //        if (!string.IsNullOrEmpty(req.CompanyName))
        //            para.Add(db.CreateParam("comp_name", DbType.String, ParameterDirection.Input, req.CompanyName));
        //        else
        //            para.Add(db.CreateParam("comp_name", DbType.String, ParameterDirection.Input, DBNull.Value));

        //        if (!string.IsNullOrEmpty(req.UserName))
        //            para.Add(db.CreateParam("user_name", DbType.String, ParameterDirection.Input, req.UserName));
        //        else
        //            para.Add(db.CreateParam("user_name", DbType.String, ParameterDirection.Input, DBNull.Value));

        //        if (!string.IsNullOrEmpty(req.CountryName))
        //            para.Add(db.CreateParam("country_name", DbType.String, ParameterDirection.Input, req.CountryName));
        //        else
        //            para.Add(db.CreateParam("country_name", DbType.String, ParameterDirection.Input, DBNull.Value));

        //        if (!string.IsNullOrEmpty(req.Active))
        //            para.Add(db.CreateParam("ActiveFlag", DbType.String, ParameterDirection.Input, req.Active));
        //        else
        //            para.Add(db.CreateParam("ActiveFlag", DbType.String, ParameterDirection.Input, DBNull.Value));

        //        if (!string.IsNullOrEmpty(req.PageNo))
        //            para.Add(db.CreateParam("iPgNo", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(req.PageNo)));
        //        else
        //            para.Add(db.CreateParam("iPgNo", DbType.Int32, ParameterDirection.Input, DBNull.Value));

        //        if (!string.IsNullOrEmpty(req.PageSize))
        //            para.Add(db.CreateParam("iPgSize", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(req.PageSize)));
        //        else
        //            para.Add(db.CreateParam("iPgSize", DbType.Int32, ParameterDirection.Input, DBNull.Value));

        //        DataTable dt = db.ExecuteSP("Offer_SelectByParaNew", para.ToArray(), false);

        //        if (dt.Rows.Count > 0)
        //        {
        //            string filename = "";
        //            string _path = ConfigurationManager.AppSettings["data"];
        //            string realpath = HostingEnvironment.MapPath("~/ExcelFile/");
        //            string _livepath = ConfigurationManager.AppSettings["LiveUrl"];

        //            filename = "OfferHistory" + DAL.Common.GetHKTime().ToString("dd.MM.yyyy");
        //            EpExcelExport.CreateOfferHistory(dt.DefaultView.ToTable(), realpath, realpath + filename + ".xlsx", _livepath, req.isAdmin, req.isEmp);

        //            string _strxml = _path + filename + ".xlsx";
        //            return Ok(new CommonResponse
        //            {
        //                Message = _strxml,
        //                Status = "1",
        //                Error = ""
        //            });
        //        }
        //        else
        //        {
        //            return Ok(new CommonResponse
        //            {
        //                Message = "No data found.",
        //                Status = "0",
        //                Error = ""
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DAL.Common.InsertErrorLog(ex, null, Request);
        //        return Ok(new CommonResponse
        //        {
        //            Message = "Something Went wrong.\nPlease try again later",
        //            Status = "0",
        //            Error = ""
        //        });
        //    }
        //}

        [HttpPost]
        public IHttpActionResult DownloadOfferHistoryDetail([FromBody]JObject data)
        {
            OfferHisRequest req = new OfferHisRequest();
            try
            {
                req = JsonConvert.DeserializeObject<OfferHisRequest>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<OfferHistoryResponse>
                {
                    Data = new List<OfferHistoryResponse>(),
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (userID > 0)
                    para.Add(db.CreateParam("iUserId", DbType.String, ParameterDirection.Input, userID));
                else
                    para.Add(db.CreateParam("iUserId", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.FromDate))
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, req.FromDate));
                else
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.ToDate))
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, req.ToDate));
                else
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.RefNo))
                    para.Add(db.CreateParam("RefNo", DbType.String, ParameterDirection.Input, (req.RefNo)));
                else
                    para.Add(db.CreateParam("RefNo", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.Status))
                    para.Add(db.CreateParam("Status", DbType.String, ParameterDirection.Input, (req.Status)));
                else
                    para.Add(db.CreateParam("Status", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.CompanyName))
                    para.Add(db.CreateParam("CompanyName", DbType.String, ParameterDirection.Input, (req.CompanyName)));
                else
                    para.Add(db.CreateParam("CompanyName", DbType.String, ParameterDirection.Input, DBNull.Value));
                if (!string.IsNullOrEmpty(req.UserName))
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, (req.UserName)));
                else
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.PageNo))
                    para.Add(db.CreateParam("iPgNo", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(req.PageNo)));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int32, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.PageSize))
                    para.Add(db.CreateParam("iPgSize", DbType.Int32, ParameterDirection.Input, Convert.ToInt32(req.PageSize)));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int32, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Location))
                    para.Add(db.CreateParam("location", DbType.String, ParameterDirection.Input, req.Location.ToUpper()));
                else
                    para.Add(db.CreateParam("location", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Shape))
                    para.Add(db.CreateParam("shape", DbType.String, ParameterDirection.Input, req.Shape.ToUpper()));
                else
                    para.Add(db.CreateParam("shape", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Pointer))
                    para.Add(db.CreateParam("pointer", DbType.String, ParameterDirection.Input, req.Pointer.ToUpper()));
                else
                    para.Add(db.CreateParam("pointer", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Lab))
                    para.Add(db.CreateParam("lab", DbType.String, ParameterDirection.Input, req.Lab.ToUpper()));
                else
                    para.Add(db.CreateParam("lab", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Color))
                    para.Add(db.CreateParam("color", DbType.String, ParameterDirection.Input, req.Color.ToUpper()));
                else
                    para.Add(db.CreateParam("color", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Clarity))
                    para.Add(db.CreateParam("clarity", DbType.String, ParameterDirection.Input, req.Clarity.ToUpper()));
                else
                    para.Add(db.CreateParam("clarity", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Cut))
                    para.Add(db.CreateParam("cut", DbType.String, ParameterDirection.Input, req.Cut.ToUpper()));
                else
                    para.Add(db.CreateParam("cut", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Polish))
                    para.Add(db.CreateParam("polish", DbType.String, ParameterDirection.Input, req.Polish.ToUpper()));
                else
                    para.Add(db.CreateParam("polish", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Symm))
                    para.Add(db.CreateParam("symm", DbType.String, ParameterDirection.Input, req.Symm.ToUpper()));
                else
                    para.Add(db.CreateParam("symm", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.Fls))
                    para.Add(db.CreateParam("fls", DbType.String, ParameterDirection.Input, req.Fls.ToUpper()));
                else
                    para.Add(db.CreateParam("fls", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.OrderBy))
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, req.OrderBy.ToUpper()));
                else
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                para.Add(db.CreateParam("p_for_FormName", DbType.String, ParameterDirection.Input, req.FormName));
                para.Add(db.CreateParam("p_for_ActivityType", DbType.String, ParameterDirection.Input, req.ActivityType));

                para.Add(db.CreateParam("Active", DbType.Boolean, ParameterDirection.Input, req.Active));
                para.Add(db.CreateParam("InActive", DbType.Boolean, ParameterDirection.Input, req.InActive));

                DataTable dtData = db.ExecuteSP("IPD_Get_Offer_History_Sunrise", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0 && userID != 6492)// restrict for jbbrothers username
                {
                    dtData.DefaultView.RowFilter = "iSr IS NOT NULL";
                    dtData = dtData.DefaultView.ToTable();
                    if (dtData != null && dtData.Rows.Count > 0)
                    {
                        string filename = "";
                        string _path = ConfigurationManager.AppSettings["data"];
                        string realpath = HostingEnvironment.MapPath("~/ExcelFile/");
                        string _livepath = ConfigurationManager.AppSettings["LiveUrl"];

                        filename = "OfferHistory " + Lib.Models.Common.GetHKTime().ToString("ddMMyyyy-HHmmss");
                        bool isAdmin = (req.isAdmin == "1" ? true : false);
                        bool isEmp = (req.isEmp == "1" ? true : false);

                        float OfferPer = 0;
                        List<Lib.Models.OfferCriteria> offerperList = GetOfferCriteriaNew();
                        if (offerperList != null && offerperList.Count > 0)
                            OfferPer = offerperList[0].OfferPer;

                        EpExcelExport.CreateOfferHistory(dtData, realpath, realpath + filename + ".xlsx", _livepath, isAdmin, isEmp, OfferPer, req.FromDate, req.ToDate);

                        string _strxml = _path + filename + ".xlsx";
                        return Ok(new CommonResponse
                        {
                            Message = _strxml,
                            Status = "1",
                            Error = ""
                        });
                    }
                    else
                    {
                        return Ok(new CommonResponse
                        {
                            Message = "No data found.",
                            Status = "1",
                            Error = ""
                        });
                    }
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Message = "No data found.",
                        Status = "1",
                        Error = ""
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0",
                    Error = ""
                });
            }
        }
        public static String EmailHeader()
        {
            return @"<html><head><style type=""text/css"">body{font-family: Verdana,'sans-serif';font-size:12px;}p{text-align:justify;margin:10px 0 !important;}
                a{color:#1a4e94;text-decoration:none;font-weight:bold;}a:hover{color:#3c92fe;}table td{font-family: Verdana,'sans-serif' !important;font-size:12px;padding:3px;border-bottom:1px solid #dddddd;}
                </style></head><body>
                <div style=""width:750px; margin:5px auto;font-family: Verdana,'sans-serif';font-size:12px;line-height:20px; background-color:#f2f2f2;"">
                <img alt=""Sunrise Diamonds Ltd"" src=""https://sunrisediamonds.com.hk/Images/email-head.png"" width=""750px"" />
                <div style=""padding:10px;"">";
        }

        public static String EmailSignature()
        {
            return @"<p>Please do let us know if you have any questions. Email us on <a href=""mailto:support@sunrisediamonds.com.hk"">support@sunrisediamonds.com.hk</a></p>
                <p>Thanks and Regards,<br />Sunrise Diamond Team,<br />Room 1,14/F, Peninsula Square<br/>East Wing, 18 Sung On Street<br/>Hunghom, Kowloon<br/>Hong Kong<br/>
                <a href=""https://sunrisediamonds.com.hk"">www.sunrisediamonds.com.hk</a></p>
                </div></div></body></html>";
        }

        #region Task Scheduler : WS_OfferExpired

        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult WS_OfferExpired()
        {
            Database db;
            System.Collections.Generic.List<System.Data.IDbDataParameter> para;
            db = new Database();
            para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

            DataTable dt = db.ExecuteSP("OfferExpired_Get", para.ToArray(), false);

            string path = HttpContext.Current.Server.MapPath("~/Offer_Expired_Log.txt");
            if (!File.Exists(@"" + path + ""))
            {
                File.Create(@"" + path + "").Dispose();
            }
            StringBuilder sb = new StringBuilder();

            if (dt != null && dt.Rows.Count > 0)
            {
                Database db_tp = new Database(Request);
                List<SqlParameter> para_tp = new List<SqlParameter>();

                SqlParameter param_tp = new SqlParameter("tableInq", SqlDbType.Structured);
                param_tp.Value = dt;
                para_tp.Add(param_tp);

                DataTable I = db_tp.ExecuteSP("OfferExpired_Scheduler_Log_Insert", para_tp.ToArray(), false);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int iUserid = Convert.ToInt32(dt.Rows[i]["iUserid"]);
                    string iId = dt.Rows[i]["iId"].ToString();
                    string sUsername = dt.Rows[i]["sUsername"].ToString();

                    try
                    {
                        OfferDelete_From_Oracle(iId, "E");

                        String _strfileName = "";
                        Random rnd = new Random();

                        DAL.Stock objstock = new DAL.Stock();
                        DataTable dt1 = objstock.Offer_Excel_Expired_Cancelled(iUserid, iId, "Expired");
                        _strfileName = HttpContext.Current.Server.MapPath("~/Temp/Excel/" + rnd.Next().ToString() + ".xlsx");

                        if (File.Exists(_strfileName) == true)
                        {
                            File.Delete(_strfileName);
                        }

                        EpExcelExport.Offer_Excel_Expired_Cancelled(dt1, _strfileName, "Expired");

                        MailMessage xloMail = new MailMessage();
                        SmtpClient xloSmtp = new SmtpClient();

                        DAL.Usermas objUser = new DAL.Usermas();
                        DataTable loEmails = objUser.UserMas_SelectEmailByUserId_For_Offer(iUserid);

                        string lsToMail = "";
                        foreach (DataRow lrEmail in loEmails.Rows)
                            lsToMail += lrEmail["sEmail"].ToString() + ",";

                        if (lsToMail.Length > 0)
                            lsToMail = lsToMail.Remove(lsToMail.Length - 1);

                        string _strresult = "";
                        if (lsToMail.Trim() != "" && _strfileName != "")
                        {
                            _strresult = "Y";

                            xloMail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromEmail"], "Sunrise Diamonds");
                            xloMail.To.Add(lsToMail);

                            xloMail.Subject = "SUNRISE DIAMONDS : Offer Expired";

                            StringBuilder loSb = new StringBuilder();
                            loSb = Email_Body(iUserid, iId, "Expired");

                            xloMail.Body = loSb.ToString();
                            xloMail.IsBodyHtml = true;
                            Attachment attachFile = new Attachment(_strfileName);
                            xloMail.Attachments.Add(attachFile);

                            try
                            {
                                System.Threading.Thread email = new System.Threading.Thread(delegate ()
                                {
                                    xloSmtp.Send(xloMail);
                                }
                                );
                                email.IsBackground = true;
                                email.Start();

                                Database db3 = new Database();
                                System.Collections.Generic.List<System.Data.IDbDataParameter> para3;
                                para3 = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

                                para3.Add(db3.CreateParam("iUserId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, iUserid));
                                para3.Add(db3.CreateParam("iId", System.Data.DbType.String, System.Data.ParameterDirection.Input, iId));
                                para3.Add(db3.CreateParam("StatusType", System.Data.DbType.String, System.Data.ParameterDirection.Input, "Expired"));
                                para3.Add(db3.CreateParam("Status", System.Data.DbType.Boolean, System.Data.ParameterDirection.Input, true));

                                db3.ExecuteSP("OfferDetail_Expired_Cancelled_Email_Status", para3.ToArray(), false);

                                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                                sb.Append("SUCCESS Offer Expired UserName : " + sUsername + ", Log Time : " + DAL.Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                                sb.AppendLine("");
                                File.AppendAllText(path, sb.ToString());
                                sb.Clear();

                            }
                            catch (Exception ex)
                            {
                                DAL.Common.InsertErrorLog(ex, null, Request);
                                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                                sb.Append("Email Send Error " + ex.Message + " for UserName : " + sUsername + ", Log Time : " + DAL.Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                                sb.AppendLine("");
                                File.AppendAllText(path, sb.ToString());
                                sb.Clear();
                            }
                        }
                        else
                        {
                            DAL.Common.InsertErrorLog(null, "Email Id List Not Not Found for UserName : " + sUsername, Request);
                            sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                            sb.Append("Email Id List Not Not Found for UserName : " + sUsername + ", Log Time : " + DAL.Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                            sb.AppendLine("");
                            File.AppendAllText(path, sb.ToString());
                            sb.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        DAL.Common.InsertErrorLog(ex, null, Request);
                        sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                        sb.Append(ex.Message + " for UserName : " + sUsername + ", Log Time : " + DAL.Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                        sb.AppendLine("");
                        File.AppendAllText(path, sb.ToString());
                        sb.Clear();
                    }
                }
            }
            else
            {
                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                sb.Append("No Any Offer Available for Expired, Log Time : " + DAL.Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                sb.AppendLine("");
                File.AppendAllText(path, sb.ToString());
                sb.Clear();
            }

            return Ok(new CommonResponse
            {
                Message = "",
                Status = "",
                Error = ""
            });
        }

        #endregion

        #region Task Scheduler : WS_OfferFortuneStatus

        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult WS_OfferFortuneStatus()
        {
            string path = HttpContext.Current.Server.MapPath("~/Offer_Fortune_Status_Update_Log.txt");
            if (!File.Exists(@"" + path + ""))
            {
                File.Create(@"" + path + "").Dispose();
            }
            StringBuilder sb = new StringBuilder();

            Oracle_DBAccess oracleDbAccess = new Oracle_DBAccess();
            List<OracleParameter> paramList = new List<OracleParameter>();

            OracleParameter p_1 = new OracleParameter("vrec", OracleDbType.RefCursor);
            p_1.Direction = ParameterDirection.Output;
            paramList.Add(p_1);

            DataTable dt = oracleDbAccess.CallSP("web_trans.offer_status_update", paramList);

            if (dt != null && dt.Rows.Count > 0)
            {
                Database db = new Database(Request);
                List<SqlParameter> para = new List<SqlParameter>();

                SqlParameter param = new SqlParameter("tableInq", SqlDbType.Structured);
                param.Value = dt;
                para.Add(param);

                DataTable dt1 = db.ExecuteSP("offer_status_update", para.ToArray(), false);
                string Status, Message, Total_Transaction, RefNo_UserId_iOfferId;

                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    Status = dt1.Rows[0]["Status"].ToString();
                    Message = dt1.Rows[0]["Message"].ToString();
                    Total_Transaction = dt1.Rows[0]["Total_Transaction"].ToString();
                    RefNo_UserId_iOfferId = dt1.Rows[0]["RefNo_UserId_iOfferId"].ToString();
                   
                    sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                    if (Status == "1")
                    {
                        sb.Append(Message + ", Total Transaction : " + Total_Transaction + (Total_Transaction != "0" ? ", RefNo_UserId_iOfferId : " + RefNo_UserId_iOfferId + " " : " ") + ", Log Time : " + DAL.Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                    }
                    else
                    {
                        sb.Append(Message + ", Log Time : " + DAL.Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                    }

                    sb.AppendLine("");
                    File.AppendAllText(path, sb.ToString());
                    sb.Clear();
                }
            }
            else
            {
                sb.AppendLine("= = = = = = = = = = = = = = = = = = = = = = = = = = = ");
                sb.Append("No Record Found in Fortune, Log Time : " + DAL.Common.GetHKTime().ToString("dd-MM-yyyy hh:mm:ss tt"));
                sb.AppendLine("");
                File.AppendAllText(path, sb.ToString());
                sb.Clear();
            }

            return Ok(new CommonResponse
            {
                Message = "",
                Status = "",
                Error = ""
            });
        }

        #endregion


        public static StringBuilder Email_Body(Int32 iUserid, String iId, String StatusType)
        {
            Database db1 = new Database();
            System.Collections.Generic.List<System.Data.IDbDataParameter> para1;
            para1 = new System.Collections.Generic.List<System.Data.IDbDataParameter>();
            para1.Add(db1.CreateParam("iiUserId", System.Data.DbType.String, System.Data.ParameterDirection.Input, iUserid));

            System.Data.DataTable dtUserDetail = db1.ExecuteSP("UserMas_SelectOne", para1.ToArray(), false);

            StringBuilder loSb = new StringBuilder();
            loSb.Append(EmailHeader());
            loSb.Append(@"<table cellpadding=""0"" cellspacing=""0"" width=""100%"">");
            if (dtUserDetail.Rows[0]["sCompName"] != null && dtUserDetail.Rows[0]["sCompName"].ToString() != "")
            {
                loSb.Append(@"<tr><td>Company Name:</td><td>" + dtUserDetail.Rows[0]["sCompName"].ToString() + "(" + dtUserDetail.Rows[0]["sCompMobile"].ToString() + ")</td></tr>");
            }
            if (dtUserDetail.Rows[0]["sCompAddress"] != null && dtUserDetail.Rows[0]["sCompAddress"].ToString() != "")
            {
                loSb.Append(@"<tr><td>Address:</td><td>" + dtUserDetail.Rows[0]["sCompAddress"].ToString() + "</td></tr>");
            }
            if (dtUserDetail.Rows[0]["AssistBy1"] != null && dtUserDetail.Rows[0]["AssistBy1"].ToString() != "")
            {
                loSb.Append(@"<tr><td>Sales Person:</td><td>" + dtUserDetail.Rows[0]["AssistBy1"].ToString() + "</td></tr>");
            }
            if (dtUserDetail.Rows[0]["mob_AssistBy1"] != null && dtUserDetail.Rows[0]["mob_AssistBy1"].ToString() != "")
            {
                loSb.Append(@"<tr><td>Mobile:</td><td>" + dtUserDetail.Rows[0]["mob_AssistBy1"].ToString() + "</td></tr>");
                loSb.Append(@"<tr><td>Whatsapp No:</td><td>" + dtUserDetail.Rows[0]["mob_AssistBy1"].ToString() + "</td></tr>");
            }
            if (dtUserDetail.Rows[0]["sWeChatId1"] != null && dtUserDetail.Rows[0]["sWeChatId1"].ToString() != "")
            {
                loSb.Append(@"<tr><td>WeChat ID:</td><td>" + dtUserDetail.Rows[0]["sWeChatId1"].ToString() + "</td></tr>");
            }
            if (dtUserDetail.Rows[0]["Email_AssistBy1"] != null && dtUserDetail.Rows[0]["Email_AssistBy1"].ToString() != "")
            {
                loSb.Append(@"<tr><td>Email:</td><td>" + dtUserDetail.Rows[0]["Email_AssistBy1"].ToString() + "</td></tr>");
            }
            if (dtUserDetail.Rows[0]["iUserType"] != null && dtUserDetail.Rows[0]["iUserType"].ToString() != "3")
            {
                string Fname = "", Lname = "";
                if (dtUserDetail.Rows[0]["sFirstName"] != null && dtUserDetail.Rows[0]["sFirstName"].ToString() != "")
                {
                    Fname = dtUserDetail.Rows[0]["sFirstName"].ToString();
                }
                if (dtUserDetail.Rows[0]["sLastName"] != null && dtUserDetail.Rows[0]["sLastName"].ToString() != "")
                {
                    Lname = dtUserDetail.Rows[0]["sLastName"].ToString();
                }
                loSb.Append(@"<tr><td>Employee Name:</td><td>" + Fname + " " + Lname + "</td></tr>");
            }
            loSb.Append("</table>");
            loSb.Append("<br/> <br/>");

            Database db2 = new Database();
            System.Collections.Generic.List<System.Data.IDbDataParameter> para2;
            para2 = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

            para2.Add(db2.CreateParam("iUserId", System.Data.DbType.Int32, System.Data.ParameterDirection.Input, iUserid));
            para2.Add(db2.CreateParam("iId", System.Data.DbType.String, System.Data.ParameterDirection.Input, iId));
            para2.Add(db2.CreateParam("StatusType", System.Data.DbType.String, System.Data.ParameterDirection.Input, StatusType));

            DataTable dtofferdetail = db2.ExecuteSP("OfferDetail_SelectAllByOrderId_Email_Expired_Cancelled", para2.ToArray(), false);

            if (dtofferdetail != null && dtofferdetail.Rows.Count > 0)
            {
                loSb.Append("<div style='width: 100%;overflow-x:scroll!important;'>");
                loSb.Append("<table border = '1' style='overflow-x:scroll !important; width:2000px !important;'>");

                loSb.Append("<tr>");

                string _strfont = "\"font-size: 12px; font-family: Tahoma;text-align:center; background-color: #83CAFF;\"";
                foreach (DataColumn column in dtofferdetail.Columns)
                {
                    loSb.Append("<th style = " + _strfont + ">");
                    loSb.Append(column.ColumnName);
                    loSb.Append("</th>");
                }
                loSb.Append("</tr>");

                _strfont = "\"font-size: 10px; font-family: Tahoma;text-align:center; \"";

                string certiNo = "";
                foreach (DataRow row1 in dtofferdetail.Rows)
                {
                    if (row1["Stock Id"].ToString() != "Total")
                    {
                        loSb.Append("<tr>");
                    }
                    else
                    {
                        loSb.Append("<tr style='background-color: #83CAFF;'>");
                    }
                    foreach (DataColumn column in dtofferdetail.Columns)
                    {
                        string _strcheck = "";
                        if (row1["Stock Id"].ToString() != "Total" && (column.ColumnName.ToString() == "Disc(%)" || column.ColumnName.ToString() == "Net Amt($)" || column.ColumnName.ToString() == "Offer Amt" || column.ColumnName.ToString() == "Offer Disc"))
                        {
                            string _strstyle = "\"font-size: 10px; font-family: Tahoma;text-align:center;color: red;\"";
                            loSb.Append("<td style = " + _strstyle + ">");
                        }
                        else if (column.ColumnName.ToString() == "Cut" || column.ColumnName.ToString() == "Polish" || column.ColumnName.ToString() == "Symm")
                        {
                            loSb.Append("<td style = " + _strfont + ">");
                            if (row1["Cut"].ToString() == "3EX" && row1["Polish"].ToString() == "EX" && row1["Symm"].ToString() == "EX")
                            {
                                loSb.Append("<b>" + row1[column.ColumnName] + "<b>");
                                _strcheck = "Y";
                            }
                        }
                        else
                            loSb.Append("<td style = " + _strfont + ">");

                        if (_strcheck != "Y")
                        {
                            if (column.ColumnName == "Image")
                            {
                                if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                    loSb.Append("<a href=\"" + row1[column.ColumnName] + "\" target=\"_blank\">Image</a>");
                                else
                                    loSb.Append("");
                            }
                            else if (column.ColumnName == "Video")
                            {
                                if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                    loSb.Append("<a href=\"" + row1[column.ColumnName] + "\" target=\"_blank\">Video</a>");
                                else
                                    loSb.Append("");
                            }
                            else if (column.ColumnName == "Dna")
                            {
                                if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                    loSb.Append("<a href=\"" + row1[column.ColumnName] + "\" target=\"_blank\">Dna</a>");
                                else
                                    loSb.Append("");
                            }
                            else if (column.ColumnName == "Lab")
                            {
                                if (row1["Certi No"] != null && row1["Certi No"].ToString() != "")
                                    certiNo = row1["Certi No"].ToString();
                                else
                                    certiNo = "";
                                if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                    loSb.Append("<a href=\"http://www.gia.edu/cs/Satellite?pagename=GST%2FDispatcher&childpagename=GIA%2FPage%2FReportCheck&c=Page&cid=1355954554547&reportno=" + certiNo + "\" target=\"_blank\">"
                                        + row1[column.ColumnName].ToString() + "</a>");
                                else
                                    loSb.Append("");
                            }
                            else if (column.ColumnName == "Rap Amt($)" || column.ColumnName == "Cts"
                                 || column.ColumnName == "Rap Price($)" || column.ColumnName == "Disc(%)"
                                 || column.ColumnName == "Net Amt($)" || column.ColumnName == "Offer Disc(%)"
                                 || column.ColumnName == "Offer Amt($)")
                            {
                                if (row1[column.ColumnName] != null && row1[column.ColumnName].ToString() != "")
                                    loSb.Append(Convert.ToDecimal(row1[column.ColumnName]).ToString("#,##0.00"));
                                else
                                    loSb.Append("");
                            }
                            else
                                loSb.Append(row1[column.ColumnName]);
                        }
                        loSb.Append("</td>");

                    }
                    loSb.Append("</tr>");
                }

                loSb.Append("</table></div>");
            }

            loSb.Append(EmailSignature());
            return loSb;
        }
        [HttpPost]
        public IHttpActionResult Offer_Delete([FromBody]JObject data)
        {
            Offer_Delete_Req req = new Offer_Delete_Req();
            try
            {
                req = JsonConvert.DeserializeObject<Offer_Delete_Req>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CommonResponse>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);

                var db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();
                para.Add(db.CreateParam("DeletedBy", DbType.Int64, ParameterDirection.Input, Convert.ToInt64(userID)));
                para.Add(db.CreateParam("Offer_Id", DbType.String, ParameterDirection.Input, req.iId));

                DataTable dtData = db.ExecuteSP("Offer_Delete", para.ToArray(), false);

                OfferDelete_From_Oracle(req.iId, "Y");
                Email_For_DeletedOffer(req.iId);

                return Ok(new CommonResponse
                {
                    Error = "",
                    Message = "Offer Delete Successfully",
                    Status = "1"
                });
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CommonResponse>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        private void OfferDelete_From_Oracle(String iId_List, String OfferType)
        {
            Database db;
            System.Collections.Generic.List<System.Data.IDbDataParameter> para;
            db = new Database();
            para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

            para.Add(db.CreateParam("iId", System.Data.DbType.String, System.Data.ParameterDirection.Input, iId_List));
            DataTable dt = db.ExecuteSP("OfferGet", para.ToArray(), false);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int userID = Convert.ToInt32(dt.Rows[i]["iUserid"]);

                String CustomerName = GetPartyNameByUserId(Convert.ToInt32(userID));
                Int32 AssistByID = GetAssistByUserId(Convert.ToInt32(userID));

                Int32 Assistby = AssistByID;
                string Party = CustomerName;

                DAL.Usermas objUser = new DAL.Usermas();
                DataTable loUserMas = objUser.UserMas_SelectOne(Convert.ToInt64(userID));
                string _stremailid = "";
                if (String.IsNullOrEmpty(loUserMas.Rows[0]["sCompEmail"].ToString()))
                    _stremailid = loUserMas.Rows[0]["sEmail"].ToString();
                else
                    _stremailid = loUserMas.Rows[0]["sCompEmail"].ToString();

                FortuneService.ServiceSoapClient wbService = new FortuneService.ServiceSoapClient();

                String sRefNo = dt.Rows[i]["sRefNo"].ToString();
                String iOfferID = dt.Rows[i]["iOfferId"].ToString();
                decimal Offer_Discount = Convert.ToDecimal(dt.Rows[i]["SOfferPer"]);
                decimal Offer_Amount = Convert.ToDecimal(dt.Rows[i]["SOfferAmt"]);

                int sValidity = Convert.ToInt32(dt.Rows[i]["SOffer_Validity"]);
                string Remark = dt.Rows[i]["SOfferRemark"].ToString();
                Int64 FortunePartyCode = Get_FortunePartyCode_ByUserId(Convert.ToInt32(userID));
                
                try
                {
                    FortuneService.CommonResultResponse cResult;
                    cResult = wbService.MakeOfferTrans_New(
                        _stremailid,
                        Convert.ToString(userID),
                        Party,
                        iOfferID.ToString(),
                        Offer_Discount,
                        Convert.ToInt16(sValidity),
                        sRefNo,
                        Remark,
                        OfferType,
                        Offer_Amount,
                        FortunePartyCode);
                }
                catch
                {

                }

            }

        }
        private void Email_For_DeletedOffer(String iId_List)
        {
            String msg = "";

            Database db;
            System.Collections.Generic.List<System.Data.IDbDataParameter> para;
            db = new Database();
            para = new System.Collections.Generic.List<System.Data.IDbDataParameter>();

            para.Add(db.CreateParam("iId", System.Data.DbType.String, System.Data.ParameterDirection.Input, iId_List));
            DataTable dt = db.ExecuteSP("OfferDeleted_Email_Get", para.ToArray(), false);

            if (dt != null && dt.Rows.Count > 0)
            {
                db = new Database();
                List<SqlParameter> para_sql = new List<SqlParameter>();

                SqlParameter param_tp = new SqlParameter("tableInq", SqlDbType.Structured);
                param_tp.Value = dt;
                para_sql.Add(param_tp);

                DataTable I = db.ExecuteSP("OfferDeleted_Email_Log_Insert", para_sql.ToArray(), false);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int iUserid = Convert.ToInt32(dt.Rows[i]["iUserid"]);
                    string iId = dt.Rows[i]["iId"].ToString();
                    string sUsername = dt.Rows[i]["sUsername"].ToString();

                    try
                    {
                        String _strfileName = "";
                        Random rnd = new Random();

                        DAL.Stock objstock = new DAL.Stock();
                        DataTable dt1 = objstock.Offer_Excel_Expired_Cancelled(iUserid, iId, "Deleted");
                        _strfileName = HttpContext.Current.Server.MapPath("~/Temp/Excel/" + rnd.Next().ToString() + ".xlsx");

                        if (File.Exists(_strfileName) == true)
                        {
                            File.Delete(_strfileName);
                        }

                        EpExcelExport.Offer_Excel_Expired_Cancelled(dt1, _strfileName, "Deleted");

                        MailMessage xloMail = new MailMessage();
                        SmtpClient xloSmtp = new SmtpClient();

                        DAL.Usermas objUser = new DAL.Usermas();
                        DataTable loEmails = objUser.UserMas_SelectEmailByUserId_For_Offer(iUserid);

                        string lsToMail = "";
                        foreach (DataRow lrEmail in loEmails.Rows)
                            lsToMail += lrEmail["sEmail"].ToString() + ",";

                        if (lsToMail.Length > 0)
                            lsToMail = lsToMail.Remove(lsToMail.Length - 1);

                        string _strresult = "";
                        if (lsToMail.Trim() != "" && _strfileName != "")
                        {
                            _strresult = "Y";

                            xloMail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromEmail"], "Sunrise Diamonds");
                            xloMail.To.Add(lsToMail);

                            xloMail.Subject = "SUNRISE DIAMONDS : Offer Deleted";

                            StringBuilder loSb = new StringBuilder();
                            loSb = Email_Body(iUserid, iId, "Deleted");

                            xloMail.Body = loSb.ToString();
                            xloMail.IsBodyHtml = true;
                            Attachment attachFile = new Attachment(_strfileName);
                            xloMail.Attachments.Add(attachFile);

                            try
                            {
                                System.Threading.Thread email = new System.Threading.Thread(delegate ()
                                {
                                    xloSmtp.Send(xloMail);
                                }
                                );
                                email.IsBackground = true;
                                email.Start();

                            }
                            catch (Exception ex)
                            {
                                msg = ex.Message;
                            }
                        }
                        else
                        {
                            msg = "Email Id List Not Found for UserName : " + sUsername;
                        }
                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                    }
                }
            }
            else
            {
                msg = "No Any Deleted Offer";
            }

        }
    }
}