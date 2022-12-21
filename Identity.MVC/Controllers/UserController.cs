using System.Text.Json.Nodes;
using AutoMapper;
using Identity.MVC.Models;
using Identity.MVC.Models.DTO;
using Identity.MVC.Repository;
using Identity.MVC.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace Identity.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityRepository _identityRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper, IIdentityRepository identityRepository)
        {

            _userRepository = userRepository;
            _mapper = mapper;
            _identityRepository = identityRepository;
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
            else
            {
                var token = _identityRepository.GenerateIdentifier(user);
                _identityRepository.SetIdentifierToResponse(token, Response);
                return RedirectToAction("WhoAmI");
            }
        }

        [HttpGet]
        public async Task<IActionResult> WhoAmI()
        {
            var token = _identityRepository.GetIdentifierFromRequest(Request);
            if (token == null) return NotFound("Token is not found. you are not logged in");
            var isTokenValid = _identityRepository.TryParseIdentifier(token);
            if (!isTokenValid) return Unauthorized("Token is not valid.");
            var user = _identityRepository.IdentifierEntity(token);
            if (user == null) return NotFound("User Not Found.");
            return View(user);
        }

        public IActionResult Logout()
        {
            _identityRepository.RemoveIdentifierFromResponse(Response);
            return RedirectToAction("Index");
        }
    }
}
