using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NugetFix
{
	public class PathHelper
	{
		readonly string pathToSln;

		public PathHelper (string pathToSln)
		{
			this.pathToSln = pathToSln;
		}

		public IEnumerable<string> GetProjectsPaths()
		{
			IEnumerable<string> csprojPaths = GetRelativePathsToProjects ();
			string slnDirPath = Path.GetDirectoryName (pathToSln);

			IEnumerable<string> pathsToProjects = csprojPaths.Select (rp => Path.Combine (slnDirPath, rp));
			return pathsToProjects;
		}

		IEnumerable<string> GetRelativePathsToProjects()
		{
			return File.ReadAllLines (pathToSln)
				.Where (l => l.StartsWith ("Project"))
				.Select (ParseProjectPath);
		}

		string ParseProjectPath(string projectLine)
		{
			// Project("{GUID}") = "Project.Name", "path\to\project.csproj", "{GUID}"
			int fCommaIndex = projectLine.IndexOf (',');
			int lCommaIndex = projectLine.LastIndexOf (',');

			string projectPath = projectLine.Substring (fCommaIndex + 1, lCommaIndex - fCommaIndex - 1);
			projectPath = ConvertToNativePath (projectPath.Trim ().Replace ("\"", string.Empty));
			return projectPath;
		}

		public static string ConvertToNativePath(string path)
		{
			return path != null ? path.Replace ('\\', Path.DirectorySeparatorChar) : null;
		}

		public static string ConvertToWindowsPath(string path)
		{
			return path != null ? path.Replace (Path.DirectorySeparatorChar, '\\') : null;
		}
	}
}

