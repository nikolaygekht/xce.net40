<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="text" encoding="Windows-1252"/>
    <xsl:template match="File" >
        <xsl:value-of select="./@RelativePath" /><xsl:text xml:space="preserve">
</xsl:text>
    </xsl:template>
</xsl:stylesheet>