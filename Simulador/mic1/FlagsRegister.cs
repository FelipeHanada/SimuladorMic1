using Simulador.components;
using Simulador.mic1.exceptions;
using Simulador.utils;
using System.Collections;

namespace Simulador.mic1;

public class FlagsRegister
{
    private readonly ISignalSender _inN;
    private readonly ISignalSender _inZ;
    private readonly ISignalSender _inCond;

    private readonly SingleSignalSender _out;
    public ISignalSender Out { get { return _out; } }

    public FlagsRegister(ISignalSender inN, ISignalSender inZ, ISignalSender inCond)
    {
        _inN = inN;
        _inN.SignalChanged += Update;
        _inZ = inZ;
        _inZ.SignalChanged += Update;
        _inCond = inCond;
        _inCond.SignalChanged += Update;
        _out = new();
    }

    private void Update(object? sender, BitArray _)
    {
        _out.Disable();
        int condition = _inCond.Signal().ToInt32();

        bool shouldEnable = condition switch
        {
            1 => _inN.Signal().HasAllSet(),
            2 => _inZ.Signal().HasAllSet(),
            3 => true,
            _ => false
        };

        if (shouldEnable)
        {
            _out.Enable();
        }
    }
}
