using System;
using System.Collections.Generic;

using NuGet;
using System.Linq;
using System.Diagnostics;

namespace NugetFix
{
	class MainClass
	{
		static string packageId = "Xamarin.Forms";

		public static void Main (string[] args)
		{
			string csproj = "/Users/rzaitov/Documents//Apps/A_Xamarin/xamarin-forms-book-preview/Chapter02/TwoButtons/TwoButtons/TwoButtons.Android/TwoButtons.Android.csproj";
			var project = new Project (csproj);

//			project.UpsertLocalReference (AssemblyReference.CreateFromPath ("..\\..\\packages\\Xamarin.AAA.Support.v13.20.0.0.4\\libAAA\\MonoAndroidAAA\\Xamarin.Android.Support.v16.dll" ));
//			project.DeleteAssemblyReference (AssemblyReference.CreateFromPath ("Xamarin.Forms.Core.dll"));
			string path = "..\\..\\packages\\Xamarin.Forms.1.2.3.6257\\build\\portable-win+net45+wp80+MonoAndroid10+MonoTouch10\\Xamarin.Forms.targets";
			project.RemoveImportByPath (new ImportReference { Path = path, Condition = string.Format("Exists('{0}')", path) });
			project.Save ();

			/*
			string pathToSln = "/Users/rzaitov/Documents//Apps/A_Xamarin/xamarin-forms-book-preview/Chapter02/TwoButtons/TwoButtons.sln";
			MigrationManager mm = new MigrationManager (pathToSln, "Xamarin.Forms", "1.2.3.6257");
			mm.Migrate ();
			*/

			/*
			IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
			var package = repo.FindPackage (packageId, new SemanticVersion ("1.3.0.6275-pre1"));
			Console.WriteLine (package);

			foreach (PackageDependencySet ds in package.DependencySets) {
				Console.WriteLine (ds);
				foreach (var d in ds.Dependencies) {
					Console.WriteLine (d);
				} 
			} 
//			List<IPackage> packages = repo.FindPackagesById(packageId).ToList();


//			foreach (var p in packages) {
//				Console.WriteLine (p);
//			}
			Stopwatch sw = new Stopwatch ();
			TimeSpan elapsed;
			PackageManager pm = new PackageManager (repo, "repo");
			sw.Start ();
			pm.InstallPackage (package, false, true);
			elapsed = sw.Elapsed;
			Console.WriteLine (elapsed);
			sw.Reset ();

			var repo2 = new LocalPackageRepository ("repo");

			PackageManager pm2 = new PackageManager (repo2, "repo2");
			sw.Start ();
			pm2.InstallPackage (package, false, true);
			sw.Start ();
			pm.InstallPackage (package, false, true);
			elapsed = sw.Elapsed;
			Console.WriteLine (elapsed);

//			pm.InstallPackage (packageId, new SemanticVersion ("1.3.0.6275-pre1"));
*/
		}
	}
}
