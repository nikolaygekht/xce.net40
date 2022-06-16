<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="text" encoding="Windows-1252"/>

    <xsl:template match="/" >
        static XmlSchema m<xsl:value-of select="/schema/@name" /> = null;

        internal static XmlSchema Create<xsl:value-of select="/schema/@name" />()
        {
            if (m<xsl:value-of select="/schema/@name" /> != null)
                return m<xsl:value-of select="/schema/@name" />;


            XmlSchemaTag root, content = null, tag;
            XmlSchemaAttribute attribute;
            <xsl:if test="count(/schema/@default) > 0"><xsl:apply-templates select="/schema/element[./@name = /schema/@default]" />
            content = tag;
            </xsl:if>
            <xsl:apply-templates select="/schema/element[./@name = /schema/@root]" />
            root = tag;

            XmlSchema schema = new XmlSchema(root, content, "<xsl:value-of select="/schema/@namespace" />", "<xsl:value-of select="/schema/@defaultPrefix" />");

            <xsl:choose>
            <xsl:when test="count(/schema/@default) > 0">
                <xsl:for-each select="/schema/element[(./@name != /schema/@root) and ./@name != (/schema/@default)]">
                    <xsl:call-template name="element" />
            schema.Add(tag);
                </xsl:for-each>
            </xsl:when>
            <xsl:otherwise>
                <xsl:for-each select="/schema/element[(./@name != /schema/@root)]">
                    <xsl:call-template name="element" />
            schema.Add(tag);
                </xsl:for-each>
            </xsl:otherwise>
            </xsl:choose>

            m<xsl:value-of select="/schema/@name" /> = schema;

            return schema;
        }
    </xsl:template>

    <xsl:template match="element" name="element" >
            tag = new XmlSchemaTag("<xsl:value-of select="./@name" />", <xsl:choose><xsl:when test="count(./@sort) > 0"><xsl:value-of select="./@sort" /></xsl:when><xsl:otherwise>false</xsl:otherwise></xsl:choose>);
            <xsl:for-each select="./parameter">
            <xsl:choose>
                <xsl:when test="count(./@xpath) > 0 and (./@xpath='yes' or ./@xpath='true')">
            attribute = new XmlSchemaAttribute("<xsl:value-of select="./@name" />", XmlSchemaAttributeType.XPath);
                </xsl:when>
                <xsl:when test="count(./value) > 0">
            attribute = new XmlSchemaAttribute("<xsl:value-of select="./@name" />", XmlSchemaAttributeType.List);
            <xsl:for-each select="./value">
            attribute.AddOption("<xsl:value-of select="./text()" />");
            </xsl:for-each>
                </xsl:when>
                <xsl:otherwise>
            attribute = new XmlSchemaAttribute("<xsl:value-of select="./@name" />", XmlSchemaAttributeType.Default);
                </xsl:otherwise>
            </xsl:choose>
            tag.AddAttribute(attribute);
            </xsl:for-each>
            <xsl:for-each select="./element">
            tag.AddChild("<xsl:value-of select="./@name" />");
            </xsl:for-each>
    </xsl:template>

</xsl:stylesheet>