namespace LogicLib1.AppDb1;

public interface IAppDbOperator
{
    Task<T> LoadThisBookAsync<T>(string bookUid);
    Task<List<T>> LoadAllBooksAsync<T>() where T : class, new();
    Task AddUserBookAsync(string bookUid);
    Task AddBookAsync(
        string driveUrl,
        string title,
        string classfication,
        string desc,
        string course = "",
        string topic = "");

    Task RemoveThisBookAsync(string bookUid);
}
