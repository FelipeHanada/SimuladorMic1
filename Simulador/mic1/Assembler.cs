using Simulador.components;
using Simulador.mic1.exceptions;
using Simulador.utils;
using System.Collections;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Simulador.mic1;

public class Assembler
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

    private static readonly Regex _lineRegex = new(
        @"^(?:([A-Za-z]+):\s+)?([A-Za-z]+)(?:\s+([A-Za-z_][A-Za-z0-9_]*|\d+))?$",
        RegexOptions.Compiled
        );

    public static void Assemble(Memory mp, string assemblyCode)
    {
        var lines = assemblyCode.Split(
            ['\n', '\r'],
            StringSplitOptions.RemoveEmptyEntries
        );

        List<(string op_code, string? op, string? label)> instructions = [];
        Dictionary<string, int> addresses = [];
        HashSet<string> variables = [];
        int current_address = 0;
        foreach (var line in lines)
        {
            if (!ProcessLine(line, out var result))
            {
                throw new Exception("instruction not valid");
            }

            instructions.Add(result);
            if (result.label is not null)
            {
                addresses.Add(result.label, current_address);
                variables.Remove(result.label);
            }
            if (result.op is not null && char.IsLetter(result.op[0]))
            {
                if (!addresses.ContainsKey(result.op))
                    variables.Add(result.op);
            }


            current_address++;
        }
        //instructions.Add(("0110", instructions.Count.ToString(), null));

        var code_size = instructions.Count;
        int var_offset = 0;
        foreach (var variable in variables)
        {
            if (addresses.ContainsKey(variable))
            {
                // label usada como variable
                continue;
            }

            addresses.Add(variable, code_size + var_offset);
            var_offset++;
        }

        current_address = 0;
        foreach (var (op_code, op, label) in instructions)
        {
            string bitString = op_code;

            if (op is not null)
            {
                if (char.IsLetter(op[0])) // variable or label
                {
                    addresses.TryGetValue(op, out var addr);
                    bitString += BitArrayExtensions.FromInt(Convert.ToInt32(addr), 16 - op_code.Length).ToBitString();
                }
                else // literal
                {
                    bitString += BitArrayExtensions.FromInt(Convert.ToInt32(op), 16 - op_code.Length).ToBitString();
                }
            } else
            {
                bitString += new string('0', 16 - op_code.Length);
            }

            mp.SetCell(current_address, BitArrayExtensions.FromBitString(bitString));
            current_address++;
        }
    }

    public static bool ProcessLine(string line, out (string op_code, string? op, string? label) result)
    {
        result = default;
        var match = _lineRegex.Match(line);
        if (!match.Success) return false;

        string op_label = match.Groups[2].Value;
        if (!InstructionSet.TryGetValue(op_label, out var op_code)) return false;

        string? label = match.Groups[1].Success ? match.Groups[1].Value : null;
        string? op = match.Groups[3].Success ? match.Groups[3].Value : null;

        result = (op_code, op, label);
        return true;
    }
}
