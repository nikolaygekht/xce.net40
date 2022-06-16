using System;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.conio;
using gehtsoft.xce.conio.win;
using gehtsoft.xce.colorer;
using ConsoleColor = gehtsoft.xce.conio.ConsoleColor;

namespace gehtsoft.xce.editor.configuration
{
    /// <summary>
    /// The dialog user interface colors
    /// </summary>
    public class XceColorScheme : IColorScheme
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
        }

        private ConsoleColor mDialogItemEditColorPair;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemEditColorPair
        {
            get
            {
                return mDialogItemEditColorPair;
            }
        }

        private ConsoleColor mDialogItemEditColorError;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemEditColorError
        {
            get
            {
                return mDialogItemEditColorError;
            }
        }

        private ConsoleColor mDialogItemEditColorKeyword;
        ///<summary>
        ///
        ///</summary>
        public ConsoleColor DialogItemEditColorKeyword
        {
            get
            {
                return mDialogItemEditColorKeyword;
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
        }
        #endregion

        #region Editor Main Window
        private ConsoleColor mTextColor;
        /// <summary>
        /// The color of the regular window text
        /// </summary>
        public ConsoleColor TextColor
        {
            get
            {
                return mTextColor;
            }
        }

        private ConsoleColor mCurrentLineTextColor;
        /// <summary>
        /// The color of the regular window text for current line
        /// </summary>
        public ConsoleColor CurrentLineTextColor
        {
            get
            {
                return mCurrentLineTextColor;
            }
        }

        private ConsoleColor mBlockTextColor;
        /// <summary>
        /// The color of the regular window text selected by block
        /// </summary>
        public ConsoleColor BlockTextColor
        {
            get
            {
                return mBlockTextColor;
            }
        }

        private ConsoleColor mCurrentLineBlockTextColor;
        /// <summary>
        /// The color of the regular window text inside the block, in current line
        /// </summary>
        public ConsoleColor CurrentLineBlockTextColor
        {
            get
            {
                return mCurrentLineBlockTextColor;
            }
        }

        private ConsoleColor mStatusLineColor;
        /// <summary>
        /// The color of the status line
        /// </summary>
        public ConsoleColor StatusLineColor
        {
            get
            {
                return mStatusLineColor;
            }
        }

        private ConsoleColor mSpellCheckErrorColor;
        /// <summary>
        /// The spell check error color
        /// </summary>
        public ConsoleColor SpellCheckErrorColor
        {
            get
            {
                return mSpellCheckErrorColor;
            }
        }

        private ConsoleColor mPairHighlightColor;

        public ConsoleColor PairHighlightColor
        {
            get
            {
                return mPairHighlightColor;
            }
        }

        private ConsoleColor mSearchHighlight;

        public ConsoleColor SearchHighlight
        {
            get
            {
                return mSearchHighlight;
            }
        }


        #endregion

        private static ConsoleColor GetColor(StyledRegion r)
        {
            return new ConsoleColor(r.ConsoleColor, r.ForegroundColor, r.BackgroundColor, r.Style);
        }

        internal XceColorScheme(ColorerFactory colorerFactory)
        {
            mMenuItem = GetColor(colorerFactory.FindStyledRegion("xce:MenuItem"));
            mMenuItemSelected = GetColor(colorerFactory.FindStyledRegion("xce:MenuItemSelected"));
            mMenuDisabledItem = GetColor(colorerFactory.FindStyledRegion("xce:MenuDisabledItem"));
            mMenuDisabledItemSelected = GetColor(colorerFactory.FindStyledRegion("xce:MenuDisabledItemSelected"));
            mMenuHotKey = GetColor(colorerFactory.FindStyledRegion("xce:MenuHotKey"));
            mMenuHotKeySelected = GetColor(colorerFactory.FindStyledRegion("xce:MenuHotKeySelected"));

            mDialogBorder = GetColor(colorerFactory.FindStyledRegion("xce:DialogBorder"));
            mDialogItemLabelColor = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemLabelColor"));
            mDialogItemLabelHotKey = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemLabelHotKey"));
            mDialogItemCheckBoxColor = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemCheckBoxColor"));
            mDialogItemCheckBoxColorFocused = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemCheckBoxColorFocused"));
            mDialogItemCheckBoxHotKey = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemCheckBoxHotKey"));
            mDialogItemCheckBoxHotKeyFocused = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemCheckBoxHotKeyFocused"));
            mDialogItemCheckBoxColorDisabled = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemCheckBoxColorDisabled"));
            mDialogItemButtonColor = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemButtonColor"));
            mDialogItemButtonColorDisabled = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemButtonColorDisabled"));
            mDialogItemButtonColorFocused = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemButtonColorFocused"));
            mDialogItemButtonHotKey = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemButtonHotKey"));
            mDialogItemButtonHotKeyFocused = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemButtonHotKeyFocused"));
            mDialogItemEditColor = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemEditColor"));
            mDialogItemEditColorDisabled = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemEditColorDisabled"));
            mDialogItemEditColorFocused = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemEditColorFocused"));
            mDialogItemEditSelection = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemEditSelection"));
            mDialogItemEditSelectionFocused = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemEditSelectionFocused"));
            mDialogItemListBoxColor = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemListBoxColor"));
            mDialogItemListBoxColorDisabled = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemListBoxColorDisabled"));
            mDialogItemListBoxColorFocused = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemListBoxColorFocused"));
            mDialogItemListBoxSelection = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemListBoxSelection"));
            mDialogItemListBoxSelectionFocused = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemListBoxSelectionFocused"));
            mDialogItemListBoxSelectionFocusedHighlighted = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemListBoxSelectionFocusedHighlighted"));
            mDialogItemListBoxSelectionHighlighted = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemListBoxSelectionHighlighted"));
            mDialogItemEditColorPair = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemEditColorPair"));
            mDialogItemEditColorError = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemEditColorError"));
            mDialogItemEditColorKeyword = GetColor(colorerFactory.FindStyledRegion("xce:DialogItemEditColorKeyword"));

            mTextColor = GetColor(colorerFactory.FindStyledRegion("xce:TextColor"));
            mCurrentLineTextColor = GetColor(colorerFactory.FindStyledRegion("xce:CurrentLineTextColor"));
            mBlockTextColor = GetColor(colorerFactory.FindStyledRegion("xce:BlockTextColor"));
            mCurrentLineBlockTextColor = GetColor(colorerFactory.FindStyledRegion("xce:CurrentLineBlockTextColor"));
            mStatusLineColor = GetColor(colorerFactory.FindStyledRegion("xce:StatusLineColor"));
            mSpellCheckErrorColor = GetColor(colorerFactory.FindStyledRegion("xce:SpellCheckErrorColor"));
            mPairHighlightColor = GetColor(colorerFactory.FindStyledRegion("xce:PairHighlightColor"));
            mSearchHighlight = GetColor(colorerFactory.FindStyledRegion("xce:SearchHighlight"));
        }
    }
}
