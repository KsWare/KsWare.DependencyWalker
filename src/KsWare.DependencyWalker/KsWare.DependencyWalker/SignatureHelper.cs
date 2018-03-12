using System;
using System.Reflection;
using System.Text;

namespace KsWare.DependencyWalker {

	// ?? System.Reflection.Emit.SignatureHelper

	public class SignatureHelper {

		private readonly SignatureMode _signatureMode;

		private bool IgnoreParameterName;
		private bool IgnoreReturnType;

		public static SignatureHelper ForCompare = new SignatureHelper(SignatureMode.Compare);
		public static SignatureHelper ForCompareIgnoreReturnType = new SignatureHelper(SignatureMode.CompareIgnoreReturnType);
		public static SignatureHelper ForCode = new SignatureHelper(SignatureMode.Code);
		public static SignatureHelper InheriteDoc = new SignatureHelper(SignatureMode.InheriteDoc);

		private SignatureHelper(SignatureMode signatureMode) { _signatureMode = signatureMode; }

		public string Sig(MethodInfo arg) {
			var sb = new StringBuilder();

			SigModifier(arg, sb);

			if (_signatureMode == SignatureMode.CompareIgnoreReturnType || _signatureMode==SignatureMode.InheriteDoc) { /*skip*/ }
			else sb.Append(Sig(arg.ReturnType) + " ");
			sb.Append(arg.Name);
			sb.Append("(");
			sb.Append(Sig(arg.GetParameters()));
			sb.Append(")");
			return sb.ToString();
		}

		private void SigModifier(MethodBase methodBase, StringBuilder sb) {
			if (_signatureMode != SignatureMode.InheriteDoc) {
				if (methodBase.IsPublic) sb.Append("public ");
				else if (methodBase.IsFamilyOrAssembly) sb.Append("protected internal ");
				else if (methodBase.IsFamilyAndAssembly) sb.Append("protected private ");
				else if (methodBase.IsAssembly) sb.Append("internal ");
				else if (methodBase.IsFamily) sb.Append("protected ");
				else sb.Append("private ");

				if (methodBase.IsStatic   ) sb.Append("static "  );
				if (methodBase.IsAbstract ) sb.Append("abstract ");
				if (methodBase.IsFinal    ) sb.Append("sealed "  );
				if (methodBase.IsVirtual) {
					sb.Append((methodBase.Attributes & MethodAttributes.NewSlot)>0
						? "virtual "
						: "override ");
				}
			}
		}

		public string Sig(ConstructorInfo constructorInfo) {
			var sb = new StringBuilder();
			SigModifier(constructorInfo, sb);

			sb.Append(constructorInfo.Name);
			sb.Append("(");
			sb.Append(Sig(constructorInfo.GetParameters()));
			sb.Append(")");
			return sb.ToString();
		}

		public string Sig(EventInfo eventInfo) {
			var sb = new StringBuilder();

			var mi = eventInfo.AddMethod; // TODO

			SigModifier(mi,sb);
			sb.Append("event ");
			sb.Append($"{eventInfo.EventHandlerType} ");
			sb.Append($"{eventInfo.Name}");

			return sb.ToString();
		}

		public string Sig(FieldInfo fieldInfo) {
			var sb = new StringBuilder();

			if (_signatureMode != SignatureMode.InheriteDoc) {
				if (fieldInfo.IsPublic) sb.Append("public ");
				else if (fieldInfo.IsFamilyOrAssembly) sb.Append("protected internal ");
				else if (fieldInfo.IsFamilyAndAssembly) sb.Append("protected private ");
				else if (fieldInfo.IsAssembly) sb.Append("internal ");
				else if (fieldInfo.IsFamily) sb.Append("protected ");
				else sb.Append("private ");

				if (fieldInfo.IsStatic && !fieldInfo.IsLiteral) sb.Append("static ");
				if (fieldInfo.IsLiteral) sb.Append("const ");
				if (fieldInfo.IsInitOnly) sb.Append("readonly ");
			};

			sb.Append(Sig(fieldInfo.FieldType));
			sb.Append(" ");
			sb.Append(fieldInfo.Name);

			return sb.ToString();
//			return $"field {fieldInfo} // not implemented";
		}

		public string Sig(PropertyInfo propertyInfo) {
			var sb=new StringBuilder();
			var getter = propertyInfo.GetMethod;
			var setter = propertyInfo.SetMethod;

			var mi = getter ?? setter;

			if (mi.IsStatic) sb.Append("static ");
			if (mi.IsFinal) sb.Append("sealed ");
			if (mi.IsAbstract) sb.Append("abstract ");
			if (mi.IsVirtual) sb.Append("virtual ");

			sb.Append(Sig(propertyInfo.PropertyType));
			sb.Append(" ");

			sb.Append(propertyInfo.Name);
			sb.Append(" {");

			if (propertyInfo.CanRead) {
				SigModifier(getter, sb);
				sb.Append("get; ");
			}
			if (propertyInfo.CanWrite) {
				SigModifier(setter, sb);
				sb.Append("set; ");
			}
			sb.Append("}");

			//return $"property {propertyInfo} // not implemented";
			return sb.ToString();
		}

		public string Sig(ParameterInfo[] parameterInfos) {
			if (parameterInfos.Length == 0) return string.Empty;
			var sb = new StringBuilder();
			foreach (var pi in parameterInfos) sb.Append(", " + Sig(pi));
			return sb.ToString(2, sb.Length                   - 2);
		}

		public string Sig(ParameterInfo parameterInfo) {
			var sb = new StringBuilder();
			//Attributes?

			switch (_signatureMode) {
				case SignatureMode.Compare:
				case SignatureMode.CompareIgnoreReturnType:
				case SignatureMode.InheriteDoc:
					sb.Append(Sig(parameterInfo.ParameterType));
					break;
				case SignatureMode.Code:
					sb.Append(Sig(parameterInfo.ParameterType));
					sb.Append(" " + parameterInfo.Name);
					break;
			}
			
			return sb.ToString();
		}

		public string Sig(Type type) {

			if (type.IsGenericType) {
				var sb   = new StringBuilder();
				var gt   = type.GetGenericTypeDefinition();
				var gtfn = gt.FullName.Substring(0, gt.FullName.IndexOf("`"));
				sb.Append(gtfn);
				sb.Append("<");
				sb.Append(Sig(type.GetGenericArguments()));
				sb.Append(">");
				return sb.ToString();
			}

			var fn = type.FullName;
			switch (fn) {
				case "System.Void":    return "void";
				case "System.UInt16":  return "ushort";
				case "System.UInt32":  return "uint";
				case "System.UInt64":  return "ulong";
				case "System.Int16":   return "short";
				case "System.Int32":   return "int";
				case "System.Int64":   return "long";
				case "System.Char":    return "char";
				case "System.String":  return "string";
				case "System.Boolean": return "bool";
				case "System.Byte":    return "byte";
				case "System.SByte":   return "sbyte";
				case "System.Double":  return "double";
				case "System.Single":  return "float";
				case "System.Decimal": return "decimal";
			}
//			if (fn.StartsWith("System.")) return fn.Substring(7);
			return fn;
		}

		public string Sig(Type[] genericArguments) {
			if (genericArguments.Length == 0) return string.Empty;
			var sb = new StringBuilder();
			foreach (var ga in genericArguments) sb.Append(", " + Sig(ga));
			return sb.ToString(2, sb.Length                     - 2);
		}

		public string Sig(MemberInfo memberInfo) {
			switch (memberInfo.MemberType) {
				case MemberTypes.Constructor: return Sig((ConstructorInfo) memberInfo);
				case MemberTypes.Event: return Sig((EventInfo) memberInfo);
				case MemberTypes.Field: return Sig((FieldInfo) memberInfo);
				case MemberTypes.Method: return Sig((MethodInfo)memberInfo);
				case MemberTypes.NestedType: return Sig((TypeInfo) memberInfo);
				case MemberTypes.Property: return Sig((PropertyInfo) memberInfo);
				default: return $"unknown {memberInfo.MemberType}";
			}
		}


		/// <summary>
        /// Return the method signature as a string.
        /// </summary>
        /// <param name="method">The Method</param>
        /// <param name="callable">Return as an callable string(public void a(string b) would return a(b))</param>
        /// <returns>Method signature</returns>
        public static string Sig2(MethodInfo method, bool callable = false)
        {
            var firstParam = true;
            var sigBuilder = new StringBuilder();
            if (callable == false)
            {
                if (method.IsPublic)
                    sigBuilder.Append("public ");
                else if (method.IsPrivate)
                    sigBuilder.Append("private ");
                else if (method.IsAssembly)
                    sigBuilder.Append("internal ");
                if (method.IsFamily)
                    sigBuilder.Append("protected ");
                if (method.IsStatic)
                    sigBuilder.Append("static ");
                sigBuilder.Append(Sig2(method.ReturnType));
                sigBuilder.Append(' ');
            }
            sigBuilder.Append(method.Name);

            // Add method generics
            if(method.IsGenericMethod)
            {
                sigBuilder.Append("<");
                foreach(var g in method.GetGenericArguments())
                {
                    if (firstParam)
                        firstParam = false;
                    else
                        sigBuilder.Append(", ");
                    sigBuilder.Append(Sig2(g));
                }
                sigBuilder.Append(">");
            }
            sigBuilder.Append("(");
            firstParam = true;
            var secondParam = false;
            foreach (var param in method.GetParameters())
            {
                if (firstParam)
                {
                    firstParam = false;
                    if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
                    {
                        if (callable)
                        {
                            secondParam = true;
                            continue;
                        }
                        sigBuilder.Append("this ");
                    }
                }
                else if (secondParam == true)
                    secondParam = false;
                else
                    sigBuilder.Append(", ");
                if (param.ParameterType.IsByRef)
                    sigBuilder.Append("ref ");
                else if (param.IsOut)
                    sigBuilder.Append("out ");
                if (!callable)
                {
                    sigBuilder.Append(Sig2(param.ParameterType));
                    sigBuilder.Append(' ');
                }
                sigBuilder.Append(param.Name);
            }
            sigBuilder.Append(")");
            return sigBuilder.ToString();
        }

		/// <summary>
		/// Get full type name with full namespace names
		/// </summary>
		/// <param name="type">Type. May be generic or nullable</param>
		/// <returns>Full type name, fully qualified namespaces</returns>
		public static string Sig2(Type type) {
			var nullableType = Nullable.GetUnderlyingType(type);
			if (nullableType != null) return nullableType.Name + "?";

			if (!(type.IsGenericType && type.Name.Contains("`")))
				switch (type.Name) {
					case "String":  return "string";
					case "Int32":   return "int";
					case "Decimal": return "decimal";
					case "Object":  return "object";
					case "Void":    return "void";
					default: {
						return string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName;
					}
				}

			var sb = new StringBuilder(type.Name.Substring(0, type.Name.IndexOf('`')));
			sb.Append('<');
			var first = true;
			foreach (var t in type.GetGenericArguments()) {
				if (!first) sb.Append(',');
				sb.Append(Sig2(t));
				first = false;
			}
			sb.Append('>');
			return sb.ToString();
		}

	}

	internal enum SignatureMode {
		Compare,
		Code,
		CompareIgnoreReturnType,
		InheriteDoc
	}

}
