using System.Collections.Generic;

public class SettingsIds
{
	public static readonly string language = "language";

	public static readonly string isMuted = "isMuted";
	public static readonly string sfxVolume = "sfxVolume";
	public static readonly string musicVolume = "musicVolume";
	
	// DEFAULT VALUES
	public static readonly Dictionary<string, string> defaults = new Dictionary<string, string>()
	{
		{ language, "Spanish" },
		{ isMuted, "1" },
		{ sfxVolume, "1" },
		{ musicVolume, "1" }
	};
}