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
		static readonly XName ImportXName = XName.Get ("Import", ns);

		static readonly XName IncludeXName = XName.Get ("Include");
		static readonly XName ProjectXName = XName.Get ("Project");
		static readonly XName ConditionXName = XName.Get ("Condition");

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

		public Project (string csproj)
		{
			this.csproj = csproj;
			xcsproj = XDocument.Load (csproj);
		}

		#region References

		public void AddReference(AssemblyReference settings)
		{
			if (string.IsNullOrEmpty (settings.Path))
				AddGlobalReference (settings);
			else
				AddLocalReference (settings);
		}

		public void AddGlobalReference(AssemblyReference settings)
		{
			throw new NotImplementedException ();
		}

		public void UpsertLocalReference(AssemblyReference settings)
		{
			string ext = Path.GetExtension (settings.Path);
			AssertTrue (ext == ".dll" || ext == ".exe");

			XElement localReference = FindReference (settings.AssemblyName);
			if (localReference != null)
				UpdateLocalReference (localReference, settings.NativePath);
			else
				AddLocalReference (settings);
		}

		public void DeleteAssemblyReference(AssemblyReference settings)
		{
			DeleteAssemblyReference (settings.AssemblyName);
		}

		public void DeleteAssemblyReference(string assemblyName)
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

		void UpdateLocalReference(XElement localReference, string pathToAsm)
		{
			var hitPathElem = localReference.Element (HintPathXName);
			hitPathElem.Value = PathHelper.ConvertToWindowsPath (pathToAsm);
		}

		void AddLocalReference(AssemblyReference settings)
		{
			var hintPathElement = new XElement (HintPathXName, settings.PathWindows);
			XElement reference = new XElement (ReferenceXName, hintPathElement);
			reference.Add (new XAttribute (IncludeXName, Path.GetFileNameWithoutExtension (settings.NativePath)));

			ItemGroupWithReferences.Add (reference);
		}

		#endregion

		#region Import

		public void RemoveImportByName(ImportReference reference)
		{
			var importToRemove = FindImportByName (reference.Name);
			if (importToRemove != null)
				importToRemove.Remove ();
		}

		public void RemoveImportByPath(ImportReference reference)
		{
			XElement importToRemove = FindImportByPath (reference.Path);
			if (importToRemove != null)
				importToRemove.Remove ();
		}

		public void UpsertImport(ImportReference reference)
		{
			var importToUpdate = FindImportByName (reference.Name);
			if (importToUpdate != null)
				UpdateImport (importToUpdate, reference);
			else
				AddImport (reference);
		}

		XElement FindImportByName(string importFileName)
		{
			IEnumerable<XElement> imports = xcsproj.Root.Elements (ImportXName);
			var result = imports.Where (i => i.Attribute (ProjectXName).Value.EndsWith (importFileName));
			return result.FirstOrDefault ();
		}

		XElement FindImportByPath(string path)
		{
			var imports = xcsproj.Root.Elements (ImportXName);
			var result = imports.Where (i => i.Attribute (ProjectXName).Value == path);
			return result.FirstOrDefault ();
		}

		public void AddImport(ImportReference reference)
		{
			var importElement = new XElement (ImportXName);
			importElement.Add (new XAttribute (ProjectXName, reference.Path));

			if (reference.Condition != null)
				importElement.Add (BuildCondition(reference));

			xcsproj.Root.Add (importElement);
		}

		void UpdateImport(XElement importElement, ImportReference reference)
		{
			var project = importElement.Attribute (ProjectXName);
			project.Value = reference.PathWindows;

			var condition = importElement.Attribute (ConditionXName);
			if (condition != null && reference.Condition == null)
				condition.Remove ();
			else if (condition != null && reference.Condition != null)
				condition.Value = reference.Condition;
			else if (condition == null && reference.Condition != null)
				importElement.Add (BuildCondition (reference));
		}

		XAttribute BuildCondition(ImportReference reference)
		{
			return new XAttribute (ConditionXName, reference.Condition);
		}

		#endregion

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

