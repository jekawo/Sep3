namespace Tagalong.Client.Components.Layout;

public enum NavSection
{
    Trips,
    Activities
}

public static class NavSections
{
    /// <param name="relativePath">A base-relative path, e.g. from NavigationManager.ToBaseRelativePath(Nav.Uri).</param>
    public static NavSection Get(string relativePath)
    {
        // Ignore query string and fragment: "trips/5/activities?x=1" -> "trips/5/activities"
        var path = relativePath.Split('?', '#')[0];

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Any(s => s.Equals("activities", StringComparison.OrdinalIgnoreCase))
            ? NavSection.Activities
            : NavSection.Trips;
    }
}
