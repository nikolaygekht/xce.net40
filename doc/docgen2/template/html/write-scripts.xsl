<?xml version="1.0" encoding="windows-1252"?>
<!-- writes scripts to the page header -->
<xsl:stylesheet
    version="1.0"
    xmlns:ext="urn:gehtsoft-exslt"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" />
    <xsl:namespace-alias stylesheet-prefix="ext" result-prefix="#default"/>
    <xsl:template match="/" >
<script language="javascript" type="text/javascript">
function hidediv( divname )
{
    document.getElementById( divname ).style.display = 'none';
}

function showdiv( divname )
{
    document.getElementById( divname ).style.display = 'inline';
}
</script>
    </xsl:template>
</xsl:stylesheet>

