using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using System.Text;

namespace NugetFix
{
	public class Project
	{
		const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";

		static readonly XName ReferenceXName = XName.Get ("Reference", ns);
		static readonly XName HintPathXName = XName.Get ("HintPath", ns);
		static readonly XName IncludeXName = XName.Get ("Include");

		readonly XDocument xcsproj;
		readonly string csproj;

		public Project (string csproj)
		{
			this.csproj = csproj;
			xcsproj = XDocument.Load (csproj);
		}

		public void UpsertReference(PackageSettings settings)
		{
			string ext = Path.GetExtension (settings.Path);
			AssertTrue (ext == "dll" || ext == "exe");

			var nativeSettingPath = ConvertToNativePath (settings.Path);
			string refname = Path.GetFileNameWithoutExtension (nativeSettingPath);
			var references = xcsproj.Descendants (ReferenceXName);

			var refToUpdate = references.First (r => r.Attribute (IncludeXName).Value == refname);

			var hitPathElem = refToUpdate.Element (HintPathXName);
			var windowsPath = hitPathElem.Value;

			string packageFolder = GetPathToPackageFolder (ConvertToNativePath (windowsPath));
			string pathToAssembly = Path.Combine (packageFolder, nativeSettingPath);
			pathToAssembly = ConvertToWindowsPath (pathToAssembly);

			hitPathElem.Value = pathToAssembly;
		}

		string GetPathToPackageFolder(string pathToAssembly)
		{
			string packageFolder = pathToAssembly;
			while (!packageFolder.EndsWith ("packages"))
				packageFolder = Path.GetDirectoryName (packageFolder);

			return packageFolder;
		}

		string ConvertToNativePath(string path)
		{
			return path.Replace ('\\', Path.DirectorySeparatorChar);
		}

		string ConvertToWindowsPath(string path)
		{
			return path.Replace (Path.DirectorySeparatorChar, '\\');
		}

		public void Save()
		{
			var ws = new XmlWriterSettings ();
			ws.Indent = true;
			ws.IndentChars = "  ";
			ws.NewLineChars = Environment.NewLine;
			ws.Encoding = new UTF8Encoding (true);

			using (XmlWriter writer = XmlWriter.Create (csproj, ws))
				xcsproj.Save (writer);
		}

		void AssertTrue(bool condition)
		{

		}
	}
}

