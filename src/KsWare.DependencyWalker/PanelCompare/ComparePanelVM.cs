using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using KsWare.DependencyWalker.AppDomainWorkers;
using KsWare.Presentation.ViewModelFramework;
using KsWare.SignatureGenerator;

namespace KsWare.DependencyWalker.PanelCompare {

	public class ComparePanelVM : ObjectVM {

		public ComparePanelVM() {
			RegisterChildren(() => this);

			SelectorA.SelectedDirectory =
				@"D:\Develop\Extern\GitHub.KsWare\KsWare.Presentation\master\src\KsWare.Presentation\bin\Debug";
			SelectorB.SelectedDirectory = @"C:\Users\KayS\Downloads\KsWare.Presentation.0.18.11\lib\net45";
		}

		public SelectorVM SelectorA { get; [UsedImplicitly] private set; }

		public SelectorVM SelectorB { get; [UsedImplicitly] private set; }

		/// <summary>
		/// Gets the <see cref="ActionVM"/> to Compare
		/// </summary>
		/// <seealso cref="DoCompare"/>
		public ActionVM CompareAction { get; [UsedImplicitly] private set; }

		public List<ICompareResult> Items { get => Fields.GetValue<List<ICompareResult>>(); set => Fields.SetValue(value); }

		/// <summary>
		/// Method for <see cref="CompareAction"/>
		/// </summary>
		[UsedImplicitly]
		private void DoCompare() {
			if (SelectorA.SelectedType != null) {
				Items = CompareType(SelectorA.SelectedType, SelectorB.SelectedType).Cast<ICompareResult>().ToList();
			}
			else if (SelectorA.SelectedAssembly != null) {
				Items = CompareAssembly(SelectorA.SelectedAssembly, SelectorB.SelectedAssembly).Cast<ICompareResult>().ToList();
			}
			else if (SelectorA.SelectedDirectory != null) {
				Items = CompareFolder(SelectorA.SelectedDirectory, SelectorB.SelectedDirectory).Cast<ICompareResult>().ToList();
			}
		}

		private List<AssemblyCompareResult> CompareFolder(string directoryA, string directoryB) {
			if (!directoryA.EndsWith("\\")) directoryA += "\\";
			if (!directoryB.EndsWith("\\")) directoryB += "\\";
			var filesA = Directory.GetFiles(directoryA)
				.Where(n => Path.GetExtension(n)?.ToLower() == ".dll" || Path.GetExtension(n)?.ToLower() == ".exe")
				.Select(f => f.Substring(directoryA.Length)).ToArray();
			var filesB = Directory.GetFiles(directoryB)
				.Where(n => Path.GetExtension(n)?.ToLower() == ".dll" || Path.GetExtension(n)?.ToLower() == ".exe")
				.Select(f => f.Substring(directoryB.Length)).ToArray();

			var all    = filesA.Concat(filesB).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
			var result = new List<AssemblyCompareResult>();
			foreach (var f in all) {
				var ra = filesA.Contains(f, StringComparer.OrdinalIgnoreCase);
				var rb = filesB.Contains(f, StringComparer.OrdinalIgnoreCase);
				if (ra  && rb) result.Add(new AssemblyCompareResult(f,  Result.None));
				if (ra  && !rb) result.Add(new AssemblyCompareResult(f, Result.OnlyLeft));
				if (!ra && rb) result.Add(new AssemblyCompareResult(f,  Result.OnlyRight));
			}

			foreach (var c in result.Where(r => r.Result == Result.None)) {
				c.AssemblyA = SelectorA.AssemblyWalker.LoadAssembly(directoryA + c.Name);
				SelectorA.AssemblyWalker.LoadDependencies(c.AssemblyA);
				SelectorA.AssemblyWalker.UpdateExportedTypes(c.AssemblyA, true);

				c.AssemblyB = SelectorB.AssemblyWalker.LoadAssembly(directoryB + c.Name);
				SelectorB.AssemblyWalker.LoadDependencies(c.AssemblyB);
				SelectorB.AssemblyWalker.UpdateExportedTypes(c.AssemblyB, true);

				c.SubResults = CompareAssembly(c.AssemblyA, c.AssemblyB);
				c.Result = c.SubResults.All(r => r.Result == Result.Equal) ? Result.Equal : Result.Different;
			}

			return result;
		}

		private TypeCompareResult[] CompareAssembly(MyAssemblyInfo assemblyA, MyAssemblyInfo assemblyB) {
			var all = assemblyA.Types.Select(t => t.FullName).Concat(assemblyB.Types.Select(t => t.FullName)).Distinct();

			var result = new List<TypeCompareResult>();
			foreach (var f in all) {
				var ra = assemblyA.Types.Any(t => t.FullName == f);
				var rb = assemblyB.Types.Any(t => t.FullName == f);
				if (ra  && rb) result.Add(new TypeCompareResult(f,  Result.None));
				if (ra  && !rb) result.Add(new TypeCompareResult(f, Result.OnlyLeft));
				if (!ra && rb) result.Add(new TypeCompareResult(f,  Result.OnlyRight));
			}

			foreach (var c in result.Where(r => r.Result == Result.None)) {
				c.TypeA = assemblyA.Types.First(t => t.FullName == c.Name);
				c.TypeB = assemblyB.Types.First(t => t.FullName == c.Name);
				c.SubResults = CompareType(c.TypeA, c.TypeB).ToArray();
				c.Result = c.SubResults.All(r => r.Result == Result.Equal) ? Result.Equal : Result.Different;
			}

			return result.ToArray();
		}

		private List<MemberCompareResult> CompareType(MyTypeInfo typeA, MyTypeInfo typeB) {
			var all=typeA.Members.Select(m => m.SigForCompareIgnoreReturnType)
				.Concat(typeB.Members.Select(m => m.SigForCompareIgnoreReturnType));

			var result = new List<MemberCompareResult>();
			foreach (var s in all) {
				var ra = typeA.Members.Any(m => m.SigForCompareIgnoreReturnType == s);
				var rb = typeB.Members.Any(m => m.SigForCompareIgnoreReturnType == s);
				if (ra  && rb) result.Add(new MemberCompareResult(s,  Result.Equal));
				if (ra  && !rb) result.Add(new MemberCompareResult(s, Result.OnlyLeft));
				if (!ra && rb) result.Add(new MemberCompareResult(s,  Result.OnlyRight));
			}

			return result;
		}

	}

}