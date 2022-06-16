using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace gehtsoft.intellisense.cs
{
    public enum CsParserExpressionContext
    {
        Attribute,
        BaseConstructorCall,
        Constraints,
        ConstraintsStart,
        Default,
        DelegateType,
        EnumBaseType,
        EventDeclaration,
        FirstParameterType,
        FullyQualifiedType,
        Global,
        IdentifierExpected,
        Importable,
        InheritableType,
        Interface,
        InterfaceDeclaration,
        InterfacePropertyDeclaration,
        MethodBody,
        Namespace,
        NonStaticNonVoidType,
        ObjectCreation,
        ObjectInitializer,
        ParameterType,
        PropertyDeclaration,
        Type,
        TypeDeclaration,
        Unknown
    }

    public class CsParserExpression
    {
        ExpressionResult mExpression;

        internal ExpressionResult ExpressionResult
        {
            get
            {
                return mExpression;
            }
        }

        internal CsParserExpression(ExpressionResult expression)
        {
            mExpression = expression;
        }

        public string Expression
        {
            get
            {
                return mExpression.Expression;
            }
        }

        public int BeginLine
        {
            get
            {
                return mExpression.Region.BeginLine;
            }
        }

        public int BeginColumn
        {
            get
            {
                return mExpression.Region.BeginColumn;
            }
        }

        public int EndLine
        {
            get
            {
                return mExpression.Region.BeginLine;
            }
        }

        public int EndColumn
        {
            get
            {
                return mExpression.Region.BeginColumn;
            }
        }

        public override string ToString()
        {
            return mExpression.Expression;
        }

        public override int GetHashCode()
        {
            return Expression.GetHashCode();
        }

        public CsParserExpressionContext Context
        {
            get
            {
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.Attribute)
                    return CsParserExpressionContext.Attribute;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.BaseConstructorCall)
                    return CsParserExpressionContext.BaseConstructorCall;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.Constraints)
                    return CsParserExpressionContext.Constraints;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.ConstraintsStart)
                    return CsParserExpressionContext.ConstraintsStart;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.Default)
                    return CsParserExpressionContext.Default;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.DelegateType)
                    return CsParserExpressionContext.DelegateType;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.EnumBaseType)
                    return CsParserExpressionContext.EnumBaseType;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.EventDeclaration)
                    return CsParserExpressionContext.EventDeclaration;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.FirstParameterType)
                    return CsParserExpressionContext.FirstParameterType;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.FullyQualifiedType)
                    return CsParserExpressionContext.FullyQualifiedType;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.Global)
                    return CsParserExpressionContext.Global;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.IdentifierExpected)
                    return CsParserExpressionContext.IdentifierExpected;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.Importable)
                    return CsParserExpressionContext.Importable;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.InheritableType)
                    return CsParserExpressionContext.InheritableType;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.Interface)
                    return CsParserExpressionContext.Interface;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.InterfaceDeclaration)
                    return CsParserExpressionContext.InterfaceDeclaration;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.InterfacePropertyDeclaration)
                    return CsParserExpressionContext.InterfacePropertyDeclaration;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.MethodBody)
                    return CsParserExpressionContext.MethodBody;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.Namespace)
                    return CsParserExpressionContext.Namespace;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.NonStaticNonVoidType)
                    return CsParserExpressionContext.NonStaticNonVoidType;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.ObjectCreation)
                    return CsParserExpressionContext.ObjectCreation;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.ObjectInitializer)
                    return CsParserExpressionContext.ObjectInitializer;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.ParameterType)
                    return CsParserExpressionContext.ParameterType;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.PropertyDeclaration)
                    return CsParserExpressionContext.PropertyDeclaration;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.Type)
                    return CsParserExpressionContext.Type;
                if (mExpression.Context != null && mExpression.Context == ExpressionContext.TypeDeclaration)
                    return CsParserExpressionContext.TypeDeclaration;
                return CsParserExpressionContext.Unknown;
            }
        }
    }
}
