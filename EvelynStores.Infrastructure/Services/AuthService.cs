using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EvelynStores.Core.DTOs;
using EvelynStores.Core.Entities;
using EvelynStores.Core.Models;
using EvelynStores.Core.Services;
using EvelynStores.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EvelynStores.Infrastructure.Services;

public class AuthService(
    IUserRepository userRepository,
    IOptions<JwtSettings> jwtSettings,
    EvelynStoresDbContext dbContext,
    IEmailService emailService) : IAuthService
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
            UserType = registerDto.UserType,
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
            UserType = user.UserType,
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

    public async Task<EvelynPhilApiResponse> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await userRepository.GetByEmailAsync(forgotPasswordDto.Email);

        if (user is null)
        {
            return EvelynPhilApiResponse.SuccessResponse("If the email exists, an OTP has been sent.");
        }

        // Invalidate any existing unused OTPs for this user
        var existingOtps = await dbContext.PasswordResetOtps
            .Where(o => o.UserId == user.Id && !o.IsUsed)
            .ToListAsync();

        foreach (var otp in existingOtps)
        {
            otp.IsUsed = true;
        }

        // Generate a 4-digit OTP
        var otpCode = RandomNumberGenerator.GetInt32(1000, 10000).ToString();

        var passwordResetOtp = new PasswordResetOtp
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            OtpCode = otpCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.PasswordResetOtps.Add(passwordResetOtp);
        await dbContext.SaveChangesAsync();

        await emailService.SendOtpEmailAsync(user.Email, otpCode);

        return EvelynPhilApiResponse.SuccessResponse("If the email exists, an OTP has been sent.");
    }

    public async Task<EvelynPhilApiResponse<string>> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
    {
        var user = await userRepository.GetByEmailAsync(verifyOtpDto.Email);

        if (user is null)
        {
            return EvelynPhilApiResponse<string>.ErrorResponse("Invalid OTP.", 400);
        }

        var otp = await dbContext.PasswordResetOtps
            .Where(o => o.UserId == user.Id
                        && o.OtpCode == verifyOtpDto.OtpCode
                        && !o.IsUsed
                        && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp is null)
        {
            return EvelynPhilApiResponse<string>.ErrorResponse("Invalid or expired OTP.", 400);
        }

        // Generate a short-lived reset token
        var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        otp.IsUsed = true;
        otp.ResetToken = resetToken;
        otp.ExpiresAt = DateTime.UtcNow.AddMinutes(15);
        await dbContext.SaveChangesAsync();

        return EvelynPhilApiResponse<string>.SuccessResponse(resetToken, "OTP verified successfully.");
    }

    public async Task<EvelynPhilApiResponse> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await userRepository.GetByEmailAsync(resetPasswordDto.Email);

        if (user is null)
        {
            return EvelynPhilApiResponse.ErrorResponse("Invalid request.", 400);
        }

        var resetRecord = await dbContext.PasswordResetOtps
            .Where(o => o.UserId == user.Id
                        && o.ResetToken == resetPasswordDto.ResetToken
                        && o.IsUsed
                        && o.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (resetRecord is null)
        {
            return EvelynPhilApiResponse.ErrorResponse("Invalid or expired reset token.", 400);
        }

        resetRecord.ResetToken = null;

        user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
        await userRepository.UpdateAsync(user);
        await dbContext.SaveChangesAsync();

        return EvelynPhilApiResponse.SuccessResponse("Password reset successfully.");
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
