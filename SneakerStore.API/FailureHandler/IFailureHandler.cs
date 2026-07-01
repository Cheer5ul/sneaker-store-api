using Microsoft.AspNetCore.Mvc;
using SneakerStore.Core.Results;

namespace SneakerStore.FailureHandler;

public interface IFailureHandler
{
    ActionResult HandleFailure(Result result, HttpContext httpContext);
}