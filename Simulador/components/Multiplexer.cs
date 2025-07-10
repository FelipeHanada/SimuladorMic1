using Simulador.utils;
using System.Collections;

namespace Simulador.components;

public class Multiplexer
{
	private readonly ISignalSender[] _in;
	private readonly ISignalSender _inCtrl;
    private ISignalSender _current;

    private readonly SignalSender _out;

    public ISignalSender Out { get { return _out; } }

	public Multiplexer(int length, ISignalSender[] inputSenders, ISignalSender controlSender)
	{
        _in = inputSenders;
        _inCtrl = controlSender;
        _inCtrl.SignalChanged += OnControlChange!;

		int index = _inCtrl.Signal().ToInt32();
        _current = _in[index];
        _current.SignalChanged += OnCurrentChange;

        _out = new(length);
        _out.SetData(_current.Signal());
	}

    public void SetOutput(int index)
    {
        _current.SignalChanged -= OnCurrentChange;
        _current = _in[index];
        _current.SignalChanged += OnCurrentChange;
        _out.SetData(_current.Signal());
    }

    public void OnControlChange(object? sender, BitArray _)
    {
        int index = _inCtrl.Signal().ToInt32();
        SetOutput(index);
    }

    public void OnCurrentChange(object? sender, BitArray _)
    {
        _out.SetData(_current.Signal());
    }
}
