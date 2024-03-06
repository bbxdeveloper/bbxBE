using bbxBE.Common.Attributes;
using bbxBE.Common.NAV;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace bbxBE.Application.Queries.ViewModels
{
    public class GetNAVXChangeViewModel
    {

        [ColumnLabel("ID")]
        [Description("ID")]
        public long ID { get; set; }


        [ColumnLabel("Időpont")]
        [Description("Időpont")]
        public DateTime CreateTime { get; set; }


        [ColumnLabel("Számla ID")]
        [Description("Számla ID")]
        public long InvoiceID { get; set; }

        [ColumnLabel("Bizonylatszám")]
        [Description("Bizonylatszám")]
        public string InvoiceNumber { get; set; }

        [ColumnLabel("Státusz")]
        [Description("Státusz")]
        public string Status { get; set; }

        [ColumnLabel("Státusz megnevezés")]
        [Description("Státusz megnevezés")]
        public string StatusX { get; set; }

        [ColumnLabel("Művelet")]
        [Description("Művelet")]
        public string Operation { get; set; }

        [ColumnLabel("Művelet megnevezése")]
        [Description("Művelet megnevezése")]
        public string OperationX { get; set; }

        [ColumnLabel("Megjegyzés")]
        [Description("Megjegyzés")]
        public string Notice { get; set; }

        [ColumnLabel("Token kérés időpont")]
        [Description("Token kérés időpont")]
        public DateTime TokenTime { get; set; }     //UTC!!

        [ColumnLabel("Token")]
        [Description("Token")]
        public string Token { get; set; }

        private FunctionCodeType? tokenFuncCode;
        [ColumnLabel("Token funkciókód")]
        [Description("Token funkciókód")]
        public string TokenFuncCode { get; set; }

        [ColumnLabel("Token üzenet")]
        [Description("Token üzenet")]
        public string TokenMessage { get; set; }


        [ColumnLabel("Küldés kérés időpont")]
        [Description("Küldés kérés időpont")]
        public DateTime SendTime { get; set; }     //UTC!!


        public string SendFuncCode { get; set; }

        [ColumnLabel("Küldés üzenet")]
        [Description("Küldés üzenet")]
        public string SendMessage { get; set; }

        [ColumnLabel("Lekérdezés kérés időpont")]
        [Description("Lekérdezés kérés időpont")]
        public DateTime QueryTime { get; set; }     //UTC!!

        private FunctionCodeType? queryFuncCode;
        [ColumnLabel("Lekérdezés funkciókód")]
        [Description("Lekérdezés funkciókód")]
        public string QueryFuncCode { get; set; }

        [ColumnLabel("Lekérdezés üzenet")]
        [Description("Lekérdezés üzenet")]
        public string QueryMessage { get; set; }

        [ColumnLabel("Tranzakció ID")]
        [Description("Tranzakció ID")]
        public string TransactionID { get; set; }

        [ColumnLabel("Eredmény sorok száma")]
        [Description("Eredmény sorok száma")]
        public int NAVXResultsCount { get { return NAVXResults != null ? NAVXResults.Count : 0; } }


        [ColumnLabel("Eredmény-sorok")]
        [Description("Eredmény-sorok")]
        public virtual IList<GetNAVXResultViewModel> NAVXResults { get; set; }

    }
}
