namespace ILCompiler.Interfaces
{
    public interface IPhaseFactory
    {
        T Create<T>() where T : IPhase;
    }
}
