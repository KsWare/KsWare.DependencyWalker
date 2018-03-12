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

		private class Events {
			public event System.EventHandler A;
			public static event System.EventHandler SA;

			public event System.EventHandler B {
				add { }
				remove {}
			}
		}

		[DataTestMethod]
		[DataRow(typeof(Events), "A", "public event System.EventHandler A")]
		[DataRow(typeof(Events), "SA", "public static event System.EventHandler SA")]
		[DataRow(typeof(Events), "B", "public event System.EventHandler B")]
		public void SigEventInfoTest(Type type, string name, string result) {
			var mi = (EventInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		private class Properties {
			public bool A { get; private set; }
			public bool B { get; internal set; }
			public bool C { get; set; }
			internal bool D { get; private set; }
			internal bool E { get; set; }
			private bool F { get; set; }

			public virtual bool VA { get; private set; }
			public virtual bool VB { get; internal set; }
			public virtual bool VC { get; set; }
		}

		[DataTestMethod]
		[DataRow(typeof(Properties), "A", "public bool A { get; private set; }")]
		[DataRow(typeof(Properties), "B", "public bool B { get; internal set; }")]
		[DataRow(typeof(Properties), "C", "public bool C { get; set; }")]
		[DataRow(typeof(Properties), "D", "internal bool D { get; private set; }")]
		[DataRow(typeof(Properties), "E", "internal bool E { get; set; }")]
		[DataRow(typeof(Properties), "F", "private bool F { get; set; }")]
		[DataRow(typeof(Properties), "VA", "public virtual bool VA { get; private set; }")]
		[DataRow(typeof(Properties), "VB", "public virtual bool VB { get; internal set; }")]
		[DataRow(typeof(Properties), "VC", "public virtual bool VC { get; set; }")]
		public void SigPropertyInfoTest(Type type, string name, string result) {
			var mi = (PropertyInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}

		private class Fields {
			public bool A;

			public static bool SA;

			public const bool CA = true;

			public readonly bool RA = true;

			public static readonly bool SRA = true;
		}

		[DataTestMethod]
		[DataRow(typeof(Fields), "A", "public bool A")]
		[DataRow(typeof(Fields), "SA", "public static bool SA")]
		[DataRow(typeof(Fields), "CA", "public const bool CA")]
		[DataRow(typeof(Fields), "RA", "public readonly bool RA")]
		[DataRow(typeof(Fields), "SRA", "public static readonly bool SRA")]
		public void SigFieldInfoTest(Type type, string name, string result) {
			var mi = (FieldInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}
		
		private class Constructors {
			public Constructors() { }

			static Constructors() { }
		}

		[DataTestMethod]
		[DataRow(typeof(Constructors), ".ctor", "public .ctor()")]
		[DataRow(typeof(Constructors), ".cctor", "static .cctor()")]
		public void SigConstructorInfoTest(Type type, string name, string result) {
			var mi = (ConstructorInfo) type.GetMember(name,
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.DeclaredOnly)[0];
			SignatureHelper.ForCompare.Sig(mi).Should().Be(result);
		}
	}

}