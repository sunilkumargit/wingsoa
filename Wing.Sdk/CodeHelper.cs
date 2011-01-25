//
// Marcelo Dezem
// Code Generation Helper.
// O codigo deste arquivo é uma copia identica do codigo em c:\deseulance2009\directframework\common\CodeHelper.cs
// reproduzi o codigo aqui para nao ser necessario registar a DLL Direct.Core.dll como publica.
//
using System;
using System.CodeDom;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class CodeHelper
    {
        public static Boolean GenerateDebuggerInfo { get; set; }

        public static String UniqueName(String identifier)
        {
            return String.Format("{0}{1}{2}", identifier, RandomHelper.Next(1000, 10000000).ToString("X"), MathHelper.Seq("X"));
        }

        public static String SafeName(String name)
        {
            return StringHelper.FilterChars(name ?? UniqueName("auto").Replace(".", "_").Replace("\\", "__").Replace("/", "__"));
        }

        public static TTYPE ConstructMember<TTYPE>() where TTYPE : CodeTypeMember
        {
            CodeTypeMember result = (TTYPE)typeof(TTYPE).GetConstructor(Type.EmptyTypes).Invoke(new Object[0]);
            if (!(result is CodeMemberProperty))
            {
                if (!GenerateDebuggerInfo)
                    result.CustomAttributes.Add(NonDebuggerAttribute());
            }
            return (TTYPE)result;
        }

        public static CodeMethodInvokeExpression InvokeMethodBase(String methodName, params CodeExpression[] parameters)
        {
            return InvokeMethod(new CodeBaseReferenceExpression(), methodName, parameters);
        }

        public static CodeMethodInvokeExpression InvokeMethodBase(CodeMemberMethod method)
        {
            CodeExpression[] parameters = new CodeExpression[method.Parameters.Count];
            for (int i = 0; i < method.Parameters.Count; i++)
                parameters[i] = VarRef(method.Parameters[i].Name);
            return InvokeMethodBase(method.Name, parameters);
        }

        public static CodeMemberMethod Method(String methodName, MemberAttributes attributes)
        {
            CodeMemberMethod method = ConstructMember<CodeMemberMethod>();
            method.Name = methodName;
            method.Attributes = attributes;
            return method;
        }

        public static CodeMemberMethod Method(String methodName, MemberAttributes attributes, CodeTypeReference resultType)
        {
            CodeMemberMethod method = ConstructMember<CodeMemberMethod>();
            method.Name = methodName;
            method.Attributes = attributes;
            method.ReturnType = resultType;
            return method;
        }

        public static CodeMemberMethod Method(String methodName, MemberAttributes attributes, Type resultType)
        {
            return Method(methodName, attributes, TypeRef(resultType));
        }

        public static CodeTypeReference TypeRef<TTYPE>()
        {
            return TypeRef(typeof(TTYPE));
        }

        public static CodeTypeDeclaration TypeDecl(String typeName)
        {
            return new CodeTypeDeclaration(typeName);
        }

        public static CodeTypeReference TypeRef(String typeName)
        {
            return new CodeTypeReference(typeName);
        }

        public static CodeTypeReferenceExpression TypeRefExp(String typeName)
        {
            return TypeRefExp(TypeRef(typeName));
        }

        public static CodeTypeReferenceExpression TypeRefExp(Type type)
        {
            return TypeRefExp(TypeRef(type));
        }

        public static CodeTypeReferenceExpression TypeRefExp(CodeTypeReference typeRef)
        {
            return new CodeTypeReferenceExpression(typeRef);
        }

        public static CodeTypeReferenceExpression TypeRefExp<TTYPE>()
        {
            return TypeRefExp(typeof(TTYPE));
        }

        public static CodeTypeReference TypeRef(Type type)
        {
            return new CodeTypeReference(type);
        }

        public static CodeAttributeDeclaration NonDebuggerAttribute()
        {
            return new CodeAttributeDeclaration(
                new CodeTypeReference(typeof(System.Diagnostics.DebuggerNonUserCodeAttribute)));
        }

        public static CodeAttributeDeclaration Attr(String attrTypeName, params CodeAttributeArgument[] args)
        {
            return Attr(TypeRef(attrTypeName), args);
        }

        public static CodeAttributeDeclaration Attr(CodeTypeReference typeRef, params CodeAttributeArgument[] args)
        {
            return new CodeAttributeDeclaration(typeRef, args);
        }

        public static CodeAttributeDeclaration Attr(Type type, params CodeAttributeArgument[] args)
        {
            return Attr(TypeRef(type), args);
        }

        public static CodeAttributeDeclaration Attr<TTYPE>() where TTYPE : Attribute
        {
            return Attr(typeof(TTYPE));
        }

        public static CodeAttributeArgument AttrArg(String argName, CodeExpression argValue)
        {
            return new CodeAttributeArgument(argName, argValue);
        }

        public static CodeAttributeArgument AttrTypedArg(String argName, String argValue)
        {
            return AttrSnippetArg(String.Format("\"{0} = {{1}}\"", argName, argValue));
        }

        public static CodeAttributeArgument AttrArg(CodeExpression argValue)
        {
            return new CodeAttributeArgument(argValue);
        }

        public static CodeAttributeArgument AttrTypeOfArg(String typeName)
        {
            return AttrTypeOfArg(TypeRef(typeName));
        }

        public static CodeAttributeArgument AttrTypeOfArg(Type type)
        {
            return AttrTypeOfArg(TypeRef(type));
        }

        public static CodeAttributeArgument AttrTypeOfArg(CodeTypeReference typeRef)
        {
            return AttrArg(TypeOf(typeRef));
        }

        public static CodeAttributeArgument AttrEnumArg<TEnumType>(String argName, TEnumType argValue)
        {
            return AttrArg(EnumSnippet<TEnumType>(argValue));
        }

        public static CodeAttributeArgument AttrPrimArg(String argName, Object argValue)
        {
            return AttrArg(argName, Primitive(argValue));
        }

        public static CodeAttributeArgument AttrPrimArg(Object argValue)
        {
            return AttrArg(Primitive(argValue));
        }

        public static CodeAttributeArgument AttrArrayArg(CodeTypeReference typeRef, params CodeExpression[] initializers)
        {
            return AttrArg(Array(typeRef, initializers));
        }

        public static CodeAttributeArgument AttrArrayArg(String typeName, params CodeExpression[] initializers)
        {
            return AttrArrayArg(TypeRef(typeName), initializers);
        }

        public static CodeAttributeArgument AttrArrayArg(Type type, params CodeExpression[] initializers)
        {
            return AttrArrayArg(TypeRef(type), initializers);
        }

        public static CodeAttributeArgument AttrSnippetArg(String expr)
        {
            return AttrArg(SnippetExp(expr));
        }

        public static CodeFieldReferenceExpression ThisFieldRef(String fieldName)
        {
            return FieldRef(new CodeThisReferenceExpression(), fieldName);
        }

        public static CodeIndexerExpression ThisIndexedFieldRef(String fieldName, CodeExpression indexer)
        {
            return new CodeIndexerExpression(CodeHelper.ThisFieldRef(fieldName), new CodeExpression[] { indexer });
        }

        public static CodeVariableReferenceExpression VarRef(String varName)
        {
            return new CodeVariableReferenceExpression(varName);
        }

        public static MemberAttributes ProtectedOverride
        {
            get { return MemberAttributes.Family | MemberAttributes.Override; }
        }

        public static CodeMethodReturnStatement ReturnNull()
        {
            return Return(Null);
        }

        public static CodeMethodReturnStatement ReturnThisField(String fieldName)
        {
            return Return(ThisFieldRef(fieldName));
        }

        public static CodeMethodReturnStatement ReturnField(String fieldName)
        {
            return Return(FieldRef(fieldName));
        }

        public static CodeSnippetExpression SnippetExp(String expr)
        {
            return new CodeSnippetExpression(expr);
        }

        public static CodeVariableDeclarationStatement VarDeclInitPrim(String typeName, String varName, Object primitiveValue)
        {
            return VarDeclInitPrim(TypeRef(typeName), varName, primitiveValue);
        }

        public static CodeVariableDeclarationStatement VarDeclInitPrim(Type type, String varName, Object primitiveValue)
        {
            return VarDeclInitPrim(TypeRef(type), varName, primitiveValue);
        }

        public static CodeVariableDeclarationStatement VarDeclInitPrim(CodeTypeReference typeRef, String varName, Object primitiveValue)
        {
            return VarDeclInit(typeRef, varName, Primitive(primitiveValue));
        }

        public static CodeVariableDeclarationStatement VarDeclInit(String typeName, String varName, CodeExpression initExpr)
        {
            return VarDeclInit(TypeRef(typeName), varName, initExpr);
        }

        public static CodeVariableDeclarationStatement VarDeclInit(Type type, String varName, CodeExpression initExpr)
        {
            return VarDeclInit(TypeRef(type), varName, initExpr);
        }

        public static CodeVariableDeclarationStatement VarDeclInit(CodeTypeReference typeRef, String varName, CodeExpression initExpr)
        {
            CodeVariableDeclarationStatement result = VarDecl(typeRef, varName);
            result.InitExpression = initExpr;
            return result;
        }

        public static CodeVariableDeclarationStatement VarDecl(String typeName, String varName)
        {
            return VarDecl(TypeRef(typeName), varName);
        }

        public static CodeVariableDeclarationStatement VarDecl(Type type, String varName)
        {
            return VarDecl(TypeRef(type), varName);
        }

        public static CodeVariableDeclarationStatement VarDecl(CodeTypeReference typeRef, String varName)
        {
            return new CodeVariableDeclarationStatement(typeRef, varName);
        }

        public static CodePrimitiveExpression Primitive(Object value)
        {
            return new CodePrimitiveExpression(value);
        }

        public static CodeSnippetStatement SnippetStat(String stat)
        {
            return new CodeSnippetStatement(stat);
        }

        public static CodeFieldReferenceExpression FieldRef(CodeExpression targetObject, String fieldName)
        {
            return new CodeFieldReferenceExpression(targetObject, fieldName);
        }

        public static CodeFieldReferenceExpression FieldRef(String fieldName)
        {
            return FieldRef(null, fieldName);
        }

        public static CodeFieldReferenceExpression StaticFieldRef(String typeName, String fieldName)
        {
            return StaticFieldRef(TypeRefExp(typeName), fieldName);
        }

        public static CodeFieldReferenceExpression StaticFieldRef(Type type, String fieldName)
        {
            return StaticFieldRef(TypeRefExp(type), fieldName);
        }

        public static CodeFieldReferenceExpression StaticFieldRef(CodeTypeReferenceExpression typeRefExp, String fieldName)
        {
            return FieldRef(typeRefExp, fieldName);
        }

        public static CodeAssignStatement Assign(CodeExpression left, CodeExpression right)
        {
            return new CodeAssignStatement(left, right);
        }

        public static CodeMemberField Field(String typeName, String fieldName, MemberAttributes attributes)
        {
            return Field(TypeRef(typeName), fieldName, attributes);
        }

        public static CodeMemberField Field(CodeTypeReference typeRef, String fieldName, MemberAttributes attributes)
        {
            CodeMemberField result = Field(typeRef, fieldName);
            result.Attributes = attributes;
            return result;
        }

        public static CodeMemberField Field(Type type, String fieldName, MemberAttributes attributes)
        {
            return Field(TypeRef(type), fieldName, attributes);
        }

        public static CodeMemberField Field(String typeName, String fieldName)
        {
            return Field(TypeRef(typeName), fieldName);
        }

        public static CodeMemberField Field(CodeTypeReference typeRef, String fieldName)
        {
            return new CodeMemberField(typeRef, fieldName);
        }

        public static CodeMemberField Field(Type type, String fieldName)
        {
            return Field(TypeRef(type), fieldName);
        }

        public static MemberAttributes PrivateStatic
        {
            get { return MemberAttributes.Private | MemberAttributes.Static; }
        }

        public static CodeMemberProperty Property(String typeName, String propertyName, MemberAttributes attributes)
        {
            return Property(TypeRef(typeName), propertyName, attributes);
        }

        public static CodeMemberProperty Property(CodeTypeReference typeRef, String propertyName, MemberAttributes attributes)
        {
            CodeMemberProperty result = Property(typeRef, propertyName);
            result.Attributes = attributes;
            return result;
        }

        public static CodeMemberProperty Property(Type type, String propertyName, MemberAttributes attributes)
        {
            return Property(TypeRef(type), propertyName, attributes);
        }

        public static CodeMemberProperty Property(String typeName, String propertyName)
        {
            return Property(TypeRef(typeName), propertyName);
        }

        public static CodeMemberProperty Property(CodeTypeReference typeRef, String propertyName)
        {
            CodeMemberProperty prop = ConstructMember<CodeMemberProperty>();
            prop.Name = propertyName;
            prop.Type = typeRef;
            return prop;
        }

        public static CodeMemberProperty Property(Type type, String propertyName)
        {
            return Property(TypeRef(type), propertyName);
        }

        public static MemberAttributes PublicStatic
        {
            get { return MemberAttributes.Public | MemberAttributes.Static; }
        }

        public static MemberAttributes PublicOverride
        {
            get { return MemberAttributes.Public | MemberAttributes.Override; }
        }

        public static CodeAssignStatement AssignCreateObject(CodeExpression left, String typeName, params CodeExpression[] parameters)
        {
            return AssignCreateObject(left, TypeRef(typeName), parameters);
        }

        public static CodeAssignStatement AssignCreateObject(CodeExpression left, Type type, params CodeExpression[] parameters)
        {
            return AssignCreateObject(left, TypeRef(type), parameters);
        }

        public static CodeAssignStatement AssignCreateObject(CodeExpression left, CodeTypeReference typeRef, params CodeExpression[] parameters)
        {
            return Assign(left, CreateObject(typeRef, parameters));
        }

        public static CodeObjectCreateExpression CreateObject(String typeName, params CodeExpression[] parameters)
        {
            return CreateObject(TypeRef(typeName), parameters);
        }

        public static CodeObjectCreateExpression CreateObject(Type type, params CodeExpression[] parameters)
        {
            return CreateObject(TypeRef(type), parameters);
        }

        public static CodeObjectCreateExpression CreateObject(CodeTypeReference typeRef, params CodeExpression[] parameters)
        {
            return new CodeObjectCreateExpression(typeRef, parameters);
        }

        public static CodeAssignStatement AssignFieldPrim(CodeExpression targetObject, String fieldName, Object primitive)
        {
            return AssignField(targetObject, fieldName, Primitive(primitive));
        }

        public static CodeAssignStatement AssignField(String fieldName, CodeExpression right)
        {
            return AssignField(null, fieldName, right);
        }

        public static CodeAssignStatement AssignField(CodeExpression targetObject, String fieldName, CodeExpression right)
        {
            return Assign(FieldRef(targetObject, fieldName), right);
        }

        public static CodeAssignStatement AssignThisField(String fieldName, CodeExpression right)
        {
            return Assign(ThisFieldRef(fieldName), right);
        }

        public static CodeAssignStatement AssignFieldSnippet(String snippetExprTargetObject, String fieldName, CodeExpression right)
        {
            return AssignField(SnippetExp(snippetExprTargetObject), fieldName, right);
        }

        public static CodeAssignStatement AssignFieldSnippetPrim(String snippetExprTargetObject, String fieldName, Object primitiveValue)
        {
            return AssignFieldSnippet(snippetExprTargetObject, fieldName, Primitive(primitiveValue));
        }

        public static CodeAssignStatement AssignFieldSnippet(String snippetExprTargetObject, String fieldName, String rightSnippetExpr)
        {
            return AssignFieldSnippet(snippetExprTargetObject, fieldName, SnippetExp(rightSnippetExpr));
        }

        public static CodeAssignStatement AssignThisFieldPrim(String fieldName, Object primitive)
        {
            return AssignThisField(fieldName, Primitive(primitive));
        }

        public static CodeMethodReturnStatement ReturnSnippetExp(String expr)
        {
            return Return(SnippetExp(expr));
        }

        public static CodeMethodReturnStatement Return(CodeExpression expr)
        {
            return new CodeMethodReturnStatement(expr);
        }

        public static CodeMethodReturnStatement ReturnPrimitive(Object value)
        {
            return Return(Primitive(value));
        }

        public static CodeMethodInvokeExpression InvokeMethod(String snippetExprTargetObject, String methodName, params CodeExpression[] parameters)
        {
            return InvokeMethod(SnippetExp(snippetExprTargetObject), methodName, parameters);
        }

        public static CodeMethodInvokeExpression InvokeMethod(CodeExpression targetObject, String methodName, params CodeExpression[] parameters)
        {
            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject, methodName), parameters);
        }

        public static CodeMethodInvokeExpression InvokeMethod(String methodName, params CodeExpression[] parameters)
        {
            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, methodName), parameters);
        }

        public static CodeMethodInvokeExpression InvokeThisMethod(String methodName, params CodeExpression[] parameters)
        {
            return InvokeMethod(This, methodName, parameters);
        }

        public static CodeCastExpression Cast(CodeTypeReference typeRef, CodeExpression expression)
        {
            return new CodeCastExpression(typeRef, expression);
        }

        public static CodeCastExpression Cast(Type type, CodeExpression expression)
        {
            return Cast(TypeRef(type), expression);
        }

        public static CodeCastExpression Cast(String typeName, CodeExpression expression)
        {
            return Cast(TypeRef(typeName), expression);
        }

        public static CodeCastExpression CastField(CodeTypeReference typeRef, CodeExpression targetObject, String fieldName)
        {
            return Cast(typeRef, FieldRef(targetObject, fieldName));
        }

        public static CodeCastExpression CastField(Type type, CodeExpression targetObject, String fieldName)
        {
            return CastField(TypeRef(type), targetObject, fieldName);
        }

        public static CodeCastExpression CastField(String typeName, CodeExpression targetObject, String fieldName)
        {
            return CastField(TypeRef(typeName), targetObject, fieldName);
        }

        public static CodeCastExpression CastThisField(CodeTypeReference typeRef, String fieldName)
        {
            return Cast(typeRef, ThisFieldRef(fieldName));
        }

        public static CodeCastExpression CastThisField(Type type, String fieldName)
        {
            return Cast(TypeRef(type), ThisFieldRef(fieldName));
        }

        public static CodeCastExpression CastThisField(String typeName, String fieldName)
        {
            return Cast(TypeRef(typeName), ThisFieldRef(fieldName));
        }

        public static CodePropertySetValueReferenceExpression SetValue()
        {
            return new CodePropertySetValueReferenceExpression();
        }

        public static CodeAssignStatement AssignPropertyPrim(CodeExpression targetObject, String propertyName, Object primitive)
        {
            return AssignProperty(targetObject, propertyName, Primitive(primitive));
        }

        public static CodeAssignStatement AssignProperty(CodeExpression targetObject, String propertyName, CodeExpression right)
        {
            return Assign(PropertyRef(targetObject, propertyName), right);
        }

        public static CodeAssignStatement AssignThisProperty(String propertyName, CodeExpression right)
        {
            return Assign(ThisPropertyRef(propertyName), right);
        }

        public static CodeAssignStatement AssignThisPropertyPrim(String propertyName, Object primitive)
        {
            return AssignThisProperty(propertyName, Primitive(primitive));
        }

        public static CodePropertyReferenceExpression PropertyRef(CodeExpression targetObject, String propertyName)
        {
            return new CodePropertyReferenceExpression(targetObject, propertyName);
        }

        public static CodePropertyReferenceExpression ThisPropertyRef(String propertyName)
        {
            return PropertyRef(This, propertyName);
        }

        public static CodeSnippetExpression EnumSnippet<TTYPE>(TTYPE value)
        {
            return SnippetExp(typeof(TTYPE).Name + "." + value.ToString());
        }

        public static CodeTypeOfExpression TypeOf(String typeName)
        {
            return TypeOf(TypeRef(typeName));
        }

        public static CodeTypeOfExpression TypeOf(CodeTypeReference typeRef)
        {
            return new CodeTypeOfExpression(typeRef);
        }

        public static CodeTypeOfExpression TypeOf(Type type)
        {
            return TypeOf(TypeRef(type));
        }

        public static CodeParameterDeclarationExpression ParamDecl(String typeName, String name)
        {
            return ParamDecl(TypeRef(typeName), name);
        }

        public static CodeParameterDeclarationExpression ParamDecl(Type type, String name)
        {
            return ParamDecl(TypeRef(type), name);
        }

        public static CodeParameterDeclarationExpression ParamDecl(CodeTypeReference typeRef, String name)
        {
            return new CodeParameterDeclarationExpression(typeRef, name);
        }

        public static CodeParameterDeclarationExpression ParamDecl<TTYPE>(String name)
        {
            return ParamDecl(typeof(TTYPE), name);
        }

        public static CodeBinaryOperatorExpression BinOp(CodeExpression left, CodeBinaryOperatorType opType, CodeExpression right)
        {
            return new CodeBinaryOperatorExpression(left, opType, right);
        }

        public static CodeBinaryOperatorExpression BinOpEq(CodeExpression left, CodeExpression right)
        {
            return BinOp(left, CodeBinaryOperatorType.ValueEquality, right);
        }

        public static CodeBinaryOperatorExpression BinOpOr(CodeExpression left, CodeExpression right)
        {
            return BinOp(left, CodeBinaryOperatorType.BooleanOr, right);
        }

        public static CodeBinaryOperatorExpression BinOpAnd(CodeExpression left, CodeExpression right)
        {
            return BinOp(left, CodeBinaryOperatorType.BooleanAnd, right);
        }

        public static CodeBinaryOperatorExpression BinOpIdIneq(CodeExpression left, CodeExpression right)
        {
            return BinOp(left, CodeBinaryOperatorType.IdentityInequality, right);
        }

        public static CodeBinaryOperatorExpression BinOpIdEq(CodeExpression left, CodeExpression right)
        {
            return BinOp(left, CodeBinaryOperatorType.IdentityEquality, right);
        }

        public static CodeArrayCreateExpression Array(CodeTypeReference typeRef, params CodeExpression[] initializers)
        {
            return new CodeArrayCreateExpression(typeRef, initializers);
        }

        public static CodeArrayCreateExpression Array(String typeName, params CodeExpression[] initializers)
        {
            return Array(TypeRef(typeName), initializers);
        }

        public static CodeArrayCreateExpression Array(Type type, params CodeExpression[] initializers)
        {
            return Array(TypeRef(type), initializers);
        }

        public static CodeArrayCreateExpression Array(CodeTypeReference typeRef, int size)
        {
            return new CodeArrayCreateExpression(typeRef, size);
        }

        public static CodeArrayCreateExpression Array(String typeName, int size)
        {
            return Array(TypeRef(typeName), size);
        }

        public static CodeArrayCreateExpression Array(Type type, int size)
        {
            return Array(TypeRef(type), size);
        }

        public static CodeConditionStatement If(CodeExpression condition, params CodeStatement[] trueStatements)
        {
            return new CodeConditionStatement(condition, trueStatements);
        }

        public static CodeMemberEvent Event(String name, CodeTypeReference typeRef, MemberAttributes attributes)
        {
            CodeMemberEvent ev = new CodeMemberEvent();
            ev.Name = name;
            ev.Type = typeRef;
            ev.Attributes = attributes;
            return ev;
        }

        public static CodeMemberEvent Event(String name, String typeName, MemberAttributes attributes)
        {
            return Event(name, TypeRef(typeName), attributes);
        }

        public static CodeMemberEvent Event(String name, Type type, MemberAttributes attributes)
        {
            return Event(name, TypeRef(type), attributes);
        }

        public static CodeMemberEvent Event(String name, Type type)
        {
            return Event(name, type, MemberAttributes.Public);
        }

        public static CodeMemberEvent Event(String name, String typeName)
        {
            return Event(name, typeName, MemberAttributes.Public);
        }

        public static CodeMemberEvent Event(String name, CodeTypeReference typeRef)
        {
            return Event(name, typeRef, MemberAttributes.Public);
        }

        public static CodeTypeMember StartRegion(CodeTypeMember member, String desc)
        {
            member.StartDirectives.Add(StartRegion(desc));
            return member;
        }

        public static CodeRegionDirective StartRegion(String desc)
        {
            return new CodeRegionDirective(CodeRegionMode.Start, desc);
        }

        public static CodeRegionDirective EndRegion()
        {
            return new CodeRegionDirective(CodeRegionMode.End, null);
        }

        public static CodeTypeMember EndRegion(CodeTypeMember member)
        {
            member.EndDirectives.Add(EndRegion());
            return member;
        }

        public static CodePrimitiveExpression Null { get { return Primitive(null); } }
        public static CodeThisReferenceExpression This { get { return new CodeThisReferenceExpression(); } }

        public static CodeExpressionStatement Stat(CodeExpression expr)
        {
            return new CodeExpressionStatement(expr);
        }

        public static CodeTypeReference GenericTypeRef(Type type, params Type[] genericParams)
        {
            CodeTypeReference result = TypeRef(type);
            foreach (Type typeArg in genericParams)
                result.TypeArguments.Add(TypeRef(typeArg));
            return result;
        }

        public static CodeTypeReference GenericTypeRef(String typeName, params String[] genericParams)
        {
            CodeTypeReference result = TypeRef(typeName);
            foreach (String typeArg in genericParams)
                result.TypeArguments.Add(TypeRef(typeArg));
            return result;
        }

        public static CodeTypeReference GenericTypeRef(String typeName, params CodeTypeReference[] genericParams)
        {
            CodeTypeReference result = TypeRef(typeName);
            foreach (CodeTypeReference typeArg in genericParams)
                result.TypeArguments.Add(typeArg);
            return result;
        }

        public static CodeTypeReference GenericIListTypeRef(String genericTypeName)
        {
            return GenericTypeRef("IList", genericTypeName);
        }

        public static CodeTypeReference GenericIListTypeRef(Type genericType)
        {
            return GenericTypeRef("IList", TypeRef(genericType));
        }

        public static CodeTypeReference GenericIListTypeRef(CodeTypeReference genericTypeRef)
        {
            return GenericTypeRef("IList", genericTypeRef);
        }

        public static CodeTypeReference GenericListTypeRef(String genericTypeName)
        {
            return GenericTypeRef("List", genericTypeName);
        }

        public static CodeTypeReference GenericListTypeRef(Type genericType)
        {
            return GenericTypeRef("List", TypeRef(genericType));
        }

        public static CodeTypeReference GenericListTypeRef(CodeTypeReference genericTypeRef)
        {
            return GenericTypeRef("List", genericTypeRef);
        }

        public static CodePropertyReferenceExpression StaticPropertyRef(String typeName, String propertyName)
        {
            return StaticPropertyRef(TypeRefExp(typeName), propertyName);
        }

        public static CodePropertyReferenceExpression StaticPropertyRef(Type type, String propertyName)
        {
            return StaticPropertyRef(TypeRefExp(type), propertyName);
        }

        public static CodePropertyReferenceExpression StaticPropertyRef(CodeTypeReferenceExpression typeRefExp, String propertyName)
        {
            return PropertyRef(typeRefExp, propertyName);
        }

        public static CodeDefaultValueExpression Default(String typeName)
        {
            return Default(TypeRef(typeName));
        }

        public static CodeDefaultValueExpression Default(CodeTypeReference typeRef)
        {
            return new CodeDefaultValueExpression(typeRef);
        }

        public static CodeDefaultValueExpression Default(Type type)
        {
            return Default(TypeRef(type));
        }

        public static CodeDefaultValueExpression Default<TType>()
        {
            return Default(typeof(TType));
        }

        public static CodeCommentStatement Comment(String text)
        {
            return new CodeCommentStatement(text);
        }

        public static CodeMemberMethod GetMethod(CodeTypeDeclaration type, String methodName)
        {
            foreach (CodeTypeMember member in type.Members)
                if (member is CodeMemberMethod)
                    if (((CodeMemberMethod)member).Name.Equals(methodName, StringComparison.Ordinal))
                        return (CodeMemberMethod)member;
            return null;
        }

        public static CodeIterationStatement While(CodeExpression testExpression)
        {
            CodeIterationStatement result = new CodeIterationStatement();
            result.TestExpression = testExpression;
            result.IncrementStatement = new CodeSnippetStatement("");
            result.InitStatement = new CodeSnippetStatement("");
            return result;
        }
    }
}
