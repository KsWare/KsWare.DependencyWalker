using KsWare.DependencyWalker.AppDomainWorkers;

namespace KsWare.DependencyWalker.PanelCompare {

	public class AssemblyCompareResult : CompareResult {

		public AssemblyCompareResult(string name, Result result) : base(name, result) { }

		public MyAssemblyInfo AssemblyA { get => Fields.GetValue<MyAssemblyInfo>(); set => Fields.SetValue(value); }
		public MyAssemblyInfo AssemblyB { get => Fields.GetValue<MyAssemblyInfo>(); set => Fields.SetValue(value); }
	}

}