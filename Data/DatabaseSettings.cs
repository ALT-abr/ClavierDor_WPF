namespace clavierdor.Data;

public static class DatabaseSettings
{
    public const string DefaultConnectionString =
        "server=127.0.0.1;port=3306;database=clavierdor_db;user=root;password=;";

    public static readonly Version XamppMariaDbVersion = new(10, 4, 32);
}
