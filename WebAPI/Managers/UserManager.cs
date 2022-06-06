using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using SharedResources;
using SharedResources.Enums;
using SharedResources.Interfaces.Data;
using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static SharedResources.Constants;

namespace WebAPI.Managers
{
    public class UserManager : IUserManager
    {
        private UserManager() { }
        public static UserManager Instance { get; } = new UserManager();

        private List<IUser> _ActiveUsers = new();
        public void RemoveActiveUser(string? uid)
        {
            if (uid == null) return;
            int found = _ActiveUsers.FindIndex(x => x.UID == uid);
            if (found >= 0 && found < _ActiveUsers.Count)
                _ActiveUsers.RemoveAt(found);
        }

        public IResponse Register(IRegisterRequest request)
        {
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest);
            IUser user = Core.CreateIUser();
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM user WHERE user.email = LOWER(@email);", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("email", request.Email));
                try
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                            if (reader.GetInt16(0) > 0) return Core.CreateErrorResponse(Result.AccountAlreadyExists);
                    }
                }
                catch { return Core.CreateErrorResponse(Result.Internal); }
            }
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM user WHERE user.username = LOWER(@username);", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("username", request.Username));
                try
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                            if (reader.GetInt16(0) > 0) return Core.CreateErrorResponse(Result.UsernameAlreadyExists);
                    }
                }
                catch { return Core.CreateErrorResponse(Result.Internal); }
            }
            user.Username = request.Username;
            user.Email = request.Email;
            user.Tag = RandomGenerator.NextStringInt(5);
            user.UID = GenerateToken(TokenType.UID);
            using (MySqlCommand command = new MySqlCommand("INSERT INTO user(uid, username, tag, email, password, email_verified) VALUES(@uid, @username, @tag, @email, @password, @email_verified)", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("uid", user.UID));
                command.Parameters.Add(new MySqlParameter("username", user.Username));
                command.Parameters.Add(new MySqlParameter("tag", user.Tag));
                command.Parameters.Add(new MySqlParameter("email", user.Email));
                command.Parameters.Add(new MySqlParameter("password", request.Password));
                command.Parameters.Add(new MySqlParameter("email_verified", false));
                try
                {
                    command.ExecuteNonQuery();
                }
                catch { return Core.CreateErrorResponse(Result.Internal); }
            }
            user.AccessToken = GenerateToken(TokenType.Access);
            user.RefreshToken = GenerateToken(TokenType.Refresh);
            user.ExpiresIn = DateTime.Now.AddSeconds(AccessTokenExpiry);
            _ActiveUsers.Add(user);
            return Core.CreateITokenResponse(user);
        }
        public IResponse Login(ILoginRequest request)
        {
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest);
            IUser user;
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM user WHERE (user.email = LOWER(@id) OR user.username = @id) LIMIT 1;", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("id", request.ID));
                try
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows) return Core.CreateErrorResponse(Result.AccountNotFound);
                        reader.Read();
                        if (reader.GetString("password") != request.Password) return Core.CreateErrorResponse(Result.InvalidCredentials);
                        user = Core.CreateIUser(reader);
                        if (user.Disabled == true) return Core.CreateErrorResponse(Result.AccountDisabled);
                    }
                }
                catch { return Core.CreateErrorResponse(Result.Internal); }
            }
            if (!user.IsValid()) return Core.CreateErrorResponse(Result.Internal);
            int found = _ActiveUsers.FindIndex(x => x.UID == user.UID);
            if (found > 0)
                return Core.CreateITokenResponse(_ActiveUsers[found]);
            user.AccessToken = GenerateToken(TokenType.Access);
            user.RefreshToken = GenerateToken(TokenType.Refresh);
            user.SetRemainingActiveTime(AccessTokenExpiry + 1);
            _ActiveUsers.Add(user);
            return Core.CreateITokenResponse(user);
        }
        public IResponse RefreshTokens(IRefreshTokenRequest request)
        {
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest);
            int refreshingUserIndex = _ActiveUsers.FindIndex(x => x.RefreshToken == request.RefreshToken);
            if (refreshingUserIndex < 0) return Core.CreateErrorResponse(Result.InvalidRefreshToken);
            _ActiveUsers[refreshingUserIndex].AccessToken = GenerateToken(TokenType.Access);
            _ActiveUsers[refreshingUserIndex].RefreshToken = GenerateToken(TokenType.Refresh);
            _ActiveUsers[refreshingUserIndex].SetRemainingActiveTime(AccessTokenExpiry);
            return Core.CreateITokenResponse(_ActiveUsers[refreshingUserIndex]);
        }

        public IResponse GetUser(IGetUserRequest request)
        {
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest);
            IUser? accessingUser = _ActiveUsers.Find(x => x.AccessToken == request.AccessToken);
            if (accessingUser == null) return Core.CreateErrorResponse(Result.InvalidAccessToken);
            IUser requestedUser;
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM user WHERE user.uid = @uid", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("uid", request.UID ?? accessingUser.UID));
                try
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows) return Core.CreateErrorResponse(Result.AccountNotFound);
                        reader.Read();
                        requestedUser = Core.CreateIUser(reader);
                    }
                }
                catch { return Core.CreateErrorResponse(Result.Internal); }
            }
            //if (requestedUser.UID != accessingUser.UID) { return Core.CreateErrorResponse(Result.Unauthorized); } //SHOWING PROFILES ALWAYS
            return Core.CreateGetUserResponse(requestedUser);
        }
        public IResponse UserExists(IUserExistsRequest request)
        {
            throw new NotImplementedException("");
        }
        public IResponse GenerateJWT(ITokenRequest request)
        {
            int index = _ActiveUsers.FindIndex(x => x.AccessToken == request.AccessToken);
            if (index < 0)
                return Core.CreateErrorResponse(Result.InvalidRequest);
            IUser user = _ActiveUsers[index];
            Claim emailClaim = new Claim(ClaimTypes.Email, user.Email);
            Claim nameClaim = new Claim(ClaimTypes.NameIdentifier, user.UID);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { emailClaim, nameClaim }, "serverAuth");

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(JWTTokenDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Core.JwtSecret), SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtTokenHandler.CreateJwtSecurityToken(securityTokenDescriptor);
            return Core.CreateIJWTResponse(jwtTokenHandler.WriteToken(jwtToken));
        }
        public IResponse AuthenticateJWT(IJWTLoginRequest request)
        {
            if (!request.IsValid())
                return Core.CreateErrorResponse(Result.InvalidRequest);
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Core.JwtSecret),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal claimPrincipal;
            try
            {
                claimPrincipal = jwtHandler.ValidateToken(request.JWTToken, validationParameters, out securityToken);
            }
            catch { return Core.CreateErrorResponse(Result.InvalidJWTToken); }
            JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)securityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                return Core.CreateErrorResponse(Result.InvalidJWTToken); //TODO: Make error & result
            string userUID;
            try { userUID = claimPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value; } catch { return Core.CreateErrorResponse(Result.InvalidJWTToken); }
            IUser user;
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM user WHERE user.uid = @uid", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("uid", userUID));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows) return Core.CreateErrorResponse(Result.InvalidJWTToken);
                    reader.Read();
                    user = Core.CreateIUser(reader);
                }
            }
            user.AccessToken = GenerateToken(TokenType.Access);
            user.RefreshToken = GenerateToken(TokenType.Refresh);
            user.SetRemainingActiveTime(AccessTokenExpiry);
            int found = _ActiveUsers.FindIndex(x => x.UID == userUID);
            if (found >= 0)
                _ActiveUsers.RemoveAt(found);
            _ActiveUsers.Add(user);
            return Core.CreateITokenResponse(user);
        }
        public IResponse CheckJWT(IJWTLoginRequest request)
        {
            if (!request.IsValid())
                return Core.CreateErrorResponse(Result.InvalidRequest);
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Core.JwtSecret),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal claimPrincipal;
            try
            {
                claimPrincipal = jwtHandler.ValidateToken(request.JWTToken, validationParameters, out securityToken);
            } catch { return Core.CreateErrorResponse(Result.InvalidJWTToken); }
            JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)securityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                return Core.CreateErrorResponse(Result.InvalidJWTToken);
            string userUID;
            try { userUID = claimPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value; } catch { return Core.CreateErrorResponse(Result.InvalidJWTToken); }
            IUser user;
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM user WHERE user.uid = @uid", Core.DatabaseManager.Database))
            {
                command.Parameters.Add(new MySqlParameter("uid", userUID));
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows) return Core.CreateErrorResponse(Result.InvalidJWTToken);
                    reader.Read();
                    user = Core.CreateIUser(reader);
                }
            }
            if (user.IsValid())
                return Core.CreateIJWTResponse(request.JWTToken);
            else
                return Core.CreateErrorResponse(Result.InvalidJWTToken);
        }

        public IUser? GetActiveUser(string accessToken) => _ActiveUsers.Find(x => x.AccessToken == accessToken);

        private string GenerateToken(TokenType type)
        {
            switch (type)
            {
                default: return "";
                case TokenType.Access: return RandomGenerator.NextString(300);
                case TokenType.Refresh: return RandomGenerator.NextString(250);
                case TokenType.UID: return RandomGenerator.NextString(100);
            }
        }
    }
}
