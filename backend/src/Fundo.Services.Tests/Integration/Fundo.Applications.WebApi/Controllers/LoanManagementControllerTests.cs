using System.Net;
using System.Net.Http.Json;
using Fundo.Applications.Data.Models;
using Fundo.Applications.Domain.Loans.Contracts;
using Fundo.Services.Tests.Integration;
using FluentAssertions;
using Xunit;

namespace Fundo.Services.Tests.Integration.Controllers;

public class LoanManagementControllerTests : IClassFixture<LoanApiFactory>
{
    private readonly LoanApiFactory _factory;

    public LoanManagementControllerTests(LoanApiFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient() =>
        _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

    private static Loan NewLoan(string name, decimal amount, decimal balance, LoanStatus status) =>
        new()
        {
            ApplicantName = name,
            Amount = amount,
            CurrentBalance = balance,
            Status = status
        };

    [Fact]
    public async Task Create_ShouldReturn201_AndBeRetrievable()
    {
        await _factory.ResetAndSeedAsync();
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/loans", new { applicantName = "Maria Silva", amount = 1500m });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<LoanResponse>();
        created!.CurrentBalance.Should().Be(1500m);
        created.Status.Should().Be("Active");

        var fetched = await client.GetFromJsonAsync<LoanResponse>($"/loans/{created.Id}");
        fetched!.Id.Should().Be(created.Id);
        fetched.ApplicantName.Should().Be("Maria Silva");
    }

    [Fact]
    public async Task List_ShouldReturnAllSeededLoans()
    {
        await _factory.ResetAndSeedAsync(
            NewLoan("Ana", 800m, 800m, LoanStatus.Active),
            NewLoan("John", 3200m, 0m, LoanStatus.Paid));
        var client = CreateClient();

        var loans = await client.GetFromJsonAsync<List<LoanResponse>>("/loans");

        loans.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_UnknownLoan_ShouldReturn404()
    {
        await _factory.ResetAndSeedAsync();
        var client = CreateClient();

        var response = await client.GetAsync($"/loans/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Payment_ShouldReduceBalance()
    {
        var loan = NewLoan("Diego", 2500m, 1000m, LoanStatus.Active);
        await _factory.ResetAndSeedAsync(loan);
        var client = CreateClient();

        var response = await client.PostAsJsonAsync($"/loans/{loan.Id}/payment", new { amount = 400m });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<LoanResponse>();
        updated!.CurrentBalance.Should().Be(600m);
        updated.Status.Should().Be("Active");
    }

    [Fact]
    public async Task Payment_SettlingBalance_ShouldMarkLoanPaid()
    {
        var loan = NewLoan("Priya", 1200m, 500m, LoanStatus.Active);
        await _factory.ResetAndSeedAsync(loan);
        var client = CreateClient();

        var response = await client.PostAsJsonAsync($"/loans/{loan.Id}/payment", new { amount = 500m });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<LoanResponse>();
        updated!.CurrentBalance.Should().Be(0m);
        updated.Status.Should().Be("Paid");
    }

    [Fact]
    public async Task Payment_OnUnknownLoan_ShouldReturn404()
    {
        await _factory.ResetAndSeedAsync();
        var client = CreateClient();

        var response = await client.PostAsJsonAsync($"/loans/{Guid.NewGuid()}/payment", new { amount = 100m });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Payment_AboveBalance_ShouldReturn409()
    {
        var loan = NewLoan("Lucas", 5000m, 500m, LoanStatus.Active);
        await _factory.ResetAndSeedAsync(loan);
        var client = CreateClient();

        var response = await client.PostAsJsonAsync($"/loans/{loan.Id}/payment", new { amount = 600m });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_WithInvalidPayload_ShouldReturn400()
    {
        await _factory.ResetAndSeedAsync();
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/loans", new { applicantName = "", amount = 0m });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
