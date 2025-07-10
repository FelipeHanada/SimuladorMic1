using Simulador.components;
using System.Collections;

namespace SimuladorTestes.components;

[TestClass]
public sealed class LatchTests
{
    [TestMethod]
    public void Latch1()
    {
        BitArray ones = new(16); ones.SetAll(true);
        BitArray zeros = new(16);

        SignalSender dataSender = new(new BitArray(16));
        SingleSignalSender controlSender = new();

        Latch latch = new(dataSender, controlSender);

        dataSender.SetData(ones);
        controlSender.Enable();

        Assert.IsFalse(latch.Out.Signal().Xor(ones).HasAnySet());

        dataSender.SetData(zeros);

        Assert.IsFalse(latch.Out.Signal().Xor(zeros).HasAnySet());

        controlSender.Disable();
        dataSender.SetData(ones);

        Assert.IsFalse(latch.Out.Signal().Xor(zeros).HasAnySet());
    }
}
