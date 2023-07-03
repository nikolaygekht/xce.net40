using System;
using gehtsoft.xce.conio.win;

namespace gehtsoft.xce.extension.charmap_impl
{
    internal class UnicodeRangeComboBox : DialogItemComboBox
    {
        internal UnicodeRangeComboBox(int id, int row, int column, int width) : base("", id, row, column, width)
        {
        }

        public override void OnKeyPressed(int scanCode, char character, bool shift, bool ctrl, bool alt)
        {
            base.OnKeyPressed(scanCode, character, shift, ctrl, alt);
            if (!alt && !ctrl && (character >= 'A' && character <= 'Z' || (character >= 'a' && character <= 'z')))
            {
                int item = CurSel + 1;
                int stop;
                if (CurSel < 1)
                    stop = Count;
                else
                    stop = CurSel;
                if (item == Count)
                    item = 0;

                while (item != stop)
                {
                    if (this[item].Label.Length > 0 &&
                        Char.ToUpper(this[item].Label[0]) == Char.ToUpper(character))
                    {
                        CurSel = item;
                        break;
                    }

                    item++;
                    if (item == stop)
                        break;
                    if (item == Count)
                        item = 0;
                }
            }
        }
    }
}

