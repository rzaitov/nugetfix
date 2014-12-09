using System;
using System.IO;
using System.Collections.Generic;

namespace NugetFix
{
	public class Parser
	{
		readonly char[] splitSymbols = new char[] { ' ', '\t' };
		readonly string path;

		PatchDescription description;
		readonly List<PatchDescription> descriptions;
		public IEnumerable<PatchDescription> Descriptions {
			get {
				return descriptions;
			}
		}

		public Parser (string path)
		{
			this.path = path;
			descriptions = new List<PatchDescription> ();
		}

		public void Parse()
		{
			IEnumerable<string> lines = File.ReadLines (path);

			foreach (var currentLine in lines) {
				string l = currentLine.Trim ();

				if (string.IsNullOrEmpty (l)) {
					;
				} else if (l.StartsWith ("[") && l.EndsWith ("]")) {
					StartNewDescription (l);
				} else {
					ParseCommand (l);
				}
			}
		}

		// [ios csproj]
		// [android packages.config]
		void StartNewDescription(string line)
		{
			description = new PatchDescription ();
			descriptions.Add (description);

			line = line.Replace ("[", string.Empty).Replace ("]", string.Empty);
			var components = line.Split (splitSymbols);
			AssertTrue (components.Length == 2);

			if (components [1].ToLower () == "csproj") {
				string platform = components [0].ToLower ();
				switch (platform) {
					case "ios":
						description.ProjectType = Type.IosProject;
						break;

					case "android":
						description.ProjectType = Type.AndroidProject;
						break;

					case "wp":
						description.ProjectType = Type.WpProject;
						break;

					default:
						throw new InvalidProgramException ();
				}
			} else if (components [1].ToLower ().EndsWith ("config")) {
				string platform = components [0].ToLower ();
				switch (platform) {
					case "ios":
						description.ProjectType = Type.IosConfig;
						break;

					case "android":
						description.ProjectType = Type.AndroidConfig;
						break;

					case "wp":
						description.ProjectType = Type.WpConfig;
						break;

					default:
						throw new InvalidProgramException ();
				}
			} else {
				throw new InvalidProgramException ();
			}
		}

		void ParseCommand(string line)
		{
			var cmd = new Command ();
			var components = line.Split (splitSymbols);

			string cmdType = components [0].ToLower();
			switch (cmdType) {
				case "update":
					cmd.CommandType = CommandType.Update;
					break;

				case "delete":
					cmd.CommandType = CommandType.Delete;
					break;

				default:
					throw new InvalidProgramException ();
			}

			// remove 2 x "
			string pathNameRaw = components [1];
			string pathName = pathNameRaw.Substring (1, pathNameRaw.Length - 2);
			if (pathName.EndsWith (".targets") || pathName.EndsWith (".dll") || pathName.EndsWith (".exe"))
				cmd.Path = pathName;
			else
				cmd.Name = pathName;


			int i = 2;
			// parse package info (version, targetFramework)
			while (i < components.Length) {
				string pInfo = components [i++];
				if (pInfo.StartsWith ("version="))
					cmd.Version = pInfo.Substring (9, pInfo.Length - 10);
				else if (pInfo.StartsWith ("targetFramework="))
					cmd.TargetFramework = pInfo.Substring (17, pInfo.Length - 18);
				else
					throw new InvalidProgramException ();
			}

			description.Commands.Add (cmd);
		}

		void AssertTrue(bool condition)
		{
			if (!condition)
				throw new InvalidProgramException ();
		}
	}
}

