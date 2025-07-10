using System;
using System.Collections;

namespace Simulador.components;

public class Latch
{
    private bool _outputEnabled;
    public bool OutputEnabled { get { return _outputEnabled; } }

    private ISignalSender? _in;
    private ISignalSender? _inCtrl;
    private readonly SignalSender _out;

    public ISignalSender Out { get { return _out; } }

    public event EventHandler<bool> EWChanged = delegate { };

    public Latch(ISignalSender dataSender, ISignalSender controlSender)
    {
        _out = new SignalSender(dataSender.Signal().Length);
        _outputEnabled = false;

        SetDataSender(dataSender);
        SetControlSender(controlSender);
    }

    public void Reset() { _out.SetData(new(_out.Signal().Length, false)); }

    public void SetDataSender(ISignalSender dataSender)
    {
        if (_in is not null)
            _in.SignalChanged -= OnDataChange;
        _in = dataSender;
        _in.SignalChanged += OnDataChange!;
    }

    public void SetControlSender(ISignalSender controlSender)
    {
        if (_inCtrl is not null)
            _inCtrl.SignalChanged -= OnControlChange!;
        _inCtrl = controlSender;
        _inCtrl.SignalChanged += OnControlChange!;
        OnControlChange(controlSender, controlSender.Signal());
    }

    protected void OnDataChange(object? sender, BitArray _)
    {
        if (_in is null || !_outputEnabled) return;
        _out.SetData(_in.Signal());
    }

    protected void OnControlChange(object? sender, BitArray _)
    {
        _outputEnabled = _inCtrl!.Signal().HasAllSet();

        if (_in is null || !_outputEnabled) return;
        _out.SetData(_in.Signal());
    }
}
