using Simulador.utils;
using System.Collections;
using System.Reflection;

namespace Simulador.components;

public class Register(int length)
{
    public event EventHandler<BitArray> DataChanged = delegate { };
    public event EventHandler<bool> ControlChanged = delegate { };

    private ISignalSender? _in;
    private ISignalSender? _inCtrl;
    private readonly SignalSender _out = new(length);

    public readonly string? Name;

    public ISignalSender Out { get { return _out; } }

    public Register(int length, string? name)
        : this(length)
    {
        Name = name;
    }

    public Register(int length, ISignalSender dataSender, ISignalSender ControlSender, string? name)
        : this(length, name)
    {
        SetDataSender(dataSender);
        SetControlSender(ControlSender);
    }

    public Register(int length, ISignalSender dataSender, ISignalSender ControlSender)
    : this(length, dataSender, ControlSender, null) {}

    public void SetDataSender(ISignalSender dataSender)
    {
        _in = dataSender;
    }

    public void SetControlSender(ISignalSender ControlSender)
    {
        if (_inCtrl is not null)
            _inCtrl.SignalChanged -= OnControlChange!;
        
        _inCtrl = ControlSender;
        _inCtrl.SignalChanged += OnControlChange!;
    }

    public void SetData(BitArray data)
    {
        _out.SetData(data);

        if (Name is not null)
        {
            System.Console.WriteLine(Name + ", changed: " + _out.Signal().ToBitString());
        }

        DataChanged?.Invoke(this, _out.Signal());
    }

    public void Reset() { SetData(new(_out.Signal().Length, false)); }
    protected void OnControlChange(object? sender, BitArray _)
    {
        if (_in is null) return;

        if (_inCtrl!.Signal().HasAllSet())
        {
            SetData(_in.Signal());
            ControlChanged?.Invoke(this, true);
        }
        else
        {
            ControlChanged?.Invoke(this, false);
        }
    }
    public override string ToString()
    {
        return Name is not null
            ? $"Register {Name} ({_out.Signal().ToBitString()})"
            : $"Register ({_out.Signal().ToBitString()})";
    }
}
