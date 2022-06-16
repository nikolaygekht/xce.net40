using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.intellisense.cs;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.text;

namespace gehtsoft.xce.intellisense.common
{
    public interface IInsightDataProvider
    {
        int CurSel
        {
            get;
            set;
        }

        int StartRow
        {
            get;
        }

        int StartColumn
        {
            get;
        }

        int Count
        {
            get;
        }

        IInsightData this[int index]
        {
            get;
        }

        bool CursorPositionChanged(int line, int column, out int args);
    }
}

