using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.editor.command;
using gehtsoft.xce.editor.textwindow;
using gehtsoft.xce.text;
using gehtsoft.xce.extension.advnav_commands;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.extension.advnav_layout
{
    internal class TransformBlockDialog : XceDialog
    {
        DialogItemRadioBox mUcase;
        DialogItemRadioBox mLcase;
        DialogItemRadioBox mSort;
        string mMode;
        
        public string Mode
        {   
            get
            {
                return mMode;
            }
        }
        
        internal TransformBlockDialog(Application application) : base(application, "Transform Block", false, 7, 21)
        {
            AddItem(new DialogItemLabel("Transform mode:", 0x1000, 0, 0));
            AddItem(mUcase = new DialogItemRadioBox("&Upper Case", 0x1001, true, 1, 1, true));
            AddItem(mLcase = new DialogItemRadioBox("&Lower Case", 0x1002, false, 2, 1, false));
            AddItem(mSort = new DialogItemRadioBox("&Sort", 0x1002, false, 3, 1, false));
            mSort.Enable(application.ActiveWindow.BlockType == TextWindowBlock.Line);
            DialogItemButton b1, b2;
            AddItem(b1 = new DialogItemButton("< Ok >", DialogResultOK, 4, 0));
            AddItem(b2 = new DialogItemButton("< Cancel >", DialogResultCancel, 4, 0));
            CenterButtons(b1, b2);
        }

        public override bool OnOK()
        {
            if (mUcase.Checked)
                mMode = "ucase";
            else if (mLcase.Checked)
                mMode = "lcase";
            else if (mSort.Checked)
                mMode = "sort";
            else
                return false;
            return base.OnOK();
        }
    }

    internal class TransformBlockCommand : FindWordBase, IEditorCommand
    {
        internal TransformBlockCommand()
        {
            
        }

        public bool IsEnabled(Application application, string param)
        {
            return application.ActiveWindow != null && application.ActiveWindow.BlockType != TextWindowBlock.None;
        }

        public bool IsChecked(Application application, string param)
        {
            return false;
        }

        static Comparison<string> mComparison = new Comparison<string>(comparison);

        static int comparison(string s1, string s2)
        {
            return string.Compare(s1, s2, true);
        }

        public void Execute(Application application, string param)
        {
            if (application.ActiveWindow == null)
                return ;
                
            if (param == null)
            {
                TransformBlockDialog dlg = new TransformBlockDialog(application);
                if (dlg.DoModal() == Dialog.DialogResultOK)
                    param = dlg.Mode;
                else
                    return ;    
            }

            application.ActiveWindow.Text.BeginUndoTransaction();
            try
            {
                if (param == "ucase" || param == "lcase" && application.ActiveWindow.BlockType != TextWindowBlock.None)
                {
                    bool ucase = param == "ucase";
                    int bls, ble, bcs, bce;
                    bls = application.ActiveWindow.BlockLineStart;
                    ble = application.ActiveWindow.BlockLineEnd;
                    bcs = application.ActiveWindow.BlockColumnStart;
                    bce = application.ActiveWindow.BlockColumnEnd;
                    TextWindowBlock t = application.ActiveWindow.BlockType;
                    int l, c, ls, ll, cs, ce;
                    XceFileBuffer b = application.ActiveWindow.Text;
                    
                    for (l = bls; l <= ble; l++)
                    {
                        if (l >= b.LinesCount)
                            break;
                        ls = b.LineStart(l);
                        ll = b.LineLength(l);
                        
                        cs = 0;
                        ce = -1;
                        
                        switch (t)
                        {
                        case    TextWindowBlock.Line:
                                cs = 0;
                                ce = ll - 1;
                                break;
                        case    TextWindowBlock.Box: 
                                cs = bcs;
                                ce = bce;
                                if (cs >= ll)
                                {
                                    cs = ll;
                                }
                                if (ce >= ll)
                                {
                                    ce = ll - 1;
                                }
                                break;
                        case    TextWindowBlock.Stream:
                                if (l == bls && l == ble)
                                {
                                    cs = bcs;
                                    ce = bce;
                                    if (cs >= ll)
                                    {
                                        cs = ll;
                                    }
                                    if (ce >= ll)
                                    {
                                        ce = ll - 1;
                                    }
                                }
                                else if (l == bls)
                                {
                                    cs = bcs;
                                    ce = ll - 1;
                                    if (cs >= ll)
                                        cs = ll;
                                }
                                else if (l == ble)
                                {
                                    cs = 0;
                                    ce = bce;
                                    if (ce >= ll)
                                        ce = ll - 1;
                                }
                                else
                                {
                                    cs = 0;
                                    ce = ll - 1;
                                }
                                break;
                        }
                        char[] tt = new char[1];
                        for (c = cs; c <= ce; c++)
                        {
                            char c0 = b[ls + c];
                            char c1;
                            if (ucase)
                                c1 = char.ToUpper(c0);
                            else
                                c1 = char.ToLower(c0);
                            if (c0 != c1)
                            {
                                b.DeleteRange(ls + c, 1);
                                tt[0] = c1;
                                b.InsertRange(ls + c, tt, 0, 1);
                            }
                        }
                    }
                }
                else if (param == "sort")
                {
                    if (application.ActiveWindow.BlockType == TextWindowBlock.Line)
                    {
                        int i, cc = application.ActiveWindow.BlockLineEnd - application.ActiveWindow.BlockLineStart + 1;
                        if (cc > 1)
                        {
                            string[] lines = new string[cc];
                            for (i = 0; i < cc; i++)
                            {
                                int line = application.ActiveWindow.BlockLineStart + i;
                                if (line < application.ActiveWindow.Text.LinesCount)
                                    lines[i] = application.ActiveWindow.Text.GetRange(application.ActiveWindow.Text.LineStart(line), application.ActiveWindow.Text.LineLength(line));
                                else
                                    lines[i] = "";
                            }
                            Array.Sort(lines, mComparison);
                            for (i = 0; i < cc; i++)
                            {
                                int line = application.ActiveWindow.BlockLineStart + i;
                                int ls;
                                bool nl = false;
                                if (line >= application.ActiveWindow.Text.LinesCount)
                                {
                                    ls = application.ActiveWindow.Text.Length;
                                    nl = true;
                                }
                                else
                                    ls = application.ActiveWindow.Text.LineStart(line);
                                application.ActiveWindow.Text.DeleteRange(ls, application.ActiveWindow.Text.LineLength(line));
                                if (lines[i].Length > 0)
                                    application.ActiveWindow.Text.InsertRange(ls, lines[i]);
                                if (nl)
                                    application.ActiveWindow.Text.InsertRange(ls + lines[i].Length, application.ActiveWindow.Text.Eol, 0, application.ActiveWindow.Text.Eol.Length);
                            }
                            lines = null;
                            GC.Collect();
                        }
                    }
                }
                    
            }
            finally
            {
                application.ActiveWindow.Text.EndUndoTransaction();
            }
        }

        public string Name
        {
            get
            {
                return "TransformBlock";
            }
        }



    }
}
