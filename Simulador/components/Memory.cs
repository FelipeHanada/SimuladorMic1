using Simulador.utils;
using System;
using System.Collections;
using System.Diagnostics.Metrics;

namespace Simulador.components;

public class Memory
{
    private readonly List<BitArray> _cells;
    private readonly int _cellLength;

    protected readonly ISignalSender _inAddress;
    protected readonly ISignalSender _inBuffer;
    protected ISignalSender? _inRD;
    protected ISignalSender? _inWR;
    protected readonly SignalSender _out;

    public ISignalSender Out { get => _out; }

    public readonly string? Name;

    public event EventHandler<int> CellChanged = delegate { };
    public event EventHandler<EventArgs> Resetted = delegate { };

    public Memory(
        int length, int cellLength,
        ISignalSender addressSender,
        ISignalSender inBufferSender,
        ISignalSender? RDSender,
        ISignalSender? WRSender,
        string? name
        )
    {
        _cells = [ ..Enumerable.Range(0, length).Select(_ => new BitArray(cellLength)) ];
        _cellLength = cellLength;

        _inAddress = addressSender;
        _inBuffer = inBufferSender;
        _inRD = RDSender;
        if (_inRD is not null) _inRD.SignalChanged += OnRDSignalChanged;
        _inWR = WRSender;
        if (_inWR is not null) _inWR.SignalChanged += OnWRSignalChanged;
        _out = new(cellLength);

        Name = name;
    }

    public Memory(
        int length, int cellLength,
        ISignalSender addressSender,
        ISignalSender inBufferSender,
        ISignalSender? RDSender,
        ISignalSender? WRSender
        )
        : this(length, cellLength, addressSender, inBufferSender, RDSender, WRSender, null) { }

    public Memory(
        IEnumerable<BitArray> data,
        int cellLength,
        ISignalSender addressSender,
        ISignalSender inBufferSender,
        ISignalSender? RDSender,
        ISignalSender? WRSender
    )
        : this(data.Count(), cellLength, addressSender, inBufferSender, RDSender, WRSender, null)
    {
        int i = 0;
        foreach (var d in data)
        {
            SetCell(i++, d.TrimOrPad(cellLength));
        }
    }

    public BitArray Cell(int cell) => _cells[cell];

    public void SetCell(int cell, BitArray data)
    {
        _cells[cell] = data.TrimOrPad(_cellLength);
        CellChanged?.Invoke(this, cell);
        if (Name is not null) System.Console.WriteLine("MEMORY (" + Name + ") changing cell (" + cell + ") to " + _cells[cell].ToBitString());
    }

    public void Reset()
    {
        foreach (BitArray cell in _cells)
        {
            cell.SetAll(false);
        }
        Resetted?.Invoke(this, EventArgs.Empty);
    }

    public void SetRDSender(ISignalSender RDSender)
    {
        if (_inRD is not null)
            _inRD.SignalChanged -= OnRDSignalChanged!;
        _inRD = RDSender;
        _inRD.SignalChanged += OnRDSignalChanged!;
    }

    public void SetWRSender(ISignalSender WRSender)
    {
        if (_inWR is not null)
            _inWR.SignalChanged -= OnWRSignalChanged!;
        _inWR = WRSender;
        _inWR.SignalChanged += OnWRSignalChanged!;
    }

    virtual protected void OnRDSignalChanged(object? sender, BitArray _)
    {
        if (!_inRD!.Signal().HasAllSet()) return;
        int address = _inAddress.Signal().ToInt32();
        _out.SetData(_cells[address]);
    }

    virtual protected void OnWRSignalChanged(object? sender, BitArray _)
    {
        if (!_inWR!.Signal().HasAllSet()) return;
        int address = _inAddress.Signal().ToInt32();
        SetCell(address, _inBuffer.Signal());
    }
}

public class SlowMemory : Memory
{
    private readonly Clock _clock;
    private readonly int _delayRD;
    private readonly int _delayWR;
    private int _counterRD;
    private int _counterWR;

    public SlowMemory(
        int length, int cellLength,
        Clock clock, int delayRD, int delayWR,
        ISignalSender addressSender,
        ISignalSender inBufferSender,
        ISignalSender? RDSender,
        ISignalSender? WRSender,
        string? name
        )
        : base(length, cellLength, addressSender, inBufferSender, RDSender, WRSender, name)
    {
        _clock = clock;
        _clock.Stepped += OnClockTick;
        _delayRD = delayRD;
        _delayWR = delayWR;
        _counterRD = _delayRD + 1;
        _counterWR = _delayWR + 1;
    }

    override protected void OnRDSignalChanged(object? sender, BitArray _)
    {
        if (!_inRD!.Signal().HasAllSet())
        {
            _counterRD = _delayRD + 1;
            return;
        }

        if (_counterRD < _delayRD) return;
        _counterRD = 0;
    }

    override protected void OnWRSignalChanged(object? sender, BitArray _)
    {
        if (!_inWR!.Signal().HasAllSet())
        {
            _counterWR = _delayWR + 1;
            return;
        }

        if (_counterWR < _delayWR) return;
        _counterWR = 0;
    }

    protected void OnClockTick(object? sender, EventArgs _)
    {
        if (_counterRD <= _delayRD) _counterRD++;
        if (_counterWR <= _delayWR) _counterWR++;

        if (_counterRD == _delayRD)
        {
            int address = _inAddress.Signal().ToInt32();
            _out.SetData(Cell(address));
        }

        if (_counterWR == _delayWR)
        {
            int address = _inAddress.Signal().ToInt32();
            SetCell(address, _inBuffer.Signal());
        }
    }
}
