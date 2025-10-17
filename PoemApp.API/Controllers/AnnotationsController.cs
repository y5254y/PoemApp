// AnnotationsController.cs
using Microsoft.AspNetCore.Mvc;
using PoemApp.Core.DTOs;
using PoemApp.Core.Interfaces;

namespace PoemApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnotationsController : ControllerBase
    {
        private readonly IAnnotationService _annotationService;

        public AnnotationsController(IAnnotationService annotationService)
        {
            _annotationService = annotationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnotationDto>>> GetAnnotations()
        {
            var annotations = await _annotationService.GetAllAnnotationsAsync();
            return Ok(annotations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnnotationDto>> GetAnnotation(int id)
        {
            var annotation = await _annotationService.GetAnnotationByIdAsync(id);
            if (annotation == null)
            {
                return NotFound();
            }
            return Ok(annotation);
        }

        [HttpGet("poem/{poemId}")]
        public async Task<ActionResult<IEnumerable<AnnotationDto>>> GetAnnotationsByPoem(int poemId)
        {
            var annotations = await _annotationService.GetAnnotationsByPoemIdAsync(poemId);
            return Ok(annotations);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AnnotationDto>>> GetAnnotationsByUser(int userId)
        {
            var annotations = await _annotationService.GetAnnotationsByUserIdAsync(userId);
            return Ok(annotations);
        }

        [HttpPost]
        public async Task<ActionResult<AnnotationDto>> PostAnnotation(CreateAnnotationDto annotationDto)
        {
            try
            {
                // 从认证中获取用户ID（这里需要根据你的认证系统实现）
                int userId = GetCurrentUserId();

                var createdAnnotation = await _annotationService.AddAnnotationAsync(annotationDto, userId);
                return CreatedAtAction(nameof(GetAnnotation), new { id = createdAnnotation.Id }, createdAnnotation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnnotation(int id, UpdateAnnotationDto annotationDto)
        {
            try
            {
                // 从认证中获取用户ID
                int userId = GetCurrentUserId();

                await _annotationService.UpdateAnnotationAsync(id, annotationDto, userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnotation(int id)
        {
            try
            {
                // 从认证中获取用户ID
                int userId = GetCurrentUserId();

                await _annotationService.DeleteAnnotationAsync(id, userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // 辅助方法：从认证中获取当前用户ID
        private int GetCurrentUserId()
        {
            // 这里需要根据你的认证系统实现
            // 例如，如果你使用JWT，可以从Claims中获取用户ID
            // 这里返回一个示例值，实际应用中需要替换为真实的用户ID获取逻辑
            return 1;
        }
    }
}