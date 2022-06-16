using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.util;
using gehtsoft.xce.text;

namespace gehtsoft.xce.extension.advnav_commands
{

    internal class ICReplacePair
    {
        internal string from;
        internal string to;
        
        internal ICReplacePair(string _from, string _to)
        {
            from = _from;
            to = _to;
        }
    };
    
    internal class ICReplacePairFactory
    {
        static internal List<ICReplacePair> create()
        {
            List<ICReplacePair> pairs = new List<ICReplacePair>();
            pairs.Add(new ICReplacePair("Resources::getErrorFirstIsNotString()", "Resources::getParamError(1, Resources::String)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSecondIsNotString()", "Resources::getParamError(2, Resources::String)"));
            pairs.Add(new ICReplacePair("Resources::getErrorThirdIsNotString()", "Resources::getParamError(3, Resources::String)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFourthIsNotString()", "Resources::getParamError(4, Resources::String)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFifthIsNotString()", "Resources::getParamError(5, Resources::String)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSixthIsNotString()", "Resources::getParamError(6, Resources::String)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFirstIsNotNumber()", "Resources::getParamError(1, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFirstIsNegative()", "Resources::getParamError(1, Resources::PosNumber)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSecondIsNotNumber()", "Resources::getParamError(2, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorThirdIsNotNumber()", "Resources::getParamError(3, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFourthIsNotNumber()", "Resources::getParamError(4, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFifthIsNotNumber()", "Resources::getParamError(5, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSixthIsNotNumber()", "Resources::getParamError(6, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSixthIsNotPositiveOrNullNumber()", "Resources::getParamError(6, Resources::PosNumberOrZero)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFirstIsNotBoolean()", "Resources::getParamError(1, Resources::Boolean)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSecondIsNotBoolean()", "Resources::getParamError(2, Resources::Boolean)"));
            pairs.Add(new ICReplacePair("Resources::getErrorThirdIsNotBoolean()", "Resources::getParamError(3, Resources::Boolean)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFourthIsNotBoolean()", "Resources::getParamError(4, Resources::Boolean)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFifthIsNotBoolean()", "Resources::getParamError(5, Resources::Boolean)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSixthIsNotBoolean()", "Resources::getParamError(6, Resources::Boolean)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFirstIsNotTable()", "Resources::getParamError(1, Resources::Table)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSecondIsNotTable()", "Resources::getParamError(2, Resources::Table)"));
            pairs.Add(new ICReplacePair("Resources::getErrorThirdIsNotTable()", "Resources::getParamError(3, Resources::Table)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFourthIsNotTable()", "Resources::getParamError(4, Resources::Table)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFifthIsNotTable()", "Resources::getParamError(5, Resources::Table)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSixthIsNotTable()", "Resources::getParamError(6, Resources::Table)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSeventhIsNotString()", "Resources::getParamError(7, Resources::String)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSeventhIsNotNumber()", "Resources::getParamError(7, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSeventhIsNotBoolean()", "Resources::getParamError(7, Resources::Boolean)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSeventhIsNotTable()", "Resources::getParamError(7, Resources::Table)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFirstIsNotSource()", "Resources::getParamError(1, Resources::Source)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFirstIsNotStream()", "Resources::getParamError(1, Resources::Stream)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFirstIsNotOutput()", "Resources::getParamError(1, Resources::Output)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSecondIsNotStream()", "Resources::getParamError(2, Resources::Stream)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSecondIsNotParams()", "Resources::getParamError(2, Resources::Params)"));
            pairs.Add(new ICReplacePair("Resources::getErrorEightIsNotNumber()", "Resources::getParamError(8, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorNinthIsNotNumber()", "Resources::getParamError(9, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorTenthIsNotNumber()", "Resources::getParamError(10, Resources::Number)"));
            pairs.Add(new ICReplacePair("Resources::getErrorThirdIsNotOutputStream()", "Resources::getParamError(3, Resources::Output)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFourthIsNotOutputStream()", "Resources::getParamError(4, Resources::Output)"));
            pairs.Add(new ICReplacePair("Resources::getErrorFifthIsNotOutputStream()", "Resources::getParamError(5, Resources::Output)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSixthIsNotOutputStream()", "Resources::getParamError(6, Resources::Output)"));
            pairs.Add(new ICReplacePair("Resources::getErrorSeventhIsNotOutputStream()", "Resources::getParamError(7, Resources::Output)"));
            pairs.Add(new ICReplacePair("Resources::getErrorThirdIsNotStream()", "Resources::getParamError(3, Resources::Stream)"));
            pairs.Add(new ICReplacePair("Resources::getErrorEightIsNotString()", "Resources::getParamError(8, Resources::String)"));
            pairs.Add(new ICReplacePair("Resources::getErrorNinthIsNotBool()", "Resources::getParamError(9, Resources::Boolean)"));
            pairs.Add(new ICReplacePair("Resources::getErrorThirdIsNotSource()", "Resources::getParamError(3, Resources::Source)"));
            pairs.Add(new ICReplacePair("Resources::getErrorNinthIsNotString()", "Resources::getParamError(9, Resources::String)"));
            pairs.Add(new ICReplacePair("Resources::getErrorThirdIsNotValueMap()", "Resources::getParamError(3, Resources::Map)"));
            return pairs;
        }
    }

    internal class ICReplace : IEditorCommand
    {
        List<ICReplacePair> pairs = ICReplacePairFactory.create();
    
        public string Name
        {
            get
            {
                return "ICReplace";
            }
        }        
        
        public bool IsEnabled(Application application, string param)
        {
            if (application.ActiveWindow == null)
                return false;
            return true;
        }
        
        public bool IsChecked(Application application, string param)
        {
            return false;
        }
        
        public void Execute(Application application, string param)
        {
            if (application.ActiveWindow == null)
                return ;
                
            TextWindow w = application.ActiveWindow;
            XceFileBuffer buff = w.Text;
            
            for (int i = 0; i < buff.LinesCount; i++)
            {
                int s, l, c;
                s = buff.LineStart(i);
                l = buff.LineLength(i);
                c = 0;
                string str = buff.GetRange(s, l);
                
                foreach (ICReplacePair pair in pairs)
                {
                    while (str.Contains(pair.from))
                    {
                        str = str.Replace(pair.from, pair.to);
                        c++;
                    }
                }
                if (c > 0)
                {
                    buff.DeleteRange(s, l);
                    buff.InsertRange(s, str);
                }
            }
        }
    }
}
