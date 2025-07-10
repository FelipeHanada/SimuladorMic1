using System.Collections;

namespace Simulador.components;

public class Clock
{
    private readonly int _cycles;
    private int _currentCycle;
    private readonly List<SingleSignalSender> _signals;

    public event EventHandler Stepped = delegate { };

    public Clock(int cycles)
    {
        _cycles = cycles;
        _currentCycle = -1;
        _signals = [..Enumerable.Range(0, _cycles).Select(_ => new SingleSignalSender())];
    }

    public int CurrentCycle() => _currentCycle;

    public List<ISignalSender> Signals()
    {
        return [.._signals.Cast<ISignalSender>()];
    }

    public ISignalSender Signal(int cycle)
    {
        return _signals[cycle];
    }

    public void Reset()
    {
        if (_currentCycle >= 0)
        {
            _signals[_currentCycle].Disable();
            //Stepped?.Invoke(this, EventArgs.Empty);
        }

        _currentCycle = -1;
    }

    public void Step()
    {
        if (_currentCycle >= 0)
        {
            _signals[_currentCycle].Disable();
            Stepped?.Invoke(this, EventArgs.Empty);
        }
     
        _currentCycle++;
        if (_currentCycle >= _cycles)
        {
            _currentCycle = 0;
        }

        _signals[_currentCycle].Enable();
    }
}

/*
 CRIAR TESTES PARA ISSO
 */

public class ClockDelayedSignalSender : ISignalSender
{
    private readonly ISignalSender _source;
    private readonly Clock _clock;
    private readonly int _delay;
    private int _counter;
    private BitArray _buffer;

    public event EventHandler<BitArray> SignalChanged = delegate { };

    public ClockDelayedSignalSender(ISignalSender source, Clock clock, int delay)
    {
        _source = source;
        _source.SignalChanged += OnSignalChanged;

        _clock = clock;
        _clock.Stepped += OnStep;

        _delay = delay;
        _counter = 0;

        _buffer = new(source.Signal().Length);
    }

    public BitArray Signal() { return new(_buffer); }

    private void OnSignalChanged(object? _, BitArray __)
    {
        if (_counter < _delay) return;

        _counter = 0;
        if (_delay == 0)
        {
            _buffer = _source.Signal();
            SignalChanged?.Invoke(this, _buffer);
        }
    }

    private void OnStep(object? _, EventArgs __)
    {
        if (_counter <= _delay)
        {
            _counter++;
        }

        if (_counter == _delay)
        {
            _buffer = _source.Signal();
            SignalChanged?.Invoke(this, _buffer);
        }
    }
}
