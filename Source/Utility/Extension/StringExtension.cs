namespace Overclock.Utility.Extension;

public static class StringExtension
{
    public static bool ContainsIgnoreCase(this string s, string target)
    {
        return s.IndexOf(target, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool ContainsAnyOfIgnoreCase(this string s, params string[] targets)
    {
        return targets.Any(s.ContainsIgnoreCase);
    }
}
