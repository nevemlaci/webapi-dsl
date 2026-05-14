using System.Net;
using System.Net.Http.Json;
using ExampleAPI.Generated.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace WebAPI_DSL_SystemTests;

[TestFixture]
public class PostsEndpointTest
{
    private WebApplicationFactory<WebAPI_DSL_TestingProject.ExampleApi> _factory;
    private HttpClient _client;
    
    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<WebAPI_DSL_TestingProject.ExampleApi>();
        
        _client = _factory.CreateClient();
    }
    
    [TearDown]
    public void Teardown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
    
    // GET /api/posts tests
    
    [Test]
    public async Task GetPosts_WhenCalled_ReturnsOkAndData()
    {
        var response = await _client.GetAsync("/api/posts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();
        posts.Should().NotBeNull();
    }
    
    [Test]
    public async Task GetPosts_WhenCalled_ReturnsSeededPosts()
    {
        var response = await _client.GetAsync("/api/posts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();
        posts.Should().HaveCount(3);
        posts.Should().Contain(p => p.Title == "Getting Started with EF Core");
        posts.Should().Contain(p => p.Title == "SQLite in Memory");
        posts.Should().Contain(p => p.Title == "C# 12 Features");
    }
    
    [Test]
    public async Task GetPosts_WhenCalled_ReturnsPostsWithCorrectContent()
    {
        var response = await _client.GetAsync("/api/posts");
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();

        var efCorePost = posts.First(p => p.Title == "Getting Started with EF Core");
        var sqlitePost = posts.First(p => p.Title == "SQLite in Memory");
        var csharpPost = posts.First(p => p.Title == "C# 12 Features");
        
        efCorePost.Content.Should().Be("Entity Framework is a powerful ORM...");
        sqlitePost.Content.Should().Be("Testing with SQLite is fast and efficient.");
        csharpPost.Content.Should().Be("Let's talk about primary constructors...");
    }
    
    [Test]
    public async Task GetPosts_WhenCalled_ReturnsPostsWithAuthorIds()
    {
        var response = await _client.GetAsync("/api/posts");
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();

        // All posts should have an AuthorId
        posts.Should().AllSatisfy(p => p.AuthorId.Should().NotBeNull());
    }
    
    [Test]
    public async Task GetPosts_WhenCalled_ReturnsAlicesAndBobsPosts()
    {
        // Get users first to identify their IDs
        var usersResponse = await _client.GetAsync("/api/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var aliceId = users.First(u => u.Username == "AliceDev").Id;
        var bobId = users.First(u => u.Username == "BobCoder").Id;
        
        var response = await _client.GetAsync("/api/posts");
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();

        var alicePosts = posts.Where(p => p.AuthorId == aliceId).ToList();
        var bobPosts = posts.Where(p => p.AuthorId == bobId).ToList();
        
        // Alice has 2 posts, Bob has 1 post based on DbInitializer
        alicePosts.Should().HaveCount(2);
        bobPosts.Should().HaveCount(1);
    }
    
    // GET /api/posts/{id} tests
    
    [Test]
    public async Task GetPost_WithValidId_ReturnsOkAndPost()
    {
        // First get all posts to get a valid ID
        var listResponse = await _client.GetAsync("/api/posts");
        var posts = await listResponse.Content.ReadFromJsonAsync<List<PostDto>>();
        var postId = posts.First().Id;
        
        var response = await _client.GetAsync($"/api/posts/{postId}");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var post = await response.Content.ReadFromJsonAsync<PostDto>();
        post.Id.Should().Be(postId);
    }
    
    [Test]
    public async Task GetPost_WithAlicesFirstPostId_ReturnsCorrectPost()
    {
        var listResponse = await _client.GetAsync("/api/posts");
        var posts = await listResponse.Content.ReadFromJsonAsync<List<PostDto>>();
        var efCorePostId = posts.First(p => p.Title == "Getting Started with EF Core").Id;
        
        var response = await _client.GetAsync($"/api/posts/{efCorePostId}");
        var post = await response.Content.ReadFromJsonAsync<PostDto>();
        
        post.Title.Should().Be("Getting Started with EF Core");
        post.Content.Should().Be("Entity Framework is a powerful ORM...");
    }
    
    [Test]
    public async Task GetPost_WithBobsPostId_ReturnsCsharpPost()
    {
        var listResponse = await _client.GetAsync("/api/posts");
        var posts = await listResponse.Content.ReadFromJsonAsync<List<PostDto>>();
        var csharpPostId = posts.First(p => p.Title == "C# 12 Features").Id;
        
        var response = await _client.GetAsync($"/api/posts/{csharpPostId}");
        var post = await response.Content.ReadFromJsonAsync<PostDto>();
        
        post.Title.Should().Be("C# 12 Features");
        post.Content.Should().Be("Let's talk about primary constructors...");
    }
    
    [Test]
    public async Task GetPost_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/posts/{invalidId}");
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    // POST /api/posts tests
    
    [Test]
    public async Task CreatePost_WithValidData_ReturnsCreatedAtAction()
    {
        var usersResponse = await _client.GetAsync("/api/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var authorId = users.First().Id;
        
        var newPost = new PostDto
        {
            Title = "New Post Title",
            Content = "This is the content of the new post.",
            AuthorId = authorId
        };
        
        var response = await _client.PostAsJsonAsync("/api/posts", newPost);
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }
    
    [Test]
    public async Task CreatePost_WithValidData_ReturnsCreatedPost()
    {
        var usersResponse = await _client.GetAsync("/api/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var authorId = users.First().Id;
        
        var newPost = new PostDto
        {
            Title = "Advanced C# Patterns",
            Content = "Understanding design patterns in C#.",
            AuthorId = authorId
        };
        
        var response = await _client.PostAsJsonAsync("/api/posts", newPost);
        var createdPost = await response.Content.ReadFromJsonAsync<PostDto>();
        
        createdPost.Title.Should().Be("Advanced C# Patterns");
        createdPost.Content.Should().Be("Understanding design patterns in C#.");
        createdPost.AuthorId.Should().Be(authorId);
        createdPost.Id.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task CreatePost_WithMultiplePosts_AllAreCreated()
    {
        var usersResponse = await _client.GetAsync("/api/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var authorId = users.First().Id;
        
        var post1 = new PostDto { Title = "Post 1", Content = "Content 1", AuthorId = authorId };
        var post2 = new PostDto { Title = "Post 2", Content = "Content 2", AuthorId = authorId };
        
        await _client.PostAsJsonAsync("/api/posts", post1);
        await _client.PostAsJsonAsync("/api/posts", post2);
        
        var response = await _client.GetAsync("/api/posts");
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();
        
        posts.Should().Contain(p => p.Title == "Post 1");
        posts.Should().Contain(p => p.Title == "Post 2");
    }
    
    // PUT /api/posts/{id} tests
    
    [Test]
    public async Task UpdatePost_WithValidId_ReturnsNoContent()
    {
        var listResponse = await _client.GetAsync("/api/posts");
        var posts = await listResponse.Content.ReadFromJsonAsync<List<PostDto>>();
        var postId = posts.First().Id;
        
        var updateDto = new UpdatePostDto
        {
            Title = "Updated Title",
            Content = "Updated content goes here."
        };
        
        var response = await _client.PutAsJsonAsync($"/api/posts/{postId}", updateDto);
        
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Test]
    public async Task UpdatePost_WithValidData_UpdatesPostSuccessfully()
    {
        var listResponse = await _client.GetAsync("/api/posts");
        var posts = await listResponse.Content.ReadFromJsonAsync<List<PostDto>>();
        var efCorePostId = posts.First(p => p.Title == "Getting Started with EF Core").Id;
        
        var updateDto = new UpdatePostDto
        {
            Title = "Advanced Entity Framework Core",
            Content = "Deep dive into EF Core best practices."
        };
        
        await _client.PutAsJsonAsync($"/api/posts/{efCorePostId}", updateDto);
        
        var getResponse = await _client.GetAsync($"/api/posts/{efCorePostId}");
        var updatedPost = await getResponse.Content.ReadFromJsonAsync<PostDto>();
        
        updatedPost.Title.Should().Be("Advanced Entity Framework Core");
        updatedPost.Content.Should().Be("Deep dive into EF Core best practices.");
    }
    
    [Test]
    public async Task UpdatePost_PartialUpdate_UpdatesOnlyProvidedFields()
    {
        var listResponse = await _client.GetAsync("/api/posts");
        var posts = await listResponse.Content.ReadFromJsonAsync<List<PostDto>>();
        var first = posts.First();
        var postId = first.Id;
        var originalContent = first.Content;
        
        var updateDto = new UpdatePostDto
        {
            Title = "Only Title Changed",
            Content = originalContent,
            AuthorId = first.AuthorId
        };
        
        var updateResponse = await _client.PutAsJsonAsync($"/api/posts/{postId}", updateDto);
        updateResponse.EnsureSuccessStatusCode();
        
        var getResponse = await _client.GetAsync($"/api/posts/{postId}");
        var updatedPost = await getResponse.Content.ReadFromJsonAsync<PostDto>();
        
        updatedPost.Title.Should().Be("Only Title Changed");
        updatedPost.Content.Should().Be(originalContent);
    }
    
    [Test]
    public async Task UpdatePost_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var updateDto = new UpdatePostDto
        {
            Title = "Updated"
        };
        
        var response = await _client.PutAsJsonAsync($"/api/posts/{invalidId}", updateDto);
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    // DELETE /api/posts/{id} tests
    
    [Test]
    public async Task DeletePost_WithValidId_ReturnsNoContent()
    {
        var usersResponse = await _client.GetAsync("/api/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var authorId = users.First().Id;
        
        // Create a post to delete
        var newPost = new PostDto
        {
            Title = "ToDelete",
            Content = "This post will be deleted.",
            AuthorId = authorId
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/posts", newPost);
        var createdPost = await createResponse.Content.ReadFromJsonAsync<PostDto>();
        
        // Delete the post
        var response = await _client.DeleteAsync($"/api/posts/{createdPost.Id}");
        
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Test]
    public async Task DeletePost_WithValidId_PostIsRemoved()
    {
        var usersResponse = await _client.GetAsync("/api/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var authorId = users.First().Id;
        
        // Create a post to delete
        var newPost = new PostDto
        {
            Title = "ToDeleteAndVerify",
            Content = "This post will be deleted and verified.",
            AuthorId = authorId
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/posts", newPost);
        var createdPost = await createResponse.Content.ReadFromJsonAsync<PostDto>();
        
        // Delete the post
        await _client.DeleteAsync($"/api/posts/{createdPost.Id}");
        
        // Verify deletion by trying to get the post
        var getResponse = await _client.GetAsync($"/api/posts/{createdPost.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task DeletePost_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var response = await _client.DeleteAsync($"/api/posts/{invalidId}");
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task DeletePost_ReducesPostCount()
    {
        var usersResponse = await _client.GetAsync("/api/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var authorId = users.First().Id;
        
        // Create a post
        var newPost = new PostDto
        {
            Title = "Temp Post",
            Content = "Temporary content.",
            AuthorId = authorId
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/posts", newPost);
        var createdPost = await createResponse.Content.ReadFromJsonAsync<PostDto>();
        
        // Get initial count
        var beforeDelete = await _client.GetAsync("/api/posts");
        var postsBeforeDelete = await beforeDelete.Content.ReadFromJsonAsync<List<PostDto>>();
        var countBefore = postsBeforeDelete.Count;
        
        // Delete the post
        await _client.DeleteAsync($"/api/posts/{createdPost.Id}");
        
        // Get new count
        var afterDelete = await _client.GetAsync("/api/posts");
        var postsAfterDelete = await afterDelete.Content.ReadFromJsonAsync<List<PostDto>>();
        var countAfter = postsAfterDelete.Count;
        
        countAfter.Should().Be(countBefore - 1);
    }
    
    // Integration tests with Users
    
    [Test]
    public async Task GetPost_WithAuthorId_ShouldMatchValidUser()
    {
        var usersResponse = await _client.GetAsync("/api/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var validUserIds = users.Select(u => u.Id).ToList();
        
        var postsResponse = await _client.GetAsync("/api/posts");
        var posts = await postsResponse.Content.ReadFromJsonAsync<List<PostDto>>();
        
        posts.Should().AllSatisfy(p => validUserIds.Should().Contain(p.AuthorId));
    }
    
    [Test]
    public async Task CreatePost_WithAliceAsAuthor_IsIncludedInAlicesPosts()
    {
        var usersResponse = await _client.GetAsync("/api/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var aliceId = users.First(u => u.Username == "AliceDev").Id;
        
        var newPost = new PostDto
        {
            Title = "Alice's New Post",
            Content = "Alice wrote this.",
            AuthorId = aliceId
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/posts", newPost);
        var createdPost = await createResponse.Content.ReadFromJsonAsync<PostDto>();
        
        // Get Alice's posts from her user profile
        var getUserResponse = await _client.GetAsync($"/api/users/{aliceId}");
        var alice = await getUserResponse.Content.ReadFromJsonAsync<UserDto>();
        alice.PostsId.Should().Contain(createdPost.Id.Value);
    }
}

