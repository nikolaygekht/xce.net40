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
        <xsl:value-of select="ext:let('transform', ext:caller('transform'))" />
        <xsl:apply-templates select="ext:caller('curr-item')" />
    </xsl:template>
    <xsl:template match="list" >
        <xsl:choose>
            <xsl:when test="./@type='num'">
        <ol class="main">
            <xsl:apply-templates select="./*" />
        </ol>
            </xsl:when>
            <xsl:otherwise>
        <ul class="main">
            <xsl:apply-templates select="./*" />
        </ul>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xsl:template match="list-item" >
        <xsl:value-of select="ext:let('item-text', '')" />
        <xsl:for-each select="./body/p">
            <xsl:if test="string-length(ext:get('item-text')) > 0"><xsl:value-of select="ext:let('item-text', concat(ext:get('item-text'), '&lt;br&gt;'))" /></xsl:if>
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
        <li><xsl:value-of select="ext:get('item-text')" disable-output-escaping="yes" /></li>
    </xsl:template>
</xsl:stylesheet>

