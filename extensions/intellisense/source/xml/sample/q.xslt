<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="text" encoding="Windows-1252"/>
    <xsl:template match="/" >
    <xsl:for-each select="/regions/region">
        private SyntaxRegion xml_<xsl:value-of select="translate(./@name, '.', '_')" />;
    </xsl:for-each>
    <xsl:for-each select="/regions/region">
        xml_<xsl:value-of select="translate(./@name, '.', '_')" /> = colorer.FindSyntaxRegion("xml:<xsl:value-of select="./@name"/>");
    </xsl:for-each>
    </xsl:template>
</xsl:stylesheet>