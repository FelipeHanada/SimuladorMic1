using Simulador.utils;
using System;
using System.Collections;

namespace Simulador.components;

public class Shifter
{
    private readonly ISignalSender _in;
    private readonly ISignalSender _inControl;

    private readonly SignalSender _out;

    public ISignalSender Out { get => _out; }
    
    public Shifter(ISignalSender input, ISignalSender inControl)
    {
        _in = input;
        _inControl = inControl;

        _in.SignalChanged += Update;
        _inControl.SignalChanged += Update;

        _out = new SignalSender(_in.Signal().Length);
    }

    private void Update(object? sender, BitArray _)
    {
        BitArray input = _in.Signal();
        int control = _inControl.Signal().ToInt32();
        BitArray result = control switch
        {
            1 => input.ShiftLeft(),
            2 => input.ShiftRight(),
            _ => input
        };

        _out.SetData(result);
    }
}
