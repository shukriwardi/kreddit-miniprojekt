namespace shared.Model;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string? Url { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();

    public Post(User user, string title = "", string content = "", string? url = null, int upvotes = 0, int downvotes = 0)
    {
        Title = title;
        Content = content;
        Url = url;
        Upvotes = upvotes;
        Downvotes = downvotes;
        User = user;
    }

    public Post()
    {
        Id = 0;
        Title = "";
        Content = "";
        Upvotes = 0;
        Downvotes = 0;
        User = new User();
    }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, Content: {Content}, Url: {Url}, Upvotes: {Upvotes}, Downvotes: {Downvotes}, User: {User}";
    }
}