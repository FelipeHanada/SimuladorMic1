using Simulador.utils;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Simulador.components;

public interface ISignalSender
{
    BitArray Signal();
    event EventHandler<BitArray> SignalChanged;
}

public class SignalSender(BitArray data) : ISignalSender
{
    private BitArray _data = data;

    public event EventHandler<BitArray> SignalChanged = delegate { };

    public SignalSender(int length)
        : this(new BitArray(length)) { }

    public void SetData(BitArray data) { _data = data.TrimOrPad(data.Length); SignalChanged?.Invoke(this, _data); }

    public BitArray Signal() => new(_data);
}

public class SingleSignalSender : SignalSender
{

    public SingleSignalSender(bool bit = false)
        : base(new BitArray([bit]))
    {
    }

    public void Enable()
    {
        if (Signal().HasAllSet()) return;
        SetData(new BitArray([true]));
    }

    public void Disable()
    {
        if (!Signal().HasAllSet()) return;
        SetData(new BitArray([false]));
    }

    public void SetEnable(bool enable)
    {
        if (enable) Enable();
        else Disable();
    }

    public void Pulse()
    {
        Enable();
        Disable();
    }
}
