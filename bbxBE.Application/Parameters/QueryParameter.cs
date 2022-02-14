using bbxBE.Application.Interfaces.Queries;

namespace bbxBE.Application.Parameters
{
    public class QueryParameter : PagingParameter, IQueryParameter
    {
        public  string OrderBy { get; set; }
        public  string ModelFields { get; set; }
    }
}