using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.DependencyWalker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace KsWare.DependencyWalker.Tests {
	[TestClass()]
	public class SignatureHelperTests {

		private class MethodModifiers {
			private bool A() => true;
			protected bool B() => true;
//			private internal bool C() => true;
			internal bool D() => true;
			protected internal bool E() => true;
			public bool F() => true;

			private static bool SA() => true;
			protected static bool SB() => true;
//			private internal static bool SC() => true;
			internal static bool SD() => true;
			protected internal static bool SE() => true;
			public static bool SF() => true;

//			private virtual bool VA() => true;
			protected virtual bool VB() => true;
//			private virtual internal bool VC() => true;
			internal virtual bool VD() => true;
			protected internal virtual bool VE() => true;
			public virtual bool VF() => true;
		}

		private class MethodModifiersB: MethodModifiers {
//			private override bool VA() => true;
			protected override bool VB() => true;
//			private internal override bool VC() => true;
			internal override bool VD() => true;
			protected internal override bool VE() => true;
			public override bool VF() => true;
		}

		private class MethodModifiersC : MethodModifiers {
//			private sealed override bool VA() => true;
			protected sealed override bool VB() => true;
//			private internal sealed override bool VC() => true;
			internal sealed override bool VD() => true;
			protected internal sealed override bool VE() => true;
			public sealed override bool VF() => true;
		}

		[DataTestMethod]
		[DataRow(typeof(MethodModifiers), "A", "private bool A()")]
		[DataRow(typeof(MethodModifiers), "B", "protected bool B()")]
//		[-------(typeof(MethodModifiers), "C", "private internal bool C()")]
		[DataRow(typeof(MethodModifiers), "D", "internal bool D()")]
		[DataRow(typeof(MethodModifiers), "E", "protected internal bool E()")]
		[DataRow(typeof(MethodModifiers), "F", "public bool F()")]
		[DataRow(typeof(MethodModifiers), "SA", "private static bool SA()")]
		[DataRow(typeof(MethodModifiers), "SB", "protected static bool SB()")]
//		[-------(typeof(MethodModifiers), "SC", "private internal static bool SC()")]
		[DataRow(typeof(MethodModifiers), "SD", "internal static bool SD()")]
		[DataRow(typeof(MethodModifiers), "SE", "protected internal static bool SE()")]
		[DataRow(typeof(MethodModifiers), "SF", "public static bool SF()")]
//		[-------(typeof(MethodModifiers), "VA", "private virtual bool VA()")]
		[DataRow(typeof(MethodModifiers), "VB", "protected virtual bool VB()")]
//		[-------(typeof(MethodModifiers), "VC", "private internal virtual bool VC()")]
		[DataRow(typeof(MethodModifiers), "VD", "internal virtual bool VD()")]
		[DataRow(typeof(MethodModifiers), "VE", "protected internal virtual bool VE()")]
		[DataRow(typeof(MethodModifiers), "VF", "public virtual bool VF()")]
//		[-------(typeof(MethodModifiersB), "VA", "private override bool VA()")]
		[DataRow(typeof(MethodModifiersB), "VB", "protected override bool VB()")]
//		[-------(typeof(MethodModifiersB), "VC", "private internal override bool VC()")]
		[DataRow(typeof(MethodModifiersB), "VD", "internal override bool VD()")]
		[DataRow(typeof(MethodModifiersB), "VE", "protected internal override bool VE()")]
		[DataRow(typeof(MethodModifiersB), "VF", "public override bool VF()")]
//		[-------(typeof(MethodModifiersC), "VA", "private sealed override bool VA()")]
		[DataRow(typeof(MethodModifiersC), "VB", "protected sealed override bool VB()")]
//		[-------(typeof(MethodModifiersC), "VC", "private internal sealed override bool VC()")]
		[DataRow(typeof(MethodModifiersC), "VD", "internal sealed override bool VD()")]
		[DataRow(typeof(MethodModifiersC), "VE", "protected internal sealed override bool VE()")]
		[DataRow(typeof(MethodModifiersC), "VF", "public sealed override bool VF()")]
		public void SigMethodInfoTest(Type type, string name, string result) {
			var mi=(MethodInfo)type.GetMember(name,BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}


	}

}