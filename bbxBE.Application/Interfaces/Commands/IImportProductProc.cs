using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces.Commands
{
    public interface IImportProductProc
    {
        Task Process(string mapFileContent, string CSVContent, string FieldSeparator, string SessionID, bool onlyInsert, CancellationToken cancellationToken);
    }

}
