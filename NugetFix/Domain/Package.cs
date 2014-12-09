using System;
using System.Xml.Linq;
using System.Linq;

namespace NugetFix
{
	public class Package : XmlWrapper
	{
		static readonly XName PackageXName = XName.Get("package");

		static readonly XName IdXName = XName.Get("id");
		static readonly XName VersionXName = XName.Get("version");
		static readonly XName TargetFrameworkXName = XName.Get("targetFramework");

		public Package (string path)
			: base (path)
		{
		}

		public void RemovePackageById(PackageInfo packageInfo)
		{
			var packageToRemove = FindPackageById (packageInfo.Id);
			if (packageToRemove != null)
				packageToRemove.Remove ();
		}

		public void UpsertPackage(PackageInfo packageInfo)
		{
			var packageElement = FindPackageById (packageInfo.Id);
			if (packageInfo != null)
				UpdatePackage (packageElement, packageInfo);
			else
				AddPackage (packageInfo);
		}

		XElement FindPackageById(string packageId)
		{
			var packages = Document.Root.Elements (PackageXName);
			return packages.FirstOrDefault (p => p.Attribute (IdXName).Value == packageId);
		}

		void UpdatePackage(XElement packageElement, PackageInfo packageInfo)
		{
			packageElement.Attribute (VersionXName).Value = packageInfo.Version;
			packageElement.Attribute (TargetFrameworkXName).Value = packageInfo.TargetFramework;
		}

		void AddPackage(PackageInfo packageInfo)
		{
			var packageElement = new XElement (PackageXName);
			packageElement.Add (new XAttribute (IdXName, packageInfo.Id));
			packageElement.Add (new XAttribute (VersionXName, packageInfo.Version));
			packageElement.Add (new XAttribute (TargetFrameworkXName, packageInfo.TargetFramework));

			Document.Root.Add (packageElement);
		}
	}
}

