using Simulador.components;
using Simulador.utils;
using System.Collections;

namespace Simulador.mic1;

public class Mic1
{
    private readonly Clock _clock;
    private readonly MIRegister _mir;
    private readonly ControlUnit _controlUnit;
    private readonly Register[] _registers;
    private readonly Multiplexer _a;
    private readonly Multiplexer _b;
    private readonly ISignalSender _c;
    private readonly Latch _latchA;
    private readonly Latch _latchB;
    private readonly Alu _alu;
    private readonly Shifter _shifter;
    private readonly Memory _memory;
    private readonly Register _mbrRD;
    private readonly Register _mbrWR;
    private readonly Register _mar;
    private readonly Multiplexer _aMux;

    public Clock Clock { get { return _clock; } }
    public MIRegister Mir { get { return _mir; } }
    public ControlUnit ControlUnit { get { return _controlUnit; } }
    public Register[] Registers { get { return _registers; } }
    public Latch LatchA { get { return _latchA; } }
    public Latch LatchB { get { return _latchB; } }
    public Alu Alu { get { return _alu; } }
    public Shifter Shifter { get { return _shifter; } }
    public Memory MP { get { return _memory; } }
    public Register MbrRD { get { return _mbrRD; } }
    public Register MbrWR { get { return _mbrWR; } }
    public Register Mar { get { return _mar; } }

    public event EventHandler<EventArgs> Resetted = delegate { };

    public Mic1()
    {
        _clock = new(4);
        _mir = new();
        _mir.SetControlSender(_clock.Signal(0));

        _registers = [
            new(16, "PC"), new(16, "AC"), new(16, "SP"), new(16, "IR"),
            new(16, "TIR"), new(16, "ZERO"), new(16, "PLUS1"), new(16, "MINUS1"),
            new(16, "AMASk"), new(16, "SMASK"), new(16, "A"), new(16, "B"),
            new(16, "C"), new(16, "D"), new(16, "E"), new(16, "F"),
        ];
        _registers[2].SetData(BitArrayExtensions.FromBitString("0001000000000000"));
        _registers[6].SetData(BitArrayExtensions.FromBitString("0000000000000001"));
        _registers[7].SetData(BitArrayExtensions.FromBitString("1111111111111111"));
        _registers[8].SetData(BitArrayExtensions.FromBitString("0000111111111111"));
        _registers[9].SetData(BitArrayExtensions.FromBitString("0000000011111111"));

        ISignalSender[] registersOut = [.. _registers.Select(r => r.Out)];
        _a = new(16, registersOut, _mir.OutA);
        _b = new(16, registersOut, _mir.OutB);
        _c = ProcessedSignalSender.Decoder4to16(_mir.OutC);

        _latchA = new(_a.Out, _clock.Signal(1));
        _latchB = new(_b.Out, _clock.Signal(1));

        _mbrRD = new(16, "MBR_RD");
        _aMux = new(16, [_latchA.Out, _mbrRD.Out], _mir.OutAMux);

        _alu = new(_aMux.Out, _latchB.Out, _mir.OutAlu);
        _shifter = new(_alu.Out, _mir.OutShifter);

        for (int i = 0; i < registersOut.Length; i++)
        {
            _registers[i].SetDataSender(_shifter.Out);
            _registers[i].SetControlSender(CombinationalSignalSender.And([
                ProcessedSignalSender.Interval(_c, i, 1),
                _mir.OutENC,
                _clock.Signal(3)
                ]));
        }

        _mbrWR = new(16, _shifter.Out, CombinationalSignalSender.And([_mir.OutMBR, _mir.OutWR, _clock.Signal(3)]), "MBR_WR");
        _mar = new(16, _latchB.Out, _clock.Signal(2), "MAR");

        //_memory = new(4096, 16,
        //    new ProcessedSignalSender(_mar.Out, (BitArray data) => { return data.TrimOrPad(8); }), _mbrWR.Out,
        //    _mir.OutRD,
        //    CombinationalSignalSender.And([_mir.OutWR, _clock.Signal(3)]),
        //    "MP"
        //    );

        _memory = new SlowMemory(4096, 16, _clock, 6, 6,
            new ProcessedSignalSender(_mar.Out, (BitArray data) => { return data.TrimOrPad(12); }), _mbrWR.Out,
            _mir.OutRD,
            _mir.OutWR,
            //CombinationalSignalSender.And([_mir.OutWR, _clock.Signal(3)]),
            "MP"
            );

        _mbrRD.SetDataSender(_memory.Out);
        _mbrRD.SetControlSender(CombinationalSignalSender.And([new ClockDelayedSignalSender(_mir.OutRD, _clock, 0), _clock.Signal(3)]));

        _controlUnit = new(_alu.OutN, _alu.OutZ, _clock, _mir);
    }

    public void Reset()
    {
        foreach (var register in _registers) register.Reset();
        _registers[2].SetData(BitArrayExtensions.FromBitString("0001000000000000"));
        _registers[6].SetData(BitArrayExtensions.FromBitString("0000000000000001"));
        _registers[7].SetData(BitArrayExtensions.FromBitString("1111111111111111"));
        _registers[8].SetData(BitArrayExtensions.FromBitString("0000111111111111"));
        _registers[9].SetData(BitArrayExtensions.FromBitString("0000000011111111"));

        _controlUnit.Reset();
        _latchA.Reset();
        _latchB.Reset();
        _memory.Reset();
        _mbrRD.Reset();
        _mbrWR.Reset();
        _mar.Reset();

        Resetted?.Invoke(this, EventArgs.Empty);
    }

    public void StepCycle()
    {
        _clock.Step();
    }

    public void StepMicro()
    {
        if (_clock.CurrentCycle() < 0) StepCycle();
        StepCycle();
        while (_clock.CurrentCycle() > 0)
            _clock.Step();
    }

    public void StepMacro()
    {
        StepMicro();
        while (_controlUnit.MPc.Out.Signal().HasAnySet())
        {
            StepMicro();
        }
    }
}
