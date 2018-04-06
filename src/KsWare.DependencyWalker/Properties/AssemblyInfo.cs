using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: AssemblyTitle("KsWare.DependencyWalker")]
[assembly: AssemblyDescription("A package and assembly dependency walker.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("KsWare")]
[assembly: AssemblyProduct("DependencyWalker")]
[assembly: AssemblyCopyright("Copyright © 2018 by KsWare. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: ThemeInfo(ResourceDictionaryLocation.None,ResourceDictionaryLocation.SourceAssembly)]
[assembly: AssemblyVersion("0.1.6")]
[assembly: AssemblyFileVersion("0.1.6")]
[assembly: AssemblyInformationalVersion("0.1.6")]

// ReSharper disable once CheckNamespace
namespace KsWare.DependencyWalker {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

	}
}
