<?xml version="1.0" encoding="windows-1252"?>
<!-- writes declaration
     param: ext:caller('curr-item') - a declaration to write

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
    <xsl:template match="declaration" >
<!-- collect declaration data -->
<xsl:value-of select="ext:let('prefix', ./@prefix)" />
<xsl:value-of select="ext:let('return', ./@return)" />
<xsl:value-of select="ext:let('name', ./@name)" />
<xsl:value-of select="ext:let('name-suffix', ./@name-suffix)" />
<xsl:value-of select="ext:let('params', ./@params)" />
<xsl:value-of select="ext:let('suffix', ./@suffix)" />
<xsl:if test="ext:get('transform')='yes'" >
    <xsl:value-of select="ext:let('prefix', ext:call('write-bbcode.xsl', ext:parsebbcode(ext:get('prefix')))) "/>
    <xsl:value-of select="ext:let('return', ext:call('write-bbcode.xsl', ext:parsebbcode(ext:get('return')))) "/>
    <xsl:value-of select="ext:let('name',  ext:call('write-bbcode.xsl', ext:parsebbcode(ext:get('name')))) "/>
    <xsl:value-of select="ext:let('name-suffix',  ext:call('write-bbcode.xsl', ext:parsebbcode(ext:get('name-suffix')))) "/>
    <xsl:value-of select="ext:let('params',  ext:call('write-bbcode.xsl', ext:parsebbcode(ext:get('params')))) "/>
    <xsl:value-of select="ext:let('suffix',  ext:call('write-bbcode.xsl', ext:parsebbcode(ext:get('suffix')))) "/>
</xsl:if>
<xsl:if test="string-length(ext:get('prefix'))"><xsl:value-of select="ext:let('prefix', concat(ext:get('prefix'), '&amp;nbsp;')) "/></xsl:if>
<!-- detect language -->
<xsl:choose>
    <xsl:when test="./@language='cpp'"><xsl:value-of select="ext:let('language', 'C++')" /></xsl:when>
    <xsl:when test="./@language='java'"><xsl:value-of select="ext:let('language', 'Java')" /></xsl:when>
    <xsl:when test="./@language='cs'"><xsl:value-of select="ext:let('language', 'C#')" /></xsl:when>
</xsl:choose>
<!-- build body depend on the member type -->
<xsl:choose>
    <!-- method -->
    <xsl:when test="../@type='method' or ../@type='constructor'">
        <xsl:value-of select="ext:let('decl-body', concat('&lt;b&gt;', ext:get('name'), ext:get('name-suffix'), '&lt;/b&gt; (', ext:get('params'), ') ', ext:get('suffix'))) " />
    </xsl:when>
    <!-- field or property -->
    <xsl:otherwise>
        <xsl:choose>
            <!-- indexed property/field -->
            <xsl:when test="string-length(ext:get('params')) > 0">
                <xsl:value-of select="ext:let('decl-body', concat('&lt;b&gt;', ext:get('name'), ext:get('name-suffix'), '&lt;/b&gt;[', ext:get('params'), '] ', ext:get('suffix'))) " />
            </xsl:when>
            <!-- indexed property/field -->
            <xsl:otherwise>
                <xsl:value-of select="ext:let('decl-body', concat('&lt;b&gt;', ext:get('name'), ext:get('name-suffix'), '&lt;/b&gt;', ext:get('suffix'))) " />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:otherwise>
</xsl:choose>
<!-- write declaration -->
<table class="decl">
 <tr><td colspan="2"><xsl:value-of select="ext:get('language')" /></td></tr>
 <tr><td width="1%" nowrap="" valign="top"><code><xsl:value-of select="concat(ext:get('prefix'), ext:get('return'), '&amp;nbsp;')" disable-output-escaping="yes" /></code></td>
 <td width="99%" valign="top"><code><xsl:value-of select="ext:get('decl-body')" disable-output-escaping="yes" /></code></td></tr>
 <xsl:if test="count(./body/*)>0">
    <xsl:value-of select="ext:let('curr-item', .)" />
    <tr><td width="1%"></td><td><pre><xsl:value-of select="ext:call('write-description.xsl', /)" disable-output-escaping="yes" /></pre></td></tr>
 </xsl:if>
</table>
    </xsl:template>
</xsl:stylesheet>

