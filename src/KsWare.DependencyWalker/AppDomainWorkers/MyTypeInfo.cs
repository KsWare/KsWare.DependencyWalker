using System;
using KsWare.CodeGenerator;

namespace KsWare.DependencyWalker {

	public class MyTypeInfo : MarshalByRefObject {

		public MyTypeInfo(Type type) {
			Type = type;
			FullName = type.FullName;
			DisplayName = Generator.ForCompare.Generate(type);
		}

		public Type Type { get; }

		public string FullName { get;  }

		public MyMemberInfo[] Members { get; set; }

		public string DisplayName { get; set; }

	}

}