public abstract class Uuid
{
    public static string GetUuid()
    {
        var guid = System.Guid.NewGuid();
        return guid.ToString();
    }
}