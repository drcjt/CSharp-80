using Internal.Runtime;
using System.Diagnostics;

namespace System.Runtime
{
    public struct ExInfo
    {
        internal StackFrameIterator _frameIter;
        internal object _exception;
    }

    internal static unsafe class ExceptionHandling
    {
        private static ushort s_originalThrowSP = 0;

        private enum EHClauseKind : byte
        {
            Typed = 0,
            Fault = 1,
            Filter = 2,
        }
        private struct EHClause
        {
            internal ushort _tryStartOffset;
            internal ushort _tryEndOffset;
            internal byte* _handlerAddress;
            internal byte* _filterAddress;
            internal void* _pTargetType;
            internal byte _kind;

            public bool ContainsCodeOffset(ushort codeOffset)
            {
                return ((codeOffset >= _tryStartOffset) && (codeOffset < _tryEndOffset));
            }
        }

        [RuntimeExport("ThrowException")]
        public static void ThrowException(ref ExInfo exceptionInfo)
        {
            if (s_originalThrowSP != 0)
            {
                exceptionInfo._frameIter.StackPointer = s_originalThrowSP;
            }
            else
            {
                s_originalThrowSP = exceptionInfo._frameIter.StackPointer;
            }

            exceptionInfo._exception ??= new NullReferenceException();

            DispatchException(ref exceptionInfo);
        }

        [RuntimeExport("ThrowDivideByZeroException")]
        public static void ThrowDivideByZeroException()
        {
            throw new DivideByZeroException();
        }

        private static void DispatchException(ref ExInfo exceptionInfo)
        {
            object exceptionObj = exceptionInfo._exception;

            // First pass

            StackFrameIterator stackFrameIterator = exceptionInfo._frameIter;

            ushort handlingFrameSp = 0xFFFF;
            byte* pCatchHandler = null;
            nuint catchingTryRegionIdx = 0xFFFF;

            // enumerate frames
            bool isValid = true;
            for (; isValid; isValid = InternalCalls.SFINext(ref exceptionInfo._frameIter))
            {
                byte* pHandler;
                if (FindFirstPassHandler(exceptionObj, ref exceptionInfo._frameIter, out catchingTryRegionIdx, out pHandler))
                {
                    // Found a handler
                    pCatchHandler = pHandler;
                    handlingFrameSp = exceptionInfo._frameIter.StackPointer;
                    break;
                }
            }

            if (pCatchHandler == null)
            {
                // Treat everything that gets here as unhandled exceptions
                UnhandledExceptionFailFast(ref exceptionInfo);
            }

            // Second pass
            //
            if (CompilerServices.RuntimeHelpers.HasFinallyHandlers)
            {
                isValid = true;
                for (; isValid; isValid = InternalCalls.SFINext(ref stackFrameIterator))
                {
                    if (stackFrameIterator.StackPointer == handlingFrameSp)
                    {
                        // Invoke a partial second pass here
                        InvokeSecondPass(ref stackFrameIterator, catchingTryRegionIdx);
                        break;
                    }
                    InvokeSecondPass(ref stackFrameIterator);
                }
            }

            s_originalThrowSP = 0;

            // Call the handler and resume execution
            InternalCalls.CallCatchHandler(exceptionObj, pCatchHandler, ref exceptionInfo._frameIter);
        }

        private static bool FindFirstPassHandler(object exception, ref StackFrameIterator frameIter, out nuint tryRegionIdx, out byte* pHandler)
        {
            pHandler = null;
            tryRegionIdx = 0xFFFF;

            EHClause nextClause;
            byte* ehEnum;

            InternalCalls.EHEnumInit(&ehEnum);

            for (nuint idx = 0; InternalCalls.EHEnumNext(&ehEnum, &nextClause); idx++)
            {
                if ((nextClause._kind != (byte)EHClauseKind.Typed && nextClause._kind != (byte)EHClauseKind.Filter)
                    || !nextClause.ContainsCodeOffset(frameIter.InstructionPointer))
                {
                    continue;
                }

                // Only handle Typed clauses in the first pass. Filter by type and IP range.
                if (nextClause._kind == (byte)EHClauseKind.Typed)
                {
                    var catchEETypePtr = (EEType*)(nextClause._pTargetType);
                    if (TypeCast.IsInstanceOfClass(catchEETypePtr, exception) != null)
                    {
                        pHandler = nextClause._handlerAddress;
                        tryRegionIdx = idx;
                        return true;
                    }
                }
                else
                {
                    bool shouldInvokeHandler = false;

                    try
                    {
                        shouldInvokeHandler = InternalCalls.CallFilterFunclet(exception, nextClause._filterAddress, frameIter.FramePointer);
                    }
                    catch
                    {
                        // Make sure no exceptions leak out of the filter funclet
                    }

                    if (shouldInvokeHandler)
                    {
                        pHandler = nextClause._handlerAddress;
                        tryRegionIdx = idx;
                        return true;
                    }
                }
            }

            return false;
        }

        private static void InvokeSecondPass(ref StackFrameIterator frameIter, nuint idxLimit = 0xFFFF)
        {
            EHClause nextClause;
            byte* ehEnum;

            InternalCalls.EHEnumInit(&ehEnum);

            for (nuint idx = 0; InternalCalls.EHEnumNext(&ehEnum, &nextClause) && idx < idxLimit; idx++)
            {
                // Only handle Typed clauses in the first pass. Filter by type and IP range.
                if (nextClause._kind == (byte)EHClauseKind.Fault && nextClause.ContainsCodeOffset(frameIter.InstructionPointer))
                {
                    // Call the fault handler. The handler will execute and return here.
                    InternalCalls.CallFinallyFunclet(nextClause._handlerAddress, frameIter.FramePointer);
                }
            }
        }

        private static void UnhandledExceptionFailFast(ref ExInfo exceptionInfo)
        {
            if (exceptionInfo._exception is Exception exceptionObject)
            {
                Console.Write("Unhandled exception. ");

                Console.WriteLine(exceptionObject.Message ?? "");

                Console.Write("IP="); 
                Console.Write(exceptionInfo._frameIter.InstructionPointer);
                Console.Write(",FP=");
                Console.WriteLine(exceptionInfo._frameIter.FramePointer);
            }

            Environment.Exit(-1);
        }
    }
}
