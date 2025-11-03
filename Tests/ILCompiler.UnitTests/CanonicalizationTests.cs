using dnlib.DotNet;
using ILCompiler.Compiler;
using ILCompiler.IL;
using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace ILCompiler.UnitTests
{
    internal class CanonicalizationTests
    {
        private ModuleDefMD _testModule;
        private DnlibModule _module;

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

            var corelibPath = Path.Combine(SolutionPath, $@".\System.Private.CoreLib\bin\Trs80\{buildConfigurationName}\net10.0\System.Private.CoreLib.dll");
            ModuleDefMD corlibModule = ModuleDefMD.Load(corelibPath, modCtx);
            ((AssemblyResolver)modCtx.AssemblyResolver).AddToCache(corlibModule);

            var options = new ModuleCreationOptions(modCtx)
            {
                CorLibAssemblyRef = corlibModule.Assembly.ToAssemblyRef()
            };
            string inputFilePath = Path.Combine(SolutionPath, $@".\Tests\CoreTestAssembly\bin\{buildConfigurationName}\net10.0\CoreTestAssembly.dll");
            _testModule = ModuleDefMD.Load(inputFilePath, options);

            var configuration = new Configuration();
            var typeSystemContext = new TypeSystemContext(configuration);
            typeSystemContext.GenericsMode = SharedGenericsMode.CanonicalReferenceTypes;
            var corLibModuleProvider = new CorLibModuleProvider();
            corLibModuleProvider.CorLibModule = corlibModule;
            _module = new DnlibModule(typeSystemContext, corLibModuleProvider, new RTILProvider());

            _referenceType = GetType("Canonicalization", "ReferenceType");
            _otherReferenceType = GetType("Canonicalization", "OtherReferenceType");
            _genericReferenceType = GetType("Canonicalization", "GenericReferenceType`1");
        }

        [Test]
        public void TestGenericTypes()
        {
            var referenceOverReference = MakeInstantiatedType(_genericReferenceType, _referenceType);
            var referenceOverOtherReference = MakeInstantiatedType(_genericReferenceType, _otherReferenceType);

            Assert.That(referenceOverReference.ConvertToCanonForm(CanonicalFormKind.Specific), 
                Is.EqualTo(referenceOverOtherReference.ConvertToCanonForm(CanonicalFormKind.Specific)));

            var referenceOverReferenceOverReference = MakeInstantiatedType(_genericReferenceType, referenceOverReference);

            Assert.That(referenceOverReference.ConvertToCanonForm(CanonicalFormKind.Specific),
                Is.EqualTo(referenceOverReferenceOverReference.ConvertToCanonForm(CanonicalFormKind.Specific)));
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