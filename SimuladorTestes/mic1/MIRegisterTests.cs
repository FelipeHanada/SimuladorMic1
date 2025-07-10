using Microsoft.Win32;
using Simulador.components;
using Simulador.mic1;
using Simulador.utils;

namespace SimuladorTestes.mic1;

[TestClass]
public sealed class MIRegisterTests
{
    [TestMethod]
    public void SplitSignals()
    {
        SignalSender addr = new SignalSender(16);
        SingleSignalSender rd = new();
        Memory ctrlStore = new ControlStore(addr, new SignalSender(0), rd, null);
        ICtrlStoreSrcFileLoader loader = new CtrlStoreTxtSrcFileLoader("mic1/control_store.txt");
        loader.Load(ctrlStore);

        MIRegister register = new();
        SingleSignalSender ctrlMIR = new();
        register.SetDataSender(ctrlStore.Out);
        register.SetControlSender(ctrlMIR);
        
        System.Console.WriteLine(register);
        rd.Pulse(); ctrlMIR.Pulse();
        System.Console.WriteLine(register);

        WriteSplitSignals(register);

        System.Console.WriteLine("-------------------------------------------------");
        addr.SetData(BitArrayExtensions.FromInt(addr.Signal().ToInt32() + 1, 16));
        rd.Pulse(); ctrlMIR.Pulse();
        WriteSplitSignals(register);
    }

    private static void WriteSplitSignals(MIRegister mir)
    {
        System.Console.WriteLine("AMux: {0}", mir.OutAMux.Signal().ToBitString());
        System.Console.WriteLine("Cond: {0}", mir.OutCond.Signal().ToBitString());
        System.Console.WriteLine("Alu: {0}", mir.OutAlu.Signal().ToBitString());
        System.Console.WriteLine("Shifter: {0}", mir.OutShifter.Signal().ToBitString());
        System.Console.WriteLine("MBR: {0}", mir.OutMBR.Signal().ToBitString());
        System.Console.WriteLine("MAR: {0}", mir.OutMAR.Signal().ToBitString());
        System.Console.WriteLine("RD: {0}", mir.OutRD.Signal().ToBitString());
        System.Console.WriteLine("WR: {0}", mir.OutWR.Signal().ToBitString());
        System.Console.WriteLine("ENC: {0}", mir.OutENC.Signal().ToBitString());
        System.Console.WriteLine("C: {0}", mir.OutC.Signal().ToBitString());
        System.Console.WriteLine("B: {0}", mir.OutB.Signal().ToBitString());
        System.Console.WriteLine("A: {0}", mir.OutA.Signal().ToBitString());
        System.Console.WriteLine("ADDR: {0}", mir.OutADDR.Signal().ToBitString());
    }
}
