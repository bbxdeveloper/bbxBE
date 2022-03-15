using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Application.DTOs.File
{
    public class FileRequest
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int FileSizeInBytes { get; set; }
    }
}
