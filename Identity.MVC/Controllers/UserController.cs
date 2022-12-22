using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using Identity.MVC.Models;
using Identity.MVC.Models.DTO;
using Identity.MVC.Repository;
using Identity.MVC.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NuGet.Protocol;

namespace Identity.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        // 1:
        private readonly IIdentityRepository<string, JwtSecurityToken> _identityRepository;
        // 2:
        //private readonly IIdentityRepository<string, User> _identityRepository;
        private readonly ICacheRepository<User> _cacheRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository
            , IMapper mapper
            // 1:
            ,IIdentityRepository<string, JwtSecurityToken> identityRepository
            // 2:
            //, IIdentityRepository<string, User> identityRepository
            , ICacheRepository<User> userCacheRepository, IDistributedCache cache)
        {

            _userRepository = userRepository;
            _mapper = mapper;
            _identityRepository = identityRepository;
            _cacheRepository = userCacheRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Get - Create
        public IActionResult Create()
        {
            return View();
        }

        // Post - Create
        [HttpPost]
        public async Task<IActionResult> Create(UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (!await _userRepository.IsUsernameUnique(userDTO.Username))
            {
                return Conflict("Username is already in use");
            }

            var user = _mapper.Map<User>(userDTO);
            await _userRepository.CreateAsync(user);
            
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (!ModelState.IsValid) return BadRequest();
            return View();
        }

        // post - login
        [HttpPost]
        public async Task<IActionResult> Login(UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest();
            var user = await _userRepository
                .GetUserFromUsernamePasswordAsync(userDTO.Username, userDTO.Password);
            if (user == null)
                return Unauthorized();
            var identifier = await _identityRepository.GenerateIdentifier(user);
            _identityRepository.SetIdentifierToResponse(identifier, Response);
            return RedirectToAction("WhoAmI");
        }

        [HttpGet]
        public async Task<IActionResult> WhoAmI()
        {
            var identifier = _identityRepository.GetIdentifierFromRequest(Request);
            if (identifier == null) 
                return NotFound($"identifier not found. you are not logged in.");
            var isIdentifierValid = _identityRepository.TryParseIdentifier(identifier, out var clearIdentifier);
            if (!isIdentifierValid) 
                return NotFound("identifier is not valid. you are not logged in.");
            var user = _identityRepository.IdentifierEntity(clearIdentifier);
            if (user == null) 
                return NotFound("Token is not valid.");
            _identityRepository.ReviveIdentifier(identifier, Response);
            return View(user);
        }

        public IActionResult Logout()
        {
            var identifier = _identityRepository.GetIdentifierFromRequest(Request);
            if (identifier == null)
                return NotFound($"identifier not found. you are not logged in.");
            var isIdentifierValid = _identityRepository.TryParseIdentifier(identifier, out var clearIdentifier);
            if (!isIdentifierValid)
                return NotFound("identifier is not valid. you are not logged in.");
            _identityRepository.RemoveIdentifierFromResponse(identifier, Response);
            return RedirectToAction("Index");
        }
    }
}
