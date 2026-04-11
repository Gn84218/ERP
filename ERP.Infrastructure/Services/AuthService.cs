using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Domain.Entities;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, JwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task RegisterAsync(RegisterRequest req, CancellationToken ct = default)
        {
            var username = req.Username.Trim();

            var exists = await _db.Users.AnyAsync(x => x.Username == username, ct);
            if (exists)
                throw new InvalidOperationException($"使用者帳號已存在：{username}");

            var user = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(req.Password),
                Role = string.IsNullOrWhiteSpace(req.Role) ? "User" : req.Role.Trim(),
                IsActive = true
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct = default)
        {
            var username = req.Username.Trim();
            var passwordHash = PasswordHasher.Hash(req.Password);

            var user = await _db.Users
                .SingleOrDefaultAsync(x => x.Username == username && x.PasswordHash == passwordHash && x.IsActive, ct);

            if (user == null)
                throw new InvalidOperationException("帳號或密碼錯誤。");

            var token = _jwtTokenGenerator.Generate(user);

            return new LoginResponse(token, user.Username, user.Role);
        }
    }
}
