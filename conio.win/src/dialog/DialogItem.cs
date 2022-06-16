using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.conio.win
{
    /// <summary>
    /// Base class for dialog item
    /// </summary>
    public abstract class DialogItem : Window
    {
        private int mRow, mColumn, mWidth, mHeight, mID;
        private Dialog mDialog;
    
        new public int Row
        {
            get
            {
                return mRow;
            }
        }
        
        new public int Column
        {
            get
            {
                return mColumn;
            }
        }
        
        new public int Height
        {
            get
            {
                return mHeight;
            }
        }
        
        new public int Width
        {
            get
            {
                return mWidth;
            }
        }
        
        public Dialog Dialog
        {
            get
            {
                return mDialog;
            }
        }
        
        internal void setDialog(Dialog dlg)
        {
            mDialog = dlg;
        }
        
        public abstract bool IsInputElement
        {
            get;
        }
        
        public abstract bool Enabled
        {
            get;
        }
        
        public virtual bool HasHotKey
        {
            get
            {
                return false;
            }
        }
        
        public virtual char HotKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

       
        public int ID
        {
            get
            {
                return mID;
            }
        }
        
        public virtual void Click()
        {
        }
        
        protected DialogItem(int id, int row, int column, int height, int width)
        {
            mID = id;
            mRow = row;
            mColumn = column;
            mHeight = height;
            mWidth = width;
        }
        
        internal DialogItem(int id, int row, int column)
        {
            mID = id;
            mRow = row;
            mColumn = column;
        }
        
        internal void setDimesion(int height, int width)
        {
            if (Exists)
                move(mRow, mColumn, height, width);
            else
            {
                mHeight = height;
                mWidth = width;
            }
        }

        public override void OnSizeChanged()
        {
            base.OnSizeChanged();
            mRow = base.Row;
            mColumn = base.Column;
            mHeight = base.Height;
            mWidth = base.Width;
        }
        
        public void repos(int row, int column, int height, int width)
        {
            if (Exists)
                move(row, column, height, width);
            else
            {
                mRow = row;
                mColumn = column;
                mHeight = height;
                mWidth = width;
            }
        }
        
        public virtual void OnHotkeyActivated()
        {
        }
    }
}
