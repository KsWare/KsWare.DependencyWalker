using System;

namespace KsWare.DependencyWalker.AppDomainWorkers {

	public class MyTypeInfo : MarshalByRefObject {

		public MyTypeInfo(Type type) { Type = type; }

		public Type Type { get; }

		public string FullName => Type.FullName;

		public MyMemberInfo[] Members { get; set; }

		public string DisplayName { get; set; }

	}

}