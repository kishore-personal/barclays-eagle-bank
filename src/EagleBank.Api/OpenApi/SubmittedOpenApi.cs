namespace EagleBank.Api.OpenApi;

public static class SubmittedOpenApi
{
    public const string DocumentPath = "/openapi.yaml";
    public const string UiPath = "/swagger";

    public static string ResolveFilePath(IWebHostEnvironment environment)
    {
        foreach (var start in new[] { environment.ContentRootPath, AppContext.BaseDirectory })
        {
            var found = Find(start);
            if (found is not null)
            {
                return found;
            }
        }

        throw new FileNotFoundException("The submitted openapi.yaml file was not found.");
    }

    private static string? Find(string start)
    {
        var directory = new DirectoryInfo(start);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "openapi.yaml");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
