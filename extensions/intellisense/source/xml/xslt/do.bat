msxsl xsd.xml createSchema.xsl > xsd.cs
msxsl xslt.xml createSchema.xsl > xslt.cs
copy xsd.cs + xslt.cs all.cs
del xsd.cs
del xslt.cs
