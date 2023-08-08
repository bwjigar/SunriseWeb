using DAL;
using EpExcelExportLib;
using Lib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sunrise.Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml;

namespace Sunrise.Services.Controllers
{
    [Authorize]
    [RoutePrefix("api/Customer")]
    public class CustomerController : ApiController
    {
        [HttpPost]
        public IHttpActionResult GetCustomer([FromBody]JObject data)
        {
            CustomerReq req = new CustomerReq();
            try
            {
                req = JsonConvert.DeserializeObject<CustomerReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Customer>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);

                para.Add(db.CreateParam("EmpId", DbType.Int32, ParameterDirection.Input, userID));

                if (!string.IsNullOrEmpty(req.SearchText))
                    para.Add(db.CreateParam("SearchText", DbType.String, ParameterDirection.Input, req.SearchText));
                else
                    para.Add(db.CreateParam("SearchText", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("UserMas_SelectByAssist_CompanyUserCustomerWise", para.ToArray(), false);
                List<Customer> customerList = new List<Customer>();
                customerList = DataTableExtension.ToList<Customer>(dt);

                if (customerList != null && customerList.Count > 0)
                {
                    return Ok(new ServiceResponse<Customer>
                    {
                        Data = customerList,
                        Message = "Success",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<Customer>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Customer>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult GetCustomerDisc([FromBody]JObject data)
        {
            CustomerDiscReq req = new CustomerDiscReq();
            try
            {
                req = JsonConvert.DeserializeObject<CustomerDiscReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CustomerDiscResponse>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (!string.IsNullOrEmpty(req.UserName))
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, req.UserName));
                else
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, DBNull.Value));

                para.Add(db.CreateParam("PageNo", DbType.Int32, ParameterDirection.Input, req.PageNo));
                para.Add(db.CreateParam("PageSize", DbType.Int32, ParameterDirection.Input, req.PageSize));

                DataTable dt = db.ExecuteSP("CustomerDisc_Select", para.ToArray(), false);

                SearchSummary searchSummary = new SearchSummary();
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] dra = dt.Select("RowNo IS NULL");
                    searchSummary.TOT_PCS = Convert.ToInt32(dra[0]["CustDiscId"]);
                }

                dt.DefaultView.RowFilter = "RowNo IS NOT NULL";
                dt = dt.DefaultView.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<CustomerDisc> customerList = new List<CustomerDisc>();
                    customerList = DataTableExtension.ToList<CustomerDisc>(dt);
                    List<CustomerDiscResponse> customerResponses = new List<CustomerDiscResponse>();

                    customerResponses.Add(new CustomerDiscResponse()
                    {
                        DataList = customerList,
                        DataSummary = searchSummary
                    });

                    return Ok(new ServiceResponse<CustomerDiscResponse>
                    {
                        Data = customerResponses,
                        Message = "Success",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<CustomerDiscResponse>
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
                return Ok(new ServiceResponse<CustomerDiscResponse>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult SaveCustomerDisc([FromBody]JObject data)
        {
            CustomerDiscReq req = new CustomerDiscReq();
            try
            {
                req = JsonConvert.DeserializeObject<CustomerDiscReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CustomerDisc>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                req.xmlStr = req.xmlStr.Replace("%3C", "<");
                req.xmlStr = req.xmlStr.Replace("%3E", ">");
                req.xmlStr = req.xmlStr.Replace("%2C", ",");
                req.xmlStr = req.xmlStr.Replace("%20", " ");
                req.xmlStr = req.xmlStr.Replace("%28", "(");
                req.xmlStr = req.xmlStr.Replace("%29", ")");

                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);

                para.Add(db.CreateParam("loggedUserId", DbType.Int32, ParameterDirection.Input, userID));
                para.Add(db.CreateParam("CustId", DbType.String, ParameterDirection.Input, req.CustId));
                para.Add(db.CreateParam("TransId", DbType.Int32, ParameterDirection.Input, req.TransId));
                para.Add(db.CreateParam("Oper", DbType.String, ParameterDirection.Input, req.Oper));
                para.Add(db.CreateParam("Input", DbType.String, ParameterDirection.Input, req.xmlStr));

                DataTable dt = db.ExecuteSP("CustomerDisc_Crud", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Msg"].ToString() == "success")
                    {
                        return Ok(new CommonResponse
                        {
                            Error = "",
                            Message = dt.Rows[0]["Msg"].ToString(),
                            Status = "1"
                        });
                    }
                    else
                    {
                        return Ok(new CommonResponse
                        {
                            Error = "",
                            Message = dt.Rows[0]["Msg"].ToString(),
                            Status = "0"
                        });
                    }
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
                return Ok(new ServiceResponse<CustomerDisc>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult GetPartyInfo([FromBody]JObject data)
        {
            PartyInfoReq req = new PartyInfoReq();
            try
            {
                req = JsonConvert.DeserializeObject<PartyInfoReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<PartyInfoResponse>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (!string.IsNullOrEmpty(req.PartyName))
                    para.Add(db.CreateParam("sPartyName", DbType.String, ParameterDirection.Input, req.PartyName));
                else
                    para.Add(db.CreateParam("sPartyName", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.ContactPerson))
                    para.Add(db.CreateParam("sContactPerson", DbType.String, ParameterDirection.Input, req.ContactPerson));
                else
                    para.Add(db.CreateParam("sContactPerson", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.PartyPrefix))
                    para.Add(db.CreateParam("sPartyPrefix", DbType.String, ParameterDirection.Input, req.PartyPrefix));
                else
                    para.Add(db.CreateParam("sPartyPrefix", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (req.CountryId > 0)
                    para.Add(db.CreateParam("sCountryId", DbType.Int32, ParameterDirection.Input, req.CountryId));
                else
                    para.Add(db.CreateParam("sCountryId", DbType.Int32, ParameterDirection.Input, DBNull.Value));

                if (req.PageNo > 0)
                    para.Add(db.CreateParam("iPgNo", DbType.Int32, ParameterDirection.Input, req.PageNo));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int32, ParameterDirection.Input, DBNull.Value));

                if (req.PageSize > 0)
                    para.Add(db.CreateParam("iPgSize", DbType.Int32, ParameterDirection.Input, req.PageSize));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int32, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(req.OrderBy))
                    para.Add(db.CreateParam("sOrderBy", DbType.String, ParameterDirection.Input, req.OrderBy));
                else
                    para.Add(db.CreateParam("sOrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("PartyInfo_SelectByPara", para.ToArray(), false);
                List<PartyInfoResponse> partyInfoList = new List<PartyInfoResponse>();
                partyInfoList = DataTableExtension.ToList<PartyInfoResponse>(dt);

                if (partyInfoList != null && partyInfoList.Count > 0)
                {
                    return Ok(new ServiceResponse<PartyInfoResponse>
                    {
                        Data = partyInfoList,
                        Message = "Success",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<PartyInfoResponse>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<PartyInfoResponse>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult GetSupplier([FromBody]JObject data)
        {
            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                DataTable dt = db.ExecuteSP("Overseas_Supplier", para.ToArray(), false);
                List<PartyInfoResponse> partyInfoList = new List<PartyInfoResponse>();
                partyInfoList = DataTableExtension.ToList<PartyInfoResponse>(dt);

                if (partyInfoList != null && partyInfoList.Count > 0)
                {
                    return Ok(new ServiceResponse<PartyInfoResponse>
                    {
                        Data = partyInfoList,
                        Message = "Success",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<PartyInfoResponse>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<PartyInfoResponse>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult GetUserDisc([FromBody]JObject data)
        {
            CustomerDiscReq req = new CustomerDiscReq();
            try
            {
                req = JsonConvert.DeserializeObject<CustomerDiscReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CustomerDiscResponse>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (!string.IsNullOrEmpty(req.UserName))
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, req.UserName));
                else
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, DBNull.Value));

                para.Add(db.CreateParam("PageNo", DbType.Int32, ParameterDirection.Input, req.PageNo));
                para.Add(db.CreateParam("PageSize", DbType.Int32, ParameterDirection.Input, req.PageSize));

                DataTable dt = db.ExecuteSP("UserDisc_Select", para.ToArray(), false);

                dt.DefaultView.RowFilter = "iTransId IS NOT NULL";
                dt = dt.DefaultView.ToTable();

                SearchSummary searchSummary = new SearchSummary();
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] dra = dt.Select("RowNo IS NULL");
                    searchSummary.TOT_PCS = Convert.ToInt32(dra[0]["iTransId"]);
                }

                dt.DefaultView.RowFilter = "RowNo IS NOT NULL";
                dt = dt.DefaultView.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<CustomerDisc> customerList = new List<CustomerDisc>();
                    customerList = DataTableExtension.ToList<CustomerDisc>(dt);
                    List<CustomerDiscResponse> customerResponses = new List<CustomerDiscResponse>();

                    customerResponses.Add(new CustomerDiscResponse()
                    {
                        DataList = customerList,
                        DataSummary = searchSummary
                    });

                    return Ok(new ServiceResponse<CustomerDiscResponse>
                    {
                        Data = customerResponses,
                        Message = "Success",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<CustomerDiscResponse>
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
                return Ok(new ServiceResponse<CustomerDiscResponse>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Get_StockDiscMgtReport([FromBody]JObject data)
        {
            CustomerDiscReq req = new CustomerDiscReq();
            try
            {
                req = JsonConvert.DeserializeObject<CustomerDiscReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CustomerDiscResponse>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                para.Add(db.CreateParam("UserId", DbType.Int32, ParameterDirection.Input, userID));

                if (!string.IsNullOrEmpty(req.UserName))
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, req.UserName));
                else
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, DBNull.Value));

                para.Add(db.CreateParam("PageNo", DbType.Int32, ParameterDirection.Input, req.PageNo));
                para.Add(db.CreateParam("PageSize", DbType.Int32, ParameterDirection.Input, req.PageSize));

                DataTable dt = db.ExecuteSP("StockDiscMgt_Select", para.ToArray(), false);

                List<GetStockDiscRes> getstockdiscres = new List<GetStockDiscRes>();
                getstockdiscres = DataTableExtension.ToList<GetStockDiscRes>(dt);

                if (getstockdiscres != null && getstockdiscres.Count > 0)
                {
                    return Ok(new ServiceResponse<GetStockDiscRes>
                    {
                        Data = getstockdiscres,
                        Message = "Success",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<GetStockDiscRes>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<GetStockDiscRes>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Excel_StockDiscMgtReport([FromBody]JObject data)
        {
            CustomerDiscReq req = new CustomerDiscReq();
            try
            {
                req = JsonConvert.DeserializeObject<CustomerDiscReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CustomerDiscResponse>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                para.Add(db.CreateParam("UserId", DbType.Int32, ParameterDirection.Input, userID));

                if (!string.IsNullOrEmpty(req.UserName))
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, req.UserName));
                else
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, DBNull.Value));

                para.Add(db.CreateParam("PageNo", DbType.Int32, ParameterDirection.Input, req.PageNo));
                para.Add(db.CreateParam("PageSize", DbType.Int32, ParameterDirection.Input, req.PageSize));

                DataTable dt = db.ExecuteSP("StockDiscMgt_Select", para.ToArray(), false);

                dt.DefaultView.RowFilter = "RowNo IS NOT NULL";
                dt = dt.DefaultView.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    string filename = "Stock & Disc Mgt. Report " + Lib.Models.Common.GetHKTime().ToString("ddMMyyyy-HHmmss");
                    string _path = ConfigurationManager.AppSettings["data"];
                    string realpath = HostingEnvironment.MapPath("~/ExcelFile/");
                    string _livepath = ConfigurationManager.AppSettings["LiveUrl"];

                    EpExcelExport.CreateStockDiscExcel(dt.DefaultView.ToTable(), realpath, realpath + filename + ".xlsx", _livepath);

                    string _strxml = _path + filename + ".xlsx";
                    return Ok(_strxml);
                }
                else
                {
                    return Ok("No data found.");
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok("Something Went wrong.\nPlease try again later");
            }
        }
        [HttpPost]
        public IHttpActionResult GetUserDisc_Excel([FromBody]JObject data)
        {
            CustomerDiscReq req = new CustomerDiscReq();
            try
            {
                req = JsonConvert.DeserializeObject<CustomerDiscReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CustomerDiscResponse>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                if (!string.IsNullOrEmpty(req.UserName))
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, req.UserName));
                else
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, DBNull.Value));

                para.Add(db.CreateParam("PageNo", DbType.Int32, ParameterDirection.Input, req.PageNo));
                para.Add(db.CreateParam("PageSize", DbType.Int32, ParameterDirection.Input, req.PageSize));

                DataTable dt = db.ExecuteSP("UserDisc_Select", para.ToArray(), false);

                dt.DefaultView.RowFilter = "iTransId IS NOT NULL";
                dt = dt.DefaultView.ToTable();

                SearchSummary searchSummary = new SearchSummary();
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] dra = dt.Select("RowNo IS NULL");
                    searchSummary.TOT_PCS = Convert.ToInt32(dra[0]["iTransId"]);
                }

                dt.DefaultView.RowFilter = "RowNo IS NOT NULL";
                dt = dt.DefaultView.ToTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    string filename = "Stock & Disc Mgt. Report " + Lib.Models.Common.GetHKTime().ToString("ddMMyyyy-HHmmss");
                    string _path = ConfigurationManager.AppSettings["data"];
                    string realpath = HostingEnvironment.MapPath("~/ExcelFile/");
                    string _livepath = ConfigurationManager.AppSettings["LiveUrl"];

                    EpExcelExport.CreateUserDiscExcel(dt.DefaultView.ToTable(), realpath, realpath + filename + ".xlsx", _livepath);

                    string _strxml = _path + filename + ".xlsx";
                    return Ok(_strxml);
                }
                else
                {
                    return Ok("No data found.");
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok("Something Went wrong.\nPlease try again later");
            }
        }

        [HttpPost]
        public IHttpActionResult SaveUserDisc([FromBody]JObject data)
        {
            CustomerDiscReq req = new CustomerDiscReq();
            try
            {
                req = JsonConvert.DeserializeObject<CustomerDiscReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<CustomerDisc>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                req.xmlStr = req.xmlStr.Replace("%3C", "<");
                req.xmlStr = req.xmlStr.Replace("%3E", ">");
                req.xmlStr = req.xmlStr.Replace("%2C", ",");
                req.xmlStr = req.xmlStr.Replace("%20", " ");

                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);

                para.Add(db.CreateParam("loggedUserId", DbType.Int32, ParameterDirection.Input, userID));
                para.Add(db.CreateParam("CustId", DbType.String, ParameterDirection.Input, req.CustId));
                para.Add(db.CreateParam("TransId", DbType.Int32, ParameterDirection.Input, req.TransId));
                para.Add(db.CreateParam("Oper", DbType.String, ParameterDirection.Input, req.Oper));
                para.Add(db.CreateParam("Input", DbType.String, ParameterDirection.Input, req.xmlStr));

                DataTable dt = db.ExecuteSP("UserDisc_Crud", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Msg"].ToString() == "success")
                    {
                        return Ok(new CommonResponse
                        {
                            Error = "",
                            Message = dt.Rows[0]["Msg"].ToString(),
                            Status = "1"
                        });
                    }
                    else
                    {
                        return Ok(new CommonResponse
                        {
                            Error = "",
                            Message = dt.Rows[0]["Msg"].ToString(),
                            Status = "0"
                        });
                    }
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
                return Ok(new ServiceResponse<CustomerDisc>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult SaveStockDisc([FromBody]JObject data)
        {
            SaveStockDiscReq savestockdiscreq = new SaveStockDiscReq();
            try
            {
                savestockdiscreq = JsonConvert.DeserializeObject<SaveStockDiscReq>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<SaveStockDiscReq>
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
                para.Add(db.CreateParam("iUserId", DbType.Int64, ParameterDirection.Input, Convert.ToInt64(userID)));
                para.Add(db.CreateParam("Type", DbType.String, ParameterDirection.Input, savestockdiscreq.Type));

                if (!string.IsNullOrEmpty(savestockdiscreq.Id.ToString()))
                    para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, savestockdiscreq.Id));
                else
                    para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(savestockdiscreq.UserIdList))
                    para.Add(db.CreateParam("UserIdList", DbType.String, ParameterDirection.Input, savestockdiscreq.UserIdList));
                else
                    para.Add(db.CreateParam("UserIdList", DbType.String, ParameterDirection.Input, DBNull.Value));

                string savestockdisc_filters = IPadCommon.ToXML<List<SaveStockDisc_Filters>>(savestockdiscreq.Filters);
                para.Add(db.CreateParam("Filters", DbType.String, ParameterDirection.Input, savestockdisc_filters));

                DataTable dtData = db.ExecuteSP("StockDiscMgt_Save", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    if (dtData.Rows[0]["Status"].ToString() == "1")
                    {
                        return Ok(new CommonResponse
                        {
                            Error = "",
                            Message = dtData.Rows[0]["Message"].ToString(),
                            Status = "1"
                        });
                    }
                    else
                    {
                        return Ok(new CommonResponse
                        {
                            Error = "",
                            Message = dtData.Rows[0]["Message"].ToString(),
                            Status = "0"
                        });
                    }
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
                return Ok(new ServiceResponse<SaveStockDiscReq>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Get_StockDiscMgt([FromBody]JObject data)
        {
            StockDiscMgtRequest stockdiscmgtrequest = new StockDiscMgtRequest();
            try
            {
                stockdiscmgtrequest = JsonConvert.DeserializeObject<StockDiscMgtRequest>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<StockDiscMgtRequest>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);
                para.Add(db.CreateParam("UserId", DbType.Int32, ParameterDirection.Input, userID));

                if (!string.IsNullOrEmpty(stockdiscmgtrequest.UserList))
                    para.Add(db.CreateParam("UserList", DbType.String, ParameterDirection.Input, stockdiscmgtrequest.UserList));
                else
                    para.Add(db.CreateParam("UserList", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(stockdiscmgtrequest.sOrderBy))
                    para.Add(db.CreateParam("sOrderBy", DbType.String, ParameterDirection.Input, stockdiscmgtrequest.sOrderBy));
                else
                    para.Add(db.CreateParam("sOrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                para.Add(db.CreateParam("iPgNo", DbType.Int32, ParameterDirection.Input, stockdiscmgtrequest.iPgNo));
                para.Add(db.CreateParam("iPgSize", DbType.Int32, ParameterDirection.Input, stockdiscmgtrequest.iPgSize));

                DataTable dt = db.ExecuteSP("Get_UserDiscUserList", para.ToArray(), false);
                List<StockDiscMgtResponse> stockdiscmgtresponse = new List<StockDiscMgtResponse>();
                stockdiscmgtresponse = DataTableExtension.ToList<StockDiscMgtResponse>(dt);

                if (stockdiscmgtresponse != null && stockdiscmgtresponse.Count > 0)
                {
                    return Ok(new ServiceResponse<StockDiscMgtResponse>
                    {
                        Data = stockdiscmgtresponse,
                        Message = "Success",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<StockDiscMgtResponse>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<StockDiscMgtResponse>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult ImportStockDisc([FromBody]JObject data)
        {
            CommonResponse resp = new CommonResponse(); 
            StockImportList objLst = new StockImportList();
            char[] charsToTrim = { ' ' };
            try
            {
                objLst = JsonConvert.DeserializeObject<StockImportList>(data.ToString());
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

            try
            {
                int userID = Convert.ToInt32((Request.GetRequestContext().Principal as ClaimsPrincipal).Claims.Where(e => e.Type == "UserID").FirstOrDefault().Value);

                DataTable dt = new DataTable();
                dt.Columns.Add("UserName", typeof(string));
                dt.Columns.Add("Supplier", typeof(string));
                dt.Columns.Add("Download", typeof(string));
                dt.Columns.Add("View", typeof(string));
                dt.Columns.Add("PriceMethod", typeof(string));
                dt.Columns.Add("PricePer", typeof(string));

                if (objLst.StockImport.Count() > 0)
                {
                    for (int i = 0; i < objLst.StockImport.Count(); i++)
                    {
                        DataRow dr = dt.NewRow();

                        if (objLst.StockImport[i].UserName.ToString() == "" || objLst.StockImport[i].Supplier.ToString() == "" ||
                            objLst.StockImport[i].Download.ToString() == "" || objLst.StockImport[i].View.ToString() == "" ||
                            objLst.StockImport[i].PriceMethod.ToString() == "" || objLst.StockImport[i].PricePer.ToString() == "")
                        {
                            resp.Status = "0";
                            resp.Message = "All Fields are Required";
                            resp.Error = "";
                            return Ok(resp);
                        }
                        if (objLst.StockImport[i].PriceMethod.ToString().Trim(charsToTrim) == "Disc" && Convert.ToDecimal(objLst.StockImport[i].PricePer.ToString().Trim(charsToTrim)) > 0)
                        {
                            resp.Status = "0";
                            resp.Message = "Disc Must be Negative in " + objLst.StockImport[i].UserName.ToString().Trim(charsToTrim) + " UserName";
                            resp.Error = "";
                            return Ok(resp);
                        }
                        if (objLst.StockImport[i].PriceMethod.ToString().Trim(charsToTrim) == "Value" && Convert.ToDecimal(objLst.StockImport[i].PricePer.ToString().Trim(charsToTrim)) < 0)
                        {
                            resp.Status = "0";
                            resp.Message = "Value Must be Positive in " + objLst.StockImport[i].UserName.ToString().Trim(charsToTrim) + " UserName";
                            resp.Error = "";
                            return Ok(resp);
                        }

                        dr["UserName"] = objLst.StockImport[i].UserName.ToString().Trim(charsToTrim);
                        dr["Supplier"] = objLst.StockImport[i].Supplier.ToString().Trim(charsToTrim);
                        dr["Download"] = objLst.StockImport[i].Download.ToString().Trim(charsToTrim);
                        dr["View"] = objLst.StockImport[i].View.ToString().Trim(charsToTrim);
                        dr["PriceMethod"] = objLst.StockImport[i].PriceMethod.ToString().Trim(charsToTrim);
                        dr["PricePer"] = objLst.StockImport[i].PricePer.ToString().Trim(charsToTrim);

                        dt.Rows.Add(dr);
                    }

                    Database db = new Database(Request);
                    DataTable dtData = new DataTable();
                    List<SqlParameter> para = new List<SqlParameter>();

                    SqlParameter param = new SqlParameter("table", SqlDbType.Structured);
                    param.Value = dt;
                    para.Add(param);

                    dtData = db.ExecuteSP("ImportStockDisc_Insert", para.ToArray(), false);

                    if (dtData != null && dtData.Rows.Count > 0 && dtData.Rows[0]["Status"].ToString() == "1")
                    {
                        resp.Status = "1";
                        resp.Message = "Stock & Disc Import Successfully";
                        resp.Error = "";
                        return Ok(resp);
                    }
                    else
                    {
                        resp.Status = "0";
                        resp.Message = "Stock & Disc Import Fail";
                        resp.Error = "";
                        return Ok(resp); 
                    }
                }
                else
                {
                    resp.Status = "0";
                    resp.Message = "No Record will be Proceed";
                    resp.Error = "";
                    return Ok(resp); 
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                resp.Status = "0";
                resp.Message = "Something Went wrong.\nPlease try again later";
                resp.Error = "";
                return Ok(resp);
            }
        }
        [HttpPost]
        public IHttpActionResult Get_SupplierPrefix([FromBody]JObject data)
        {
            SuppPrefix_Request req = new SuppPrefix_Request();
            try
            {
                req = JsonConvert.DeserializeObject<SuppPrefix_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<SuppPrefix_Response>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                para.Add(db.CreateParam("Supplier_Id", DbType.String, ParameterDirection.Input, req.Supplier_Id));

                DataTable dt = db.ExecuteSP("SupplierPrefix_select", para.ToArray(), false);
                List<SuppPrefix_Response> get_suppprefix = new List<SuppPrefix_Response>();
                get_suppprefix = DataTableExtension.ToList<SuppPrefix_Response>(dt);

                if (get_suppprefix != null && get_suppprefix.Count > 0)
                {
                    return Ok(new ServiceResponse<SuppPrefix_Response>
                    {
                        Data = get_suppprefix,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<SuppPrefix_Response>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<SuppPrefix_Response>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Save_SuppPrefix([FromBody]JObject data)
        {
            Save_SuppPrefix_Request req = new Save_SuppPrefix_Request();
            try
            {
                req = JsonConvert.DeserializeObject<Save_SuppPrefix_Request>(data.ToString());
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
                DataTable dt = new DataTable();
                dt.Columns.Add("SupplierId", typeof(string));
                dt.Columns.Add("Pointer_Id", typeof(string));
                dt.Columns.Add("Prefix", typeof(string));

                if (req.SuppPre.Count() > 0)
                {
                    for (int i = 0; i < req.SuppPre.Count(); i++)
                    {
                        DataRow dr = dt.NewRow();

                        dr["SupplierId"] = req.SuppPre[i].Supplier_Id.ToString();
                        dr["Pointer_Id"] = req.SuppPre[i].Pointer_Id.ToString();
                        dr["Prefix"] = req.SuppPre[i].Prefix.ToString();

                        dt.Rows.Add(dr);
                    }
                }

                Database db = new Database();
                DataTable dtData = new DataTable();
                List<SqlParameter> para = new List<SqlParameter>();

                SqlParameter param = new SqlParameter("tabledt", SqlDbType.Structured);
                param.Value = dt;
                para.Add(param);

                dtData = db.ExecuteSP("Supplier_Prefix_CRUD", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0 && dtData.Rows[0]["Status"].ToString() == "1")
                {
                    return Ok(new CommonResponse
                    {
                        Error = null,
                        Message = dtData.Rows[0]["Message"].ToString(),
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Error = null,
                        Message = "Prefix Set Fail",
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
        public IHttpActionResult Delete_SuppPrefix([FromBody]JObject data)
        {
            SuppPrefix_Request req = new SuppPrefix_Request();
            try
            {
                req = JsonConvert.DeserializeObject<SuppPrefix_Request>(data.ToString());
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
                Database db = new Database(Request);
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                para.Add(db.CreateParam("Supplier_Id", DbType.String, ParameterDirection.Input, req.Supplier_Id));

                DataTable dt = db.ExecuteSP("SupplierPrefix_Delete", para.ToArray(), false);

                return Ok(new CommonResponse
                {
                    Error = null,
                    Message = "SUCCESS",
                    Status = "1"
                });
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
        public IHttpActionResult StockUpload([FromBody]JObject data)
        {
            StockUploadRequest req = new StockUploadRequest();
            try
            {
                req = JsonConvert.DeserializeObject<StockUploadRequest>(data.ToString());
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
                string filename = req.connString;
                string MapPath = HostingEnvironment.MapPath("~/ExcelFile/");
                string filePath = MapPath + filename;

                string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                DataTable dtData = Utility.ConvertXSLXtoDataTable("", connString);
                DataTable dtClone = new DataTable();

                if (dtData != null && dtData.Rows.Count > 0)
                {
                    DataColumn Col = dtData.Columns.Add("SUPPLIER_ID", System.Type.GetType("System.String"));
                    Col.SetOrdinal(0);// to put the column in position 0;
                    foreach (DataRow row in dtData.Rows)
                    {
                        row.SetField("SUPPLIER_ID", req.Supplier);
                        for (int i = 0; i < row.ItemArray.Length; i++)
                        {
                            if (row[i].ToString() == "")
                            {
                                row[i] = DBNull.Value;
                            }
                        }
                    }

                    dtClone = dtData.Clone(); //just copy structure, no data
                    for (int i = 0; i < dtClone.Columns.Count; i++)
                    {
                        if (dtClone.Columns[i].DataType != typeof(string))
                            dtClone.Columns[i].DataType = typeof(string);
                    }
                    foreach (DataRow dr in dtData.Rows)
                    {
                        dtClone.ImportRow(dr);
                    }


                    if (dtClone != null && dtClone.Rows.Count > 0)
                    {
                        Database db = new Database(Request);
                        List<SqlParameter> para = new List<SqlParameter>();

                        SqlParameter param = new SqlParameter("tableInq", SqlDbType.Structured);
                        param.Value = dtClone;
                        para.Add(param);

                        DataTable dt = db.ExecuteSP("ManualStockDetail_Ora_Insert", para.ToArray(), false);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            return Ok(new CommonResponse
                            {
                                Error = null,
                                Message = dt.Rows[0]["Message"].ToString(),
                                Status = dt.Rows[0]["Status"].ToString()
                            });
                        }
                        else
                        {
                            return Ok(new CommonResponse
                            {
                                Error = null,
                                Message = "Stock Upload in Issue",
                                Status = "0"
                            });
                        }
                    }
                    else
                    {
                        return Ok(new CommonResponse
                        {
                            Error = null,
                            Message = "Stock Not Found",
                            Status = "0"
                        });
                    }
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Error = null,
                        Message = "Stock Not Found",
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
        public IHttpActionResult Get_SupplierMaster([FromBody]JObject data)
        {
            Get_APIMst_Request get_apiuploadmst = new Get_APIMst_Request();
            try
            {
                get_apiuploadmst = JsonConvert.DeserializeObject<Get_APIMst_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<SuppPrefix_Response>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                if (get_apiuploadmst.Id > 0)
                    para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.Id));
                else
                    para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.SupplierId))
                    para.Add(db.CreateParam("SupplierId", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.SupplierId));
                else
                    para.Add(db.CreateParam("SupplierId", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.Search))
                    para.Add(db.CreateParam("Search", DbType.String, ParameterDirection.Input, get_apiuploadmst.Search));
                else
                    para.Add(db.CreateParam("Search", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.FromDate))
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, get_apiuploadmst.FromDate));
                else
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.ToDate))
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, get_apiuploadmst.ToDate));
                else
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (get_apiuploadmst.iPgNo > 0)
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.iPgNo));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (get_apiuploadmst.iPgSize > 0)
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.iPgSize));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.OrderBy))
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, get_apiuploadmst.OrderBy));
                else
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("SupplierMaster_select", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Get_APIMst_Response> list = new List<Get_APIMst_Response>();
                    list = DataTableExtension.ToList<Get_APIMst_Response>(dt);

                    return Ok(new ServiceResponse<Get_APIMst_Response>
                    {
                        Data = list,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<Get_APIMst_Response>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<SuppPrefix_Response>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult SaveSupplierMaster([FromBody]JObject data)
        {
            Save_APIMst_Request save_apimst_req = new Save_APIMst_Request();
            try
            {
                save_apimst_req = JsonConvert.DeserializeObject<Save_APIMst_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Error = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                CommonResponse resp = new CommonResponse();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, save_apimst_req.Id));
                para.Add(db.CreateParam("SupplierURL", DbType.String, ParameterDirection.Input, save_apimst_req.SupplierURL));
                para.Add(db.CreateParam("SupplierId", DbType.Int64, ParameterDirection.Input, save_apimst_req.SupplierId));

                para.Add(db.CreateParam("SupplierResponseFormat", DbType.String, ParameterDirection.Input, save_apimst_req.SupplierResponseFormat));
                para.Add(db.CreateParam("FileName", DbType.String, ParameterDirection.Input, save_apimst_req.FileName));
                para.Add(db.CreateParam("FileLocation", DbType.String, ParameterDirection.Input, save_apimst_req.FileLocation));
                para.Add(db.CreateParam("LocationExportType", DbType.String, ParameterDirection.Input, save_apimst_req.LocationExportType));
                para.Add(db.CreateParam("RepeateveryType", DbType.String, ParameterDirection.Input, save_apimst_req.RepeateveryType));
                para.Add(db.CreateParam("Repeatevery", DbType.String, ParameterDirection.Input, save_apimst_req.Repeatevery));
                para.Add(db.CreateParam("Active", DbType.Boolean, ParameterDirection.Input, save_apimst_req.Active));
                para.Add(db.CreateParam("DiscInverse", DbType.Boolean, ParameterDirection.Input, save_apimst_req.DiscInverse));

                if (!string.IsNullOrEmpty(save_apimst_req.SupplierAPIMethod))
                    para.Add(db.CreateParam("SupplierAPIMethod", DbType.String, ParameterDirection.Input, save_apimst_req.SupplierAPIMethod));
                else
                    para.Add(db.CreateParam("SupplierAPIMethod", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(save_apimst_req.SupplierHitUrl))
                    para.Add(db.CreateParam("SupplierHitUrl", DbType.String, ParameterDirection.Input, save_apimst_req.SupplierHitUrl));
                else
                    para.Add(db.CreateParam("SupplierHitUrl", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(save_apimst_req.UserName))
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, save_apimst_req.UserName));
                else
                    para.Add(db.CreateParam("UserName", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(save_apimst_req.Password))
                    para.Add(db.CreateParam("Password", DbType.String, ParameterDirection.Input, save_apimst_req.Password));
                else
                    para.Add(db.CreateParam("Password", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("SupplierMaster_CRUD", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Id"].ToString() != "0")
                    {
                        return Ok(new CommonResponse
                        {
                            Error = null,
                            Message = dt.Rows[0]["Id"].ToString(),
                            Status = "1"
                        });
                    }
                    else
                    {
                        return Ok(new CommonResponse
                        {
                            Error = null,
                            Message = dt.Rows[0]["Message"].ToString(),
                            Status = "0"
                        });
                    }
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Error = null,
                        Message = "No Data Found",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Error = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult Get_SuppColSettMas([FromBody]JObject data)
        {
            Get_APIMst_Request get_apiuploadmst = new Get_APIMst_Request();
            try
            {
                get_apiuploadmst = JsonConvert.DeserializeObject<Get_APIMst_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Get_SuppColSettMas_Response>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                if (get_apiuploadmst.Id > 0)
                    para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.Id));
                else
                    para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.Search))
                    para.Add(db.CreateParam("Search", DbType.String, ParameterDirection.Input, get_apiuploadmst.Search));
                else
                    para.Add(db.CreateParam("Search", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.FromDate))
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, get_apiuploadmst.FromDate));
                else
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.ToDate))
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, get_apiuploadmst.ToDate));
                else
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (get_apiuploadmst.iPgNo > 0)
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.iPgNo));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (get_apiuploadmst.iPgSize > 0)
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.iPgSize));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.OrderBy))
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, get_apiuploadmst.OrderBy));
                else
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("SupplierColSettingsMas_select", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Get_SuppColSettMas_Response> list = new List<Get_SuppColSettMas_Response>();
                    list = DataTableExtension.ToList<Get_SuppColSettMas_Response>(dt);

                    return Ok(new ServiceResponse<Get_SuppColSettMas_Response>
                    {
                        Data = list,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<Get_SuppColSettMas_Response>
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
                return Ok(new ServiceResponse<Get_SuppColSettMas_Response>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Get_Column_Mas_Select([FromBody]JObject data)
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                DataTable dt = db.ExecuteSP("COLUMN_MAS_select", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Get_Column_Mas_Response> list = new List<Get_Column_Mas_Response>();
                    list = DataTableExtension.ToList<Get_Column_Mas_Response>(dt);

                    return Ok(new ServiceResponse<Get_Column_Mas_Response>
                    {
                        Data = list,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<Get_Column_Mas_Response>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Get_Column_Mas_Response>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult SupplierColSettings_ExistorNot([FromBody]JObject data)
        {
            Get_APIMst_Request get_apiuploadmst = new Get_APIMst_Request();
            try
            {
                get_apiuploadmst = JsonConvert.DeserializeObject<Get_APIMst_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Error = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }
            
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.Id));

                DataTable dt = db.ExecuteSP("SupplierColSettings_ExistorNot", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Ok(new CommonResponse
                    {
                        Error = null,
                        Message = dt.Rows[0]["Id"].ToString(),
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Error = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Error = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Get_SuppColSettDet([FromBody]JObject data)
        {
            Get_APIMst_Request get_apiuploadmst = new Get_APIMst_Request();
            try
            {
                get_apiuploadmst = JsonConvert.DeserializeObject<Get_APIMst_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Get_SuppColSettDet_Response>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }
            
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                para.Add(db.CreateParam("SupplierColSettingsMasId", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.Id));

                DataTable dt = db.ExecuteSP("SupplierColSettingsDet_select", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Get_SuppColSettDet_Response> list = new List<Get_SuppColSettDet_Response>();
                    list = DataTableExtension.ToList<Get_SuppColSettDet_Response>(dt);

                    return Ok(new ServiceResponse<Get_SuppColSettDet_Response>
                    {
                        Data = list,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<Get_SuppColSettDet_Response>
                    {
                        Data = null,
                        Message = "No data found.",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Get_SuppColSettDet_Response>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Save_SuppColSettMas([FromBody]JObject data)
        {
            Save_SuppColSettMas_Request save_supcolsetmas = new Save_SuppColSettMas_Request();
            try
            {
                save_supcolsetmas = JsonConvert.DeserializeObject<Save_SuppColSettMas_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Error = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Supplier_Mas_Id", typeof(string));
                dt.Columns.Add("SupplierColumnName", typeof(string));
                dt.Columns.Add("Column_Mas_Id", typeof(string));
                dt.Columns.Add("DisplayOrder", typeof(string));

                if (save_supcolsetmas.SuppColSett.Count() > 0)
                {
                    for (int i = 0; i < save_supcolsetmas.SuppColSett.Count(); i++)
                    {
                        DataRow dr = dt.NewRow();

                        dr["Supplier_Mas_Id"] = save_supcolsetmas.SuppColSett[i].Supplier_Mas_Id.ToString();
                        dr["SupplierColumnName"] = save_supcolsetmas.SuppColSett[i].SupplierColumnName.ToString();
                        dr["Column_Mas_Id"] = save_supcolsetmas.SuppColSett[i].Column_Mas_Id.ToString();
                        dr["DisplayOrder"] = save_supcolsetmas.SuppColSett[i].DisplayOrder.ToString();

                        dt.Rows.Add(dr);
                    }
                }

                Database db = new Database();
                DataTable dtData = new DataTable();
                List<SqlParameter> para = new List<SqlParameter>();

                SqlParameter param = new SqlParameter("tableCol", SqlDbType.Structured);
                param.Value = dt;
                para.Add(param);

                dtData = db.ExecuteSP("SupplierColSettings_CRUD", para.ToArray(), false);

                if (dtData != null && dtData.Rows.Count > 0 && dtData.Rows[0]["Status"].ToString() == "1")
                {
                    return Ok(new CommonResponse
                    {
                        Error = null,
                        Message = dtData.Rows[0]["Id"].ToString() + "_414_" + dtData.Rows[0]["Message"].ToString(),
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new CommonResponse
                    {
                        Error = null,
                        Message = "",
                        Status = "0"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new CommonResponse
                {
                    Error = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        
        public static int TotCount = 0;
        [HttpPost]
        public IHttpActionResult SupplierColumnsGetFromAPI([FromBody]JObject data)
        {
            Get_APIMst_Request get_apiuploadmst = new Get_APIMst_Request();
            try
            {
                get_apiuploadmst = JsonConvert.DeserializeObject<Get_APIMst_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Get_SupplierColumnsFromAPI_Response>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            int Supplier_Mas_Id = 0;
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();
                para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.Id));
                DataTable dtAPI = db.ExecuteSP("SupplierMaster_select", para.ToArray(), false);

                if (dtAPI != null && dtAPI.Rows.Count > 0)
                {
                    TotCount = dtAPI.Rows.Count;
                    try
                    {
                        Supplier_Mas_Id = Convert.ToInt32(dtAPI.Rows[0]["Id"].ToString());

                        string _API = String.Empty, UserName = String.Empty, Password = String.Empty, filename = String.Empty, filefullpath = String.Empty;

                        DataTable dt_APIRes = new DataTable();

                        if (dtAPI.Rows[0]["SupplierResponseFormat"].ToString().ToUpper() == "XML")
                        {
                            _API = dtAPI.Rows[0]["SupplierURL"].ToString();
                            string[] words = _API.Split('?');
                            String InputPara = string.Empty;
                            if (words.Length == 2)
                            {
                                InputPara = words[1].ToString();
                            }

                            WebClient client = new WebClient();
                            client.Headers["Content-type"] = "application/x-www-form-urlencoded";
                            client.Encoding = Encoding.UTF8;
                            ServicePointManager.Expect100Continue = false;
                            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                            string xml = client.UploadString(_API, InputPara);
                            ConvertXmlStringToDataTable xDt = new ConvertXmlStringToDataTable();
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(xml);
                            XmlElement root = doc.DocumentElement;
                            XmlNodeList elemList = root.GetElementsByTagName("Row");
                            dt_APIRes = xDt.ConvertXmlNodeListToDataTable(elemList);
                        }
                        else if (dtAPI.Rows[0]["SupplierResponseFormat"].ToString().ToUpper() == "JSON")
                        {
                            if (dtAPI.Rows[0]["SupplierAPIMethod"].ToString().ToUpper() == "POST")
                            {
                                string json = string.Empty, Token = string.Empty;
                                _API = dtAPI.Rows[0]["SupplierURL"].ToString();
                                string[] words = _API.Split('?');
                                String InputPara = string.Empty;
                                if (words.Length == 2)
                                {
                                    InputPara = words[1].ToString();
                                }

                                if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://API1.ANKITGEMS.COM:4443/APIUSER/LOGINCHECK")
                                {
                                    string Name = dtAPI.Rows[0]["UserName"].ToString();
                                    string password = dtAPI.Rows[0]["Password"].ToString();

                                    WebClient client = new WebClient();
                                    client.Headers.Add("Content-type", "application/json");
                                    client.Encoding = Encoding.UTF8;
                                    json = client.UploadString("https://api1.ankitgems.com:4443/apiuser/logincheck?Name=" + Name + "&password=" + password, "POST", "");

                                    AnkitGems _data = new AnkitGems();
                                    _data = (new JavaScriptSerializer()).Deserialize<AnkitGems>(json);
                                    Token = _data.data.accessToken;

                                    WebClient client1 = new WebClient();
                                    client1.Headers.Add("Authorization", "Bearer " + Token);
                                    client1.Headers.Add("Content-type", "application/json");
                                    client1.Encoding = Encoding.UTF8;
                                    json = client1.UploadString("https://api1.ankitgems.com:4443/apistock/stockdetail?page=1&limit=10000", "POST", "");

                                    JObject o = JObject.Parse(json);
                                    var t = string.Empty;
                                    if (o != null)
                                    {
                                        var test = o.First;
                                        if (test != null)
                                        {
                                            var test2 = test.First;
                                            if (test2 != null)
                                            {
                                                Console.Write(test2);
                                                t = test2.Root.Last.First.First.First.ToString();
                                            }
                                        }
                                    }
                                    var json_1 = JsonConvert.DeserializeObject<List<dynamic>>(t);
                                    json = JsonConvert.SerializeObject(json_1);
                                }
                                else if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://SHAIRUGEMS.NET:8011/API/BUYER/GETSTOCKDATA")
                                {
                                    string Name = dtAPI.Rows[0]["UserName"].ToString();
                                    string password = dtAPI.Rows[0]["Password"].ToString();

                                    SGLoginRequest sgl = new SGLoginRequest();
                                    sgl.UserName = Name;
                                    sgl.Password = password;

                                    String InputLRJson = (new JavaScriptSerializer()).Serialize(sgl);

                                    WebClient client = new WebClient();
                                    client.Headers.Add("Content-type", "application/json");
                                    client.Encoding = Encoding.UTF8;
                                    json = client.UploadString("https://shairugems.net:8011/api/Buyer/login", "POST", InputLRJson);

                                    SGLoginResponse sglr = new SGLoginResponse();
                                    sglr = (new JavaScriptSerializer()).Deserialize<SGLoginResponse>(json);

                                    SGStockRequest sgr = new SGStockRequest();
                                    sgr.UserId = sglr.UserId;
                                    sgr.TokenId = sglr.TokenId;

                                    String InputSRJson = (new JavaScriptSerializer()).Serialize(sgr);

                                    WebClient client1 = new WebClient();
                                    client1.Headers.Add("Content-type", "application/json");
                                    client1.Encoding = Encoding.UTF8;
                                    json = client1.UploadString("https://shairugems.net:8011/api/Buyer/GetStockData", "POST", InputSRJson);

                                    var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
                                    var json_1 = JsonConvert.DeserializeObject<SGStockResponse>(json, settings);

                                    //json_1=json_1.r
                                    json = JsonConvert.SerializeObject(json_1.Data, settings);
                                    json = json.Replace("null", "");
                                }
                                else if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://SHAIRUGEMS.NET:8011/API/BUYER/GETSTOCKDATAINDIA")
                                {
                                    string Name = dtAPI.Rows[0]["UserName"].ToString();
                                    string password = dtAPI.Rows[0]["Password"].ToString();

                                    SGLoginRequest sgl = new SGLoginRequest();
                                    sgl.UserName = Name;
                                    sgl.Password = password;

                                    String InputLRJson = (new JavaScriptSerializer()).Serialize(sgl);

                                    WebClient client = new WebClient();
                                    client.Headers.Add("Content-type", "application/json");
                                    client.Encoding = Encoding.UTF8;
                                    json = client.UploadString("https://shairugems.net:8011/api/Buyer/login", "POST", InputLRJson);

                                    SGLoginResponse sglr = new SGLoginResponse();
                                    sglr = (new JavaScriptSerializer()).Deserialize<SGLoginResponse>(json);

                                    SGStockRequest sgr = new SGStockRequest();
                                    sgr.UserId = sglr.UserId;
                                    sgr.TokenId = sglr.TokenId;

                                    String InputSRJson = (new JavaScriptSerializer()).Serialize(sgr);

                                    WebClient client1 = new WebClient();
                                    client1.Headers.Add("Content-type", "application/json");
                                    client1.Encoding = Encoding.UTF8;
                                    json = client1.UploadString("https://shairugems.net:8011/api/Buyer/GetStockDataIndia", "POST", InputSRJson);

                                    var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
                                    var json_1 = JsonConvert.DeserializeObject<SGStockResponse>(json, settings);

                                    //json_1=json_1.r
                                    json = JsonConvert.SerializeObject(json_1.Data, settings);
                                    json = json.Replace("null", "");
                                }
                                else if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://SHAIRUGEMS.NET:8011/API/BUYER/GETSTOCKDATADUBAI")
                                {
                                    string Name = dtAPI.Rows[0]["UserName"].ToString();
                                    string password = dtAPI.Rows[0]["Password"].ToString();

                                    SGLoginRequest sgl = new SGLoginRequest();
                                    sgl.UserName = Name;
                                    sgl.Password = password;

                                    String InputLRJson = (new JavaScriptSerializer()).Serialize(sgl);

                                    WebClient client = new WebClient();
                                    client.Headers.Add("Content-type", "application/json");
                                    client.Encoding = Encoding.UTF8;
                                    json = client.UploadString("https://shairugems.net:8011/api/Buyer/login", "POST", InputLRJson);

                                    SGLoginResponse sglr = new SGLoginResponse();
                                    sglr = (new JavaScriptSerializer()).Deserialize<SGLoginResponse>(json);

                                    SGStockRequest sgr = new SGStockRequest();
                                    sgr.UserId = sglr.UserId;
                                    sgr.TokenId = sglr.TokenId;

                                    String InputSRJson = (new JavaScriptSerializer()).Serialize(sgr);

                                    WebClient client1 = new WebClient();
                                    client1.Headers.Add("Content-type", "application/json");
                                    client1.Encoding = Encoding.UTF8;
                                    json = client1.UploadString("https://shairugems.net:8011/api/Buyer/GetStockDataDubai", "POST", InputSRJson);

                                    var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
                                    var json_1 = JsonConvert.DeserializeObject<SGStockResponse>(json, settings);

                                    //json_1=json_1.r
                                    json = JsonConvert.SerializeObject(json_1.Data, settings);
                                    json = json.Replace("null", "");
                                }
                                else if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTP://PDHK.DIAMX.NET/API/STOCKSEARCH?APITOKEN=3C0DB41E-7B79-48C4-8CBD-1F718DB7263A")
                                {
                                    WebClient client = new WebClient();
                                    client.Headers.Add("Content-type", "application/json");
                                    client.Encoding = Encoding.UTF8;
                                    json = client.UploadString("http://pdhk.diamx.net/API/StockSearch?APIToken=3c0db41e-7b79-48c4-8cbd-1f718db7263a", "POST", "");

                                    JObject o = JObject.Parse(json);
                                    var t = string.Empty;
                                    if (o != null)
                                    {
                                        var test = o.First;
                                        if (test != null)
                                        {
                                            var test2 = test.First;
                                            if (test2 != null)
                                            {
                                                Console.Write(test2);
                                                t = test2.Root.Last.First.ToString();
                                            }
                                        }
                                    }
                                    var json_1 = JsonConvert.DeserializeObject<List<dynamic>>(t);
                                    json = JsonConvert.SerializeObject(json_1);
                                }
                                else if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://STOCK.DDPL.COM/DHARAMWEBAPI/API/STOCKDISPAPI/GETDIAMONDDATA")
                                {
                                    Dharam _data = new Dharam();
                                    _data.uniqID = 23835;
                                    _data.company = "SUNRISE DIAMONDS LTD";
                                    _data.actCode = "Su@D123#4nd23";
                                    _data.selectAll = "";
                                    _data.StartIndex = 1;
                                    _data.count = 80000;
                                    _data.columns = "";
                                    _data.finder = "";
                                    _data.sort = "";

                                    string inputJson = (new JavaScriptSerializer()).Serialize(_data);

                                    WebClient client = new WebClient();
                                    client.Headers.Add("Content-type", "application/json");
                                    client.Encoding = Encoding.UTF8;

                                    json = client.UploadString("https://stock.ddpl.com/DharamWebApi/api/stockdispapi/getDiamondData", "POST", inputJson);

                                    JObject o = JObject.Parse(json);
                                    var t = string.Empty;
                                    if (o != null)
                                    {
                                        var test = o.First;
                                        if (test != null)
                                        {
                                            var test2 = test.First;
                                            if (test2 != null)
                                            {
                                                Console.Write(test2);
                                                t = test2.Root.Last.First.ToString();
                                            }
                                        }
                                    }
                                    var json_1 = JsonConvert.DeserializeObject<List<dynamic>>(t);
                                    json = JsonConvert.SerializeObject(json_1);
                                }
                                else
                                {
                                    WebClient client = new WebClient();
                                    //client.Headers.Add("Authorization", "Bearer " + Token);
                                    client.Headers.Add("Content-type", "application/json");
                                    client.Encoding = Encoding.UTF8;
                                    json = client.UploadString(_API, "POST", InputPara);

                                    if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://VAIBHAVGEMS.CO/PROVIDESTOCK.SVC/GETSTOCK")
                                    {
                                        JObject o = JObject.Parse(json);
                                        var t = string.Empty;
                                        if (o != null)
                                        {
                                            var test = o.First;
                                            if (test != null)
                                            {
                                                var test2 = test.First;
                                                if (test2 != null)
                                                {
                                                    Console.Write(test2);
                                                    t = test2.First.First.ToString();
                                                }
                                            }
                                        }
                                        var json_1 = JsonConvert.DeserializeObject<List<dynamic>>(t);
                                        json = JsonConvert.SerializeObject(json_1);
                                    }
                                }

                                ConvertJsonStringToDataTable jDt = new ConvertJsonStringToDataTable();
                                dt_APIRes = jDt.JsonStringToDataTable(json);

                            }
                            else
                            {
                                _API = dtAPI.Rows[0]["SupplierURL"].ToString();
                                string[] words = _API.Split('?');
                                String InputPara = string.Empty;
                                if (words.Length == 2)
                                {
                                    InputPara = words[1].ToString();
                                }

                                WebClient client = new WebClient();
                                client.Headers["User-Agent"] = @"Mozilla/4.0 (Compatible; Windows NT 5.1;MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                                ServicePointManager.Expect100Continue = false;
                                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                string json = client.DownloadString(_API);

                                if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://PCKNGNDSRV.AZUREWEBSITES.NET/ADMIN/STOCKSHARE/STOCKSHAREAPIRESULT?USERNAME=SUNRISEDIAMONDS&ACCESS_KEY=IXL8-1KGS-SA3C-E6HW-BRBA-IW4G-DSTU")
                                {
                                    JObject o = JObject.Parse(json);
                                    var t = string.Empty;
                                    if (o != null)
                                    {
                                        var test = o.First;
                                        if (test != null)
                                        {
                                            var test2 = test.First;
                                            if (test2 != null)
                                            {
                                                Console.Write(test2);
                                                t = o.Last.Last.ToString();
                                            }
                                        }
                                    }
                                    var json_1 = JsonConvert.DeserializeObject<List<dynamic>>(t);
                                    json = JsonConvert.SerializeObject(json_1);

                                    ConvertJsonStringToDataTable jDt = new ConvertJsonStringToDataTable();
                                    dt_APIRes = jDt.JsonStringToDataTable(json);
                                }
                                else if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTP://WWW.DIAMJOY.COM/API/USER/STOCK/11229/729F7B484FA22A5276B0CDADABC75147/?LANG=EN")
                                {
                                    JOY _data = (new JavaScriptSerializer()).Deserialize<JOY>(json);
                                    ConvertJsonObjectToDataTable jodt = new ConvertJsonObjectToDataTable();
                                    dt_APIRes = jodt.StringArrayToDataTable(_data.keys, _data.rows);

                                }
                                else if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://API.DIAMARTHK.COM/API/CHANNELPARTNER/GETINVENTORY/SUNRISE/SUNRISE@1401")
                                {
                                    DiamartResponse res = (new JavaScriptSerializer()).Deserialize<DiamartResponse>(json);
                                    ConvertJsonStringToDataTable jDt = new ConvertJsonStringToDataTable();
                                    dt_APIRes = jDt.JsonStringToDataTable(json);
                                }
                                else if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://SJWORLDAPI.AZUREWEBSITES.NET/SHARE/SJAPI.ASMX/GETDATA?LOGINNAME=SUNRISE&PASSWORD=SUNRISE321")
                                {
                                    JObject o = JObject.Parse(json);
                                    var t = string.Empty;
                                    if (o != null)
                                    {
                                        var test = o.First;
                                        if (test != null)
                                        {
                                            var test2 = test.First;
                                            if (test2 != null)
                                            {
                                                Console.Write(test2);
                                                t = o.Last.Last.ToString();
                                            }
                                        }
                                    }
                                    var json_1 = JsonConvert.DeserializeObject<List<dynamic>>(t);
                                    json = JsonConvert.SerializeObject(json_1);

                                    ConvertJsonStringToDataTable jDt = new ConvertJsonStringToDataTable();
                                    dt_APIRes = jDt.JsonStringToDataTable(json);
                                }
                                else
                                {
                                    ConvertJsonStringToDataTable jDt = new ConvertJsonStringToDataTable();
                                    dt_APIRes = jDt.JsonStringToDataTable(json);
                                }

                            }

                        }
                        else if (dtAPI.Rows[0]["SupplierResponseFormat"].ToString().ToUpper() == "HTML")
                        {
                            if (dtAPI.Rows[0]["SupplierAPIMethod"].ToString().ToUpper() == "GET")
                            {
                                if (dtAPI.Rows[0]["SupplierURL"].ToString().ToUpper() == "HTTPS://WWW.1314PG.COM/API/USER/STOCK/11738/8789AE77D94A9CFB109C1BA5143ABAB6/")
                                {
                                    _API = dtAPI.Rows[0]["SupplierURL"].ToString();
                                    WebClient client = new WebClient();
                                    client.Headers["User-Agent"] = @"Mozilla/4.0 (Compatible; Windows NT 5.1;MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                                    ServicePointManager.Expect100Continue = false;
                                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                    string response = client.DownloadString(_API);
                                    string[] res = response.Split('\n');

                                    string[] columns = res.Where(w => w == res[0]).ToArray();

                                    string[] rows = res.Where(w => w != res[0]).ToArray();


                                    ConvertStringArrayToDatatable saDt = new ConvertStringArrayToDatatable();

                                    dt_APIRes = saDt.StringArrayToDataTable(columns, rows);
                                }
                            }
                        }

                        if (dt_APIRes != null && dt_APIRes.Rows.Count > 0)
                        {
                            SuppColsGetFromAPI_Log_Ins(Supplier_Mas_Id, "Success " + dt_APIRes.Columns.Count + " Columns Found.");

                            DataTable dtResult = new DataTable();
                            int currecs = 1;

                            dtResult.Columns.Add("Id", typeof(int));
                            dtResult.Columns.Add("SupplierColumn", typeof(string));

                            foreach (DataColumn column in dt_APIRes.Columns)
                            {
                                DataRow dr = dtResult.NewRow();
                                dr["Id"] = currecs;
                                dr["SupplierColumn"] = column.ColumnName;
                                currecs += 1;

                                dtResult.Rows.Add(dr);
                            }

                            List<Get_SupplierColumnsFromAPI_Response> list = new List<Get_SupplierColumnsFromAPI_Response>();
                            list = DataTableExtension.ToList<Get_SupplierColumnsFromAPI_Response>(dtResult);

                            return Ok(new ServiceResponse<Get_SupplierColumnsFromAPI_Response>
                            {
                                Data = list,
                                Message = "SUCCESS",
                                Status = "1"
                            });
                        }
                        else
                        {
                            SuppColsGetFromAPI_Log_Ins(Supplier_Mas_Id, "Supplier API in Columns not Found.");

                            return Ok(new ServiceResponse<Get_SupplierColumnsFromAPI_Response>
                            {
                                Data = null,
                                Message = "Supplier API in Columns not found.",
                                Status = "2"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        SuppColsGetFromAPI_Log_Ins(Supplier_Mas_Id, ex.Message.ToString() + ' ' + ex.StackTrace.ToString());

                        return Ok(new ServiceResponse<Get_SupplierColumnsFromAPI_Response>
                        {
                            Data = null,
                            Message = ex.Message,
                            Status = "0"
                        });
                    }
                }
                else
                {
                    return Ok(new ServiceResponse<Get_SupplierColumnsFromAPI_Response>
                    {
                        Data = null,
                        Message = "Supplier Not Found.",
                        Status = "2"
                    });
                }
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request); 
                return Ok(new ServiceResponse<Get_SupplierColumnsFromAPI_Response>
                {
                    Data = null,
                    Message = ex.Message,
                    Status = "0"
                });
            }
        }
        public static void SuppColsGetFromAPI_Log_Ins(int Supplier_Mas_Id, string message)
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para = new List<IDbDataParameter>();

                para.Add(db.CreateParam("Supplier_Mas_Id", DbType.Int64, ParameterDirection.Input, Supplier_Mas_Id));
                para.Add(db.CreateParam("Message", DbType.String, ParameterDirection.Input, message));

                DataTable dt = db.ExecuteSP("SuppColsGetFromAPI_Log_Ins", para.ToArray(), false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public IHttpActionResult Get_Supplier_PriceList([FromBody]JObject data)
        {
            Get_APIMst_Request get_apiuploadmst = new Get_APIMst_Request();
            try
            {
                get_apiuploadmst = JsonConvert.DeserializeObject<Get_APIMst_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Get_Supplier_PriceList_Response>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                if (get_apiuploadmst.Id > 0)
                    para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.Id));
                else
                    para.Add(db.CreateParam("Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (get_apiuploadmst.Supplier_Mas_Id > 0)
                    para.Add(db.CreateParam("Supplier_Mas_Id", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.Supplier_Mas_Id));
                else
                    para.Add(db.CreateParam("Supplier_Mas_Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.Search))
                    para.Add(db.CreateParam("Search", DbType.String, ParameterDirection.Input, get_apiuploadmst.Search));
                else
                    para.Add(db.CreateParam("Search", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.FromDate))
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, get_apiuploadmst.FromDate));
                else
                    para.Add(db.CreateParam("FromDate", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.ToDate))
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, get_apiuploadmst.ToDate));
                else
                    para.Add(db.CreateParam("ToDate", DbType.String, ParameterDirection.Input, DBNull.Value));

                if (get_apiuploadmst.iPgNo > 0)
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.iPgNo));
                else
                    para.Add(db.CreateParam("iPgNo", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (get_apiuploadmst.iPgSize > 0)
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.iPgSize));
                else
                    para.Add(db.CreateParam("iPgSize", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (!string.IsNullOrEmpty(get_apiuploadmst.OrderBy))
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, get_apiuploadmst.OrderBy));
                else
                    para.Add(db.CreateParam("OrderBy", DbType.String, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("Supplier_PriceList_select", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Get_Supplier_PriceList_Response> list = new List<Get_Supplier_PriceList_Response>();
                    list = DataTableExtension.ToList<Get_Supplier_PriceList_Response>(dt);

                    return Ok(new ServiceResponse<Get_Supplier_PriceList_Response>
                    {
                        Data = list,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<Get_Supplier_PriceList_Response>
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
                return Ok(new ServiceResponse<Get_Supplier_PriceList_Response>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult SupplierGetFrom_PriceList([FromBody]JObject data)
        {
            Get_APIMst_Request get_apiuploadmst = new Get_APIMst_Request();
            try
            {
                get_apiuploadmst = JsonConvert.DeserializeObject<Get_APIMst_Request>(data.ToString());
            }
            catch (Exception ex)
            {
                DAL.Common.InsertErrorLog(ex, null, Request);
                return Ok(new ServiceResponse<Get_Supplier_PriceList_Response>
                {
                    Data = null,
                    Message = "Input Parameters are not in the proper format",
                    Status = "0"
                });
            }

            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                if (get_apiuploadmst.Supplier_Mas_Id > 0)
                    para.Add(db.CreateParam("Supplier_Mas_Id", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.Supplier_Mas_Id));
                else
                    para.Add(db.CreateParam("Supplier_Mas_Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                if (get_apiuploadmst.SupplierPriceList_Id > 0)
                    para.Add(db.CreateParam("SupplierPriceList_Id", DbType.Int64, ParameterDirection.Input, get_apiuploadmst.SupplierPriceList_Id));
                else
                    para.Add(db.CreateParam("SupplierPriceList_Id", DbType.Int64, ParameterDirection.Input, DBNull.Value));

                DataTable dt = db.ExecuteSP("SupplierGetFrom_PriceList", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Get_Supplier_PriceList_Response> list = new List<Get_Supplier_PriceList_Response>();
                    list = DataTableExtension.ToList<Get_Supplier_PriceList_Response>(dt);

                    return Ok(new ServiceResponse<Get_Supplier_PriceList_Response>
                    {
                        Data = list,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<Get_Supplier_PriceList_Response>
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
                return Ok(new ServiceResponse<Get_Supplier_PriceList_Response>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
        [HttpPost]
        public IHttpActionResult Get_API_StockFilter([FromBody]JObject data)
        {
            try
            {
                Database db = new Database();
                List<IDbDataParameter> para;
                para = new List<IDbDataParameter>();

                DataTable dt = db.ExecuteSP("API_StockFilter", para.ToArray(), false);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Get_API_StockFilter_Response> list = new List<Get_API_StockFilter_Response>();
                    list = DataTableExtension.ToList<Get_API_StockFilter_Response>(dt);

                    return Ok(new ServiceResponse<Get_API_StockFilter_Response>
                    {
                        Data = list,
                        Message = "SUCCESS",
                        Status = "1"
                    });
                }
                else
                {
                    return Ok(new ServiceResponse<Get_API_StockFilter_Response>
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
                return Ok(new ServiceResponse<Get_API_StockFilter_Response>
                {
                    Data = null,
                    Message = "Something Went wrong.\nPlease try again later",
                    Status = "0"
                });
            }
        }
    }
}
