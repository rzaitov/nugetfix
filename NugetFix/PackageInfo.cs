using System;

namespace NugetFix
{
	public class PackageInfo
	{
		public string Id { get; set; }
		public string Version { get; set; }

		public override string ToString ()
		{
			return string.Format ("[PackageInfo: Id={0}, Version={1}]", Id, Version);
		}
	}
}

