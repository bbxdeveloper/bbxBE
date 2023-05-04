namespace bbxBE.Application.Commands.ResultModels
{
    public class ImportedItemsStatistics
    {
        public int AllItemsCount { get; set; }
        public int CreatedItemsCount { get; set; }
        public int UpdatedItemsCount { get; set; }
        public int ErroredItemsCount { get; set; }
        public bool HasErrorDuringImport { get; set; } = false;
    }
}
