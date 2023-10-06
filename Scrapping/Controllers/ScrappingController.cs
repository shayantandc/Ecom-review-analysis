using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Scrapping_Namesapce;
using Sentiment_Analysis_Service;

namespace Scrapping.Controllers
{
    [Route("ScrapingTool/[controller]")]
    [ApiController]
    public class ScrappingController : ControllerBase
    {
        private readonly IScrapping _scraptool;
        private readonly IMemoryCache _cache;

        public ScrappingController(IScrapping scraptool, IMemoryCache memoryCache)
        {
            _scraptool = scraptool;
            _cache = memoryCache;
        }

        [HttpGet("GetSentimentalAnalysis")]
        public async Task<IActionResult> GetSentimentalAnalysis(string url)
        {
            try
            {
                var updatedListingDetail = await _scraptool.PerformSentimentAnalysisAsync(url);
                _cache.Set("UpdatedListingDetail", updatedListingDetail);

                return Ok(updatedListingDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAverageSentiment")]
        public IActionResult GetAverageSentiment()
        {
            try
            {
                if (_cache.TryGetValue("UpdatedListingDetail", out List<ListingDetail> updatedListingDetail))
                {
                    string result = _scraptool.AnalyzeAverageSentiment(updatedListingDetail);
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Data not found in cache.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
