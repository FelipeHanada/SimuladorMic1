namespace Simulador.mic1.exceptions;

public class CtrlStoreSrcFIleInvalidLineException : Exception
{
    public int? LineNumber { get; }

    public CtrlStoreSrcFIleInvalidLineException() {}

    public CtrlStoreSrcFIleInvalidLineException(string message)
        : base(message) {}

    public CtrlStoreSrcFIleInvalidLineException(string message, Exception inner)
        : base(message, inner) {}

    public CtrlStoreSrcFIleInvalidLineException(int lineNumber, string message)
        : base($"Invalid line on Control Store source file (line {lineNumber}): {message}")
    {
        LineNumber = lineNumber;
    }
}
