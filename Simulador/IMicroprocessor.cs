namespace Simulador;

internal interface IMicroprocessor
{
    public void NextCycle();
    public void NextMacroCycle();
    public void NextMicroCycle();
}
