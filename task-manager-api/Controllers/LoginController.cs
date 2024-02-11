using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaskAPI.Data;
using TaskAPI.Data.Entities;
using TaskAPI.Views;

namespace TaskAPI.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly TaskContext _context;
    private readonly IConfiguration _configuration;

    public LoginController(
        IMapper mapper,
        TaskContext context,
        IConfiguration configuration)
    {
        _mapper = mapper;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost(Name = "Login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Login(LoginView view)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == view.Email);
        if (user == null)
            return BadRequest("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(view.Password, user.PasswordHash))
            return BadRequest("Wrong password.");

        string token = CreateToken(user);
        
        return Ok(token);
    }

    [HttpPost("create-account", Name="Create Account")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Create(UserView view)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(view.Password);
        var model = _mapper.Map<UserModel>(view);
        model.PasswordHash = passwordHash;
        if (model == null)
            return BadRequest();

        _context.Users.Add(model);
        _context.SaveChanges();

        return Ok(model);
    }

    private string CreateToken(UserModel user)
    {
        var key = Encoding.ASCII.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value!);
        
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var tokenConfig = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenConfig);
        var jwt = tokenHandler.WriteToken(token);

        return jwt;
    }
}