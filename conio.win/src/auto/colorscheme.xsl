<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="text" encoding="Windows-1252"/>
    <xsl:template match="/" >
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
        <xsl:for-each select="./color-schemes/class/group">
        #region <xsl:value-of select="./@doc" />
        <xsl:for-each select="./color">
        ///&lt;summary&gt;
        ///<xsl:value-of select="./@doc" />
        ///&lt;/summary&gt;
        ConsoleColor <xsl:value-of select="../@name" /><xsl:value-of select="./@name" />
        {
            get;
        }
        </xsl:for-each>
        #endregion
        </xsl:for-each>
    }

    public class ColorScheme : IColorScheme
    {
        <xsl:for-each select="./color-schemes/class/group">
        #region <xsl:value-of select="./@doc" />
        <xsl:for-each select="./color">
        private ConsoleColor m<xsl:value-of select="../@name" /><xsl:value-of select="./@name" />;
        ///&lt;summary&gt;
        ///<xsl:value-of select="./@doc" />
        ///&lt;/summary&gt;
        public ConsoleColor <xsl:value-of select="../@name" /><xsl:value-of select="./@name" />
        {
            get
            {
                return m<xsl:value-of select="../@name" /><xsl:value-of select="./@name" />;
            }
            set
            {
                m<xsl:value-of select="../@name" /><xsl:value-of select="./@name" /> = value;
            }
        }
        </xsl:for-each>
        #endregion
        </xsl:for-each>

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
                    <xsl:for-each select="./color-schemes/class/group">
                    <xsl:for-each select="./color">
                    <xsl:choose>
                    <xsl:when test="count(./@fg) > 0  and count(./@bg) > 0">
                    mDefault.<xsl:value-of select="../@name" /><xsl:value-of select="./@name" /> = new ConsoleColor(<xsl:value-of select="./@console" />, ConsoleColor.rgb(<xsl:value-of select="./@fg" />), ConsoleColor.rgb(<xsl:value-of select="./@bg" />));
                    </xsl:when>
                    <xsl:otherwise>
                    mDefault.<xsl:value-of select="../@name" /><xsl:value-of select="./@name" /> = new ConsoleColor(<xsl:value-of select="./@console" />);
                    </xsl:otherwise>
                    </xsl:choose>
                    </xsl:for-each>
                    </xsl:for-each>
                }
                return mDefault;
            }
        }
    }
}
    </xsl:template>
</xsl:stylesheet>

