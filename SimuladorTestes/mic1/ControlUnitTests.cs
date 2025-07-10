using Simulador.components;
using Simulador.mic1;
using Simulador.utils;

namespace SimuladorTestes.mic1;

[TestClass]
public sealed class ControlUnitTests
{
    [TestMethod]
    public void ControlUnitTest()
    {
        SingleSignalSender inN = new();
        SingleSignalSender inZ = new();
        ControlUnit controlUnit = new(inN, inZ);

        var mir = controlUnit.Mir;
        Assert.IsNotNull(mir);

        // 0: mar := pc; rd;
        controlUnit.Clock.Step(); // ciclo 1: busca instrução
        System.Console.WriteLine(mir.ToString());
        Assert.IsTrue(mir.Out.Signal().Compare(BitArrayExtensions.FromBitString("00010000110000000000000000000000")));
        
        Assert.IsTrue(mir.OutMAR.Signal().HasAllSet());
        Assert.AreEqual(0, mir.OutB.Signal().ToInt32());
        Assert.IsTrue(mir.OutRD.Signal().HasAllSet());
        controlUnit.Clock.Step(); // ciclo 2
        controlUnit.Clock.Step(); // ciclo 3
        controlUnit.Clock.Step(); // ciclo 4: incrementa PC

        // 1: pc := pc + 1; rd;
        controlUnit.Clock.Step(); // ciclo 1: busca instrução
        System.Console.WriteLine(mir.ToString());
        Assert.IsTrue(mir.Out.Signal().Compare(BitArrayExtensions.FromBitString("00000000010100000110000000000000")));
        controlUnit.Clock.Step(); // ciclo 2
        controlUnit.Clock.Step(); // ciclo 3
        controlUnit.Clock.Step(); // ciclo 4: incrementa PC

        // 2: ir := mbr; if n then goto 28;
        inN.Enable();
        controlUnit.Clock.Step(); // ciclo 1: busca instrução
        System.Console.WriteLine(mir.ToString());
        Assert.IsTrue(mir.Out.Signal().Compare(BitArrayExtensions.FromBitString("10110000000100110000000000011100")));
        controlUnit.Clock.Step(); // ciclo 2
        controlUnit.Clock.Step(); // ciclo 3
        controlUnit.Clock.Step(); // ciclo 4

        // 28: ;
        controlUnit.Clock.Step(); // ciclo 1: busca instrução
        System.Console.WriteLine(mir.ToString());
        Assert.IsTrue(mir.Out.Signal().Compare(BitArrayExtensions.FromBitString("00100100000101000011001100101000")));
    }

    [TestMethod]
    public void ControlUnitTest2()
    {
        SingleSignalSender inN = new();
        SingleSignalSender inZ = new();
        ControlUnit controlUnit = new(inN, inZ);

        var mir = controlUnit.Mir;
        Assert.IsNotNull(mir);

        // 0: mar := pc; rd;
        controlUnit.Clock.Step(); // ciclo 1: busca instrução
        System.Console.WriteLine(mir.ToString());
        System.Console.WriteLine(mir.OutShifter.Signal().ToBitString());
        controlUnit.Clock.Step(); // ciclo 2
        controlUnit.Clock.Step(); // ciclo 3
        controlUnit.Clock.Step(); // ciclo 4: incrementa PC

        // 1: pc := pc + 1; rd;
        controlUnit.Clock.Step(); // ciclo 1: busca instrução
        System.Console.WriteLine(mir.ToString());
        System.Console.WriteLine(mir.OutShifter.Signal().ToBitString());
        controlUnit.Clock.Step(); // ciclo 2
        controlUnit.Clock.Step(); // ciclo 3
        controlUnit.Clock.Step(); // ciclo 4: incrementa PC

        // 2: ir := mbr; if n then goto 28;
        controlUnit.Clock.Step(); // ciclo 1: busca instrução
        System.Console.WriteLine(mir.ToString());
        System.Console.WriteLine(mir.OutShifter.Signal().ToBitString());
        controlUnit.Clock.Step(); // ciclo 2
        controlUnit.Clock.Step(); // ciclo 3
        controlUnit.Clock.Step(); // ciclo 4

        // 3: ;
        controlUnit.Clock.Step(); // ciclo 1: busca instrução
        System.Console.WriteLine(mir.ToString());
        System.Console.WriteLine(mir.OutShifter.Signal().ToBitString());
    }
}
