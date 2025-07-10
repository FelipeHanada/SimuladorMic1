using Simulador.components;
using Simulador.mic1;
using Simulador.utils;
using System;
using System.Collections;

namespace SimuladorTestes.mic1;

[TestClass]
public sealed class AssemblerTests
{
    [TestMethod]
    public void Assembler1()
    {
        Memory memory = new(4096, 16, new SignalSender(0), new SignalSender(0), null, null, "MP");

        Assembler.Assemble(memory, "LOCO 1\nSTOD 4");

        Assert.IsTrue(BitArrayExtensions.FromBitString("0111000000000001").Compare(memory.Cell(0)));
        Assert.IsTrue(BitArrayExtensions.FromBitString("0001000000000100").Compare(memory.Cell(1)));
    }

    [TestMethod]
    public void Assembler2()
    {
        Memory memory = new(4096, 16, new SignalSender(0), new SignalSender(0), null, null, "MP");

        Assembler.Assemble(memory, "LOCO 1\nSTOD var1\nSTOD var1\nSTOD var2\nSTOD var2");

        Assert.IsTrue(BitArrayExtensions.FromBitString("0111000000000001").Compare(memory.Cell(0)));
        Assert.IsTrue(memory.Cell(1).Compare(memory.Cell(2)));
        Assert.IsTrue(memory.Cell(3).Compare(memory.Cell(4)));
    }

    [TestMethod]
    public void Example1()
    {
        Mic1 mic1 = new();
        mic1.Registers[1].DataChanged += (object? _, BitArray __) => { System.Console.WriteLine("Happened"); };
        
        Assembler.Assemble(mic1.MP, "LOCO 1\r\nSTOD var1\r\nLOCO 2\r\nADDD var1\r\nSTOD var2");

        mic1.StepMacro();
        Assert.AreEqual(1, mic1.Registers[1].Out.Signal().ToInt32());
        Assert.AreEqual(1, mic1.Registers[0].Out.Signal().ToInt32());
    }

    [TestMethod]
    public void Example2()
    {
        Mic1 mic1 = new();

        Assembler.Assemble(mic1.MP, "START: LOCO 16\r\nSTOD var1\r\nLOCO 1\r\nSTOD var2\r\nLOCO 0\r\nPUSH\r\nLOCO 1\r\nPUSH\r\nLOOP: LODL 1\r\nADDL 0\r\nPUSH\r\nLODD var1\r\nSUBD var2\r\nSTOD var1\r\nJZER END\r\nJUMP LOOP\r\nEND: JUMP END");
    }
}
