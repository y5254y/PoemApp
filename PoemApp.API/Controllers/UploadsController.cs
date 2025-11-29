using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadsController> _logger;

    public UploadsController(IWebHostEnvironment env, ILogger<UploadsController> logger)
    {
        _env = env;
        _logger = logger;
    }

    [HttpPost("images")]
    public async Task<IActionResult> UploadImage()
    {
        try
        {
            if (!Request.HasFormContentType) return BadRequest("Expected form data");
            var form = await Request.ReadFormAsync();
            var file = form.Files.GetFile("file");
            if (file == null || file.Length == 0) return BadRequest("No file");

            // basic validation
            if (!file.ContentType.StartsWith("image/")) return BadRequest("Invalid content type");
            if (file.Length > 10 * 1024 * 1024) return BadRequest("File too large");

            var uploads = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var ext = Path.GetExtension(file.FileName);
            var fileName = Path.GetRandomFileName() + (string.IsNullOrEmpty(ext) ? ".jpg" : ext);
            var filePath = Path.Combine(uploads, fileName);

            // save original
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            // generate a thumbnail (max width 1200)
            try
            {
                using var image = Image.Load(filePath);
                var ratio = Math.Min(1.0, 1200.0 / image.Width);
                var thumbPath = Path.Combine(uploads, "thumb_" + fileName);
                if (ratio < 1.0)
                {
                    image.Mutate(x => x.Resize((int)(image.Width * ratio), 0));
                    var encoder = new JpegEncoder { Quality = 85 };
                    await image.SaveAsync(thumbPath, encoder);
                }
                else
                {
                    // copy original as thumb if small
                    System.IO.File.Copy(filePath, thumbPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Thumbnail generation failed for {File}", fileName);
            }

            var baseUrl = Request.Scheme + "://" + Request.Host.Value;
            var url = $"{baseUrl}/uploads/{fileName}";
            return Ok(new { url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed");
            return StatusCode(500, "Upload failed");
        }
    }
}
