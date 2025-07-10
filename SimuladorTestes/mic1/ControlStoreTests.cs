using Simulador.components;
using Simulador.mic1;
using Simulador.utils;

namespace SimuladorTestes.mic1;

[TestClass]
public sealed class ControlStoreTests
{
    [TestMethod]
    public void LoadTest()
    {
        Memory ctrlStore = new ControlStore(
            new SignalSender(16),
            new SignalSender(32),
            null, null
            );

        ICtrlStoreSrcFileLoader loader = new CtrlStoreTxtSrcFileLoader("mic1/control_store.txt");
        loader.Load(ctrlStore);

        for (int i=0; i<=78; i++)
        {
            Assert.AreNotEqual(0, ctrlStore.Cell(i).ToInt32());
        }
    }
}
