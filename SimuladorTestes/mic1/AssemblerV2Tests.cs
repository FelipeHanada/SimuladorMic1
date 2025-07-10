using Simulador.components;
using Simulador.mic1;
using Simulador.utils;
using System;
using System.Collections;

namespace SimuladorTestes.mic1;

[TestClass]
public sealed class AssemblerV2Tests
{
    [TestMethod]
    public void Assembler1()
    {
        Memory memory = new(4096, 16, new SignalSender(0), new SignalSender(0), null, null, "MP");
        string assemblyCode = """
            LOCO 1
            STOD 4
            """;

        AssemblerV2.Assemble(memory, assemblyCode);

        Assert.IsTrue(BitArrayExtensions.FromBitString("0111000000000001").Compare(memory.Cell(0)));
        Assert.IsTrue(BitArrayExtensions.FromBitString("0001000000000100").Compare(memory.Cell(1)));
    }

    [TestMethod]
    public void Assembler2()
    {
        Memory memory = new(4096, 16, new SignalSender(0), new SignalSender(0), null, null, "MP");
        string assemblyCode = """
            LOCO 1
            STOD var1
            STOD var1
            STOD var2
            STOD var2
            """;

        AssemblerV2.Assemble(memory, assemblyCode);

        Assert.IsTrue(BitArrayExtensions.FromBitString("0111000000000001").Compare(memory.Cell(0)));
        Assert.IsTrue(memory.Cell(1).Compare(memory.Cell(2)));
        Assert.IsTrue(memory.Cell(3).Compare(memory.Cell(4)));
    }

    [TestMethod]
    public void Assembler3()
    {
        Memory memory = new(4096, 16, new SignalSender(0), new SignalSender(0), null, null, "MP");
        string assemblyCode = """
            var1 = 0
            var2 = 0
            LOCO 1
            STOD var1
            STOD var1
            STOD var2
            STOD var2
            """;

        AssemblerV2.Assemble(memory, assemblyCode);

        Assert.IsTrue(BitArrayExtensions.FromBitString("0111000000000001").Compare(memory.Cell(0)));
        Assert.IsTrue(memory.Cell(1).Compare(memory.Cell(2)));
        Assert.IsTrue(memory.Cell(3).Compare(memory.Cell(4)));
    }

    [TestMethod]
    public void Example1()
    {
        Mic1 mic1 = new();
        string assemblyCode = """
            LOCO 1
            STOD var1
            LOCO 2
            ADDD var1
            STOD var2
            """;

        AssemblerV2.Assemble(mic1.MP, assemblyCode);

        mic1.StepMacro();
        Assert.AreEqual(1, mic1.Registers[1].Out.Signal().ToInt32());
        Assert.AreEqual(1, mic1.Registers[0].Out.Signal().ToInt32());
    }

    [TestMethod]
    public void Example2()
    {
        Mic1 mic1 = new();
        string assemblyCode = """
            var1 = 0
            var2 = 0
            START: LOCO 16
            STOD var1
            LOCO 1
            STOD var2
            LOCO 0
            PUSH
            LOCO 1
            PUSH
            LOOP: LODL 1
            ADDL 0
            PUSH
            LODD var1
            SUBD var2
            STOD var1
            JZER END
            JUMP LOOP
            END: JUMP END
            """;

        AssemblerV2.Assemble(mic1.MP, assemblyCode);
    }

    [TestMethod]
    public void Constant()
    {
        Mic1 mic1 = new();
        string assemblyCode = """
            C0: 0
            C1: 1
            C10: 10
            LOCO C0
            LOCO C1
            LOCO C10
            """;

        AssemblerV2.Assemble(mic1.MP, assemblyCode);

        Console.WriteLine(mic1.MP.Cell(0).ToBitString());

        Assert.IsTrue(mic1.MP.Cell(0).Compare(BitArrayExtensions.FromBitString("0111000000000000")));
        Assert.IsTrue(mic1.MP.Cell(1).Compare(BitArrayExtensions.FromBitString("0111000000000001")));
        Assert.IsTrue(mic1.MP.Cell(2).Compare(BitArrayExtensions.FromBitString("0111000000001010")));
    }
}
