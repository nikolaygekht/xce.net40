using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.intellisense.common;

namespace gehtsoft.xce.extension.intellisense_impl
{
    internal class SuggestionDialog : XceDialog
    {
        ICodeCompletionItem mSelected = null;

        internal ICodeCompletionItem Selected
        {
            get
            {
                return mSelected;
            }
        }

        DialogItemSearchingList mList;
        DialogItemButton mOk;
        DialogItemButton mCancel;

        internal SuggestionDialog(Application application, ICodeCompletionItemCollection coll) : base(application, "Suggestions", true, 20, 60)
        {
            AddItem(mList = new DialogItemSearchingList(0x1000, 0, 0, 17, 58));
            foreach (ICodeCompletionItem item in coll)
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

        private void AddItem(ICodeCompletionItem item)
        {
            string s = "";
            switch (item.Type)
            {
            case CodeCompletionItemType.Text:
            case CodeCompletionItemType.Tag:
                    s = item.Name;
                    break;
            case CodeCompletionItemType.Class:
                    s = string.Format("c:{0}", item.Name);
                    break;
            case CodeCompletionItemType.Method:
                    if (item.OverloadsCount > 0)
                        s = string.Format("m:{0} +{1}", item.Name, item.OverloadsCount);
                    else
                        s = string.Format("m:{0}", item.Name);
                    break;
            case CodeCompletionItemType.Property:
                    if (item.OverloadsCount > 0)
                        s = string.Format("p:{0} +{1}", item.Name, item.OverloadsCount);
                    else
                        s = string.Format("p:{0}", item.Name);
                    break;
            case CodeCompletionItemType.Field:
                    s = string.Format("f:{0}", item.Name);
                    break;
            case CodeCompletionItemType.Event:
                    s = string.Format("e:{0}", item.Name);
                    break;
            default:
                    s = string.Format("?:{0}", item.Name);
                    break;
            }
            mList.AddItem(s, item);
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
                mSelected = mList[mList.CurSel].UserData as ICodeCompletionItem;
                if (mSelected != null)
                    return true;
            }
            mApplication.ShowMessage("Please make a selection", "Suggestions");
            return false;
        }
    }
}
