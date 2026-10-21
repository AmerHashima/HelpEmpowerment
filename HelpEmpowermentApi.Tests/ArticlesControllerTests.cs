using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.Controllers;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Xunit;

namespace HelpEmpowermentApi.Tests;

public sealed class ArticlesControllerTests
{
    [Fact]
    public async Task Update_then_get_returns_the_saved_article_values()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"articles-{Guid.NewGuid()}")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;
        await using var db = new ApplicationDbContext(options);
        var id = Guid.NewGuid();
        db.Articles.Add(new Article
        {
            Oid = id,
            Category = "general",
            Slug = "before-update",
            TitleEn = "Before",
            TitleAr = "قبل",
            Status = "Published",
            ContentJson = "[]"
        });
        await db.SaveChangesAsync();

        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var controller = new ArticlesController(db, configuration)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        [new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())], "test"))
                }
            }
        };
        var update = await controller.Update(new UpdateArticleDto
        {
            Oid = id,
            Category = "general",
            Slug = "after-update",
            TitleEn = "After",
            TitleAr = "بعد",
            SummaryEn = "Saved summary",
            Status = "Published",
            ReadingTimeMinutes = 5,
            Blocks = [new ArticleBlockDto { Type = "details", TextEn = "Saved content", TextAr = "محتوى محفوظ", OrderNo = 1 }]
        }, CancellationToken.None);

        Assert.IsType<OkObjectResult>(update.Result);
        db.ChangeTracker.Clear();

        var get = await controller.Get(id, CancellationToken.None);
        var response = Assert.IsType<ApiResponse<ArticleDto>>(Assert.IsType<OkObjectResult>(get.Result).Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("after-update", response.Data!.Slug);
        Assert.Equal("After", response.Data.TitleEn);
        Assert.Equal("Saved summary", response.Data.SummaryEn);
        Assert.Equal("Saved content", response.Data.Blocks.Single().TextEn);
    }
}
