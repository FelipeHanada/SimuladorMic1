using Simulador.components;
using Simulador.utils;

namespace SimuladorTestes.components;

[TestClass]
public sealed class MemoryTests
{
    [TestMethod]
    public void Memory1()
    {
        SignalSender addressSignal = new(4);
        SignalSender bufferInSignal = new(16);
        SingleSignalSender rd = new();
        SingleSignalSender wr = new();

        Memory memory = new(16, 16, addressSignal, bufferInSignal, rd, wr);

        addressSignal.SetData(BitArrayExtensions.FromInt(10, 16)); // address = 10
        bufferInSignal.SetData(BitArrayExtensions.FromInt(15, 16)); // data = 15

        rd.Pulse();
        Assert.AreEqual(memory.Out.Signal().ToInt32(), 0);

        wr.Pulse();

        rd.Pulse();
        Assert.AreEqual(memory.Out.Signal().ToInt32(), 15);
    }

    [TestMethod]
    public void Memory2()
    {
        SingleSignalSender rd = new();
        SingleSignalSender wr = new();
        Register mbr = new(16);
        Register mar = new(16);

        Memory memory = new(4096, 16, mar.Out, mbr.Out, rd, wr);
        mbr.SetDataSender(memory.Out);
        mbr.SetControlSender(rd);

        memory.SetCell(0, BitArrayExtensions.FromInt(10, 16));      // MP[0] = 10

        Assert.AreEqual(0, mbr.Out.Signal().ToInt32());

        rd.Pulse();
        Assert.AreEqual(10, mbr.Out.Signal().ToInt32());

        mar.SetData(BitArrayExtensions.FromInt(1, 16));             // MAR = 1

        wr.Pulse();                                                 // MP[1] = 10
        rd.Pulse();
        Assert.AreEqual(memory.Cell(0).ToInt32(), memory.Cell(1).ToInt32());
    }
}
