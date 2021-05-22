namespace ILCompiler.z80
{
    public class RomRoutines : IRomRoutines
    {
        private readonly IZ80Assembly _assembly;
        public RomRoutines(IZ80Assembly assembly)
        {
            _assembly = assembly;
        }

        public void Display()
        {
            // Output to current cursor position
            _assembly.Pop(R16.HL);
            _assembly.Ld(R8.A, R8.L);
            _assembly.Call(0x0033);  // ROM routine to display character at current cursor position
        }
    }
}
