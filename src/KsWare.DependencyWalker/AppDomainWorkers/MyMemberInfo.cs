using System;
using System.Reflection;
using KsWare.CodeGenerator;

namespace KsWare.DependencyWalker {

	public class MyMemberInfo : MarshalByRefObject {

		public MyMemberInfo(MyTypeInfo typeInfo, MemberInfo memberInfo) {
			TypeInfo   = typeInfo;
			MemberInfo = memberInfo;

			Signature = Generator.ForSignature.Generate(memberInfo);
			DeclareCode = Generator.ForDeclare.Generate(memberInfo);
			Documentation = Generator.ForInheriteDoc.Generate(memberInfo);

			DisplayName = Documentation;
		}

		

		public MyTypeInfo TypeInfo { get; }
		public MemberInfo MemberInfo { get; }
		public string DisplayName { get; set; }


		public string Signature { get; }

		public string DeclareCode { get; }

		public string Documentation { get; }
	}

}