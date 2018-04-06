using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using KsWare.CodeGenerator;
using KsWare.CodeGenerator.Extensions;

namespace KsWare.DependencyWalker {

	public class AssemblyWalker : MarshalByRefObject {

		private static int LastAppDomainNumber;
		private static readonly Dictionary<string,AssemblyWalker> _assemblyWalkers=new Dictionary<string, AssemblyWalker>(StringComparer.OrdinalIgnoreCase);
		private static AppDomain _defaultAppDomain;

		[SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
		[SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
		[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
		public static AssemblyWalker GetInstance(string baseFolder) {
			if (baseFolder == null) throw new ArgumentNullException(nameof(baseFolder));
			if(!Directory.Exists(baseFolder)) throw new ArgumentException(@"Directory not found!",nameof(baseFolder));
			if (_defaultAppDomain == null) _defaultAppDomain = AppDomain.CurrentDomain;
			else if(AppDomain.CurrentDomain != _defaultAppDomain) throw new InvalidOperationException("Only default AppDomain should call this method.");

			baseFolder = baseFolder.EndsWith("\\") ? baseFolder : baseFolder + "\\";
			if(_assemblyWalkers.TryGetValue(baseFolder, out var value)) return value;

			var ads = new AppDomainSetup {
				ApplicationBase          = AppDomain.CurrentDomain.BaseDirectory,
				DisallowBindingRedirects = false,
				DisallowCodeDownload     = true,
				ConfigurationFile        = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
			};
			LastAppDomainNumber++;
			var ad = AppDomain.CreateDomain($"AD #{LastAppDomainNumber}", null, ads);

			var proxy = (AssemblyWalker) ad.CreateInstanceAndUnwrap(
				assemblyName: Assembly.GetExecutingAssembly().FullName, 
				typeName: typeof(AssemblyWalker).FullName,  
				ignoreCase: false, 
				bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance, 
				binder:null,
				args: new object[] {
					baseFolder
				}, 
				culture: CultureInfo.CurrentCulture,  
				activationAttributes: null	
			);
			_assemblyWalkers.Add(baseFolder,proxy);
			return proxy;
		}

		private readonly List<string> _searchPath=new List<string>();
		private readonly List<string> _extensions=new List<string>(new []{".dll",".exe"});
		private readonly string _baseFolder;

		private AssemblyWalker(string baseFolder) {
			_baseFolder = baseFolder;
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.ReflectionOnlyAssemblyResolve += AtResolveAssembly;
			_searchPath.Add(baseFolder);
		}

		private Assembly AtResolveAssembly(object sender, ResolveEventArgs args) {
			foreach (var path in _searchPath) {
				foreach (var ext in _extensions) {
					string assemblyPath = Path.Combine(path, new AssemblyName(args.Name).Name + ext);
					if (File.Exists(assemblyPath)) {
						return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
					}
				}
			}
			return Assembly.ReflectionOnlyLoad(args.Name);
		}

		public MyAssemblyInfo LoadAssembly(string fileName) {
			var assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
			return new MyAssemblyInfo(assembly) {
				AssemblyName = assembly.GetName(false),
				FileName = fileName
			};
		}

		public MyAssemblyInfo LoadAssembly(AssemblyName name) {
			Assembly assembly;

			try {
				// assembly = Assembly.ReflectionOnlyLoad(name.FullName); // ReflectionOnlyAssemblyResolve not triggered
				assembly = AtResolveAssembly(null, new ResolveEventArgs(name.FullName));
			}
			finally {

			}
			
			return new MyAssemblyInfo(assembly) {
				AssemblyName = name,
				FileName = assembly.Location
			};
		}

		public void LoadAssembly(MyAssemblyInfo assemblyInfo) {
			MyAssemblyInfo ai;
			if (assemblyInfo.AssemblyName != null) {
				ai = LoadAssembly(assemblyInfo.AssemblyName);
			}
			else if (assemblyInfo.FileName != null) {
				ai = LoadAssembly(assemblyInfo.FileName);
			}
			else {
				return;
			}

			assemblyInfo.Assembly     = ai.Assembly;
			assemblyInfo.AssemblyName = ai.AssemblyName;
			assemblyInfo.FileName     = ai.FileName;
		}

		public void LoadDependencies(MyAssemblyInfo assemblyInfo) {
			assemblyInfo.ReferencedAssemblies = assemblyInfo.Assembly.GetReferencedAssemblies().Select(n => new MyAssemblyInfo(n)).ToArray();
			foreach (var rai in assemblyInfo.ReferencedAssemblies) {
				LoadAssembly(rai);
			}
			foreach (var rai in assemblyInfo.ReferencedAssemblies) {
				if(Path.GetDirectoryName(assemblyInfo.FileName) != Path.GetDirectoryName(rai.FileName)) continue; // skip recursive load
				LoadDependencies(rai);
			}
		}

		public MyTypeInfo[] GetExportedTypes(Assembly assembly, bool includeMembers) {
			var types = assembly.GetExportedTypes().Select(t=> new MyTypeInfo(t)).ToArray();

			if (includeMembers) {
				foreach (var typeInfo in types) {
					var all = typeInfo.Type.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
						.Select(m => new MyMemberInfo(typeInfo, m));
					typeInfo.Members = all.Where(MemberFilter).ToArray();

					foreach (var memberInfo in typeInfo.Members) {
						memberInfo.DisplayName = Generator.ForCompare.Generate(memberInfo.MemberInfo);
					}
				}
			}
			return types;
		}

		private bool MemberFilter(MyMemberInfo m) {
			switch (m.MemberInfo.MemberType) {
				case MemberTypes.Constructor:
				case MemberTypes.Event:
				case MemberTypes.Field:
				case MemberTypes.Property: return true;
				case MemberTypes.Method: return !((MethodInfo) m.MemberInfo).IsAccessor();
				case MemberTypes.TypeInfo:
				case MemberTypes.NestedType:
				case MemberTypes.Custom:
				default: return false;
			}
		}

		public void UpdateExportedTypes(MyAssemblyInfo assemblyInfo, bool includeMembers) {
			assemblyInfo.Types = GetExportedTypes(assemblyInfo.Assembly, includeMembers);
		}
	}


}
