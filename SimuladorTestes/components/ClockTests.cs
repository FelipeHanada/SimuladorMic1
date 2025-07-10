using Simulador.components;
using Simulador.utils;
using System.Collections;

namespace SimuladorTestes.components;

[TestClass]
public sealed class ClockTests
{
    [TestMethod]
    public void Clock1()
    {
        Clock clock = new(4);
        List<ISignalSender> clockSignals = clock.Signals();

        Assert.AreEqual(4, clockSignals.Count);

        clock.Step();
        Assert.IsTrue(clockSignals[0].Signal().HasAllSet());
            
        clock.Step();
        Assert.IsFalse(clockSignals[0].Signal().HasAllSet());
        Assert.IsTrue(clockSignals[1].Signal().HasAllSet());

        clock.Step();
        Assert.IsFalse(clockSignals[1].Signal().HasAllSet());
        Assert.IsTrue(clockSignals[2].Signal().HasAllSet());

        clock.Step();
        Assert.IsFalse(clockSignals[2].Signal().HasAllSet());
        Assert.IsTrue(clockSignals[3].Signal().HasAllSet());

        clock.Step();
        Assert.IsFalse(clockSignals[3].Signal().HasAllSet());
        Assert.IsTrue(clockSignals[0].Signal().HasAllSet());

        /* --------------------------------------------------- */

        clock.Step();
        Assert.IsFalse(clockSignals[0].Signal().HasAllSet());
        Assert.IsTrue(clockSignals[1].Signal().HasAllSet());

        clock.Step();
        Assert.IsFalse(clockSignals[1].Signal().HasAllSet());
        Assert.IsTrue(clockSignals[2].Signal().HasAllSet());

        clock.Step();
        Assert.IsFalse(clockSignals[2].Signal().HasAllSet());
        Assert.IsTrue(clockSignals[3].Signal().HasAllSet());

        clock.Step();
        Assert.IsFalse(clockSignals[3].Signal().HasAllSet());
        Assert.IsTrue(clockSignals[0].Signal().HasAllSet());
    }

    [TestMethod]
    public void ClockDelayedSignal1()
    {
        Clock clock = new(4);
        SingleSignalSender signal = new();
        ClockDelayedSignalSender delayedSignal = new(signal, clock, 3);

        clock.Step();
        System.Console.WriteLine("clock: " + clock.CurrentCycle());
        System.Console.WriteLine("signal: " + signal.Signal().ToBitString());
        System.Console.WriteLine("delayedSignal: " + delayedSignal.Signal().ToBitString());

        signal.Enable();
        System.Console.WriteLine("clock: " + clock.CurrentCycle());
        System.Console.WriteLine("signal: " + signal.Signal().ToBitString());
        System.Console.WriteLine("delayedSignal: " + delayedSignal.Signal().ToBitString());

        Assert.IsFalse(signal.Signal().Compare(delayedSignal.Signal()));

        clock.Step();
        System.Console.WriteLine("clock: " + clock.CurrentCycle());
        System.Console.WriteLine("signal: " + signal.Signal().ToBitString());
        System.Console.WriteLine("delayedSignal: " + delayedSignal.Signal().ToBitString());

        Assert.IsFalse(signal.Signal().Compare(delayedSignal.Signal()));


        clock.Step();
        System.Console.WriteLine("clock: " + clock.CurrentCycle());
        System.Console.WriteLine("signal: " + signal.Signal().ToBitString());
        System.Console.WriteLine("delayedSignal: " + delayedSignal.Signal().ToBitString());

        Assert.IsFalse(signal.Signal().Compare(delayedSignal.Signal()));

        clock.Step();
        System.Console.WriteLine("clock: " + clock.CurrentCycle());
        System.Console.WriteLine("signal: " + signal.Signal().ToBitString());
        System.Console.WriteLine("delayedSignal: " + delayedSignal.Signal().ToBitString());

        Assert.IsTrue(signal.Signal().Compare(delayedSignal.Signal()));

        clock.Step();
        System.Console.WriteLine("clock: " + clock.CurrentCycle());
        System.Console.WriteLine("signal: " + signal.Signal().ToBitString());
        System.Console.WriteLine("delayedSignal: " + delayedSignal.Signal().ToBitString());

        Assert.IsTrue(signal.Signal().Compare(delayedSignal.Signal()));
    }

    [TestMethod]
    public void ClockDelayedSignalZero()
    {
        Clock clock = new(4);
        SingleSignalSender signal = new();
        ClockDelayedSignalSender delayedSignal = new(signal, clock, 0);

        signal.SignalChanged += (object? _, BitArray __) =>
        {
            System.Console.WriteLine("signal changed");
        };

        delayedSignal.SignalChanged += (object? _, BitArray __) =>
        {
            System.Console.WriteLine("delayedSignal changed");
        };

        clock.Step();

        signal.Enable();
        Assert.IsTrue(signal.Signal().Compare(delayedSignal.Signal()));

        signal.Disable();
        Assert.IsTrue(signal.Signal().Compare(delayedSignal.Signal()));
    }
}
