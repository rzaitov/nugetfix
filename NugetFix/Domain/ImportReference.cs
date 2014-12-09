using System;

namespace NugetFix
{
	public class ImportReference
	{
		public string Path { get; set; }
		public string Condition { get; set; }

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

		public string Name {
			get {
				return System.IO.Path.GetFileName (NativePath);
			}
		}

		public ImportReference ()
		{
		}
	}
}

