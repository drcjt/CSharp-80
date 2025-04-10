using dnlib.DotNet;
using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.TypeSystem.RuntimeDetermined;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace ILCompiler.UnitTests
{
    internal class RuntimeDeterminedTypesTests
    {
        private ModuleDefMD _testModule;
        private DnlibModule _module;
        private TypeSystemContext _typeSystemContext;

        private MetadataType _referenceType;
        private MetadataType _otherReferenceType;
        private MetadataType _genericReferenceType;

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..");

        [SetUp]
        public void Setup()
        {
            ModuleContext modCtx = ModuleDef.CreateModuleContext();

            var currentType = MethodBase.GetCurrentMethod().DeclaringType;
            var assemblyConfigurationAttribute = currentType.Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;

            var corelibPath = Path.Combine(SolutionPath, $@".\System.Private.CoreLib\bin\Trs80\{buildConfigurationName}\net9.0\System.Private.CoreLib.dll");
            ModuleDefMD corlibModule = ModuleDefMD.Load(corelibPath, modCtx);
            ((AssemblyResolver)modCtx.AssemblyResolver).AddToCache(corlibModule);

            var options = new ModuleCreationOptions(modCtx)
            {
                CorLibAssemblyRef = corlibModule.Assembly.ToAssemblyRef()
            };
            string inputFilePath = Path.Combine(SolutionPath, $@".\Tests\CoreTestAssembly\bin\{buildConfigurationName}\net9.0\CoreTestAssembly.dll");
            _testModule = ModuleDefMD.Load(inputFilePath, options);

            _typeSystemContext = new TypeSystemContext(new Configuration());
            _typeSystemContext.GenericsMode = SharedGenericsMode.CanonicalReferenceTypes;
            var corLibModuleProvider = new CorLibModuleProvider();
            corLibModuleProvider.CorLibModule = corlibModule;
            _module = new DnlibModule(_typeSystemContext, corLibModuleProvider, null);

            _referenceType = GetType("Canonicalization", "ReferenceType");
            _otherReferenceType = GetType("Canonicalization", "OtherReferenceType");
            _genericReferenceType = GetType("Canonicalization", "GenericReferenceType`1");
        }

        [Test]
        public void SharedRuntimeDeterminedForm_OfGenericReferenceTypeInstantiatedWithDifferentReferenceTypes_AreTheSame()
        {
            var genericReferenceTypeOverReferenceType = MakeInstantiatedType(_genericReferenceType,_referenceType);
            var genericReferenceTypeOverOtherReferenceType = MakeInstantiatedType(_genericReferenceType, _otherReferenceType);
            var genericReferenceTypeOverReferenceTypeShared = genericReferenceTypeOverReferenceType.ConvertToSharedRuntimeDeterminedForm();
            var genericReferenceTypeOverOtherReferenceTypeShared = genericReferenceTypeOverOtherReferenceType.ConvertToSharedRuntimeDeterminedForm();

            Assert.That(genericReferenceTypeOverReferenceTypeShared, Is.EqualTo(genericReferenceTypeOverOtherReferenceTypeShared));
        }

        [Test]
        public void InstantiationArgument_OfGenericReferenceTypeInstantiatedWithReferenceTypes_IsRuntimeDeterminedType()
        {
            var genericReferenceTypeOverReferenceType = MakeInstantiatedType(_genericReferenceType, _referenceType);
            var genericReferenceTypeOverReferenceTypeShared = genericReferenceTypeOverReferenceType.ConvertToSharedRuntimeDeterminedForm();

            var typeArgument = genericReferenceTypeOverReferenceTypeShared.Instantiation[0];
            Assert.That(typeArgument, Is.TypeOf<RuntimeDeterminedType>());
        }

        [Test]
        public void CanonicalTypeOfSharedRuntimeForm_OfGenericReferenceTypeInstantiatedWithReferenceTypes_IsCanonType()
        {
            var genericReferenceTypeOverReferenceType = MakeInstantiatedType(_genericReferenceType, _referenceType);
            var genericReferenceTypeOverReferenceTypeShared = genericReferenceTypeOverReferenceType.ConvertToSharedRuntimeDeterminedForm();

            var typeArgument = genericReferenceTypeOverReferenceTypeShared.Instantiation[0];
            var runtimeDeterminedType = (RuntimeDeterminedType)typeArgument;
            Assert.That(runtimeDeterminedType.CanonicalType, Is.EqualTo(_typeSystemContext.CanonType));
        }

        [Test]
        public void SharedRuntimeFormDetailsType_OfGenericReferenceTypeInstantiatedWithReferenceTypes_IsTFromGenericDefinition()
        {
            var genericReferenceTypeOverReferenceType = MakeInstantiatedType(_genericReferenceType, _referenceType);
            var genericReferenceTypeOverReferenceTypeShared = genericReferenceTypeOverReferenceType.ConvertToSharedRuntimeDeterminedForm();

            var typeArgument = genericReferenceTypeOverReferenceTypeShared.Instantiation[0];
            var runtimeDeterminedType = (RuntimeDeterminedType)typeArgument;
            Assert.That(runtimeDeterminedType.RuntimeDeterminedDetailsType, Is.EqualTo(_genericReferenceType.Instantiation[0]));
        }

        [Test]
        public void CanonicalForm_OfGenericReferenceTypeOverReferenceTypeShared_IsSameAsCanonicalFormOfGenericReferenceTypeOverReferenceType()
        {
            var genericReferenceTypeOverReferenceType = MakeInstantiatedType(_genericReferenceType, _referenceType);
            var genericReferenceTypeOverOtherReferenceType = MakeInstantiatedType(_genericReferenceType, _otherReferenceType);
            var genericReferenceTypeOverReferenceTypeShared = genericReferenceTypeOverReferenceType.ConvertToSharedRuntimeDeterminedForm();
            var genericReferenceTypeOverOtherReferenceTypeShared = genericReferenceTypeOverOtherReferenceType.ConvertToSharedRuntimeDeterminedForm();

            Assert.That(genericReferenceTypeOverReferenceTypeShared.ConvertToCanonForm(CanonicalFormKind.Specific),
                Is.EqualTo(genericReferenceTypeOverReferenceType.ConvertToCanonForm(CanonicalFormKind.Specific)));
        }

        public static InstantiatedType MakeInstantiatedType(MetadataType typeDef, params TypeDesc[] genericParameters)
        {
            return typeDef.Context.GetInstantiatedType(typeDef, new Instantiation(genericParameters));
        }

        public MetadataType GetType(string nameSpace, string name)
        {
            var typeDef = _testModule.Find($"{nameSpace}.{name}", false);
            return (MetadataType)_module.Create(typeDef);
        }
    }
}
