// using System.Windows.Markup;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
        private readonly IPhotoService _photoService;

        public UsersController(DataContext context, IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            // _context = context;
            _mapper = mapper;
            _userRepository = userRepository;
            _photoService = photoService;
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

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
                //  return _mapper.Map<PhotoDto>(photo)
            }

            return BadRequest("problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("this is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting the main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting photo");
        }
    }
}
