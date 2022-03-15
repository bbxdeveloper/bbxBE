using bbxBE.Application.DTOs.File;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces
{
    public interface IFileService
    {
        Task SendAsync(FileRequest request);
    }
}
