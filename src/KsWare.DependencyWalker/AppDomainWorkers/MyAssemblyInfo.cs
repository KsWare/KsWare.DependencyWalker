using System;
using System.Reflection;

namespace KsWare.DependencyWalker.AppDomainWorkers {

	public class MyAssemblyInfo : MarshalByRefObject {

		public MyAssemblyInfo() { }

		public MyAssemblyInfo(Assembly assembly) {
			Assembly = assembly;
		}

		public MyAssemblyInfo(AssemblyName assemblyName) {
			AssemblyName = assemblyName;
		}

		public string FileName { get; set; }

		public AssemblyName AssemblyName { get; set; }

		public Assembly Assembly { get; set; }

		public MyAssemblyInfo[] ReferencedAssemblies { get; set; }

		public MyTypeInfo[] Types { get; set; }
	}

}