using Simulador.components;
using Simulador.mic1.exceptions;
using Simulador.utils;
using System.Collections;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Simulador.mic1;

public partial class AssemblerV2
{
    private static readonly Dictionary<string, string> InstructionSet =
        new(StringComparer.OrdinalIgnoreCase)
    {
        {"LODD", "0000"},
        {"STOD", "0001"},
        {"ADDD", "0010"},
        {"SUBD", "0011"},
        {"JPOS", "0100"},
        {"JZER", "0101"},
        {"JUMP", "0110"},
        {"LOCO", "0111"},
        {"LODL", "1000"},
        {"STOL", "1001"},
        {"ADDL", "1010"},
        {"SUBL", "1011"},
        {"JNEG", "1100"},
        {"JNZE", "1101"},
        {"CALL", "1110"},
        {"PSHI", "1111000"},
        {"POPI", "1111001"},
        {"PUSH", "1111010"},
        {"POP", "1111011"},
        {"RETN", "1111100"},
        {"SWAP", "1111101"},
        {"INSP", "1111110"},
        {"DESP", "1111111"}
    };

    const string Identifier = @"[a-zA-Z][a-zA-Z0-9]*";
    const string Literal = @"\d+";
    const string Comment = @"(\s*/.*)?";

    static readonly Regex RegexInstruction = new($@"^((?<label>{Identifier})\s*:\s*)?(?<mnemonic>[a-zA-Z]+)(\s+(?<operand>{Literal}|{Identifier}))?{Comment}$");
    static readonly Regex RegexSymbol = new($@"^(?<symbol>{Identifier})\s*=\s*(?<value>{Literal}){Comment}$");
    static readonly Regex RegexConstant = new($@"^(?<constant>{Identifier}):\s*(?<value>{Literal}){Comment}$");
    static readonly Regex RegexLabelOnly = new($@"^(?<label>{Identifier}):{Comment}$");
    static readonly Regex RegexBlankOrComment = new($@"^{Comment}$");

    public static void Assemble(Memory mp, string assemblyCode)
    {
        Dictionary<string, int> symbols = [];
        HashSet<string> labels = [];
        Dictionary<string, int> variables = [];

        var lines = assemblyCode.Replace("\r", "").Split('\n');

        List<(int lineNumber, string op_code, string? operand)> instructions = [];
        foreach (var (line, lineNumber) in lines.Select((line, index) => (line, index + 1)))
        {
            Match match = RegexInstruction.Match(line);
            if (match.Success) // instrução
            {
                string op_label = match.Groups["mnemonic"].Value;
                if (!InstructionSet.TryGetValue(op_label, out var op_code))
                    throw new OpCodeNotDefinedException(lineNumber, op_label);

                string? label = match.Groups["label"].Success ? match.Groups["label"].Value : null;
                if (label is not null)
                {
                    labels.Add(label);
                    variables.Remove(label);
                    if (!symbols.TryAdd(label, instructions.Count))
                        throw new DuplicattedSymbolException(lineNumber, label);
                }

                string? operand = match.Groups["operand"].Success ? match.Groups["operand"].Value : null;
                if (operand is not null && !Regex.IsMatch(operand, $"^{Literal}$") && !symbols.ContainsKey(operand))
                    variables.TryAdd(operand, 0); // default value

                instructions.Add((lineNumber, op_code, operand));
                continue;
            }

            match = RegexSymbol.Match(line); // A = 3
            if (match.Success)
            {
                string identifier = match.Groups["symbol"].Value;
                int value = Convert.ToInt32(match.Groups["value"].Value);
                variables.Add(identifier, value);
                if (!symbols.TryAdd(identifier, -1)) // it will change the address later
                    throw new DuplicattedSymbolException(lineNumber, identifier);

                continue;
            }

            match = RegexConstant.Match(line); // C: 2
            if (match.Success)
            {
                string identifier = match.Groups["constant"].Value;
                int value = Convert.ToInt32(match.Groups["value"].Value);
                if (!symbols.TryAdd(identifier, value))
                    throw new DuplicattedSymbolException(lineNumber, identifier);

                variables.Remove(identifier);

                continue;
            }

            match = RegexLabelOnly.Match(line); // LABEL:
            if (match.Success)
            {
                string identifier = match.Groups["label"].Value;
                labels.Add(identifier);
                variables.Remove(identifier);
                if (!symbols.TryAdd(identifier, instructions.Count))
                    throw new DuplicattedSymbolException(lineNumber, identifier);

                variables.Remove(identifier);

                continue;
            }

            match = RegexBlankOrComment.Match(line);
            if (match.Success) continue;

            throw new SyntaxErrorException(lineNumber, line);
        }

        int programSize = instructions.Count;
        int var_offset = 0;
        foreach (var (identifier, _) in variables)
        {
            symbols[identifier] = programSize + var_offset++;
        }

        int currentAddress = 0;
        foreach (var (lineNumber, op_code, operand) in instructions)
        {
            int value = 0;
            if (operand is not null)
            {
                if (Regex.IsMatch(operand, $"^{Literal}$"))
                {
                    value = Convert.ToInt32(operand);
                }
                else
                {
                    if (!symbols.TryGetValue(operand, out value))
                        throw new SymbolNotDefinedException(lineNumber, operand);
                }
            }

            string bitString = op_code + BitArrayExtensions.FromInt(value, 16 - op_code.Length).ToBitString();

            mp.SetCell(currentAddress++, BitArrayExtensions.FromBitString(bitString));
        }

        foreach (var (identifier, value) in variables)
        {
            mp.SetCell(symbols[identifier], BitArrayExtensions.FromInt(value, 16));
        }
    }
}
