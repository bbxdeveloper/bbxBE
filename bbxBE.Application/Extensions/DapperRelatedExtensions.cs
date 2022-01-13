namespace bbxBE.Application.Extensions
{
    public static class DapperRelatedExtensions
    {
        public static string EncodeForLikeOperator(this string str)
        {
            return "%" + str.Replace("[", "[[]").Replace("%", "[%]") + "%";
        }
    }
}
