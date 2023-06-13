﻿using issueTruckerApi.Models;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace IssueTrackerApi.Controllers;

public class IssuesController : ControllerBase
{
    private readonly IDocumentStore _documentStore;

    public IssuesController(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    [HttpGet("/open-issues")]
    public async Task<ActionResult> GetOpenIssues()
    {
        using var session = _documentStore.LightweightSession();
        var data = await session.Query<IssueCreatedResponse>().Where(issue => issue.ClosedAt == null).ToListAsync();

        return Ok(new { issues = data });
    }

    [HttpPost("/open-issues")]
    public async Task<ActionResult> AddAnIssue([FromBody] IssueCreateRequest request)
    {
      
        var response = new IssueCreatedResponse
        {
            Id = Guid.NewGuid(),
            Issue = request.Issue,
            From = request.From,
            CreatedAt = DateTime.UtcNow,
        };

        using var session = _documentStore.LightweightSession();
        session.Insert(response);
        await session.SaveChangesAsync();

        return Ok(response);
    }

}