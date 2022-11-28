using bbxBE.Application.Interfaces.Queries;

namespace bbxBE.Application.Parameters
{
    public class QueryParameter : PagingParameter, IQueryParameter
    {
        public string OrderBy { get; set; }
        public long? ID { get; set; }
        //       public  string Fields { get; set; }
    }
}