namespace InterViewV2.Services.Interfaces
{
    public interface IDocumentService
    {
        byte[] ReplaceInWord(byte[] document, Dictionary<string, string> model);
        byte[] ReplaceInWord(byte[] document, string tableName, IEnumerable<Dictionary<string, string>> items);
    }
}
