using Simulador.mic1;

namespace SimuladorApp.Components.Store;

public class Mic1Store
{
    private static Mic1Store? _instance;
    public static Mic1Store Instance
    {
        get
        {
            _instance ??= new Mic1Store();
            return _instance;
        }
    }

    private Mic1 _mic1;

    public Mic1 Mic1 { get { return _mic1; } }

    private Mic1Store()
    {
        _mic1 = new();
    }
}
