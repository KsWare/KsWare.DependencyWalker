using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KsWare.SignatureGenerator;

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

		public static MyAssemblyInfo LoadAssembly(string fileName) {
			var assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
			return new MyAssemblyInfo(assembly) {
				AssemblyName = assembly.GetName(false),
				FileName = fileName
			};
		}

		public static MyAssemblyInfo LoadAssembly(AssemblyName name, string folder) {
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
			return new MyAssemblyInfo(assembly) {
				AssemblyName = name,
				FileName = assembly.Location
			};
		}

		public static void LoadAssembly(MyAssemblyInfo assemblyInfo, string folder) {
			MyAssemblyInfo ai;
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

		public static void LoadDependencies(MyAssemblyInfo assemblyInfo, bool recursive) {
			assemblyInfo.ReferencedAssemblies = assemblyInfo.Assembly.GetReferencedAssemblies().Select(n => new MyAssemblyInfo(n)).ToArray();
			foreach (var rai in assemblyInfo.ReferencedAssemblies) {
				LoadAssembly(rai, Path.GetDirectoryName(assemblyInfo.FileName));
			}
			foreach (var rai in assemblyInfo.ReferencedAssemblies) {
				if(Path.GetDirectoryName(assemblyInfo.FileName) != Path.GetDirectoryName(rai.FileName)) continue; // skip recursive load
				LoadDependencies(rai, true);
			}
		}

		public static MyTypeInfo[] GetExportedTypes(Assembly assembly, bool includeMembers) {
			var types = assembly.GetExportedTypes().Select(t=> new MyTypeInfo(t)).ToArray();

			if (includeMembers) {
				foreach (var typeInfo in types) {
					typeInfo.DisplayName = SignatureHelper.ForCompare.Sig(typeInfo.Type);
				}

				foreach (var typeInfo in types) {
					var sm = typeInfo.Type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
						.Select(m => new MyMemberInfo(typeInfo, m));
					var im = typeInfo.Type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
						.Select(m => new MyMemberInfo(typeInfo, m));
					typeInfo.Members = sm.Concat(im).ToArray();

					foreach (var memberInfo in typeInfo.Members) {
						memberInfo.DisplayName = SignatureHelper.ForCompare.Sig(memberInfo.MemberInfo);
					}
				}
			}
			return types;
		}
	}

	public class MyTypeInfo {

		public MyTypeInfo(Type type) { Type = type; }

		public Type Type { get; }

		public string FullName => Type.FullName;

		public MyMemberInfo[] Members { get; set; }

		public string DisplayName { get; set; }

	}

	public class MyMemberInfo {

		public MyMemberInfo(MyTypeInfo typeInfo, MemberInfo memberInfo) {
			TypeInfo = typeInfo;
			MemberInfo = memberInfo;
		}

		public MyTypeInfo TypeInfo { get; }
		public MemberInfo MemberInfo { get; }
		public string DisplayName { get; set; }
	}

	public class MyAssemblyInfo {

		public MyAssemblyInfo() { }

		public MyAssemblyInfo(Assembly assembly) {
			Assembly=assembly;
		}

		public MyAssemblyInfo(AssemblyName assemblyName) {
			AssemblyName=assemblyName;
		}

		public string FileName { get; set; }

		public AssemblyName AssemblyName { get; set; }

		public Assembly Assembly { get; set; }

		public MyAssemblyInfo[] ReferencedAssemblies { get; set; }

		public MyTypeInfo[] Types { get; set; }
	}

	

}
