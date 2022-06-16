
        static XmlSchema mXsd = null;

        internal static XmlSchema CreateXsd()
        {
            if (mXsd != null)
                return mXsd;


            XmlSchemaTag root, content = null, tag;
            XmlSchemaAttribute attribute;

            tag = new XmlSchemaTag("schema", false);

            attribute = new XmlSchemaAttribute("targetNamespace", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("attributeFormDefault", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("elementFormDefault", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("blockDefault", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("finalDefault", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("version", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("include");

            tag.AddChild("import");

            tag.AddChild("redefine");

            tag.AddChild("annotation");

            tag.AddChild("simpleType");

            tag.AddChild("complexType");

            tag.AddChild("group");

            tag.AddChild("attributeGroup");

            tag.AddChild("element");

            tag.AddChild("attribute");

            root = tag;

            XmlSchema schema = new XmlSchema(root, content, "http://www.w3.org/2001/XMLSchema", "xs");


            tag = new XmlSchemaTag("include", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("schemaLocation", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            schema.Add(tag);

            tag = new XmlSchemaTag("import", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("namespace", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("schemaLocation", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            schema.Add(tag);

            tag = new XmlSchemaTag("refedine", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("schemaLocation", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("simpleType");

            tag.AddChild("complexType");

            tag.AddChild("group");

            tag.AddChild("attributeGroup");

            schema.Add(tag);

            tag = new XmlSchemaTag("annotation", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("appinfo");

            tag.AddChild("documentation");

            schema.Add(tag);

            tag = new XmlSchemaTag("appinfo", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("documentation", false);

            attribute = new XmlSchemaAttribute("xml:lang", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("simpleType", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("restriction");

            tag.AddChild("list");

            tag.AddChild("union");

            schema.Add(tag);

            tag = new XmlSchemaTag("complexType", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("abstract", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("mixed", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("simpleContent");

            tag.AddChild("complexContent");

            tag.AddChild("group");

            tag.AddChild("all");

            tag.AddChild("choice");

            tag.AddChild("sequence");

            tag.AddChild("attribute");

            tag.AddChild("attributeGroup");

            tag.AddChild("anyAttribute");

            schema.Add(tag);

            tag = new XmlSchemaTag("group", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("ref", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("maxOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("1");

            attribute.AddOption("2");

            attribute.AddOption("3");

            attribute.AddOption("unbounded");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("minOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("0");

            attribute.AddOption("1");

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("all");

            tag.AddChild("choice");

            tag.AddChild("sequence");

            schema.Add(tag);

            tag = new XmlSchemaTag("attributeGroup", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("ref", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("attribute");

            tag.AddChild("attributeGroup");

            tag.AddChild("anyAttribute");

            schema.Add(tag);

            tag = new XmlSchemaTag("element", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("ref", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("type", XmlSchemaAttributeType.List);

            attribute.AddOption("$ns$string");

            attribute.AddOption("$ns$normalizedString");

            attribute.AddOption("$ns$token");

            attribute.AddOption("$ns$language");

            attribute.AddOption("$ns$boolean");

            attribute.AddOption("$ns$base64Binary");

            attribute.AddOption("$ns$hexBinary");

            attribute.AddOption("$ns$float");

            attribute.AddOption("$ns$double");

            attribute.AddOption("$ns$decimal");

            attribute.AddOption("$ns$integer");

            attribute.AddOption("$ns$anyURI");

            attribute.AddOption("$ns$QName");

            attribute.AddOption("$ns$duration");

            attribute.AddOption("$ns$dateTime");

            attribute.AddOption("$ns$date");

            attribute.AddOption("$ns$time");

            attribute.AddOption("$ns$nonPositiveInteger");

            attribute.AddOption("$ns$negativeInteger");

            attribute.AddOption("$ns$nonNegativeInteger");

            attribute.AddOption("$ns$positiveInteger");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("substitutionGroup", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("default", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("fixed", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("form", XmlSchemaAttributeType.List);

            attribute.AddOption("qualified");

            attribute.AddOption("unqualified");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("maxOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("1");

            attribute.AddOption("2");

            attribute.AddOption("3");

            attribute.AddOption("unbounded");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("minOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("0");

            attribute.AddOption("1");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("nillable", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("abstract", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("block", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("final", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("simpleType");

            tag.AddChild("complexType");

            tag.AddChild("unique");

            tag.AddChild("key");

            tag.AddChild("keyref");

            schema.Add(tag);

            tag = new XmlSchemaTag("attribute", false);

            attribute = new XmlSchemaAttribute("default", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("fixed", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("form", XmlSchemaAttributeType.List);

            attribute.AddOption("qualified");

            attribute.AddOption("unqualified");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("ref", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("type", XmlSchemaAttributeType.List);

            attribute.AddOption("$ns$string");

            attribute.AddOption("$ns$normalizedString");

            attribute.AddOption("$ns$token");

            attribute.AddOption("$ns$language");

            attribute.AddOption("$ns$boolean");

            attribute.AddOption("$ns$base64Binary");

            attribute.AddOption("$ns$hexBinary");

            attribute.AddOption("$ns$float");

            attribute.AddOption("$ns$double");

            attribute.AddOption("$ns$decimal");

            attribute.AddOption("$ns$integer");

            attribute.AddOption("$ns$anyURI");

            attribute.AddOption("$ns$QName");

            attribute.AddOption("$ns$duration");

            attribute.AddOption("$ns$dateTime");

            attribute.AddOption("$ns$date");

            attribute.AddOption("$ns$time");

            attribute.AddOption("$ns$nonPositiveInteger");

            attribute.AddOption("$ns$negativeInteger");

            attribute.AddOption("$ns$nonNegativeInteger");

            attribute.AddOption("$ns$positiveInteger");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("use", XmlSchemaAttributeType.List);

            attribute.AddOption("optional");

            attribute.AddOption("prohibited");

            attribute.AddOption("required");

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("simpleType");

            schema.Add(tag);

            tag = new XmlSchemaTag("list", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("itemType", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("simpleType");

            schema.Add(tag);

            tag = new XmlSchemaTag("union", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("memberTypes", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("all", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("maxOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("1");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("minOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("0");

            attribute.AddOption("1");

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("element");

            schema.Add(tag);

            tag = new XmlSchemaTag("choice", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("maxOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("1");

            attribute.AddOption("2");

            attribute.AddOption("3");

            attribute.AddOption("unbounded");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("minOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("0");

            attribute.AddOption("1");

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("element");

            tag.AddChild("group");

            tag.AddChild("choice");

            tag.AddChild("sequence");

            tag.AddChild("any");

            schema.Add(tag);

            tag = new XmlSchemaTag("sequence", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("maxOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("1");

            attribute.AddOption("2");

            attribute.AddOption("3");

            attribute.AddOption("unbounded");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("minOccurs", XmlSchemaAttributeType.List);

            attribute.AddOption("0");

            attribute.AddOption("1");

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("element");

            tag.AddChild("group");

            tag.AddChild("choice");

            tag.AddChild("sequence");

            tag.AddChild("any");

            schema.Add(tag);

            tag = new XmlSchemaTag("unique", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("selector");

            tag.AddChild("field");

            schema.Add(tag);

            tag = new XmlSchemaTag("key", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("selector");

            tag.AddChild("field");

            schema.Add(tag);

            tag = new XmlSchemaTag("keyref", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("refer", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("selector");

            tag.AddChild("field");

            schema.Add(tag);

            tag = new XmlSchemaTag("any", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("maxOccurs", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("minOccurs", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("namespace", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("processContents", XmlSchemaAttributeType.List);

            attribute.AddOption("lax");

            attribute.AddOption("skip");

            attribute.AddOption("strict");

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            schema.Add(tag);

            tag = new XmlSchemaTag("anyAttribute", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("maxOccurs", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("minOccurs", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("namespace", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("processContents", XmlSchemaAttributeType.List);

            attribute.AddOption("lax");

            attribute.AddOption("skip");

            attribute.AddOption("strict");

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            schema.Add(tag);

            tag = new XmlSchemaTag("selector", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("xpath", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("field", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("xpath", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            schema.Add(tag);

            tag = new XmlSchemaTag("simpleContent", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("restriction");

            tag.AddChild("extension");

            schema.Add(tag);

            tag = new XmlSchemaTag("restriction", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("base", XmlSchemaAttributeType.List);

            attribute.AddOption("$ns$string");

            attribute.AddOption("$ns$normalizedString");

            attribute.AddOption("$ns$token");

            attribute.AddOption("$ns$language");

            attribute.AddOption("$ns$boolean");

            attribute.AddOption("$ns$base64Binary");

            attribute.AddOption("$ns$hexBinary");

            attribute.AddOption("$ns$float");

            attribute.AddOption("$ns$double");

            attribute.AddOption("$ns$decimal");

            attribute.AddOption("$ns$integer");

            attribute.AddOption("$ns$anyURI");

            attribute.AddOption("$ns$QName");

            attribute.AddOption("$ns$duration");

            attribute.AddOption("$ns$dateTime");

            attribute.AddOption("$ns$date");

            attribute.AddOption("$ns$time");

            attribute.AddOption("$ns$nonPositiveInteger");

            attribute.AddOption("$ns$negativeInteger");

            attribute.AddOption("$ns$nonNegativeInteger");

            attribute.AddOption("$ns$positiveInteger");

            tag.AddAttribute(attribute);

            tag.AddChild("minExclusive");

            tag.AddChild("minInclusive");

            tag.AddChild("maxExclusive");

            tag.AddChild("maxInclusive");

            tag.AddChild("totalDigits");

            tag.AddChild("fractionDigits");

            tag.AddChild("length");

            tag.AddChild("minLength");

            tag.AddChild("maxLength");

            tag.AddChild("enumeration");

            tag.AddChild("whiteSpace");

            tag.AddChild("pattern");

            schema.Add(tag);

            tag = new XmlSchemaTag("extension", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("base", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("annotation");

            tag.AddChild("group");

            tag.AddChild("all");

            tag.AddChild("choice");

            tag.AddChild("sequence");

            tag.AddChild("attribute");

            tag.AddChild("attributeGroup");

            tag.AddChild("anyAttribute");

            schema.Add(tag);

            tag = new XmlSchemaTag("minExclusive", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("minInclusive", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("maxExclusive", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("maxInclusive", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("totalDigits", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("fractionDigits", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("length", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("minLength", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("maxLength", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("enumeration", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("whiteSpace", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("pattern", false);

            schema.Add(tag);


            mXsd = schema;

            return schema;
        }

        static XmlSchema mXslt = null;

        internal static XmlSchema CreateXslt()
        {
            if (mXslt != null)
                return mXslt;


            XmlSchemaTag root, content = null, tag;
            XmlSchemaAttribute attribute;

            tag = new XmlSchemaTag("content", false);

            tag.AddChild("apply-imports");

            tag.AddChild("apply-templates");

            tag.AddChild("call-template");

            tag.AddChild("choose");

            tag.AddChild("if");

            tag.AddChild("for-each");

            tag.AddChild("copy");

            tag.AddChild("copy-of");

            tag.AddChild("decimal-format");

            tag.AddChild("element");

            tag.AddChild("number");

            tag.AddChild("text");

            tag.AddChild("value-of");

            content = tag;

            tag = new XmlSchemaTag("stylesheet", false);

            attribute = new XmlSchemaAttribute("id", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("version", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("extension-element-prefixes", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("extension-element-prefixes", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("template");

            tag.AddChild("import");

            tag.AddChild("include");

            tag.AddChild("namespace-alias");

            tag.AddChild("output");

            tag.AddChild("variable");

            tag.AddChild("param");

            root = tag;

            XmlSchema schema = new XmlSchema(root, content, "http://www.w3.org/1999/XSL/Transform", "xsl");


            tag = new XmlSchemaTag("import", false);

            attribute = new XmlSchemaAttribute("href", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("include", false);

            attribute = new XmlSchemaAttribute("href", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("namespace-alias", false);

            attribute = new XmlSchemaAttribute("stylesheet-prefix", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("result-prefix", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("output", false);

            attribute = new XmlSchemaAttribute("method", XmlSchemaAttributeType.List);

            attribute.AddOption("xml");

            attribute.AddOption("html");

            attribute.AddOption("text");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("version", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("encoding", XmlSchemaAttributeType.List);

            attribute.AddOption("us-ascii");

            attribute.AddOption("windows-1251");

            attribute.AddOption("windows-1252");

            attribute.AddOption("utf-7");

            attribute.AddOption("utf-8");

            attribute.AddOption("iso-8859-1");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("omit-xml-declaration", XmlSchemaAttributeType.List);

            attribute.AddOption("yes");

            attribute.AddOption("no");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("standalone", XmlSchemaAttributeType.List);

            attribute.AddOption("yes");

            attribute.AddOption("no");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("doctype-public", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("doctype-system", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("cdata-section-elements", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("indent", XmlSchemaAttributeType.List);

            attribute.AddOption("yes");

            attribute.AddOption("no");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("media-type", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("template", false);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("match", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("pattern", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("mode", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("priority", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("variable");

            tag.AddChild("param");

            tag.AddChild("apply-imports");

            tag.AddChild("apply-templates");

            tag.AddChild("call-template");

            tag.AddChild("choose");

            tag.AddChild("if");

            tag.AddChild("for-each");

            tag.AddChild("copy");

            tag.AddChild("copy-of");

            tag.AddChild("decimal-format");

            tag.AddChild("element");

            tag.AddChild("number");

            tag.AddChild("text");

            tag.AddChild("value-of");

            schema.Add(tag);

            tag = new XmlSchemaTag("apply-imports", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("apply-templates", false);

            attribute = new XmlSchemaAttribute("select", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("mode", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("sort");

            schema.Add(tag);

            tag = new XmlSchemaTag("call-template", false);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("with-param");

            schema.Add(tag);

            tag = new XmlSchemaTag("with-param", false);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("select", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("variable", false);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("select", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("param", false);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("select", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("choose", false);

            tag.AddChild("when");

            tag.AddChild("otherwise");

            schema.Add(tag);

            tag = new XmlSchemaTag("when", false);

            attribute = new XmlSchemaAttribute("test", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("apply-imports");

            tag.AddChild("apply-templates");

            tag.AddChild("call-template");

            tag.AddChild("choose");

            tag.AddChild("if");

            tag.AddChild("for-each");

            tag.AddChild("copy");

            tag.AddChild("copy-of");

            tag.AddChild("decimal-format");

            tag.AddChild("element");

            tag.AddChild("number");

            tag.AddChild("text");

            tag.AddChild("value-of");

            schema.Add(tag);

            tag = new XmlSchemaTag("otherwise", false);

            tag.AddChild("apply-imports");

            tag.AddChild("apply-templates");

            tag.AddChild("call-template");

            tag.AddChild("choose");

            tag.AddChild("if");

            tag.AddChild("for-each");

            tag.AddChild("copy");

            tag.AddChild("copy-of");

            tag.AddChild("decimal-format");

            tag.AddChild("element");

            tag.AddChild("number");

            tag.AddChild("text");

            tag.AddChild("value-of");

            schema.Add(tag);

            tag = new XmlSchemaTag("if", false);

            attribute = new XmlSchemaAttribute("test", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("apply-imports");

            tag.AddChild("apply-templates");

            tag.AddChild("call-template");

            tag.AddChild("choose");

            tag.AddChild("if");

            tag.AddChild("for-each");

            tag.AddChild("copy");

            tag.AddChild("copy-of");

            tag.AddChild("decimal-format");

            tag.AddChild("element");

            tag.AddChild("number");

            tag.AddChild("text");

            tag.AddChild("value-of");

            schema.Add(tag);

            tag = new XmlSchemaTag("for-each", false);

            attribute = new XmlSchemaAttribute("select", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("sort");

            tag.AddChild("apply-imports");

            tag.AddChild("apply-templates");

            tag.AddChild("call-template");

            tag.AddChild("choose");

            tag.AddChild("if");

            tag.AddChild("for-each");

            tag.AddChild("copy");

            tag.AddChild("copy-of");

            tag.AddChild("decimal-format");

            tag.AddChild("element");

            tag.AddChild("number");

            tag.AddChild("text");

            tag.AddChild("value-of");

            schema.Add(tag);

            tag = new XmlSchemaTag("sort", false);

            attribute = new XmlSchemaAttribute("select", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("lang", XmlSchemaAttributeType.List);

            attribute.AddOption("ar");

            attribute.AddOption("zh");

            attribute.AddOption("cs");

            attribute.AddOption("da");

            attribute.AddOption("nl");

            attribute.AddOption("en");

            attribute.AddOption("eo");

            attribute.AddOption("es");

            attribute.AddOption("et");

            attribute.AddOption("fi");

            attribute.AddOption("fr");

            attribute.AddOption("de");

            attribute.AddOption("el");

            attribute.AddOption("he");

            attribute.AddOption("it");

            attribute.AddOption("en");

            attribute.AddOption("ja");

            attribute.AddOption("ko");

            attribute.AddOption("la");

            attribute.AddOption("lt");

            attribute.AddOption("po");

            attribute.AddOption("ro");

            attribute.AddOption("ru");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("data-type", XmlSchemaAttributeType.List);

            attribute.AddOption("text");

            attribute.AddOption("number");

            attribute.AddOption("qname");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("order", XmlSchemaAttributeType.List);

            attribute.AddOption("ascending");

            attribute.AddOption("descending");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("case-order", XmlSchemaAttributeType.List);

            attribute.AddOption("upper-first");

            attribute.AddOption("lower-first");

            tag.AddAttribute(attribute);

            tag.AddChild("apply-imports");

            tag.AddChild("apply-templates");

            tag.AddChild("call-template");

            tag.AddChild("choose");

            tag.AddChild("if");

            tag.AddChild("for-each");

            tag.AddChild("copy");

            tag.AddChild("copy-of");

            tag.AddChild("decimal-format");

            tag.AddChild("element");

            tag.AddChild("number");

            tag.AddChild("text");

            tag.AddChild("value-of");

            schema.Add(tag);

            tag = new XmlSchemaTag("copy", false);

            schema.Add(tag);

            tag = new XmlSchemaTag("copy-of", false);

            attribute = new XmlSchemaAttribute("select", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("decimal-format", false);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("decimal-separator", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("grouping-separator", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("infinity", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("NaN", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("percent", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("per-mille", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("zero-digit", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("digit", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("pattern-separator", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("element", false);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("namespace", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("attribute");

            tag.AddChild("apply-imports");

            tag.AddChild("apply-templates");

            tag.AddChild("call-template");

            tag.AddChild("choose");

            tag.AddChild("if");

            tag.AddChild("for-each");

            tag.AddChild("copy");

            tag.AddChild("copy-of");

            tag.AddChild("decimal-format");

            tag.AddChild("element");

            tag.AddChild("number");

            tag.AddChild("text");

            tag.AddChild("value-of");

            schema.Add(tag);

            tag = new XmlSchemaTag("attribute", false);

            attribute = new XmlSchemaAttribute("name", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            tag.AddChild("apply-imports");

            tag.AddChild("apply-templates");

            tag.AddChild("call-template");

            tag.AddChild("choose");

            tag.AddChild("if");

            tag.AddChild("for-each");

            tag.AddChild("copy");

            tag.AddChild("copy-of");

            tag.AddChild("decimal-format");

            tag.AddChild("element");

            tag.AddChild("number");

            tag.AddChild("text");

            tag.AddChild("value-of");

            schema.Add(tag);

            tag = new XmlSchemaTag("number", false);

            attribute = new XmlSchemaAttribute("count", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("level", XmlSchemaAttributeType.List);

            attribute.AddOption("single");

            attribute.AddOption("multiple");

            attribute.AddOption("any");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("from", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("value", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("format", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("lang", XmlSchemaAttributeType.List);

            attribute.AddOption("ar");

            attribute.AddOption("zh");

            attribute.AddOption("cs");

            attribute.AddOption("da");

            attribute.AddOption("nl");

            attribute.AddOption("en");

            attribute.AddOption("eo");

            attribute.AddOption("es");

            attribute.AddOption("et");

            attribute.AddOption("fi");

            attribute.AddOption("fr");

            attribute.AddOption("de");

            attribute.AddOption("el");

            attribute.AddOption("he");

            attribute.AddOption("it");

            attribute.AddOption("en");

            attribute.AddOption("ja");

            attribute.AddOption("ko");

            attribute.AddOption("la");

            attribute.AddOption("lt");

            attribute.AddOption("po");

            attribute.AddOption("ro");

            attribute.AddOption("ru");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("letter-value", XmlSchemaAttributeType.List);

            attribute.AddOption("alphabetic");

            attribute.AddOption("traditional");

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("groupping-separator", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("groupping-size", XmlSchemaAttributeType.Default);

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("text", false);

            attribute = new XmlSchemaAttribute("disable-output-escaping", XmlSchemaAttributeType.List);

            attribute.AddOption("yes");

            attribute.AddOption("no");

            tag.AddAttribute(attribute);

            schema.Add(tag);

            tag = new XmlSchemaTag("value-of", false);

            attribute = new XmlSchemaAttribute("select", XmlSchemaAttributeType.XPath);

            tag.AddAttribute(attribute);

            attribute = new XmlSchemaAttribute("disable-output-escaping", XmlSchemaAttributeType.List);

            attribute.AddOption("yes");

            attribute.AddOption("no");

            tag.AddAttribute(attribute);

            schema.Add(tag);


            mXslt = schema;

            return schema;
        }
