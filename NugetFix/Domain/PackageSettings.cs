using System;
using System.IO;

namespace NugetFix
{
	// target or assemblly
	public class PackageSettings
	{
		// Directory separator is unspecified
		// Relative to repository path
		public string Path { get; set; }

		public string NativePath {
			get {
				return PathHelper.ConvertToNativePath (Path);
			}
		}

		public string PathWindows {
			get {
				return PathHelper.ConvertToWindowsPath (Path);
			}
		}

		public string AssemblyName {
			get {
				var nativePackagePath = PathHelper.ConvertToNativePath (Path);
				return System.IO.Path.GetFileNameWithoutExtension (nativePackagePath);
			}
		}
	}
}

