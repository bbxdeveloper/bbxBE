using bbxBE.Application.Parameters;

namespace bbxBE.Application.Wrappers
{
    public class PagedResponse<T> : Response<T>
    {
        public virtual int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
        public decimal SummaryNet { get; set; }
        public decimal SummaryVat { get; set; }
        public decimal SummaryGross { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize, RecordsCount recordsCount, decimal summaryNet = 0, decimal summaryVat = 0, decimal summaryGross = 0)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.RecordsFiltered = recordsCount.RecordsFiltered;
            this.RecordsTotal = recordsCount.RecordsTotal;
            this.Data = data;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
            this.SummaryNet = summaryNet;
            this.SummaryVat = summaryVat;
            this.SummaryGross = summaryGross;
        }
    }
}