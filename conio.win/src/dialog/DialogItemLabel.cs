using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.conio.win
{
    public class DialogItemLabel : DialogItem
    {
        private string mTitle;
        private char mHotKey;
        private int mHotKeyPosition;
        
        public DialogItemLabel(string title, int id, int row, int column) : base(id, row, column)
        {
            mHotKeyPosition = StringUtil.processHotKey(ref title);
            if (mHotKeyPosition >= 0)
                mHotKey = title[mHotKeyPosition];
            mTitle = title;                
            setDimesion(1, mTitle.Length);
        }
        
        public string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                string title = value;
                if (title == null)
                    title = "";
                mHotKeyPosition = StringUtil.processHotKey(ref title);
                if (mHotKeyPosition >= 0)
                    mHotKey = title[mHotKeyPosition];
                mTitle = title;
                setDimesion(1, mTitle.Length);
                invalidate();                
            }
        }
        
        public override bool HasHotKey
        {
            get
            {
                return mHotKeyPosition >= 0;
            }
        }

        public override char HotKey
        {
            get
            {
                if (!HasHotKey)
                    throw new InvalidOperationException();
                return mHotKey;
            }
        }

        public override bool IsInputElement
        {
            get 
            { 
                return false;
            }
        }

        public override bool Enabled
        {
            get 
            { 
                return true;
            }
        }

        public override void OnPaint(Canvas canvas)
        {
            canvas.fill(0, 0, 1, Width, Dialog.Colors.DialogItemLabelColor);
            canvas.write(0, 0, mTitle);
            if (mHotKeyPosition >= 0)
                canvas.write(0, mHotKeyPosition, Dialog.Colors.DialogItemLabelHotKey);
        }
    }
}
