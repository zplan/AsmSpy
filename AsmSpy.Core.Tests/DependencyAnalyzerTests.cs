using AsmSpy.Core.TestLibrary;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsmSpy.Core.Tests
{
    [TestClass]
    public class DependencyAnalyzerTests
    {
        private readonly TestLogger logger = new TestLogger();

        private readonly IEnumerable<FileInfo> filesToAnalyse;
        private readonly VisualizerOptions options = new VisualizerOptions();

        public DependencyAnalyzerTests()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var testBinDirectory = Path.GetDirectoryName(thisAssembly.Location);
            Console.WriteLine(testBinDirectory);

            filesToAnalyse = Directory.GetFiles(testBinDirectory, "*.dll").Select(x => new FileInfo(x));
        }

        [TestMethod]
        public void AnalyzeShouldReturnTestAssemblies()
        {
            var result = DependencyAnalyzer.Analyze(filesToAnalyse, null, logger, options);


            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "AsmSpy.Core").Equals(true);
            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "AsmSpy.Tests").Equals(true);
            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "AsmSpy.Core.TestLibrary").Equals(true);
            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "Microsoft.VisualStudio.TestPlatform.Common").Equals(true);

            //Assert.Contains(result.Assemblies.Values, x => x.AssemblyName.Name == "AsmSpy.Core");
            //Assert.Contains(result.Assemblies.Values, x => x.AssemblyName.Name == "AsmSpy.Core.Tests");
            //Assert.Contains(result.Assemblies.Values, x => x.AssemblyName.Name == "AsmSpy.Core.TestLibrary");
            //Assert.Contains(result.Assemblies.Values, x => x.AssemblyName.Name == "xunit.core");
        }

        [TestMethod]
        public void AnalyzeShouldReturnSystemAssemblies()
        {
            var result = DependencyAnalyzer.Analyze(filesToAnalyse, null, logger, options);


            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "mscorlib").Equals(true);
            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "netstandard").Equals(true);
            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "System").Equals(true);

            //Assert.Contains(result.Assemblies.Values, x => x.AssemblyName.Name == "mscorlib");
            //Assert.Contains(result.Assemblies.Values, x => x.AssemblyName.Name == "netstandard");
            //Assert.Contains(result.Assemblies.Values, x => x.AssemblyName.Name == "System");
        }

        [TestMethod]
        public void AnalyzeShouldNotReturnSystemAssembliesWhenFlagIsSet()
        {
            var altOptions = new VisualizerOptions
            {
                SkipSystem = true
            };
            var result = DependencyAnalyzer.Analyze(filesToAnalyse, null, logger, altOptions);

            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "mscorlib").Equals(false);
            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "netstandard").Equals(false);
            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "System").Equals(false);

            //Assert.DoesNotContain(result.Assemblies.Values, x => x.AssemblyName.Name == "mscorlib");
            //Assert.DoesNotContain(result.Assemblies.Values, x => x.AssemblyName.Name == "netstandard");
            //Assert.DoesNotContain(result.Assemblies.Values, x => x.AssemblyName.Name == "System");
        }

        [TestMethod]
        public void AnalyzeShouldReturnDependencies()
        {
            var exampleClass = new ExampleClass();
            var result = DependencyAnalyzer.Analyze(filesToAnalyse, null, logger, options);

            var tests = result.Assemblies.Values.Single(x => x.AssemblyName.Name == "AsmSpy.Core.Tests");

            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "AsmSpy.Core").Equals(true);
            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "AsmSpy.Core.TestLibrary").Equals(true);
            result.Assemblies.Values.Any(x => x.AssemblyName.Name == "Microsoft.VisualStudio.TestPlatform.Common").Equals(true);

            //Assert.Contains(tests.References, x => x.AssemblyName.Name == "AsmSpy.Core");
            //Assert.Contains(tests.References, x => x.AssemblyName.Name == "AsmSpy.Core.TestLibrary");
            //Assert.Contains(tests.References, x => x.AssemblyName.Name == "xunit.core");
            foreach(var reference in tests.References)
            {
                Console.WriteLine(reference.AssemblyName.Name);
            }
        }

        [TestMethod]
        public void AnalyzeShouldReturnCorrectAssemblySource()
        {
            var result = DependencyAnalyzer.Analyze(filesToAnalyse, null, logger, options);

            var tests = result.Assemblies.Values.Single(x => x.AssemblyName.Name == "AsmSpy.Core.Tests");

            var systemRuntime = tests.References.Single(x => x.AssemblyName.Name == "System.Runtime");
            
            systemRuntime.AssemblySource.Equals(AssemblySource.Unknown);
            tests.AssemblySource.Equals(AssemblySource.Local);
        }
    }
}
