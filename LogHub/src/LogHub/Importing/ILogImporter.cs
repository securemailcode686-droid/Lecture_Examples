using System.Collections.Generic;
using LogHub.Model;

namespace LogHub.Importing
{
    public interface ILogImporter
    {
        IEnumerable<LogRecord?> Import(LogInput input);
    }
}
