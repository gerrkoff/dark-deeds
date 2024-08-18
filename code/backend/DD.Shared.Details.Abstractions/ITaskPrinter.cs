using DD.Shared.Details.Abstractions.Dto;

namespace DD.Shared.Details.Abstractions;

public interface ITaskPrinter
{
    string PrintWithSymbolCodes(TaskDto task);

    string PrintContent(TaskDto task);
}
