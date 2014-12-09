using System;
using System.Xml;
using System.Text;
using System.Xml.Linq;

namespace NugetFix
{
	public class XmlWrapper
	{
		protected string Path { get; private set; }
		protected XDocument Document { get; private set; }

		public XmlWrapper (string path)
		{
			Path = path;
			Document = XDocument.Load (path);
		}

		public void Save()
		{
			var ws = new XmlWriterSettings ();
			ws.Indent = true;
			ws.IndentChars = "  ";
			ws.NewLineChars = Environment.NewLine;
			ws.Encoding = new UTF8Encoding (true);

			using (XmlWriter writer = XmlWriter.Create (Path, ws))
				Document.Save (writer);
		}

		protected void AssertTrue(bool condition)
		{
			if (!condition)
				throw new InvalidProgramException ();
		}

	}
}

