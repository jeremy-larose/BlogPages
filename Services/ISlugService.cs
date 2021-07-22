namespace BlogProject.Services
{
    public interface ISlugService
    {
        string URLFriendly(string title);
        bool IsUnique(string slug);
        
    }
}