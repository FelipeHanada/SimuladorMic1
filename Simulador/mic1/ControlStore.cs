using Simulador.components;
using Simulador.mic1.exceptions;
using Simulador.utils;
using System.Collections;

namespace Simulador.mic1;

public class ControlStore : Memory
{
    public ControlStore(
        ISignalSender addressSender,
        ISignalSender inBufferSender,
        ISignalSender? RDSender,
        ISignalSender? WRSender
        )
        : base(256, 32, addressSender, inBufferSender, RDSender, WRSender, null)
    {}
}

public interface ICtrlStoreSrcFileLoader
{
    public string SourceFilePath();
    public void Load(Memory memory);
}

public abstract class CtrlStoreSrcFileLoader(string sourceFilePath) : ICtrlStoreSrcFileLoader
{
    private readonly string _sourceFilePath = sourceFilePath;
    public string SourceFilePath() => _sourceFilePath;
    public abstract void Load(Memory memory);
}

public class CtrlStoreTxtSrcFileLoader(string sourceFilePath) : CtrlStoreSrcFileLoader(sourceFilePath)
{
    override public void Load(Memory memory)
    {
        if (!File.Exists(SourceFilePath()))
        {
            throw new FileNotFoundException("control store source file not found.");
        }

        int lineIndex = 0;
        foreach (string line in File.ReadLines(SourceFilePath()))
        {
            var (index, bits) = InterpretLine(line);
            memory.SetCell(index, bits);
            lineIndex++;
        }
    }

    private static (int, BitArray) InterpretLine(string line)
    {
        string[] parts = line.Replace(" ", "").Split(":");

        switch (parts.Length)
        {
            case 1:
                throw new CtrlStoreSrcFIleInvalidLineException(
                    $"Invalid line: no ':' were found. Content: '{line}'"
                );
            default:
                if (parts.Length != 2)
                {
                    throw new CtrlStoreSrcFIleInvalidLineException(
                        $"Invalid line: more than one ':' were found on a single line. Content: '{line}'"
                    );
                }
                break;
        }

        int index = int.Parse(parts[0]);
        BitArray bits = BitArrayExtensions.FromBitString(parts[1]);

        return (index, bits);
    }
}
