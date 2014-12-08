using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NugetFix
{
	public class MigrationManager
	{
		PathHelper pathHelper;
		string slnDirectory;
		string packageFolder;

		readonly string packageId;
		readonly string version;

		public MigrationManager (string pathToSln, string packageId, string version)
		{
			this.packageId = packageId;
			this.version = version;

			pathHelper = new PathHelper (pathToSln);
			slnDirectory = Path.GetDirectoryName (pathToSln);
			packageFolder = Path.Combine (slnDirectory, "package");
		}

		public void Migrate()
		{
			IEnumerable<string> pathToProjects = pathHelper.GetProjectsPaths ();
			var toMigrate = pathToProjects.Select(GetProjWithConfig).Where(pc => File.Exists(pc.ConfigPath));

			foreach (var migrateItem in toMigrate)
				Migrate (migrateItem);
		}

		MigrationItem GetProjWithConfig(string csprojPath)
		{
			string csprojDir = Path.GetDirectoryName (csprojPath);
			string configPath = Path.Combine (csprojDir, "packages.config");
			return new MigrationItem {
				CsprojPath = csprojDir,
				ConfigPath = configPath
			};
		}

		void Migrate(MigrationItem migrateItem)
		{
			IEnumerable<PackageInfo> packageInfos = GetPackageInfos (migrateItem);

			foreach (var info in packageInfos)
				Console.WriteLine (info);
		}

		IEnumerable<PackageInfo> GetPackageInfos(MigrationItem migrateItem)
		{
			XElement root = XElement.Load (migrateItem.ConfigPath);
			IEnumerable<XElement> packageElements = root.Descendants ("package");
			return packageElements.Select (Parse);
		}

		PackageInfo Parse(XElement packageElement)
		{
			return new PackageInfo {
				Id = packageElement.Attribute("id").Value,
				Version = packageElement.Attribute("version").Value
			};
		}
	}
}

