using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using ICSharpCode.SharpDevelop.Dom;
using gehtsoft.intellisense.cs;

namespace gehtsoft.xce.extension.csintellisense_impl
{
    internal class SuggestionDialog : XceDialog
    {
        ICsCodeCompletionItem mSelected = null;
    
        internal ICsCodeCompletionItem Selected
        {
            get
            {
                return mSelected;
            }
        }
        
        DialogItemSearchingList mList;
        DialogItemButton mOk;
        DialogItemButton mCancel;
    
        internal SuggestionDialog(Application application, CsCodeCompletionItemCollection coll) : base(application, "Suggestions", true, 20, 60)
        {
            AddItem(mList = new DialogItemSearchingList(0x1000, 0, 0, 17, 58));
            foreach (ICsCodeCompletionItem item in coll)
                AddItem(item);
            if (coll.DefaultIndex >= 0 && coll.DefaultIndex < mList.Count)
            {
                mList.CurSel = coll.DefaultIndex;
                mList.EnsureVisible(mList.CurSel);
                if (coll.Preselection != null && coll.Preselection.Length > 0)
                {
                    int idx = mList[mList.CurSel].Label.IndexOf(coll.Preselection);
                    mList.setHighlight(idx, coll.Preselection);   
                }
            }
            AddItem(mOk = new DialogItemButton("< &Ok >", DialogResultOK, 17, 0));
            AddItem(mCancel = new DialogItemButton("< &Cancel >", DialogResultCancel, 17, 0));
            CenterButtons(mOk, mCancel);
        }

        private void AddItem(ICsCodeCompletionItem item)
        {
            switch (item.Type)
            {
            case CsParserCodeCompletionItemType.Text:
                mList.AddItem(item.Name, item);
                break;
            case CsParserCodeCompletionItemType.Class:
            case CsParserCodeCompletionItemType.Member:
                {
                    IEntity entity = item.Entity;
                    string s;
                    char p;
                    if (entity is IClass)
                        p = 'c';
                    else if (entity is IMethod)
                        p = 'm';
                    else if (entity is IProperty)
                        p = 'p';
                    else if (entity is IField)
                        p = 'f';
                    else if (entity is IEvent)
                        p = 'e';
                    else 
                        p = '?';
                    
                    if (item.OverloadsCount > 0)
                        s = string.Format("{0}:{1} +{2}", p, item.Name, item.OverloadsCount);
                    else
                        s = string.Format("{0}:{1}", p, item.Name);
                    mList.AddItem(s, item);
                }
                break;
            default:
                mList.AddItem(string.Format("?:{0}", item.Name), item);
                break;
            }
        }
        
        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            if (Height < 9 || Width < 20)
                move(Row, Column, Math.Max(Height, 9), Math.Max(Width, 20));
            else
            {
                int height, width;
                height = Height;
                width = Width;
                
                mList.move(0, 0, height - 3, width - 2);
                mOk.move(height - 3, 0, 1, mOk.Title.Length);
                mCancel.move(height - 3, 0, 1, mCancel.Title.Length);
                CenterButtons(mOk, mCancel);
                invalidate();
            }
        }

        public override bool OnOK()
        {
            if (mList.CurSel >= 0 && mList.CurSel < mList.Count)
            {
                mSelected = mList[mList.CurSel].UserData as ICsCodeCompletionItem;
                if (mSelected != null)
                    return true;
            }
            mApplication.ShowMessage("Please select a namespace, class or class member", "Suggestions");
            return false;
        }
    }
}
