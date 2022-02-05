using bbxBE.Application.Interfaces.Queries;

namespace bbxBE.Application.Parameters
{
    public class PagingParameter : IPagingParameter
    {
        private const int maxPageSize = 200;
        private int _pageSize = 10;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public int PageNumber { get; set; } = 1;
    }
}