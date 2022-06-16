<?xml version="1.0" encoding="windows-1252"?>
<!-- writes article
     param: ext:caller('article') - an article to write

  -->
<xsl:stylesheet
    version="1.0"
    xmlns:ext="urn:gehtsoft-exslt"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" />
    <xsl:namespace-alias stylesheet-prefix="ext" result-prefix="#default"/>
    <xsl:template match="/" >
        <xsl:apply-templates select="ext:caller('curr-member')" />
    </xsl:template>
    <xsl:template match="member" >
<xsl:value-of select="ext:let('transform', ext:get('default-transform', 'no'))" />
<xsl:for-each select="ancestor-or-self::*">
    <xsl:if test="count(./@transform) > 0 and ./@transform!='def'">
        <xsl:value-of select="ext:let('transform', ./@transform)" />
    </xsl:if>
</xsl:for-each>
<html>
<head><title><xsl:value-of select="concat(./@name, ' in ', ../@name)" /></title>
<xsl:value-of select="ext:call('write-scripts.xsl', /)" disable-output-escaping="yes" />
<xsl:value-of select="ext:call('write-css.xsl', /)" disable-output-escaping="yes" />
</head>
<body>
<xsl:value-of select="ext:let('title', concat('element &lt;b&gt;',./@name, '&lt;/b&gt; in ', ../@name))" />
<p><font size="+1"><code><xsl:value-of select="ext:get('title')" disable-output-escaping="yes" /></code></font></p>
        <xsl:value-of select="ext:let('curr-item', .)" />
        <xsl:value-of select="ext:let('help-index-keyword', concat(./@name, ' in ', ../@name))" />
        <xsl:value-of select="ext:let('help-index-name', concat(./@name, ' in ', ../@name))" />
        <xsl:value-of select="ext:let('content-node', ext:caller('content-node'))" />
        <xsl:value-of select="ext:call('write-member-body-common.xsl', /, ext:get('codepage'))" disable-output-escaping="yes" />
</body>
</html>
    </xsl:template>
</xsl:stylesheet>

