using PoemApp.Core.DTOs;

namespace PoemApp.Admin.Services;

public class LoginDtoValidation
{
    public bool Validate(LoginDto model)
    {
        return !string.IsNullOrEmpty(model.Username) &&
               !string.IsNullOrEmpty(model.Password);
    }
}