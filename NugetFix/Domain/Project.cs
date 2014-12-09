using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace NugetFix
{
	public class Project
	{
		const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";

		static readonly XName ReferenceXName = XName.Get ("Reference", ns);
		static readonly XName HintPathXName = XName.Get ("HintPath", ns);
		static readonly XName ItemGroupXName = XName.Get ("ItemGroup", ns);

		static readonly XName IncludeXName = XName.Get ("Include");

		readonly XDocument xcsproj;
		readonly string csproj;

		XElement itemGroupWithReferences;
		XElement ItemGroupWithReferences {
			get {
				if (itemGroupWithReferences == null)
					itemGroupWithReferences = GetItemGroupWithRefs ();

				return itemGroupWithReferences;
			}
		}

		IEnumerable<XElement> References {
			get {
				return ItemGroupWithReferences.Elements (ReferenceXName);
			}
		}

		public string PathToPackageRepository { get; set; }

		public Project (string csproj)
		{
			this.csproj = csproj;
			xcsproj = XDocument.Load (csproj);
		}

		public void UpsertPackageReference(PackageSettings settings)
		{
			string ext = Path.GetExtension (settings.Path);
			AssertTrue (ext == ".dll" || ext == ".exe");

			XElement localReference = FindReference (settings.AssemblyName);
			if (localReference != null)
				UpdatePackageReference (localReference, settings.NativePath);
			else
				AddPackageReference (settings.NativePath);
		}

		public void DeletePackageReference(PackageSettings settings)
		{
			string ext = Path.GetExtension (settings.Path);
			AssertTrue (ext == ".dll" || ext == ".exe");

			DeletePackageReference (settings.AssemblyName);
		}

		public void DeletePackageReference(string assemblyName)
		{
			var refToDelete = FindReference (assemblyName);
			if(refToDelete != null)
				refToDelete.Remove ();
		}

		XElement FindReference(string assemblyName)
		{
			return References.FirstOrDefault (r => r.Attribute (IncludeXName).Value == assemblyName);
		}

		XElement GetItemGroupWithRefs()
		{
			var itemGroups = xcsproj.Root.Elements (ItemGroupXName);
			return itemGroups.FirstOrDefault (ContainsAnyReferenceElement);
		}

		bool ContainsAnyReferenceElement(XElement itemGroup)
		{
			return itemGroup.Element (ReferenceXName) != null;
		}

		void UpdatePackageReference(XElement localReference, string nativePackagePath)
		{
			var hitPathElem = localReference.Element (HintPathXName);
			hitPathElem.Value = BuildHintPath (nativePackagePath);
		}

		void AddPackageReference(string nativePackagePath)
		{
			var hintPathElement = new XElement (HintPathXName, BuildHintPath (nativePackagePath));
			XElement reference = new XElement (ReferenceXName, hintPathElement);
			reference.Add (new XAttribute (IncludeXName, Path.GetFileNameWithoutExtension (nativePackagePath)));

			ItemGroupWithReferences.Add (reference);
		}

		string BuildHintPath(string nativePackagePath)
		{
			string pathToAssembly = Path.Combine (PathToPackageRepository, nativePackagePath);
			return PathHelper.ConvertToWindowsPath (pathToAssembly);
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
			if (!condition)
				throw new InvalidProgramException ();
		}
	}
}

