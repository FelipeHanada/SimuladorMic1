namespace SimuladorApp.Components.MainComponents.Diagram
{
    public enum ComponentShape { Rect, Alu }

    public enum ComponentSide { Left, Right, Top, Bottom }

    public enum PositionAnchor { Start, Center, End }

    public class DiagramDisplayData(
        int x = 0,
        int y = 0,
        int width = 0,
        int height = 0,
        PositionAnchor xAnchor = PositionAnchor.Start,
        PositionAnchor yAnchor = PositionAnchor.Start
        )
    {
        public readonly int X = xAnchor switch
        {
            PositionAnchor.Start => x,
            PositionAnchor.Center => x - width / 2,
            PositionAnchor.End => x - width,
            _ => x
        };
        public readonly int Y = yAnchor switch
        {
            PositionAnchor.Start => y,
            PositionAnchor.Center => y - height / 2,
            PositionAnchor.End => y - height,
            _ => y
        };
        public readonly int Width = width;
        public readonly int Height = height;

        public int Left => X;
        public int Right => X + Width;
        public int Top => Y;
        public int Bottom => Y + Height;
        public int MidX => X + Width / 2;
        public int MidY => Y + Height / 2;

        public DiagramDisplayData(
            (int, int) pos,
            int width,
            int height,
            PositionAnchor xAnchor = PositionAnchor.Start,
            PositionAnchor yAnchor = PositionAnchor.Start
        ) : this(pos.Item1, pos.Item2, width, height, xAnchor, yAnchor)
        {}

        public (int, int) GetConector(ComponentSide side, int index = 0, int subdivision = 2)
        {
            return side switch
            {
                ComponentSide.Left => (X, Y + Height * index / subdivision),
                ComponentSide.Right => (X + Width, Y + Height * index / subdivision),
                ComponentSide.Top => (X + Width * index / subdivision, Y),
                ComponentSide.Bottom => (X + Width * index / subdivision, Y + Height),
                _ => (X, Y),
            };
        }
    }

    public enum DiagramComponentID
    {
        PC, AC, SP, IR, TIR, Zero, POne, MOne, AM, SM, A, B, C, D, E, F,
        MPC,
    }

    public abstract class Registers {
        public readonly static Dictionary<DiagramComponentID, string> Labels = new([
            new(DiagramComponentID.PC, "PC"),
            new(DiagramComponentID.AC, "AC"),
            new(DiagramComponentID.SP, "SP"),
            new(DiagramComponentID.IR, "IR"),
            new(DiagramComponentID.TIR, "TIR"),
            new(DiagramComponentID.Zero, "0"),
            new(DiagramComponentID.POne, "+1"),
            new(DiagramComponentID.MOne, "-1"),
            new(DiagramComponentID.AM, "AM"),
            new(DiagramComponentID.SM, "SM"),
            new(DiagramComponentID.A, "A"),
            new(DiagramComponentID.B, "B"),
            new(DiagramComponentID.C, "C"),
            new(DiagramComponentID.D, "D"),
            new(DiagramComponentID.E, "E"),
            new(DiagramComponentID.F, "F"),
        ]);

        public readonly static List<DiagramComponentID> RegisterIds = [
            DiagramComponentID.PC,
            DiagramComponentID.AC,
            DiagramComponentID.SP,
            DiagramComponentID.IR,
            DiagramComponentID.TIR,
            DiagramComponentID.Zero,
            DiagramComponentID.POne,
            DiagramComponentID.MOne,
            DiagramComponentID.AM,
            DiagramComponentID.AM,
            DiagramComponentID.A,
            DiagramComponentID.B,
            DiagramComponentID.C,
            DiagramComponentID.D,
            DiagramComponentID.E,
            DiagramComponentID.F,
        ];
    }

    public enum BusID
    {
        Clock_MIR,
        Clock_LatchA, Clock_LatchB,
        Clock_MAR,
        Clock_DecodC, Clock_MPC, Clock_Reg, Clock_MBR,
        MIR_Log,
        MIR_Shifter,
        MIR_MAR,
        MIR_AMUX,
        MIR_ALU,
        MIR_MBR,
        MIR_RD,
        MIR_WR,
        MIR_MP,
        MIR_MMUX,
        MIR_DecodA,
        MIR_DecodB,
        MIR_DecodC,
        MIR_ENC,
        MP_MAR, MP_MBR,
        Reg_LatchA, Reg_LatchB,
        LatchA_AMUX,
        LatchB_ALU, LatchB_MAR,
        AMUX_ALU,
        MBR_AMUX,
        ALU_Shifter, ALU_Log,
        Shifter_MBR, Shifter_Reg,

        Log_MMUX,
        MMUX_MPC,

        DecodA_Reg,
        DecodB_Reg,
        DecodC_Reg,

        MPC_ControlStore,
        ControlStore_MIR,
    }
}

/*

AMUX    -48
COND    -40
ALU     -32
SH      -24
MBR     -16

MAR     -8
RD      0
WR      +8

ENC     +16
C       +24
B       +32
A       +40
ADDR    +48
 
 
 
 */
