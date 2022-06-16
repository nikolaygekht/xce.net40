using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace gehtsoft.intellisense.cs
{
    public sealed class CsProjectLoader 
    {
        public static CsProject load(string path)
        {
            CsProject project = new CsProject(path);

            XmlReaderSettings settings;
            XmlReader reader;
            XmlDocument projectFile = new XmlDocument();
            settings = new XmlReaderSettings();
            settings.ConformanceLevel = System.Xml.ConformanceLevel.Document;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            reader = XmlReader.Create(path, settings);
            projectFile.Load(reader);
            reader.Close();
            
            Walk(projectFile.DocumentElement, project);
            
            return project;   
        }
        
        static void Walk(XmlNode node, CsProject project)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                if (node.Name == "Reference")
                {
                    if (node.Attributes["Include"] != null)
                    {
                        string name = node.Attributes["Include"].Value;
                        string path = null;
                        
                        foreach (XmlNode c in node.ChildNodes)
                        {
                            if (c.NodeType == XmlNodeType.Element && c.Name == "HintPath")
                            {
                                path = ((XmlElement)c).InnerText.Trim();
                                FileInfo fi = new FileInfo(project.Name);
                                path = Path.Combine(fi.Directory.FullName, path);
                                if (File.Exists(path))
                                {
                                    fi = new FileInfo(path);
                                    path = fi.FullName;
                                    break;
                                }
                            }
                        }
                        
                        project.References.Add(new CsProjectReference(name, path));
                    }    
                }
                else if (node.Name == "Compile")
                {
                    if (node.Attributes["Include"] != null)
                    {
                        string name = node.Attributes["Include"].Value;
                        FileInfo fi = new FileInfo(project.Name);
                        name = Path.Combine(fi.Directory.FullName, name);
                        if (File.Exists(name))
                        {
                            fi = new FileInfo(name);
                            project.Sources.Add(new CsProjectSource(fi.FullName));
                        }
                    }
                }
                else foreach (XmlNode c in node.ChildNodes)
                    Walk(c, project);
            }            
        }
    }
}
