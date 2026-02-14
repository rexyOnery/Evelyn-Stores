using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Models;
using EvelynStores.Core.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EvelynStores.Infrastructure.Services;

public class AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<EvelynPhilApiResponse> RegisterAsync(RegisterDto registerDto)
    {
        if (await EmailExistsAsync(registerDto.Email))
        {
            return EvelynPhilApiResponse.ErrorResponse("An account with this email already exists.", 409);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = registerDto.Name.Trim(),
            Email = registerDto.Email.Trim().ToLowerInvariant(),
            PasswordHash = HashPassword(registerDto.Password),
            CreatedAt = DateTime.UtcNow
        };

        await userRepository.AddAsync(user);

        return EvelynPhilApiResponse.SuccessResponse("Registration successful.", 201);
    }

    public async Task<EvelynPhilApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await userRepository.GetByEmailAsync(loginDto.Email);

        if (user is null)
        {
            return EvelynPhilApiResponse<LoginResponseDto>.ErrorResponse("Invalid email or password.", 401);
        }

        if (!VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return EvelynPhilApiResponse<LoginResponseDto>.ErrorResponse("Invalid email or password.", 401);
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var loginResponse = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.ExpirationMinutes * 60,
            UserName = user.Name,
            Email = user.Email
        };

        return EvelynPhilApiResponse<LoginResponseDto>.SuccessResponse(loginResponse, "Login successful.");
    }

    public async Task<EvelynPhilApiResponse> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return EvelynPhilApiResponse.ErrorResponse("User not found.", 404);
        }

        if (!VerifyPassword(changePasswordDto.OldPassword, user.PasswordHash))
        {
            return EvelynPhilApiResponse.ErrorResponse("Old password is incorrect.", 400);
        }

        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
        {
            return EvelynPhilApiResponse.ErrorResponse("New password and confirm password do not match.", 400);
        }

        user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
        await userRepository.UpdateAsync(user);

        return EvelynPhilApiResponse.SuccessResponse("Password changed successfully.");
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await userRepository.EmailExistsAsync(email);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim("sliding_expiration", _jwtSettings.SlidingExpirationMinutes.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHashBytes = Convert.FromBase64String(parts[1]);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return CryptographicOperations.FixedTimeEquals(hash, storedHashBytes);
    }

    private static string HashPassword(string password)
    {
        const int saltSize = 16;
        const int hashSize = 32;
        const int iterations = 100_000;

        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            hashSize);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
}
