using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.editor.application;

namespace gehtsoft.xce.extension.template_impl
{
    internal class ChooseTemplateDialog : XceDialog
    {
        private DialogItemListBox mList;

        internal ChooseTemplateDialog(Application application, string fileName, TemplateFileTypeCollection collection) :
                 base(application, "Choose template", false, 20, 60)
        {
            AddItem(mList = new DialogItemListBox(0x1000, 0, 0, 17, 58));
            foreach (TemplateFileType t in collection)
                if (t.Match(fileName))
                {
                    foreach (Template t1 in t.Templates)
                        mList.AddItem(t1.Name, t1);
                }

            DialogItemButton b1, b2;
            AddItem(b1 = new DialogItemButton("< &Ok >", DialogResultOK, 17, 0));
            AddItem(b2 = new DialogItemButton("< &Cancel >", DialogResultCancel, 17, 0));
            CenterButtons(b1, b2);
        }

        private Template mTemplate = null;

        internal Template Template
        {
            get
            {
                return mTemplate;
            }
        }

        internal bool HasTemplates
        {
            get
            {
                return mList.Count > 0;
            }
        }


        public override bool OnOK()
        {
            if (mList.CurSel >= 0 && mList.CurSel < mList.Count)
            {
                mTemplate = (Template)mList[mList.CurSel].UserData;
                return true;
            }
            else
                return false;
        }
    }

    internal class TemplateParametersDialog : XceDialog
    {
        List<DialogItemSingleLineTextBox> mParams = new List<DialogItemSingleLineTextBox>();
        List<string> mValues = new List<string>();

        internal TemplateParametersDialog(Application application, Template template) :
                 base(application, "Insert Template", false, Math.Min(template.Params.Count, 20) + 3, 60)
        {
            int i;
            int max;

            DialogItemLabel label;
            DialogItemSingleLineTextBox text;

            for (i = 0, max = 0; i < 20 && i < template.Params.Count; i++)
            {
                if (template.Params[i].Length + 1 > max)
                    max = template.Params[i].Length + 1;
            }

            if (max > 30)
                max = 30;

            for (i = 0; i < 20 && i < template.Params.Count; i++)
            {
                string t = template.Params[i];
                if (t.Length > 29)
                    t = t.Substring(0, 29) + "...";
                t = t + ":";

                label = new DialogItemLabel(t, 0x1000 + i * 2, i, 0);
                text = new DialogItemSingleLineTextBox("", 0x1000 + i * 2 + 1, i, max, 58 - max);

                AddItem(label);
                AddItem(text);
                mParams.Add(text);
            }
            DialogItemButton b1, b2;
            AddItem(b1 = new DialogItemButton("< &Ok >", DialogResultOK, i, 0));
            AddItem(b2 = new DialogItemButton("< &Cancel >", DialogResultCancel, i, 0));
            CenterButtons(b1, b2);
        }

        public override bool OnOK()
        {
            mValues.Clear();
            for (int i = 0; i < mParams.Count; i++)
                mValues.Add(mParams[i].Text);
            return true;
        }

        internal string GetParam(int index)
        {
            if (index >= 0 && index < mValues.Count)
                return mValues[index];
            return "";
        }


    }
}
