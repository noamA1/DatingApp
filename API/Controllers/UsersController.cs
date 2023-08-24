// using System.Windows.Markup;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]

    // [ApiController]
    // [Route("api/[controller]")] // /api/users
    public class UsersController : BaseApiController
    {
        // private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(DataContext context, IUserRepository userRepository, IMapper mapper)
        {
            // _context = context;
            _mapper = mapper;
            _userRepository = userRepository;

        }


        [HttpGet]
        // public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            // var users = await _context.Users.ToListAsync();
            // return users;
            var users = await _userRepository.GetMembersAsync();
            // var users = await _userRepository.GetUsersAysnc();
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            return Ok(users);

        }


        // [HttpGet("{id}")] // /api/users/2
        // public async Task<ActionResult<AppUser>> GetUser(int id)
        // {
        //     return await _context.Users.FindAsync(id);
        // }
        [HttpGet("{username}")] // /api/users/2
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            // var users = await _userRepository.GetUserByUsernameAsync(username);
            return await _userRepository.GetMemberAsync(username);

            // return _mapper.Map<MemberDto>(users);
        }
    }
}
