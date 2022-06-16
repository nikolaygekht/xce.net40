<?xml version="1.0" encoding="windows-1252"?>
<xsl:stylesheet
    version="1.0"
    xmlns:ext="urn:gehtsoft-exslt"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="text" />
    <!-- eliminate ext declaration -->
    <xsl:namespace-alias stylesheet-prefix="ext" result-prefix="#default"/>
    <xsl:template match="/" >
        <xsl:apply-templates select="ext:caller('class-xml')/doxygen/compounddef" />
    </xsl:template>
    <xsl:template match="compounddef">
@class
    @name=<xsl:value-of select="./compoundname/text()" />
    @brief=
    @type=<xsl:value-of select="./@kind" />
    @ingroup=<xsl:value-of select="ext:get('group')" /><xsl:text>&#013;&#010;</xsl:text>
    <xsl:for-each select="./basecompoundref">    @parent=[link=<xsl:value-of select="./text()" />]<xsl:value-of select="./text()" />[/link]<xsl:text>&#013;&#010;</xsl:text></xsl:for-each>

    <xsl:call-template name="process-members">
        <xsl:with-param name="kind">public-attrib</xsl:with-param>
        <xsl:with-param name="scope">instance</xsl:with-param>
        <xsl:with-param name="type">property</xsl:with-param>
        <xsl:with-param name="visibility">public</xsl:with-param>
    </xsl:call-template>
    <xsl:call-template name="process-members">
        <xsl:with-param name="kind">public-func</xsl:with-param>
        <xsl:with-param name="scope">instance</xsl:with-param>
        <xsl:with-param name="type">method</xsl:with-param>
        <xsl:with-param name="visibility">public</xsl:with-param>
    </xsl:call-template>
    <xsl:call-template name="process-members">
        <xsl:with-param name="kind">public-static-func</xsl:with-param>
        <xsl:with-param name="scope">class</xsl:with-param>
        <xsl:with-param name="type">method</xsl:with-param>
        <xsl:with-param name="visibility">public</xsl:with-param>
    </xsl:call-template>

@end
    </xsl:template>

    <xsl:template name="process-members">
        <xsl:param name="kind" />
        <xsl:param name="scope" />
        <xsl:param name="type" />
        <xsl:param name="visibility" />
        <xsl:for-each select="./sectiondef[@kind=$kind]/memberdef">
    @member
        @visibility=<xsl:value-of select="$visibility" />
        @scope=<xsl:value-of select="$scope" />
        @type=<xsl:value-of select="$type" />
        @name=<xsl:value-of select="./name/text()" />
        @divisor=.
        @brief=

        <xsl:value-of select="ext:let('ret', normalize-space(ext:call('process-type.xsl', ./type)))" />
        <xsl:value-of select="ext:let('param', '')" />
        <xsl:value-of select="ext:let('i', 0)" />
        <xsl:for-each select="./param">
            <xsl:if test="ext:get('i')!=0">
                <xsl:value-of select="ext:let('param', concat(ext:get('param'), ', '))" />
            </xsl:if>
            <xsl:value-of select="ext:let('i', ext:get('i') + 1)" />
            <xsl:value-of select="ext:let('param', concat(ext:get('param'), normalize-space(ext:call('process-type.xsl', ./type)), ' ', ./declname/text()))" />
        </xsl:for-each>

        @declaration
            @language=cpp
            @return=<xsl:value-of select="ext:get('ret')" />
            @prefix=<xsl:if test="$scope='class'">static </xsl:if><xsl:if test="./@virt='virtual' or ./@virt='pure-virtual'">virtual </xsl:if>
            @suffix=<xsl:if test="./@virt='pure-virtual'"> = 0</xsl:if>
            @params=<xsl:value-of select="ext:get('param')" />
        @end

        <xsl:for-each select="./param">
        @param
            @name=<xsl:value-of select="./declname/text()" />
        @end
        </xsl:for-each>
    @end
    </xsl:for-each>
</xsl:template>


</xsl:stylesheet>


