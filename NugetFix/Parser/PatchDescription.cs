using System;
using System.Collections.Generic;
using System.Text;

namespace NugetFix
{
	public enum CommandType
	{
		Update,
		Delete
	}

	public enum Type
	{
		AndroidProject,
		IosProject,
		WpProject,
		SharedProject,

		AndroidConfig,
		IosConfig,
		WpConfig,
		SharedConfig,
	}

	public class PatchDescription
	{
		public Type ProjectType { get; set; }
		public List<Command> Commands { get; set; }

		public PatchDescription()
		{
			Commands = new List<Command> ();
		}

		public override string ToString ()
		{
			var sb = new StringBuilder ();
			foreach (var cmd in Commands)
				sb.AppendLine (cmd.ToString());
			return string.Format ("PatchDescription: ProjectType={0}\nCommands:\n{1}\n", ProjectType, sb);
		}
	}

	public class Command
	{
		public CommandType CommandType { get; set; }

		public string Path { get; set; }
		public string Name { get; set; }

		// package info area

		public string Version { get; set; }
		public string TargetFramework { get; set; }

		public override string ToString ()
		{
			return string.Format ("[Command: CommandType={0}, Path={1}, Name={2}, Version={3}, TargetFramework={4}]", CommandType, Path, Name, Version, TargetFramework);
		}
	}
}

