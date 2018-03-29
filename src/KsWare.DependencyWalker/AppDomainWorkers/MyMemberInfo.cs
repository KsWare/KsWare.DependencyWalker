using System;
using System.Reflection;
using KsWare.CodeGenerator;

namespace KsWare.DependencyWalker.AppDomainWorkers {

	public class MyMemberInfo : MarshalByRefObject {

		public MyMemberInfo(MyTypeInfo typeInfo, MemberInfo memberInfo) {
			TypeInfo   = typeInfo;
			MemberInfo = memberInfo;

			SigForCompare = Generator.ForCompare.Generate(memberInfo);
			SigForCompareIgnoreReturnType = Generator.ForCompareIgnoreReturnType.Generate(memberInfo);
			SigForCode = Generator.ForCode.Generate(memberInfo);
		}

		

		public MyTypeInfo TypeInfo { get; }
		public MemberInfo MemberInfo { get; }
		public string DisplayName { get; set; }

		public string SigForCode { get;  }

		public string SigForCompareIgnoreReturnType { get; }

		public string SigForCompare { get; }
	}

}