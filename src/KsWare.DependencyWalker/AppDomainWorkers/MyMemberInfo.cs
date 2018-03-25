using System;
using System.Reflection;
using KsWare.SignatureGenerator;

namespace KsWare.DependencyWalker.AppDomainWorkers {

	public class MyMemberInfo : MarshalByRefObject {

		public MyMemberInfo(MyTypeInfo typeInfo, MemberInfo memberInfo) {
			TypeInfo   = typeInfo;
			MemberInfo = memberInfo;

			SigForCompare = SignatureHelper.ForCompare.Sig(memberInfo);
			SigForCompareIgnoreReturnType = SignatureHelper.ForCompareIgnoreReturnType.Sig(memberInfo);
			SigForForCode = SignatureHelper.ForCode.Sig(memberInfo);
		}

		

		public MyTypeInfo TypeInfo { get; }
		public MemberInfo MemberInfo { get; }
		public string DisplayName { get; set; }

		public string SigForForCode { get;  }

		public string SigForCompareIgnoreReturnType { get; }

		public string SigForCompare { get; }
	}

}