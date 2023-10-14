using System.Runtime.InteropServices;

namespace System
{
    public partial struct DateTime
    {
        [DllImport(Libraries.Runtime, EntryPoint = "GETDATETIME")]
        private static extern DateTime GetDateTime();

    }
}
