using bbxBE.Common.Attributes;
using bbxBE.Common.Enums;
using bbxBE.Common.NAV;
using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace bbxBE.Domain.Entities
{
    [Description("NAV adatcsere")]
    public class NAVXChange : BaseEntity
    {

        [ColumnLabel("Számla ID")]
        [Description("Számla ID")]
        public long InvoiceID { get; set; }

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylatszám")]
        public string InvoiceNumber { get; set; }


        [ColumnLabel("Invoice XML")]
        [Description("Invoice XML")]
        public string InvoiceXml { get; set; }

        private enNAVStatus status;
        [ColumnLabel("Státusz")]
        [Description("Státusz")]
        public string Status
        {
            get { return Enum.GetName(typeof(enNAVStatus), status); }
            set
            {
                if (value != null)
                    status = (enNAVStatus)Enum.Parse(typeof(enNAVStatus), value);
                else
                    status = enNAVStatus.UNKNOWN;

            }
        }

        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Notice { get; set; }

        private enNAVOperation operation;
        [ColumnLabel("Művelet")]
        [Description("Művelet")]
        public string Operation
        {
            get { return Enum.GetName(typeof(enNAVOperation), operation); }
            set
            {
                if (value != null)
                    operation = (enNAVOperation)Enum.Parse(typeof(enNAVOperation), value);
                else
                    operation = enNAVOperation.CREATE;

            }
        }

        #region Token

        [ColumnLabel("Token kérés időpont")]
        [Description("Token kérés időpont")]
        public DateTime TokenTime { get; set; }     //UTC!!

        [ColumnLabel("Token kérés")]
        [Description("Token kérés")]
        public string TokenRequest { get; set; }

        [ColumnLabel("Token")]
        [Description("Token")]
        public string Token { get; set; }

        [ColumnLabel("Token válasz")]
        [Description("Token válasz")]
        public string TokenResponse { get; set; }

        private FunctionCodeType tokenFuncCode;
        [ColumnLabel("Token funkciókód")]
        [Description("Token funkciókód")]
        public string TokenFuncCode
        {
            get { return Enum.GetName(typeof(FunctionCodeType), tokenFuncCode); }
            set
            {
                if (value != null)
                    tokenFuncCode = (FunctionCodeType)Enum.Parse(typeof(FunctionCodeType), value);
            }
        }

        [ColumnLabel("Token üzenet")]
        [Description("Token üzenet")]
        public string TokenMessage { get; set; }

        #endregion

        #region Send

        [ColumnLabel("Küldés kérés időpont")]
        [Description("Küldés kérés időpont")]
        public DateTime SendTime { get; set; }     //UTC!!

        [ColumnLabel("Küldés kérés")]
        [Description("Küldés kérés")]
        public string SendRequest { get; set; }

        [ColumnLabel("Küldés válasz")]
        [Description("Küldés válasz")]
        public string SendResponse { get; set; }

        private FunctionCodeType sendFuncCode;
        [ColumnLabel("Küldés funkciókód")]
        [Description("Küldés funkciókód")]
        public string SendFuncCode
        {
            get { return Enum.GetName(typeof(FunctionCodeType), sendFuncCode); }
            set
            {
                if (value != null)
                    sendFuncCode = (FunctionCodeType)Enum.Parse(typeof(FunctionCodeType), value);
            }
        }

        [ColumnLabel("Küldés üzenet")]
        [Description("Küldés üzenet")]
        public string SendMessage { get; set; }

        #endregion

        #region Query

        [ColumnLabel("Lekérdezés kérés időpont")]
        [Description("Lekérdezés kérés időpont")]
        public DateTime QueryTime { get; set; }     //UTC!!

        [ColumnLabel("Lekérdezés kérés")]
        [Description("Lekérdezés kérés")]
        public string QueryRequest { get; set; }

        [ColumnLabel("Lekérdezés válasz")]
        [Description("Lekérdezés válasz")]
        public string QueryResponse { get; set; }

        private FunctionCodeType queryFuncCode;
        [ColumnLabel("Lekérdezés funkciókód")]
        [Description("Lekérdezés funkciókód")]
        public string QueryFuncCode
        {
            get { return Enum.GetName(typeof(FunctionCodeType), queryFuncCode); }
            set
            {
                if (value != null)
                    queryFuncCode = (FunctionCodeType)Enum.Parse(typeof(FunctionCodeType), value);
            }
        }

        [ColumnLabel("Lekérdezés üzenet")]
        [Description("Lekérdezés üzenet")]
        public string QueryMessage { get; set; }

        #endregion

        [ColumnLabel("Tranzakció ID")]
        [Description("Tranzakció ID")]
        public string TransactionID { get; set; }


        [ColumnLabel("Eredmény-sorok")]
        [Description("Eredmény-sorok")]
        public virtual ICollection<NAVXResult> Results { get; set; }


    }
}
