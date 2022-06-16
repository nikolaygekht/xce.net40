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
    internal class BrowseSourceDialog : XceDialog
    {
        private IProjectBrowserItem mSel;
        internal IProjectBrowserItem Selected
        {
            get
            {
                return mSel;
            }
        }

        private DialogItemSearchingList mList;
        private DialogItemButton mOk;
        private DialogItemButton mCancel;

        internal BrowseSourceDialog(Application application, IProjectBrowserItemCollection collection, IProjectBrowserItem curSel) : base(application, "Project", true, 20, 60)
        {
            AddItem(mList = new DialogItemSearchingList(0x1000, 0, 0, 17, 58));
            AddItems(collection, curSel, 0);
            if (mList.CurSel >= 0)
                mList.EnsureVisible(mList.CurSel);
            AddItem(mOk = new DialogItemButton("< &Ok >", DialogResultOK, 17, 0));
            AddItem(mCancel = new DialogItemButton("< &Cancel >", DialogResultCancel, 17, 0));
            CenterButtons(mOk, mCancel);
        }

        private void AddItems(IProjectBrowserItemCollection collection, IProjectBrowserItem curSel, int depth)
        {
            string sdepth = new string(' ', depth);
            string prefix;

            foreach (IProjectBrowserItem item in collection)
            {

                switch (item.ItemType)
                {
                case    ProjectBrowserItemType.File:
                case    ProjectBrowserItemType.Tag:
                case    ProjectBrowserItemType.Text:
                        prefix = "";
                        break;
                case    ProjectBrowserItemType.Class:
                        prefix = "class ";
                        break;
                case    ProjectBrowserItemType.Property:
                        prefix = "p:";
                        break;
                case    ProjectBrowserItemType.Method:
                        prefix = "m:";
                        break;
                case    ProjectBrowserItemType.Field:
                        prefix = "f:";
                        break;
                case    ProjectBrowserItemType.Event:
                        prefix = "e:";
                        break;
                default:
                        prefix = "?:";
                        break;

                }
                mList.AddItem(sdepth + prefix + item.Name, item);

                if (item == curSel)
                    mList.CurSel = mList.Count - 1;

                if (item.HasChildren)
                    AddItems(item.Children, curSel, depth + 1);

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
                mSel = mList[mList.CurSel].UserData as IProjectBrowserItem;
                if (mSel != null)
                    return true;
            }
            mApplication.ShowMessage("Please make a selection", "Project");
            return false;
        }
    }
}
