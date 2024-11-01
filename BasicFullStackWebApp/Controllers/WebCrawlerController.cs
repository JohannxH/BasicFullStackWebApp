using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WebCrawlerController : ControllerBase
{
    private readonly WebCrawlerService _webCrawlerService;

    public WebCrawlerController(WebCrawlerService webCrawlerService)
    {
        _webCrawlerService = webCrawlerService;
    }

    [HttpGet("crawl")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Crawl([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("URL is required.");
        }

        await _webCrawlerService.CrawlAsync(url);
        return Ok("Crawling completed.");
    }

    [HttpGet("fetch-html")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> FetchHtml([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("URL is required.");
        }

        try
        {
            string htmlContent = await _webCrawlerService.FetchPageAsync(url);
            return Ok(htmlContent);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching HTML content: {ex.Message}");
        }
    }

    [HttpGet("extract-data")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExtractData([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("URL is required.");
        }

        try
        {
            List<DatasetInfo> datasets = await _webCrawlerService.ExtractDataAsync(url);
            return Ok(datasets);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error extracting data: {ex.Message}");
        }
    }
}
