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
//		string slnDirectory;
//		string packageFolder;

		Dictionary<Type, PatchDescription> patchMap;

		public MigrationManager (string pathToSln, IEnumerable<PatchDescription> patch)
		{
			pathHelper = new PathHelper (pathToSln);
//			slnDirectory = Path.GetDirectoryName (pathToSln);
//			packageFolder = Path.Combine (slnDirectory, "package");

			patchMap = new Dictionary<Type, PatchDescription> ();
			foreach (PatchDescription p in patch)
				patchMap [p.ProjectType] = p;
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
				CsprojPath = csprojPath,
				ConfigPath = configPath
			};
		}

		void Migrate(MigrationItem migrateItem)
		{
			Project project = new Project (migrateItem.CsprojPath);
			Package package = new Package (migrateItem.ConfigPath);

			ProjectTypeFinder ptf = new ProjectTypeFinder ();
			ProjectType projectType = ptf.FetchType (project);

			PatchDescription projectPatch = FindProjectPatch (projectType);
			if (projectPatch == null)
				return;

			PatchDescription packagePatch = FindPackagePatch (projectType);
			if (packagePatch == null)
				return;

			PatchProject (project, projectPatch);
			PatchPackage (package, packagePatch);

			project.Save ();
			package.Save();
		}

		void PatchProject(Project project, PatchDescription patch)
		{
			foreach (var p in patch.Commands) {
				switch (p.CommandType) {
					case CommandType.Update:
						if (p.Path.EndsWith (".targets")) {
							var importReference = new ImportReference {
								Path = p.Path,
								Condition = string.Format ("Exists('{0}')", p.Path)
							};
							project.UpsertImport (importReference);
						} else {
							project.UpsertLocalReference (AssemblyReference.CreateFromPath (p.Path));
						}
						break;

					case CommandType.Delete:
						if (p.Name != null)
							project.DeleteAssemblyReference (p.Name);
						else if (p.Path.EndsWith (".targets"))
							project.RemoveImportByName (new ImportReference { Path = p.Path });
						else
							project.DeleteAssemblyReference (AssemblyReference.CreateFromPath (p.Path));
						break;

					default:
						throw new NotImplementedException ();
				}
			}
		}

		void PatchPackage(Package package, PatchDescription patch)
		{
			foreach (var cmd in patch.Commands) {
				switch (cmd.CommandType) {
					case CommandType.Update:
						package.UpsertPackage (new PackageInfo {
							Id = cmd.Name,
							Version = cmd.Version,
							TargetFramework = cmd.TargetFramework
						});
						break;

					case CommandType.Delete:
						package.RemovePackageById (new PackageInfo{ Id = cmd.Name });
						break;

					default:
						throw new NotImplementedException ();
			}
			}
		}

		PatchDescription FindProjectPatch(ProjectType projectType)
		{
			PatchDescription patch = null;

			switch (projectType) {
				case ProjectType.Android:
					patchMap.TryGetValue (Type.AndroidProject, out patch);
					break;

				case ProjectType.Ios:
					patchMap.TryGetValue (Type.IosProject, out patch);
					break;

				case ProjectType.Wp:
					patchMap.TryGetValue (Type.WpProject, out patch);
					break;

				case ProjectType.Shared:
					patchMap.TryGetValue (Type.SharedProject, out patch);
					break;
			}

			if(patch == null)
				Console.WriteLine ("can't find project patch task for {0}", projectType);

			return patch;
		}

		PatchDescription FindPackagePatch(ProjectType projectType)
		{
			PatchDescription patch = null;

			switch (projectType) {
				case ProjectType.Android:
					patchMap.TryGetValue (Type.AndroidConfig, out patch);
					break;

				case ProjectType.Ios:
					patchMap.TryGetValue (Type.IosConfig, out patch);
					break;

				case ProjectType.Wp:
					patchMap.TryGetValue (Type.WpConfig, out patch);
					break;

				case ProjectType.Shared:
					patchMap.TryGetValue (Type.SharedConfig, out patch);
					break;
			}

			if(patch == null)
				Console.WriteLine ("can't find package.config patch task for {0}", projectType);

			return patch;
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

