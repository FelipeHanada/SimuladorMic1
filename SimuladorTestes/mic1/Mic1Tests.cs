using Simulador.components;
using Simulador.mic1;
using Simulador.utils;

namespace SimuladorTestes.mic1;

[TestClass]
public sealed class Mic1Tests
{
    [TestMethod]
    public void LOCO()
    {
        Mic1 mic1 = new();
        mic1.MP.SetCell(0, BitArrayExtensions.FromBitString("0111001001001001"));   // LOCO
        mic1.StepMacro();

        Assert.IsTrue(BitArrayExtensions.FromBitString("0000001001001001").Compare(mic1.Registers[1].Out.Signal()));
    }

    [TestMethod]
    public void STOD()
    {
        Mic1 mic1 = new();

        mic1.MP.SetCell(0, BitArrayExtensions.FromBitString("0111001001001001"));   // LOCO
        mic1.MP.SetCell(1, BitArrayExtensions.FromBitString("0001000000000100"));   // STOD 4

        mic1.StepMacro();
        mic1.StepMacro();

        Assert.IsTrue(mic1.MP.Cell(4).Compare(BitArrayExtensions.FromBitString("0000001001001001")));
    }

    [TestMethod]
    public void SumOfTwoNumbers()
    {
        Mic1 mic1 = new();

        mic1.MP.SetCell(0, BitArrayExtensions.FromBitString("0111000000000001"));   // LOCO 1 : AC = 1
        mic1.MP.SetCell(1, BitArrayExtensions.FromBitString("0001000000001000"));   // STOD 8 : m[8] = 1
        mic1.MP.SetCell(2, BitArrayExtensions.FromBitString("0111000000000010"));   // LOCO 2 : AC = 2
        mic1.MP.SetCell(3, BitArrayExtensions.FromBitString("0010000000001000"));   // ADDD 8 : AC = 3
        mic1.MP.SetCell(4, BitArrayExtensions.FromBitString("0001000000001001"));   // STOD 9 : m[9] = 3

        mic1.StepMacro();
        mic1.StepMacro();
        mic1.StepMacro();
        mic1.StepMacro();
        mic1.StepMacro();
     
        Assert.AreEqual(1, mic1.MP.Cell(8).ToInt32());
        Assert.AreEqual(3, mic1.MP.Cell(9).ToInt32());
    }

    [TestMethod]
    public void Stack()
    {
        Mic1 mic1 = new();
        mic1.MP.SetCell(0, BitArrayExtensions.FromBitString("0111000000000001"));   // LOCO 1 : AC = 1
        mic1.MP.SetCell(1, BitArrayExtensions.FromBitString("1111010000000000"));   // PUSH
        mic1.MP.SetCell(2, BitArrayExtensions.FromBitString("0111000000000010"));   // LOCO 2 : AC = 2
        mic1.MP.SetCell(3, BitArrayExtensions.FromBitString("1111010000000000"));   // PUSH
        mic1.MP.SetCell(4, BitArrayExtensions.FromBitString("0111000000000011"));   // LOCO 3 : AC = 3
        mic1.MP.SetCell(5, BitArrayExtensions.FromBitString("1111010000000000"));   // PUSH

        mic1.StepMacro();
        mic1.StepMacro();
        mic1.StepMacro();
        mic1.StepMacro();
        mic1.StepMacro();
        mic1.StepMacro();

        for (int i=4090; i<4096; i++)
        {
            System.Console.WriteLine(i + ": " + mic1.MP.Cell(i).ToBitString());
        }

        Assert.AreEqual(1, mic1.MP.Cell(4095).ToInt32());
        Assert.AreEqual(2, mic1.MP.Cell(4094).ToInt32());
        Assert.AreEqual(3, mic1.MP.Cell(4093).ToInt32());
    }
}
