using System.Net;
using System.Net.Http.Json;
using ExampleAPI.Generated.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace WebAPI_DSL_SystemTests;

[TestFixture]
public class UsersEndpointTest
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
    
    // GET /api/users tests
    
    [Test]
    public async Task GetUsers_WhenCalled_ReturnsOkAndData()
    {
        var response = await _client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        users.Should().NotBeNull();
    }
    
    [Test]
    public async Task GetUsers_WhenCalled_ReturnsSeededUsers()
    {
        var response = await _client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        users.Should().HaveCount(2);
        users.Should().Contain(u => u.Username == "AliceDev");
        users.Should().Contain(u => u.Username == "BobCoder");
    }
    
    [Test]
    public async Task GetUsers_WhenCalled_ReturnsUsersWithCorrectAccountAge()
    {
        var response = await _client.GetAsync("/api/users");
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();

        var alice = users.First(u => u.Username == "AliceDev");
        var bob = users.First(u => u.Username == "BobCoder");
        
        alice.AccountAge.Should().Be(5);
        bob.AccountAge.Should().Be(2);
    }
    
    [Test]
    public async Task GetUsers_WhenCalled_ReturnsBestFriendRelationships()
    {
        var response = await _client.GetAsync("/api/users");
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();

        var alice = users.First(u => u.Username == "AliceDev");
        var bob = users.First(u => u.Username == "BobCoder");
        
        // Both users should have a BestFriendId
        alice.BestFriendId.Should().NotBeNull();
        bob.BestFriendId.Should().NotBeNull();
        
        // They should be each other's best friends
        alice.BestFriendId.Should().Be(bob.Id);
        bob.BestFriendId.Should().Be(alice.Id);
    }
    
    [Test]
    public async Task GetUsers_WhenCalled_ReturnsFriendRelationships()
    {
        var response = await _client.GetAsync("/api/users");
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();

        var alice = users.First(u => u.Username == "AliceDev");
        var bob = users.First(u => u.Username == "BobCoder");
        
        // Both users should have friends
        alice.FirendsId.Should().NotBeNull();
        bob.FirendsId.Should().NotBeNull();
        
        // They should be in each other's friend lists
        alice.FirendsId.Should().Contain(bob.Id.Value);
        bob.FirendsId.Should().Contain(alice.Id.Value);
    }
    
    [Test]
    public async Task GetUsers_WhenCalled_ReturnsUserPostIds()
    {
        var response = await _client.GetAsync("/api/users");
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();

        var alice = users.First(u => u.Username == "AliceDev");
        var bob = users.First(u => u.Username == "BobCoder");
        
        // Alice has 2 posts, Bob has 1 post based on DbInitializer
        alice.PostsId.Should().NotBeNull();
        alice.PostsId.Should().HaveCount(2);
        
        bob.PostsId.Should().NotBeNull();
        bob.PostsId.Should().HaveCount(1);
    }
    
    // GET /api/users/{id} tests
    
    [Test]
    public async Task GetUser_WithValidId_ReturnsOkAndUser()
    {
        // First get all users to get a valid ID
        var listResponse = await _client.GetAsync("/api/users");
        var users = await listResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var userId = users.First().Id;
        
        var response = await _client.GetAsync($"/api/users/{userId}");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Id.Should().Be(userId);
    }
    
    [Test]
    public async Task GetUser_WithAliceId_ReturnsAliceDev()
    {
        var listResponse = await _client.GetAsync("/api/users");
        var users = await listResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var aliceId = users.First(u => u.Username == "AliceDev").Id;
        
        var response = await _client.GetAsync($"/api/users/{aliceId}");
        var alice = await response.Content.ReadFromJsonAsync<UserDto>();
        
        alice.Username.Should().Be("AliceDev");
        alice.AccountAge.Should().Be(5);
    }
    
    [Test]
    public async Task GetUser_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/users/{invalidId}");
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    // POST /api/users tests
    
    [Test]
    public async Task CreateUser_WithValidData_ReturnsCreatedAtAction()
    {
        var newUser = new UserDto
        {
            Username = "CharlieDev",
            AccountAge = 3
        };
        
        var response = await _client.PostAsJsonAsync("/api/users", newUser);
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }
    
    [Test]
    public async Task CreateUser_WithValidData_ReturnsCreatedUser()
    {
        var newUser = new UserDto
        {
            Username = "DianaCode",
            AccountAge = 1
        };
        
        var response = await _client.PostAsJsonAsync("/api/users", newUser);
        var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();
        
        createdUser.Username.Should().Be("DianaCode");
        createdUser.AccountAge.Should().Be(1);
        createdUser.Id.Should().NotBeEmpty();
    }
    
    // PUT /api/users/{id} tests
    
    [Test]
    public async Task UpdateUser_WithValidId_ReturnsNoContent()
    {
        var listResponse = await _client.GetAsync("/api/users");
        var users = await listResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var userId = users.First().Id;
        
        var updateDto = new UpdateUserDto
        {
            Username = "UpdatedUsername",
            AccountAge = 10
        };
        
        var response = await _client.PutAsJsonAsync($"/api/users/{userId}", updateDto);
        
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Test]
    public async Task UpdateUser_WithValidData_UpdatesUserSuccessfully()
    {
        var listResponse = await _client.GetAsync("/api/users");
        var users = await listResponse.Content.ReadFromJsonAsync<List<UserDto>>();
        var aliceId = users.First(u => u.Username == "AliceDev").Id;
        
        var updateDto = new UpdateUserDto
        {
            Username = "AliceUpdated",
            AccountAge = 6
        };
        
        await _client.PutAsJsonAsync($"/api/users/{aliceId}", updateDto);
        
        var getResponse = await _client.GetAsync($"/api/users/{aliceId}");
        var updatedUser = await getResponse.Content.ReadFromJsonAsync<UserDto>();
        
        updatedUser.Username.Should().Be("AliceUpdated");
        updatedUser.AccountAge.Should().Be(6);
    }
    
    [Test]
    public async Task UpdateUser_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var updateDto = new UpdateUserDto
        {
            Username = "Updated"
        };
        
        var response = await _client.PutAsJsonAsync($"/api/users/{invalidId}", updateDto);
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    // DELETE /api/users/{id} tests
    
    [Test]
    public async Task DeleteUser_WithValidId_ReturnsNoContent()
    {
        // Create a user to delete
        var newUser = new UserDto
        {
            Username = "ToDelete",
            AccountAge = 0
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/users", newUser);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        
        // Delete the user
        var response = await _client.DeleteAsync($"/api/users/{createdUser.Id}");
        
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Test]
    public async Task DeleteUser_WithValidId_UserIsRemoved()
    {
        // Create a user to delete
        var newUser = new UserDto
        {
            Username = "ToDeleteAndVerify",
            AccountAge = 0
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/users", newUser);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        
        // Delete the user
        await _client.DeleteAsync($"/api/users/{createdUser.Id}");
        
        // Verify deletion by trying to get the user
        var getResponse = await _client.GetAsync($"/api/users/{createdUser.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task DeleteUser_WithInvalidId_ReturnsNotFound()
    {
        var invalidId = Guid.NewGuid();
        var response = await _client.DeleteAsync($"/api/users/{invalidId}");
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    // Search endpoint tests
    
    [Test]
    public async Task SearchUsers_WithUsernameFilter_ReturnsMatchingUsers()
    {
        var response = await _client.GetAsync("/api/users/search?username=AliceDev");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        
        users.Should().HaveCount(1);
        users.First().Username.Should().Contain("Alice");
    }
    
    [Test]
    public async Task SearchUsers_WithAccountAgeRange_ReturnsMatchingUsers()
    {
        var response = await _client.GetAsync("/api/users/search?minAccountAge=2&maxAccountAge=5");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        
        users.Should().AllSatisfy(u => u.AccountAge.Should().BeGreaterThanOrEqualTo(2).And.BeLessThanOrEqualTo(5));
    }
    
    [Test]
    public async Task SearchUsers_WithNoFilters_ReturnsAllUsers()
    {
        var response = await _client.GetAsync("/api/users/search");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        
        users.Should().NotBeEmpty();
    }
}