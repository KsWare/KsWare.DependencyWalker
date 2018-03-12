using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.DependencyWalker {

	public static class AssemblyWalker {

		private static List<string> SearchPath=new List<string>();
		private static List<string> Extensions=new List<string>(new []{".dll",".exe"});
		private static bool SearchSubFolder = false;
		private static bool EnableAssemblyResolver = false;

		static AssemblyWalker() {
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.ReflectionOnlyAssemblyResolve += ResolveAssembly;
		}

		public static Assembly ResolveAssembly(object sender, ResolveEventArgs args) {
			if (EnableAssemblyResolver == false) return null;
			foreach (var path in SearchPath) {
				foreach (var ext in Extensions) {
					string assemblyPath = Path.Combine(path, new AssemblyName(args.Name).Name + ext);
					if (File.Exists(assemblyPath)) {
						return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
					}
				}
			}
			return null;
		}

		public static AssemblyInfo LoadAssembly(string fileName) {
			var assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
			return new AssemblyInfo(assembly) {
				AssemblyName = assembly.GetName(false),
				FileName = fileName
			};
		}

		public static AssemblyInfo LoadAssembly(AssemblyName name, string folder) {
			Assembly assembly;
			try {
				assembly = Assembly.ReflectionOnlyLoad(name.FullName);
			}
			catch (System.IO.FileNotFoundException e1) {
				SearchPath.Add(folder);
				EnableAssemblyResolver = true;
				try {
					// assembly = Assembly.ReflectionOnlyLoad(name.FullName); // ReflectionOnlyAssemblyResolve not triggered
					assembly = ResolveAssembly(null, new ResolveEventArgs(name.FullName));
				}
				finally {
					SearchPath.Remove(folder);
					EnableAssemblyResolver = false;
				}
			}
			return new AssemblyInfo(assembly) {
				AssemblyName = name,
				FileName = assembly.Location
			};
		}

		public static void LoadAssembly(AssemblyInfo assemblyInfo, string folder) {
			AssemblyInfo ai;
			if (assemblyInfo.AssemblyName != null) {
				ai = LoadAssembly(assemblyInfo.AssemblyName, folder);
			}
			else if (assemblyInfo.FileName != null) {
				ai = LoadAssembly(assemblyInfo.FileName);
			}
			else {
				return;
			}

			assemblyInfo.Assembly = ai.Assembly;
			assemblyInfo.AssemblyName = ai.AssemblyName;
			assemblyInfo.FileName = ai.FileName;
		}

		public static void LoadDependencies(AssemblyInfo assemblyInfo, bool recursive) {
			assemblyInfo.ReferencedAssemblies = assemblyInfo.Assembly.GetReferencedAssemblies().Select(n => new AssemblyInfo(n)).ToArray();
			foreach (var rai in assemblyInfo.ReferencedAssemblies) {
				LoadAssembly(rai, Path.GetDirectoryName(assemblyInfo.FileName));
			}
			foreach (var rai in assemblyInfo.ReferencedAssemblies) {
				if(Path.GetDirectoryName(assemblyInfo.FileName) != Path.GetDirectoryName(rai.FileName)) continue; // skip recursive load
				LoadDependencies(rai, true);
			}
		}
	}

	public class AssemblyInfo {

		public AssemblyInfo() { }

		public AssemblyInfo(Assembly assembly) {
			Assembly=assembly;
		}

		public AssemblyInfo(AssemblyName assemblyName) {
			AssemblyName=assemblyName;
		}

		public string FileName { get; set; }
		public AssemblyName AssemblyName { get; set; }
		public Assembly Assembly { get; set; }
		public AssemblyInfo[] ReferencedAssemblies { get; set; }
	}

}
