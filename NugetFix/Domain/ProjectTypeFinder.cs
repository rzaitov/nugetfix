using System;

namespace NugetFix
{
	public enum ProjectType
	{
		Unknown,
		Android,
		Ios,
		Wp,
		Shared
	}

	public class ProjectTypeFinder
	{
		public static string XamarinAndroidAppGuid {
			get {
				return "EFBA0AD7-5A72-4C68-AF49-83D382785DCF";
			}
		}

		public static string XamarinAndroidLibGuid {
			get {
				return "10368E6C-D01B-4462-8E8B-01FC667A7035";
			}
		}

		public static string XamarinIosClassicAppGuid {
			get {
				return "6BC8ED88-2882-458C-8E55-DFD12B67127B";
			}
		}

		public static string XamarinIosUnifiedAppGuid {
			get {
				return "FEACFBD2-3405-455C-9665-78FE426C6842";
			}
		}

		public static string WinPhoneAppGuid {
			get {
				return "C089C8C0-30E0-4E22-80C0-CE093F111A43";
			}
		}

		public static string PclProjectGuid {
			get {
				return "786C830F-07A1-408B-BD7F-6EE04809D6DB";
			}
		}

		public ProjectTypeFinder ()
		{
		}

		public bool IsIos(Project project)
		{
			var guidStr = project.GetProjectGuids ();
			return guidStr.Contains (XamarinIosClassicAppGuid) || guidStr.Contains (XamarinIosUnifiedAppGuid);
		}

		public bool IsAndroid(Project project)
		{
			var guidStr = project.GetProjectGuids ();
			return guidStr.Contains (XamarinAndroidAppGuid) || guidStr.Contains(XamarinAndroidLibGuid);
		}

		public bool IsWp(Project project)
		{
			var guidStr = project.GetProjectGuids ();
			return guidStr.Contains (WinPhoneAppGuid);
		}

		public bool IsPcl(Project project)
		{
			var guidStr = project.GetProjectGuids ();
			return guidStr.Contains (PclProjectGuid);
		}

		public ProjectType FetchType(Project project)
		{
			if (IsIos (project))
				return ProjectType.Ios;

			if (IsAndroid (project))
				return ProjectType.Android;

			if (IsWp (project))
				return ProjectType.Wp;

			if (IsPcl (project))
				return ProjectType.Shared;

			return ProjectType.Unknown;
		}
	}
}

