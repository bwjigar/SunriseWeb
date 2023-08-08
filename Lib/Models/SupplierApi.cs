using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Models
{
    public class ShairuApiLoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class ShairuApiLoginResponse
    {
        public string UserName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public int TokenId { get; set; }
    }

    public class ShairuApiHoldRequest
    {
        public string StoneID { get; set; }
        public string Comments { get; set; }
        public int UserID { get; set; }
        public string TokenId { get; set; }
    }
    public class ShairuApiHoldResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
    public class JBApiHoldResponse
    {
        public string Status { get; set; }
        public string RefNo { get; set; }
        public string Price { get; set; }
        public Int32 ErrorNo { get; set; }
        public string ErrorMsg { get; set; }

        public string ResultText { get; set; }
    }

    public class SupplierApiOrderRequest_
    {
        public int Mas_Id { get; set; }
    }
    public class SupplierApiOrderRequest
    {
        public List<ObjOrderLst> Orders { get; set; }
        public SupplierApiOrderRequest()
        {
            Orders = new List<ObjOrderLst>();
        }
        public string iOrderid_sRefNo { get; set; }
        public string DeviceType { get; set; }
        public string IpAddress { get; set; }
    }

    public class ObjOrderLst
    {
        public string Refno { get; set; }
        public int Orderid { get; set; }
        public int UserId { get; set; }
        public string SuppValue { get; set; }
        public string Comments { get; set; }
    }
    public class ConfirmPlaceOrderResponse
    {
        public string RefNo { get; set; }
        public string SunriseStatus { get; set; }
        public string SupplierName { get; set; }
        public string SupplierStatus { get; set; }
        public string LabEntryStatus { get; set; }
    }
    public class SupplierApiOrderRequest_AUTO
    {
        public string iOrderid_sRefNo { get; set; }
        public string DeviceType { get; set; }
        public string IpAddress { get; set; }
    }
    public class RatnaTokenResponse
    {
        public string Status { get; set; }
        public string Token { get; set; }
    }
    public class RatnaHoldResponse
    {
        public string orderno { get; set; }
        public string status { get; set; }
        public string stoneno { get; set; }
    }

    public class VenusTokenResponse
    {
        public string Token_Id { get; set; }
        public string Status { get; set; }
        public string Session_Time_out { get; set; }
        public string Status_Cd { get; set; }
    }
    public class VenusHoldResponse
    {
        public ObjVenusHoldResponseLst Status { get; set; }
        public VenusHoldResponse()
        {
            Status = new ObjVenusHoldResponseLst();
        }
        public string Memo_No { get; set; }
        public string ReturnStones { get; set; }
        public string Message { get; set; }
    }
    public class ObjVenusHoldResponseLst
    {
        public string status_code { get; set; }
        public string status_message { get; set; }
    }
}