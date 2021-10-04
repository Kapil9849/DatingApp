using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class UsersController : BaseApiController
    {

        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _pservice;

        public UsersController(IUserRepository repository, IMapper mapper,
        IPhotoService pservice)
        {
            _pservice = pservice;
            this._repository = repository;
            this._mapper = mapper;
        }

        [HttpGet]

        public async Task<ActionResult<MemberDto>> GetUsers()
        {
            // var users=await _repository.GetUsersAsync();
            // var result=_mapper.Map<IEnumerable<MemberDto>>(users);
            var result = await _repository.GetMembersAsync();
            return Ok(result);

        }

        [HttpGet("{username}",Name="GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _repository.GetMemberAsync(username);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto member)
        {
            var username = User.GetUserName();
            var user = await _repository.GetUserByUsernameAsync(username);

            _mapper.Map(member, user);

            _repository.Update(user);

            if (await _repository.SaveAllAsync())
                return NoContent();
            else
                return BadRequest("Failed to Update UserData");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user =await _repository.GetUserByUsernameAsync(User.GetUserName());
            var result = await _pservice.AddPhotoAsync(file);

            if(result.Error != null) 
            {
            return BadRequest(result.Error.Message);
            }

            var photo= new Photo
            {
                Url=result.SecureUrl.AbsoluteUri,
                PublicId=result.PublicId
            };

            if(user.Photos.Count==0)
            {
                photo.IsMain=true;
            }
            user.Photos.Add(photo);
            if(await _repository.SaveAllAsync())
            {
                // return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser",new {username=user.userName},_mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem Adding Photo");
        }

        [HttpPut("set-main-photo/{photoid}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user=await _repository.GetUserByUsernameAsync(User.GetUserName());

            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if(photo.IsMain)
            {
                return BadRequest("This is already your main photo");
            }

            var currentMain=user.Photos.FirstOrDefault(x=>x.IsMain);
            if(currentMain!=null)
            {
                currentMain.IsMain=false;

            }
            photo.IsMain=true;
            if(await _repository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to set Main Photo");
        }

        [HttpDelete("delete-photo/{id}")]
        public async Task<ActionResult> DeletePhoto(int id)
        {
            var user=await _repository.GetUserByUsernameAsync(User.GetUserName());
            var photo=user.Photos.FirstOrDefault(x=>x.Id==id);
            if(photo==null) return NotFound();

            if(photo.IsMain) return BadRequest("You Cannot delete");
            if(photo.PublicId!=null)
            {
                var result=await _pservice.DeletePhotoAsync(photo.PublicId);
                if(result.Error!=null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if(await _repository.SaveAllAsync()) return Ok();

            return BadRequest("Failes to delete photo");
        }

    }
}