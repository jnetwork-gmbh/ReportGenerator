﻿using System;
using System.IO;
using System.Linq;
using DotNetConfig;
using Palmmedia.ReportGenerator.Core.Logging;
using Xunit;

namespace Palmmedia.ReportGenerator.Core.Test
{
    /// <summary>
    /// This is a test class for ReportConfigurationBuilder and is intended
    /// to contain all ReportConfigurationBuilder Unit Tests
    /// </summary>
    [Collection("FileManager")]
    public class ReportConfigurationBuilderTest : IDisposable
    {
        private static readonly string ReportPath = Path.Combine(FileManager.GetCSharpReportDirectory(), "OpenCover.xml");

        private ReportConfigurationBuilder reportConfigurationBuilder;

        private string currentDir;

        public ReportConfigurationBuilderTest()
        {
            this.reportConfigurationBuilder = new ReportConfigurationBuilder();
            this.currentDir = Directory.GetCurrentDirectory();
        }

        public void Dispose() => Directory.SetCurrentDirectory(this.currentDir);

        [Fact]
        public void InitWithNamedArguments_OldFilters_AllPropertiesApplied()
        {
            string[] namedArguments = new string[]
            {
                "-reports:" + ReportPath,
                "-targetdir:C:\\temp",
                "-reporttype:Latex",
                "-filters:+Test;-Test",
                "-verbosity:" + VerbosityLevel.Info.ToString()
            };

            var configuration = this.reportConfigurationBuilder.Create(namedArguments);

            Assert.True(configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.True(configuration.ReportTypes.Contains("Latex"), "Wrong report type applied.");
            Assert.True(configuration.AssemblyFilters.Contains("+Test"), "AssemblyFilters does not exist in ReportFiles.");
            Assert.True(configuration.AssemblyFilters.Contains("-Test"), "AssemblyFilters does not exist in ReportFiles.");
            Assert.NotNull(configuration.ReportFiles);
            Assert.NotNull(configuration.AssemblyFilters);
            Assert.NotNull(configuration.ClassFilters);
        }

        [Fact]
        public void InitWithNamedArguments_NewFilters_AllPropertiesApplied()
        {
            string[] namedArguments = new string[]
            {
                "-reports:" + ReportPath,
                "-targetdir:C:\\temp",
                "-reporttype:Latex",
                "-assemblyfilters:+Test;-Test",
                "-classfilters:+Test2;-Test2",
                "-verbosity:" + VerbosityLevel.Info.ToString()
            };

            var configuration = this.reportConfigurationBuilder.Create(namedArguments);

            Assert.True(configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.Equal("C:\\temp", configuration.TargetDirectory);
            Assert.True(configuration.ReportTypes.Contains("Latex"), "Wrong report type applied.");
            Assert.True(configuration.AssemblyFilters.Contains("+Test"), "AssemblyFilters does not exist in ReportFiles.");
            Assert.True(configuration.ClassFilters.Contains("+Test2"), "ClassFilters does not exist in ReportFiles.");
            Assert.True(configuration.AssemblyFilters.Contains("-Test"), "AssemblyFilters does not exist in ReportFiles.");
            Assert.True(configuration.ClassFilters.Contains("-Test2"), "ClassFilters does not exist in ReportFiles.");
            Assert.NotNull(configuration.ReportFiles);
            Assert.NotNull(configuration.AssemblyFilters);
            Assert.NotNull(configuration.ClassFilters);
        }


        [Fact]
        public void ConfigProvidesMissingArguments()
        {
            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);
            var config = Config.Build(dir).GetSection(ReportConfigurationBuilder.SectionName);

            config.SetString("reports", ReportPath);
            config.SetString("targetdir", "C:\\temp");
            config.SetString("reporttype", "Latex");
            config.SetString("assemblyfilters", "+Test;-Test");
            config.SetString("classfilters", "+Test2;-Test2");
            config.SetString("verbosity", VerbosityLevel.Info.ToString());

            Directory.SetCurrentDirectory(dir);

            string[] namedArguments = new string[0];

            var configuration = this.reportConfigurationBuilder.Create(namedArguments);

            Assert.True(configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.Equal("C:\\temp", configuration.TargetDirectory);
            Assert.True(configuration.ReportTypes.Contains("Latex"), "Wrong report type applied.");
            Assert.True(configuration.AssemblyFilters.Contains("+Test"), "AssemblyFilters does not exist in ReportFiles.");
            Assert.True(configuration.ClassFilters.Contains("+Test2"), "ClassFilters does not exist in ReportFiles.");
            Assert.True(configuration.AssemblyFilters.Contains("-Test"), "AssemblyFilters does not exist in ReportFiles.");
            Assert.True(configuration.ClassFilters.Contains("-Test2"), "ClassFilters does not exist in ReportFiles.");
            Assert.NotNull(configuration.ReportFiles);
            Assert.NotNull(configuration.AssemblyFilters);
            Assert.NotNull(configuration.ClassFilters);
        }

        [Fact]
        public void ConfigProvidesMultiValuedSettings()
        {
            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);
            var config = Config.Build(dir).GetSection(ReportConfigurationBuilder.SectionName);

            config.SetString("reports", ReportPath);

            config.AddString("reporttype", "Latex");
            config.AddString("reporttype", "Html");

            config.AddString("assemblyfilter", "+Test");
            config.AddString("assemblyfilter", "-Test");

            config.AddString("classfilter", "+Test2");
            config.AddString("classfilter", "-Test2");

            config.AddString("filefilter", "+cs");
            config.AddString("filefilter", "-vb");

            config.AddString("sourcedir", "src");
            config.AddString("sourcedir", "test");

            config.AddString("plugin", "xunit");
            config.AddString("plugin", "moq");

            Directory.SetCurrentDirectory(dir);

            string[] namedArguments = new string[0];

            var configuration = this.reportConfigurationBuilder.Create(namedArguments);

            Assert.Contains(ReportPath, configuration.ReportFiles);

            Assert.Contains("Latex", configuration.ReportTypes);
            Assert.Contains("Html", configuration.ReportTypes);

            Assert.Contains("+Test", configuration.AssemblyFilters);
            Assert.Contains("-Test", configuration.AssemblyFilters);

            Assert.Contains("+Test2", configuration.ClassFilters);
            Assert.Contains("-Test2", configuration.ClassFilters);

            Assert.Contains("+cs", configuration.FileFilters);
            Assert.Contains("-vb", configuration.FileFilters);

            Assert.Contains("src", configuration.SourceDirectories);
            Assert.Contains("test", configuration.SourceDirectories);

            Assert.Contains("xunit", configuration.Plugins);
            Assert.Contains("moq", configuration.Plugins);
        }
    }
}
