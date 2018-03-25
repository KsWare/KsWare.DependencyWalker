using KsWare.DependencyWalker.AppDomainWorkers;

namespace KsWare.DependencyWalker.PanelCompare {

	public class MemberCompareResult : CompareResult {

		public MemberCompareResult(string name, Result result) : base(name, result) { }

		public MyMemberInfo AssemblyA { get => Fields.GetValue<MyMemberInfo>(); set => Fields.SetValue(value); }
		public MyMemberInfo AssemblyB { get => Fields.GetValue<MyMemberInfo>(); set => Fields.SetValue(value); }
	}

}