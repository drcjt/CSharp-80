namespace ILCompiler.z80.Interfaces
{
    public interface IOptimizer
    {
        public void Optimize(IZ80Assembly assembly);
    }
}
