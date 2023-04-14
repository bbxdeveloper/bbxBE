using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Commands.cmdImport
{
    public interface IImportProductProc
    {
        Task Process(string mapFileContent, string CSVContent, string FieldSeparator, string SessionID, CancellationToken cancellationToken);
    }

}
