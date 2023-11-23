using System.ComponentModel;

namespace ILCompiler.Compiler.EvaluationStack
{
    public static class StackEntryExtensions
    {
        public static bool IsIntCnsOrI(this StackEntry entry)
        {
            if (entry is Int32ConstantEntry) return true;

            // Ignore NativeIntConstants using a symbolname e.g. T1.
            if (entry is NativeIntConstantEntry nativeEntry && nativeEntry.SymbolName == String.Empty) return true;

            return false;

        }

        public static int GetIntConstant(this StackEntry entry)
        {
            if (entry is Int32ConstantEntry int32Entry) return int32Entry.Value;
            if (entry is NativeIntConstantEntry nativeIntEntry) return nativeIntEntry.Value;

            throw new ArgumentException("Cannot get int constant from entry that isn't a Int32 or NativeInt");
        }

        public static bool IsContainedIntOrI(this StackEntry entry) => entry.Contained && entry.IsIntCnsOrI();

        public static bool IsIntegralConstant(this StackEntry entry, int constantValue)
        {
            if (entry is Int32ConstantEntry && entry.As<Int32ConstantEntry>().Value == constantValue)
            {
                return true;
            }
            if (entry is NativeIntConstantEntry && entry.As<NativeIntConstantEntry>().Value == constantValue)
            {
                return true;
            }
            return false;
        }
    }
}
