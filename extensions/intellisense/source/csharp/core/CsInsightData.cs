using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using gehtsoft.xce.intellisense.common;


namespace gehtsoft.intellisense.cs
{
    internal class CsInsightData : IInsightData
    {
        private string mTip;
        private IMethodOrProperty mObject;

        public string Tip
        {
            get
            {
                return mTip;
            }
        }

        public IMethodOrProperty Object
        {
            get
            {
                return mObject;
            }
        }

        public int ArgsCount
        {
            get
            {
                return mObject.Parameters.Count;
            }
        }

        public CsInsightData(string tip, IMethodOrProperty data)
        {
            mTip = tip;
            mObject = data;
        }
    }
}
