using System;

namespace NugetFix
{
	// target or assemblly
	public class AssemblyReference
	{
		// Directory separator is unspecified
		public string Path { get; private set; }
		public string AssemblyName { get; private set; }

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

		private AssemblyReference()
		{
		}

		public static AssemblyReference CreateFromName(string name)
		{
			var ar = new AssemblyReference () {
				AssemblyName = name
			};
			return ar;
		}

		public static AssemblyReference CreateFromPath(string path)
		{
			var nativePackagePath = PathHelper.ConvertToNativePath (path);
			var ar = new AssemblyReference () {
				Path = path,
				AssemblyName = System.IO.Path.GetFileNameWithoutExtension (nativePackagePath)
			};

			return ar;
		}
	}
}

