﻿using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.conio.win
{
    internal class StringUtil
    {
        internal static int processHotKey(ref string label)
        {
            int hkposition = -1;
            string newLabel = "";
            bool prevAmp = false;
            
            foreach (char c in label)
            {
                if (c == '&')
                {
                    if (prevAmp)
                    {
                        newLabel += c;
                        prevAmp = false;
                    }
                    else
                    {
                        prevAmp = true;
                    }
                }
                else
                {
                    if (prevAmp)
                    {
                        if (hkposition < 0)
                            hkposition = newLabel.Length;
                        prevAmp = false;
                    }
                    newLabel += c;
                }
            }
            label = newLabel;
            return hkposition;
        }
    }
}
