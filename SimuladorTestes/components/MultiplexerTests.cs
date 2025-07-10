using Simulador.components;
using System.Collections;

namespace SimuladorTestes.components;

[TestClass]
public sealed class MultiplexerTests
{
    [TestMethod]
    public void Multiplexer1()
    {
        SignalSender sig0 = new(new BitArray([false, false]));
        SignalSender sig1 = new(new BitArray([true, false]));
        SignalSender sig2 = new(new BitArray([false, true]));
        SignalSender sig3 = new(new BitArray([true, true]));
        SignalSender controlSender = new(new BitArray([false, false]));
        Multiplexer mult = new(2, [sig0, sig1, sig2, sig3], controlSender);

        Assert.IsFalse(mult.Out.Signal().Xor(sig0.Signal()).HasAnySet());

        controlSender.SetData(new([true, false]));
        Assert.IsFalse(mult.Out.Signal().Xor(sig1.Signal()).HasAnySet());

        controlSender.SetData(new([false, true]));
        Assert.IsFalse(mult.Out.Signal().Xor(sig2.Signal()).HasAnySet());

        controlSender.SetData(new([true, true]));
        Assert.IsFalse(mult.Out.Signal().Xor(sig3.Signal()).HasAnySet());

        controlSender.SetData(new([true, false]));
        Assert.IsFalse(mult.Out.Signal().Xor(sig1.Signal()).HasAnySet());

        controlSender.SetData(new([false, true]));
        Assert.IsFalse(mult.Out.Signal().Xor(sig2.Signal()).HasAnySet());

        controlSender.SetData(new([true, true]));
        Assert.IsFalse(mult.Out.Signal().Xor(sig3.Signal()).HasAnySet());
    }
}
