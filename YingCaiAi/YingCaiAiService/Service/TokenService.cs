using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using YingCaiAiModel;

namespace YingCaiAiWin.Services
{
    /// <summary>
    /// Token 服务类
    /// </summary>
    public class TokenService
    {
        // Redis 连接
        private static ConnectionMultiplexer _redis;
        private static IDatabase _redisDb;

        // JWT 配置
        private static readonly string JwtSecretKey = "YingCaiAI_JWT_Secret_Key_2023";
        private static readonly string JwtIssuer = "YingCaiAI";
        private static readonly string JwtAudience = "YingCaiAIUsers";
        private static readonly int JwtExpiryInMinutes = 60; // Token 有效期为 60 分钟

        /// <summary>
        /// 构造函数
        /// </summary>
        public TokenService()
        {
            InitializeRedisConnection();
        }

        /// <summary>
        /// 初始化 Redis 连接
        /// </summary>
        private void InitializeRedisConnection()
        {
            try
            {
                if (_redis == null || !_redis.IsConnected)
                {
                    // 连接到 Redis 服务器
                    //_redis = ConnectionMultiplexer.Connect("localhost:6379");
                    //_redisDb = _redis.GetDatabase();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Redis 连接失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 生成 JWT Token
        /// </summary>
        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("UserId", user.Id.ToString())
            };

            // 添加角色声明
            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    //claims = claims.Append(new Claim(ClaimTypes.Role, role)).ToArray();
                }
            }

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(JwtExpiryInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 将 Token 存储到 Redis
        /// </summary>
        public void StoreTokenInRedis(string username, string token)
        {
            try
            {
                if (_redisDb != null)
                {
                    // 使用用户名作为 key，token 作为 value
                    string key = $"user:token:{username}";

                    // 设置 token 到 Redis，并设置过期时间
                    _redisDb.StringSet(key, token, TimeSpan.FromMinutes(JwtExpiryInMinutes));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Token 存储失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 从 Redis 获取 Token
        /// </summary>
        public string GetTokenFromRedis(string username)
        {
            try
            {
                if (_redisDb != null)
                {
                    // 使用用户名作为 key
                    string key = $"user:token:{username}";

                    // 从 Redis 获取 token
                    return _redisDb.StringGet(key);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"获取 Token 失败: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// 从 Redis 删除 Token
        /// </summary>
        public void RemoveTokenFromRedis(string username)
        {
            try
            {
                if (_redisDb != null)
                {
                    // 使用用户名作为 key
                    string key = $"user:token:{username}";

                    // 从 Redis 删除 token
                    _redisDb.KeyDelete(key);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"删除 Token 失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 验证 Token
        /// </summary>
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey)),
                ValidateIssuer = true,
                ValidIssuer = JwtIssuer,
                ValidateAudience = true,
                ValidAudience = JwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从 Token 中获取用户信息
        /// </summary>
        public User GetUserFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey)),
                ValidateIssuer = true,
                ValidIssuer = JwtIssuer,
                ValidateAudience = true,
                ValidAudience = JwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                var user = new User
                {
                    Username = principal.FindFirst(ClaimTypes.Name)?.Value,
                    Id = int.Parse(principal.FindFirst("UserId")?.Value ?? "0"),
                    Token = token,
                    //Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                };

                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}