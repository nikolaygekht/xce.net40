<?xml version="1.0" encoding="windows-1252"?>
<!-- writes body description

     @param ext:caller('curr-item') - a object which has description to write
   -->
<xsl:stylesheet
    version="1.0"
    xmlns:ext="urn:gehtsoft-exslt"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" />
    <xsl:namespace-alias stylesheet-prefix="ext" result-prefix="#default"/>
    <xsl:template match="/" >
        <xsl:apply-templates select="ext:caller('curr-item')" />
    </xsl:template>
    <xsl:template match="example" >
<xsl:value-of select="ext:let('transform', ext:get('default-transform', 'no'))" />
<xsl:for-each select="ancestor-or-self::*">
    <xsl:if test="count(./@transform) > 0 and ./@transform!='def'">
        <xsl:value-of select="ext:let('transform', ./@transform)" />
    </xsl:if>
</xsl:for-each>
        <xsl:choose><xsl:when test="count(./@show)=0 or @show='no'"><xsl:value-of select="ext:let('style1', 'display:inline')" /><xsl:value-of select="ext:let('style2', 'display:none')" /></xsl:when>
                    <xsl:otherwise><xsl:value-of select="ext:let('style1', 'display:none')" /><xsl:value-of select="ext:let('style2', 'display:inline')" /></xsl:otherwise></xsl:choose>
        <xsl:value-of select="ext:let('curr-item', .)" />
        <xsl:value-of select="ext:letglobal('g-example-serial', ext:get('g-example-serial') + 1)" />
        <xsl:element name="div"><xsl:attribute name="id">open<xsl:value-of select="ext:get('g-example-serial')" /></xsl:attribute><xsl:attribute name="style"><xsl:value-of select="ext:get('style1')" /></xsl:attribute>
            <p><xsl:choose><xsl:when test="ext:get('transform')='yes'"><xsl:value-of select="ext:call('write-bbcode.xsl', ext:parsebbcode(./@title))" disable-output-escaping="yes"  /></xsl:when><xsl:otherwise><xsl:value-of select="./@title" /></xsl:otherwise></xsl:choose><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:element name="a">
                <xsl:attribute name="href">javascript: showdiv('close<xsl:value-of select="ext:get('g-example-serial')" />'); hidediv('open<xsl:value-of select="ext:get('g-example-serial')" />');</xsl:attribute>
                <xsl:attribute name="onclick">javascript: showdiv('close<xsl:value-of select="ext:get('g-example-serial')" />'); hidediv('open<xsl:value-of select="ext:get('g-example-serial')" />');</xsl:attribute>[show]</xsl:element></p>
        </xsl:element>
        <xsl:element name="div"><xsl:attribute name="id">close<xsl:value-of select="ext:get('g-example-serial')" /></xsl:attribute><xsl:attribute name="style"><xsl:value-of select="ext:get('style2')" /></xsl:attribute>
            <p><xsl:choose><xsl:when test="ext:get('transform')='yes'"><xsl:value-of select="ext:call('write-bbcode.xsl', ext:parsebbcode(./@title))" disable-output-escaping="yes"  /></xsl:when><xsl:otherwise><xsl:value-of select="./@title" /></xsl:otherwise></xsl:choose><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:element name="a">
                <xsl:attribute name="href">javascript: showdiv('open<xsl:value-of select="ext:get('g-example-serial')" />'); hidediv('close<xsl:value-of select="ext:get('g-example-serial')" />');</xsl:attribute>
                <xsl:attribute name="onclick">javascript: showdiv('open<xsl:value-of select="ext:get('g-example-serial')" />'); hidediv('close<xsl:value-of select="ext:get('g-example-serial')" />');</xsl:attribute>[hide]</xsl:element></p>
            <xsl:value-of select="ext:let('item-text', '')" /><xsl:for-each select="./body/p">
                <xsl:choose>
                    <xsl:when test="ext:caller('transform')='yes'">
                        <xsl:value-of select="ext:let('text', ext:call('write-bbcode.xsl', ext:parsebbcode(./text())))" disable-output-escaping="yes" />
                    </xsl:when>
                    <xsl:otherwise>
                        <xsl:value-of select="ext:let('text', ./text())" disable-output-escaping="yes" />
                    </xsl:otherwise>
                </xsl:choose>
                <xsl:value-of select="ext:let('item-text', concat(ext:get('item-text'), ext:get('text')))" />
            </xsl:for-each>
            <xsl:element name="pre"><xsl:if test="./@gray='yes'"><xsl:attribute name="class">example</xsl:attribute></xsl:if><xsl:value-of select="ext:get('item-text')" disable-output-escaping="yes" /></xsl:element>
        </xsl:element>
    </xsl:template>
</xsl:stylesheet>

