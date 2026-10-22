using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

using shared.Model;

namespace kreddit_app.Data;

public class ApiService
{
    private readonly HttpClient http;
    private readonly IConfiguration configuration;
    private readonly string baseAPI = "";

    private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient http, IConfiguration configuration)
    {
        this.http = http;
        this.configuration = configuration;
        this.baseAPI = configuration["base_api"];
    }

    public async Task<Post[]> GetPosts()
    {
        string url = $"{baseAPI}posts/";
        return await http.GetFromJsonAsync<Post[]>(url);
    }

    public async Task<Post> GetPost(int id)
    {
        string url = $"{baseAPI}posts/{id}/";
        return await http.GetFromJsonAsync<Post>(url);
    }

    public async Task<Post> CreatePost(string title, string? content, string? url_, string username)
    {
        string url = $"{baseAPI}posts/";

        HttpResponseMessage msg = await http.PostAsJsonAsync(url, new { title, content, url = url_, username });

        string json = msg.Content.ReadAsStringAsync().Result;

        Post? newPost = JsonSerializer.Deserialize<Post>(json, jsonOptions);

        return newPost;
    }

    public async Task<Comment> CreateComment(string content, int postId, string username)
    {
        string url = $"{baseAPI}posts/{postId}/comments";

        HttpResponseMessage msg = await http.PostAsJsonAsync(url, new { content, username });

        string json = msg.Content.ReadAsStringAsync().Result;

        Comment? newComment = JsonSerializer.Deserialize<Comment>(json, jsonOptions);

        return newComment;
    }

    public async Task<Post> UpvotePost(int id)
    {
        string url = $"{baseAPI}posts/{id}/upvote/";

        HttpResponseMessage msg = await http.PutAsJsonAsync(url, "");

        string json = msg.Content.ReadAsStringAsync().Result;

        Post? updatedPost = JsonSerializer.Deserialize<Post>(json, jsonOptions);

        return updatedPost;
    }

    public async Task<Post> DownvotePost(int id)
    {
        string url = $"{baseAPI}posts/{id}/downvote/";

        HttpResponseMessage msg = await http.PutAsJsonAsync(url, "");

        string json = msg.Content.ReadAsStringAsync().Result;

        Post? updatedPost = JsonSerializer.Deserialize<Post>(json, jsonOptions);

        return updatedPost;
    }

    public async Task<Comment> UpvoteComment(int id)
    {
        string url = $"{baseAPI}comments/{id}/upvote/";

        HttpResponseMessage msg = await http.PutAsJsonAsync(url, "");

        string json = msg.Content.ReadAsStringAsync().Result;

        Comment? updatedComment = JsonSerializer.Deserialize<Comment>(json, jsonOptions);

        return updatedComment;
    }

    public async Task<Comment> DownvoteComment(int id)
    {
        string url = $"{baseAPI}comments/{id}/downvote/";

        HttpResponseMessage msg = await http.PutAsJsonAsync(url, "");

        string json = msg.Content.ReadAsStringAsync().Result;

        Comment? updatedComment = JsonSerializer.Deserialize<Comment>(json, jsonOptions);

        return updatedComment;
    }
}