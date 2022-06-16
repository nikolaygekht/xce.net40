using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using gehtsoft.xce.editor.application;
using gehtsoft.xce.colorer;

namespace gehtsoft.xce.extension.template_impl
{
    //:type mask
    //:template name
    //:param name
    //:param name
    //:param name
    //body...
    //
    internal class TemplateReader
    {
        internal static TemplateFileTypeCollection read(Application app)
        {
            TemplateFileTypeCollection coll = new TemplateFileTypeCollection();
            TemplateFileType currType = null;
            Template currTemplate = null;
            
            string name = Path.Combine(app.ApplicationPath, "templates.ini");
            string line;
            int lineNo = 0;
            
            if (File.Exists(name))
            {
                StreamReader r = new StreamReader(name, Encoding.UTF8);
                Regex re = new Regex("/^\\:(type|template|parameter)\\s+(\\S.+\\S)\\s*$/");
                try
                {
                    while ((line = r.ReadLine()) != null)
                    {
                        lineNo++;
                            if (line.Length > 0 && line[0] == ';')
                                continue;
                            else if (line.Length > 0 && line[0] == ':' && re.Match(line))
                            {
                                if (re.Value(1) == "type")
                                {
                                    string mask = re.Value(2);
                                    try
                                    {   
                                        currType = new TemplateFileType(mask);
                                        currTemplate = null;
                                        coll.Add(currType);
                                    }
                                    catch (Exception e)
                                    {
                                        currType = null;   
                                        Console.WriteLine("template.ini({0}) : Can't parse regex {1}", lineNo, e.Message);
                                    }   
                                }   
                                else if (re.Value(1) == "template")
                                {
                                    if (currType != null)
                                    {
                                        currTemplate = new Template(re.Value(2));
                                        currType.Templates.Add(currTemplate);
                                    }
                                    else
                                    {
                                        Console.WriteLine("template.ini({0}) : Template is declated before the type", lineNo);
                                    }
                                }
                                else if (re.Value(1) == "parameter")
                                {
                                    if (currTemplate != null)
                                    {
                                        currTemplate.Params.Add(re.Value(2));
                                    }
                                    else
                                    {
                                        Console.WriteLine("template.ini({0}) : Parameter is declated before the template", lineNo);
                                    }

                                }
                            }
                            else
                            {
                                if (currTemplate != null)
                                    currTemplate.Body.Add(line);
                                else if (line.Length > 0)
                                    Console.WriteLine("template.ini({0}) line outside the template body", lineNo);
                            }
                    }
                }
                finally
                {
                    r.Close();
                    re.Dispose();
                }
            }
            
            return coll;
        }
    }
}
