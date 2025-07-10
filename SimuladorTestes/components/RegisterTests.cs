using Simulador.components;
using System.Collections;

namespace SimuladorTestes.components;

[TestClass]
public sealed class RegisterTests
{
    [TestMethod]
    public void Register1()
    {
        BitArray ones = new(16); ones.SetAll(true);
        BitArray zeros = new(16);

        SignalSender dataSender = new(new BitArray(16));
        SingleSignalSender controlSender = new();

        Register reg = new(16, dataSender, controlSender);

        dataSender.SetData(ones);
        controlSender.Enable();

        Assert.IsFalse(reg.Out.Signal().Xor(ones).HasAnySet());

        dataSender.SetData(zeros);
        controlSender.Disable();

        Assert.IsFalse(reg.Out.Signal().Xor(ones).HasAnySet());
    }
}
