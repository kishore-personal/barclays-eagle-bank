namespace EagleBank.Api.OpenApi;

public static class SubmittedOpenApi
{
    public const string DocumentPath = "/openapi.yaml";
    public const string UiPath = "/swagger";

    public const string UiImplementedOpsBanner =
        """
        <div style="padding:12px 20px;background:#f6f6f6;border-bottom:1px solid #ddd;font-family:sans-serif;font-size:14px">
        <strong>Wired in this repository:</strong>
        POST /v1/users, GET /v1/users/{userId}, POST /v1/auth/login,
        POST /v1/accounts, GET /v1/accounts/{accountNumber},
        POST /v1/accounts/{accountNumber}/transactions (deposit and withdrawal),
        GET /v1/accounts/{accountNumber}/transactions/{transactionId}.
        <strong>In the spec but not wired:</strong>
        list accounts, list transactions, PATCH, and DELETE.
        </div>
        """;

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
