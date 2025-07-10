using Simulador.components;

namespace Simulador.mic1;

public class MIRegister : Register
{
    public ISignalSender OutAMux { get; }
    public ISignalSender OutCond { get; }
    public ISignalSender OutAlu { get; }
    public ISignalSender OutShifter { get; }
    public ISignalSender OutMBR { get; }
    public ISignalSender OutMAR { get; }
    public ISignalSender OutRD { get; }
    public ISignalSender OutWR { get; }
    public ISignalSender OutENC { get; }
    public ISignalSender OutC { get; }
    public ISignalSender OutB { get; }
    public ISignalSender OutA { get; }
    public ISignalSender OutADDR { get; }

    public MIRegister() : base(32)
    {
        OutAMux    = ProcessedSignalSender.Interval(this.Out, 31, 1);
        OutCond    = ProcessedSignalSender.Interval(this.Out, 29, 2);
        OutAlu     = ProcessedSignalSender.Interval(this.Out, 27, 2);
        OutShifter = ProcessedSignalSender.Interval(this.Out, 25, 2);
        OutMBR     = ProcessedSignalSender.Interval(this.Out, 24, 1);
        OutMAR     = ProcessedSignalSender.Interval(this.Out, 23, 1);
        OutRD      = ProcessedSignalSender.Interval(this.Out, 22, 1);
        OutWR      = ProcessedSignalSender.Interval(this.Out, 21, 1);
        OutENC     = ProcessedSignalSender.Interval(this.Out, 20, 1);
        OutC       = ProcessedSignalSender.Interval(this.Out, 16, 4);
        OutB       = ProcessedSignalSender.Interval(this.Out, 12, 4);
        OutA       = ProcessedSignalSender.Interval(this.Out, 8, 4);
        OutADDR = ProcessedSignalSender.Interval(this.Out, 0, 8);
    }
}
