using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.extension.scriptimpl
{
    internal class PromptDialog : Dialog
    {
        DialogItemSingleLineTextBox mText;
        string mValue;
        
        internal string Value
        {
            get
            {
                return mValue;
            }
        }
        
        internal PromptDialog(string title, string label, string defaultValue, int promptWidth, IColorScheme colors) : base(title, colors, false, 5, Math.Max(Math.Max(promptWidth, label.Length), "< Ok > < Cancel >".Length) + 2)
        {
            mValue = defaultValue;
            
            AddItem(new DialogItemLabel(label, 0x1000, 0, 0));
            AddItem(mText = new DialogItemSingleLineTextBox(defaultValue, 0x1001, 1, 0, promptWidth));
            DialogItemButton b1, b2;
            AddItem(b1 = new DialogItemButton("< Ok >", Dialog.DialogResultOK, 2, 0));
            AddItem(b2 = new DialogItemButton("< Cancel >", Dialog.DialogResultCancel, 2, 0));
            CenterButtons(b1, b2);
        }

        public override bool OnOK()
        {
            bool rc = base.OnOK();
            if (rc)
                mValue = mText.Text;
            return rc;
        }
    }
}
