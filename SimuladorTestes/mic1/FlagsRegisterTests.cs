using Simulador.components;
using Simulador.mic1;
using Simulador.utils;

using System.Collections;

namespace SimuladorTestes.mic1;

[TestClass]
public sealed class FlagsRegisterTests
{
    [TestMethod]
    public void MMux0Test()
    {
        SingleSignalSender inN = new();
        SingleSignalSender inZ = new();
        SignalSender Cond = new(2);
        FlagsRegister flags = new(inN, inZ, Cond);
        SignalSender mpcInc = new(new BitArray([true, true, true, true, true, true, true, true]));
        SignalSender addr = new(8);
        Multiplexer mMux = new(8, [mpcInc, addr], flags.Out);

        /* 0 - Não desvia */
        Cond.SetData(new([false, false]));

        // N = 0, Z = 0;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), mpcInc.Signal()));

        inN.SetEnable(true); // N = 1, Z = 0;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), mpcInc.Signal()));

        inZ.SetEnable(true); // N = 1, Z = 1;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), mpcInc.Signal()));

        inN.SetEnable(false); // N = 0, Z = 1;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), mpcInc.Signal()));
    }

    [TestMethod]
    public void MMux1Test()
    {
        SingleSignalSender inN = new();
        SingleSignalSender inZ = new();
        SignalSender Cond = new(2);
        FlagsRegister flags = new(inN, inZ, Cond);
        SignalSender mpcInc = new(new BitArray([true, true, true, true, true, true, true, true]));
        SignalSender addr = new(8);
        Multiplexer mMux = new(8, [mpcInc, addr], flags.Out);

        /* 1 - Desvia se N = 1 */
        Cond.SetData(new([true, false]));

        // N = 0, Z = 0;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), mpcInc.Signal()));

        inN.SetEnable(true); // N = 1, Z = 0;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), addr.Signal()));

        inZ.SetEnable(true); // N = 1, Z = 1;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), addr.Signal()));

        inN.SetEnable(false); // N = 0, Z = 1;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), mpcInc.Signal()));
    }

    [TestMethod]
    public void MMux2Test()
    {
        SingleSignalSender inN = new();
        SingleSignalSender inZ = new();
        SignalSender Cond = new(2);
        FlagsRegister flags = new(inN, inZ, Cond);
        SignalSender mpcInc = new(new BitArray([true, true, true, true, true, true, true, true]));
        SignalSender addr = new(8);
        Multiplexer mMux = new(8, [mpcInc, addr], flags.Out);

        /* 2 - Desvia se Z = 1 */
        Cond.SetData(new([false, true]));

        // N = 0, Z = 0;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), mpcInc.Signal()));

        inN.SetEnable(true); // N = 1, Z = 0;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), mpcInc.Signal()));

        inZ.SetEnable(true); // N = 1, Z = 1;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), addr.Signal()));

        inN.SetEnable(false); // N = 0, Z = 1;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), addr.Signal()));
    }

    [TestMethod]
    public void MMux3Test()
    {
        SingleSignalSender inN = new();
        SingleSignalSender inZ = new();
        SignalSender Cond = new(2);
        FlagsRegister flags = new(inN, inZ, Cond);
        SignalSender mpcInc = new(new BitArray([true, true, true, true, true, true, true, true]));
        SignalSender addr = new(8);
        Multiplexer mMux = new(8, [mpcInc, addr], flags.Out);

        /* 3 - Desvia */
        Cond.SetData(new([true, true]));

        // N = 0, Z = 0;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), addr.Signal()));

        inN.SetEnable(true); // N = 1, Z = 0;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), addr.Signal()));

        inZ.SetEnable(true); // N = 1, Z = 1;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), addr.Signal()));

        inN.SetEnable(false); // N = 0, Z = 1;
        Assert.IsTrue(BitArrayExtensions.Compare(mMux.Out.Signal(), addr.Signal()));
    }
}
