﻿using bbxBE.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.Commands.ResultModels
{
    public class ImportedItemsStatistics
    {
        public int AllItemsCount { get; set; }
        public int CreatedItemsCount { get; set; }
        public int UpdatedItemsCount { get; set; }
        public int ErroredItemssCount { get; set; }
        public bool HasErrorDuringImport { get; set; } = false;
    }
}