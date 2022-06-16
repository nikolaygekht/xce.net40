
//------------------------------------------------------------------------
//This is auto-generated code. Do NOT modify it.
//Modify ./auto/colorscheme.xml and ./auto/colorscheme.xslt instead!!!
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace gehtsoft.xce.conio.win
{
    public interface IColorScheme
    {
        
        #region Menu colors
        ///<summary>
        ///
        ///</summary>
        ConsoleColor MenuItem
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor MenuItemSelected
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor MenuDisabledItem
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor MenuDisabledItemSelected
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor MenuHotKey
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor MenuHotKeySelected
        {
            get;
        }
        
        #endregion
        
        #region Dialog box colors
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogBorder
        {
            get;
        }
        
        #endregion
        
        #region Dialog box label colors
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemLabelColor
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemLabelHotKey
        {
            get;
        }
        
        #endregion
        
        #region Dialog box check box label colors
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemCheckBoxColor
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemCheckBoxColorFocused
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemCheckBoxHotKey
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemCheckBoxHotKeyFocused
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemCheckBoxColorDisabled
        {
            get;
        }
        
        #endregion
        
        #region Dialog box button colors
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemButtonColor
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemButtonColorDisabled
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemButtonColorFocused
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemButtonHotKey
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemButtonHotKeyFocused
        {
            get;
        }
        
        #endregion
        
        #region Dialog box edit color
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemEditColor
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemEditColorDisabled
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemEditColorFocused
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemEditSelection
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemEditSelectionFocused
        {
            get;
        }
        
        #endregion
        
        #region Dialog box listbox color
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemListBoxColor
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemListBoxColorDisabled
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemListBoxColorFocused
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemListBoxSelection
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemListBoxSelectionFocused
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemListBoxSelectionHighlighted
        {
            get;
        }
        
        ///<summary>
        ///
        ///</summary>
        ConsoleColor DialogItemListBoxSelectionFocusedHighlighted
        {
            get;
        }
        
        #endregion
        
    }

    public class ColorScheme : IColorScheme
    {
        
        #region Menu colors
        private ConsoleColor mMenuItem;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor MenuItem
        {
            get
            {
                return mMenuItem;
            }
            set
            {
                mMenuItem = value;
            }
        }
        
        private ConsoleColor mMenuItemSelected;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor MenuItemSelected
        {
            get
            {
                return mMenuItemSelected;
            }
            set
            {
                mMenuItemSelected = value;
            }
        }
        
        private ConsoleColor mMenuDisabledItem;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor MenuDisabledItem
        {
            get
            {
                return mMenuDisabledItem;
            }
            set
            {
                mMenuDisabledItem = value;
            }
        }
        
        private ConsoleColor mMenuDisabledItemSelected;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor MenuDisabledItemSelected
        {
            get
            {
                return mMenuDisabledItemSelected;
            }
            set
            {
                mMenuDisabledItemSelected = value;
            }
        }
        
        private ConsoleColor mMenuHotKey;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor MenuHotKey
        {
            get
            {
                return mMenuHotKey;
            }
            set
            {
                mMenuHotKey = value;
            }
        }
        
        private ConsoleColor mMenuHotKeySelected;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor MenuHotKeySelected
        {
            get
            {
                return mMenuHotKeySelected;
            }
            set
            {
                mMenuHotKeySelected = value;
            }
        }
        
        #endregion
        
        #region Dialog box colors
        private ConsoleColor mDialogBorder;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogBorder
        {
            get
            {
                return mDialogBorder;
            }
            set
            {
                mDialogBorder = value;
            }
        }
        
        #endregion
        
        #region Dialog box label colors
        private ConsoleColor mDialogItemLabelColor;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemLabelColor
        {
            get
            {
                return mDialogItemLabelColor;
            }
            set
            {
                mDialogItemLabelColor = value;
            }
        }
        
        private ConsoleColor mDialogItemLabelHotKey;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemLabelHotKey
        {
            get
            {
                return mDialogItemLabelHotKey;
            }
            set
            {
                mDialogItemLabelHotKey = value;
            }
        }
        
        #endregion
        
        #region Dialog box check box label colors
        private ConsoleColor mDialogItemCheckBoxColor;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemCheckBoxColor
        {
            get
            {
                return mDialogItemCheckBoxColor;
            }
            set
            {
                mDialogItemCheckBoxColor = value;
            }
        }
        
        private ConsoleColor mDialogItemCheckBoxColorFocused;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemCheckBoxColorFocused
        {
            get
            {
                return mDialogItemCheckBoxColorFocused;
            }
            set
            {
                mDialogItemCheckBoxColorFocused = value;
            }
        }
        
        private ConsoleColor mDialogItemCheckBoxHotKey;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemCheckBoxHotKey
        {
            get
            {
                return mDialogItemCheckBoxHotKey;
            }
            set
            {
                mDialogItemCheckBoxHotKey = value;
            }
        }
        
        private ConsoleColor mDialogItemCheckBoxHotKeyFocused;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemCheckBoxHotKeyFocused
        {
            get
            {
                return mDialogItemCheckBoxHotKeyFocused;
            }
            set
            {
                mDialogItemCheckBoxHotKeyFocused = value;
            }
        }
        
        private ConsoleColor mDialogItemCheckBoxColorDisabled;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemCheckBoxColorDisabled
        {
            get
            {
                return mDialogItemCheckBoxColorDisabled;
            }
            set
            {
                mDialogItemCheckBoxColorDisabled = value;
            }
        }
        
        #endregion
        
        #region Dialog box button colors
        private ConsoleColor mDialogItemButtonColor;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemButtonColor
        {
            get
            {
                return mDialogItemButtonColor;
            }
            set
            {
                mDialogItemButtonColor = value;
            }
        }
        
        private ConsoleColor mDialogItemButtonColorDisabled;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemButtonColorDisabled
        {
            get
            {
                return mDialogItemButtonColorDisabled;
            }
            set
            {
                mDialogItemButtonColorDisabled = value;
            }
        }
        
        private ConsoleColor mDialogItemButtonColorFocused;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemButtonColorFocused
        {
            get
            {
                return mDialogItemButtonColorFocused;
            }
            set
            {
                mDialogItemButtonColorFocused = value;
            }
        }
        
        private ConsoleColor mDialogItemButtonHotKey;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemButtonHotKey
        {
            get
            {
                return mDialogItemButtonHotKey;
            }
            set
            {
                mDialogItemButtonHotKey = value;
            }
        }
        
        private ConsoleColor mDialogItemButtonHotKeyFocused;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemButtonHotKeyFocused
        {
            get
            {
                return mDialogItemButtonHotKeyFocused;
            }
            set
            {
                mDialogItemButtonHotKeyFocused = value;
            }
        }
        
        #endregion
        
        #region Dialog box edit color
        private ConsoleColor mDialogItemEditColor;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemEditColor
        {
            get
            {
                return mDialogItemEditColor;
            }
            set
            {
                mDialogItemEditColor = value;
            }
        }
        
        private ConsoleColor mDialogItemEditColorDisabled;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemEditColorDisabled
        {
            get
            {
                return mDialogItemEditColorDisabled;
            }
            set
            {
                mDialogItemEditColorDisabled = value;
            }
        }
        
        private ConsoleColor mDialogItemEditColorFocused;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemEditColorFocused
        {
            get
            {
                return mDialogItemEditColorFocused;
            }
            set
            {
                mDialogItemEditColorFocused = value;
            }
        }
        
        private ConsoleColor mDialogItemEditSelection;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemEditSelection
        {
            get
            {
                return mDialogItemEditSelection;
            }
            set
            {
                mDialogItemEditSelection = value;
            }
        }
        
        private ConsoleColor mDialogItemEditSelectionFocused;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemEditSelectionFocused
        {
            get
            {
                return mDialogItemEditSelectionFocused;
            }
            set
            {
                mDialogItemEditSelectionFocused = value;
            }
        }
        
        #endregion
        
        #region Dialog box listbox color
        private ConsoleColor mDialogItemListBoxColor;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemListBoxColor
        {
            get
            {
                return mDialogItemListBoxColor;
            }
            set
            {
                mDialogItemListBoxColor = value;
            }
        }
        
        private ConsoleColor mDialogItemListBoxColorDisabled;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemListBoxColorDisabled
        {
            get
            {
                return mDialogItemListBoxColorDisabled;
            }
            set
            {
                mDialogItemListBoxColorDisabled = value;
            }
        }
        
        private ConsoleColor mDialogItemListBoxColorFocused;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemListBoxColorFocused
        {
            get
            {
                return mDialogItemListBoxColorFocused;
            }
            set
            {
                mDialogItemListBoxColorFocused = value;
            }
        }
        
        private ConsoleColor mDialogItemListBoxSelection;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemListBoxSelection
        {
            get
            {
                return mDialogItemListBoxSelection;
            }
            set
            {
                mDialogItemListBoxSelection = value;
            }
        }
        
        private ConsoleColor mDialogItemListBoxSelectionFocused;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemListBoxSelectionFocused
        {
            get
            {
                return mDialogItemListBoxSelectionFocused;
            }
            set
            {
                mDialogItemListBoxSelectionFocused = value;
            }
        }
        
        private ConsoleColor mDialogItemListBoxSelectionHighlighted;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemListBoxSelectionHighlighted
        {
            get
            {
                return mDialogItemListBoxSelectionHighlighted;
            }
            set
            {
                mDialogItemListBoxSelectionHighlighted = value;
            }
        }
        
        private ConsoleColor mDialogItemListBoxSelectionFocusedHighlighted;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemListBoxSelectionFocusedHighlighted
        {
            get
            {
                return mDialogItemListBoxSelectionFocusedHighlighted;
            }
            set
            {
                mDialogItemListBoxSelectionFocusedHighlighted = value;
            }
        }
        
        #endregion
        

        public ColorScheme()
        {
        }

        private static ColorScheme mDefault = null;

        public static IColorScheme Default
        {
            get
            {
                if (mDefault == null)
                {
                    mDefault = new ColorScheme();
                    
                    mDefault.MenuItem = new ConsoleColor(0x30, ConsoleColor.rgb(0, 0, 0), ConsoleColor.rgb(173, 207, 248));
                    
                    mDefault.MenuItemSelected = new ConsoleColor(0x0f, ConsoleColor.rgb(0, 0, 0), ConsoleColor.rgb(255, 255, 255));
                    
                    mDefault.MenuDisabledItem = new ConsoleColor(0x37, ConsoleColor.rgb(128, 128, 128), ConsoleColor.rgb(173, 207, 248));
                    
                    mDefault.MenuDisabledItemSelected = new ConsoleColor(0x07, ConsoleColor.rgb(128, 128, 128), ConsoleColor.rgb(255, 255, 255));
                    
                    mDefault.MenuHotKey = new ConsoleColor(0x3e, ConsoleColor.rgb(196, 196, 0), ConsoleColor.rgb(173, 207, 248));
                    
                    mDefault.MenuHotKeySelected = new ConsoleColor(0x0e, ConsoleColor.rgb(196, 196, 0), ConsoleColor.rgb(255, 255, 255));
                    
                    mDefault.DialogBorder = new ConsoleColor(0x70);
                    
                    mDefault.DialogItemLabelColor = new ConsoleColor(0x70);
                    
                    mDefault.DialogItemLabelHotKey = new ConsoleColor(0x7e);
                    
                    mDefault.DialogItemCheckBoxColor = new ConsoleColor(0x70);
                    
                    mDefault.DialogItemCheckBoxColorFocused = new ConsoleColor(0x70);
                    
                    mDefault.DialogItemCheckBoxHotKey = new ConsoleColor(0x7e);
                    
                    mDefault.DialogItemCheckBoxHotKeyFocused = new ConsoleColor(0x7e);
                    
                    mDefault.DialogItemCheckBoxColorDisabled = new ConsoleColor(0x78);
                    
                    mDefault.DialogItemButtonColor = new ConsoleColor(0x70);
                    
                    mDefault.DialogItemButtonColorDisabled = new ConsoleColor(0x78);
                    
                    mDefault.DialogItemButtonColorFocused = new ConsoleColor(0x30);
                    
                    mDefault.DialogItemButtonHotKey = new ConsoleColor(0x7e);
                    
                    mDefault.DialogItemButtonHotKeyFocused = new ConsoleColor(0x3e);
                    
                    mDefault.DialogItemEditColor = new ConsoleColor(0x37);
                    
                    mDefault.DialogItemEditColorDisabled = new ConsoleColor(0x38);
                    
                    mDefault.DialogItemEditColorFocused = new ConsoleColor(0x30);
                    
                    mDefault.DialogItemEditSelection = new ConsoleColor(0x1f);
                    
                    mDefault.DialogItemEditSelectionFocused = new ConsoleColor(0x9f);
                    
                    mDefault.DialogItemListBoxColor = new ConsoleColor(0x37);
                    
                    mDefault.DialogItemListBoxColorDisabled = new ConsoleColor(0x38);
                    
                    mDefault.DialogItemListBoxColorFocused = new ConsoleColor(0x30);
                    
                    mDefault.DialogItemListBoxSelection = new ConsoleColor(0x1f);
                    
                    mDefault.DialogItemListBoxSelectionFocused = new ConsoleColor(0x9f);
                    
                    mDefault.DialogItemListBoxSelectionHighlighted = new ConsoleColor(0xb7);
                    
                    mDefault.DialogItemListBoxSelectionFocusedHighlighted = new ConsoleColor(0x3f);
                    
                }
                return mDefault;
            }
        }
    }
}
    