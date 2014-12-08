using System;

namespace NugetFix
{
	// target or assemblly
	public class PackageSettings
	{
		// Directory separator is unspecified
		// Relative to repository path
		public string Path { get; set; }

		public string PathWindows {
			get {
				return Path.Replace ("/", "\\");
			}
		}
	}
}

