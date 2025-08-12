namespace System.Diagnostics
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = false)]
    public sealed class DebuggableAttribute : Attribute
    {
        [Flags]
        public enum DebuggingModes
        {
            None = 0x0,
            Default = 0x1,
            DisableOptimizations = 0x100,
            IgnoreSymbolStoreSequencePoints = 0x2,
            EnableEditAndContinue = 0x4
        }

        public DebuggableAttribute(bool isJITTrackingEnabled, bool isJITOptimizerDisabled)
        {
            DebuggingFlags = 0;

            if (isJITTrackingEnabled)
            {
                DebuggingFlags |= DebuggingModes.Default;
            }

            if (isJITOptimizerDisabled)
            {
                DebuggingFlags |= DebuggingModes.DisableOptimizations;
            }
        }

        public DebuggableAttribute(DebuggingModes modes)
        {
            DebuggingFlags = modes;
        }

        public bool IsJITTrackingEnabled => (DebuggingFlags & DebuggingModes.Default) != 0;

        public bool IsJITOptimizerDisabled => (DebuggingFlags & DebuggingModes.DisableOptimizations) != 0;

        public DebuggingModes DebuggingFlags { get; }

    }
}
