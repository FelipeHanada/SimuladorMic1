using Simulador.components;

namespace Simulador.mic1;

public class ControlUnit
{
    private readonly Clock _clock;
    private readonly MIRegister _mir;
    private readonly FlagsRegister _flags;
    private readonly Register _mpc;
    private readonly ISignalSender _mpcIncrement;
    private readonly Multiplexer _mMux;
    private readonly ControlStore _controlStore;

    public Clock Clock { get { return _clock; } }
    public MIRegister Mir { get { return _mir; } }
    public Register MPc { get { return _mpc; } }

    public ControlUnit(ISignalSender inN, ISignalSender inZ, Clock clock, MIRegister mir)
    {
        _clock = clock;
        _mir = mir;
        _flags = new(inN, inZ, _mir.OutCond);
        _mpc = new(8, "MPC");
        _mpcIncrement = ProcessedSignalSender.Increment(_mpc.Out);
        _mMux = new(8, [_mpcIncrement, _mir.OutADDR], _flags.Out);

        _mpc.SetDataSender(_mMux.Out);
        _mpc.SetControlSender(_clock.Signal(3));

        _controlStore = new(_mpc.Out, new SignalSender(0), _clock.Signal(0), null);
        CtrlStoreTxtSrcFileLoader loader = new("mic1/control_store.txt");
        loader.Load(_controlStore);

        _mir.SetDataSender(_controlStore.Out);
        _mir.SetControlSender(_clock.Signal(0));
    }

    public ControlUnit(ISignalSender inN, ISignalSender inZ)
        : this(inN, inZ, new(4), new()) {}

    public void Reset()
    {
        _clock.Reset();
        _mir.Reset();
        _mpc.Reset();
    }
}
