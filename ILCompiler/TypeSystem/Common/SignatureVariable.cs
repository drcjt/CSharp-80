﻿using ILCompiler.TypeSystem.Canon;
using System.Diagnostics;

namespace ILCompiler.TypeSystem.Common
{
    public abstract class SignatureVariable : TypeDesc
    {
        private readonly TypeSystemContext _context;
        private readonly int _index;

        internal SignatureVariable(TypeSystemContext context, int index)
        {
            _context = context;
            _index = index;
        }

        public int Index => _index;
        public override TypeSystemContext Context => _context;

        public override bool IsRuntimeDeterminedSubtype => throw new Exception("IsRuntimeDeterminedSubtype of an indefinite type");
    }

    public sealed class SignatureTypeVariable : SignatureVariable
    {
        internal SignatureTypeVariable(TypeSystemContext context, int index) : base(context, index)
        {
        }

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            return typeInstantiation == null ? this : typeInstantiation[Index];
        }

        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind)
        {
            throw new Exception("ConvertToCanonFormImpl for an indefinite type");
        }

        public override bool IsCanonicalSubtype(CanonicalFormKind policy)
        {
            throw new Exception("IsCanonicalSubtype of an indefinite type");
        }
    }

    public sealed class SignatureMethodVariable : SignatureVariable
    {
        internal SignatureMethodVariable(TypeSystemContext context, int index) : base(context, index)
        {
        }

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            return methodInstantiation == null ? this : methodInstantiation[Index];
        }

        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind)
        {
            throw new Exception("ConvertToCanonFormImpl for an indefinite type");
        }

        public override bool IsCanonicalSubtype(CanonicalFormKind policy)
        {
            throw new Exception("IsCanonicalSubtype of an indefinite type");
        }
    }
}
