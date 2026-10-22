using Microsoft.EntityFrameworkCore;
using kreddit_api.Data;
using shared.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=kreddit.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed data if the database is empty
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Posts.Any())
    {
        var alice = new User("Alice");
        var bob = new User("Bob");

        var post1 = new Post(alice, "Best pizza in Aarhus?", "I think Mama's is the best, change my mind.");
        var post2 = new Post(bob, "Cool C# tutorial", content: "", url: "https://learn.microsoft.com/en-us/dotnet/csharp/");

        post1.Comments.Add(new Comment("I agree, Mama's is amazing!", user: bob));
        post1.Comments.Add(new Comment("Nah, Il Tempio is better.", user: alice));

        db.Posts.AddRange(post1, post2);
        db.SaveChanges();
    }
}

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// GET all posts, newest first
app.MapGet("/posts", async (AppDbContext db) =>
{
    return await db.Posts
        .Include(p => p.User)
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();
});

// GET one post with its comments
app.MapGet("/posts/{id}", async (int id, AppDbContext db) =>
{
    var post = await db.Posts
        .Include(p => p.User)
        .Include(p => p.Comments)
            .ThenInclude(c => c.User)
        .FirstOrDefaultAsync(p => p.Id == id);

    return post is not null ? Results.Ok(post) : Results.NotFound();
});

// POST create a new post
app.MapPost("/posts", async (Post input, AppDbContext db) =>
{
    if (db.Entry(input.User).State == EntityState.Detached && input.User.Id == 0)
    {
        db.Users.Add(input.User);
    }

    input.CreatedAt = DateTime.UtcNow;
    db.Posts.Add(input);
    await db.SaveChangesAsync();

    return Results.Created($"/posts/{input.Id}", input);
});

// POST create a comment on a post
app.MapPost("/posts/{postId}/comments", async (int postId, Comment input, AppDbContext db) =>
{
    var post = await db.Posts.FindAsync(postId);
    if (post is null) return Results.NotFound();

    if (db.Entry(input.User).State == EntityState.Detached && input.User.Id == 0)
    {
        db.Users.Add(input.User);
    }

    input.CreatedAt = DateTime.UtcNow;
    post.Comments.Add(input);
    await db.SaveChangesAsync();

    return Results.Ok(input);
});

// PUT upvote a post
app.MapPut("/posts/{id}/upvote", async (int id, AppDbContext db) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post is null) return Results.NotFound();

    post.Upvotes++;
    await db.SaveChangesAsync();
    return Results.Ok(post);
});

// PUT downvote a post
app.MapPut("/posts/{id}/downvote", async (int id, AppDbContext db) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post is null) return Results.NotFound();

    post.Downvotes++;
    await db.SaveChangesAsync();
    return Results.Ok(post);
});

// PUT upvote a comment
app.MapPut("/comments/{id}/upvote", async (int id, AppDbContext db) =>
{
    var comment = await db.Comments.FindAsync(id);
    if (comment is null) return Results.NotFound();

    comment.Upvotes++;
    await db.SaveChangesAsync();
    return Results.Ok(comment);
});

// PUT downvote a comment
app.MapPut("/comments/{id}/downvote", async (int id, AppDbContext db) =>
{
    var comment = await db.Comments.FindAsync(id);
    if (comment is null) return Results.NotFound();

    comment.Downvotes++;
    await db.SaveChangesAsync();
    return Results.Ok(comment);
});

app.Run();