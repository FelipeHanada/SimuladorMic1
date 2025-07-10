namespace Simulador.mic1.exceptions;

public class AssemblerV2Exception : Exception {
    public AssemblerV2Exception() { }

    public AssemblerV2Exception(string message)
        : base(message) { }

    public AssemblerV2Exception(string message, Exception inner)
        : base(message, inner) { }
}

public class SyntaxErrorException : AssemblerV2Exception
{
    public int? LineNumber { get; }
    public SyntaxErrorException(int lineNumber, string line)
        : base($"SyntaxError on the line {lineNumber}: {line}")
    {
        LineNumber = lineNumber;
    }
}

public class DuplicattedSymbolException : AssemblerV2Exception
{
    public int? LineNumber { get; }
    public string? Symbol { get; }

    public DuplicattedSymbolException() {}

    public DuplicattedSymbolException(string message)
        : base(message) {}

    public DuplicattedSymbolException(string message, Exception inner)
        : base(message, inner) {}

    public DuplicattedSymbolException(int lineNumber, string symbol)
        : base($"Duplicatted declaration of the symbol '{symbol}' on the line {lineNumber}")
    {
        LineNumber = lineNumber;
        Symbol = symbol;
    }
}

public class SymbolNotDefinedException : AssemblerV2Exception
{
    public int? LineNumber { get; }
    public string? Symbol { get; }

    public SymbolNotDefinedException() { }

    public SymbolNotDefinedException(string message)
        : base(message) { }

    public SymbolNotDefinedException(string message, Exception inner)
        : base(message, inner) { }

    public SymbolNotDefinedException(int lineNumber, string symbol)
        : base($"Use of undefined symbol '{symbol}' on the line {lineNumber}")
    {
        LineNumber = lineNumber;
        Symbol = symbol;
    }
}

public class OpCodeNotDefinedException : AssemblerV2Exception
{
    public int? LineNumber { get; }
    public string? OpCode { get; }

    public OpCodeNotDefinedException() { }

    public OpCodeNotDefinedException(string message)
        : base(message) { }

    public OpCodeNotDefinedException(string message, Exception inner)
        : base(message, inner) { }

    public OpCodeNotDefinedException(int lineNumber, string opCode)
        : base($"OpCode '{opCode}' not defined on the line {lineNumber}")
    {
        LineNumber = lineNumber;
        OpCode = opCode;
    }
}

