using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    
    public class UsersController : BaseApiController
    {
        
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

        [HttpGet]

        public async Task<ActionResult<MemberDto>> GetUsers()
        {
            // var users=await _repository.GetUsersAsync();
            // var result=_mapper.Map<IEnumerable<MemberDto>>(users);
            var result=await _repository.GetMembersAsync();
            return Ok(result);

        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _repository.GetMemberAsync(username);
            
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto member)
        {
            var username= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user=await _repository.GetUserByUsernameAsync(username);

            _mapper.Map(member,user);

            _repository.Update(user);

            if(await _repository.SaveAllAsync())
            return NoContent();
            else
            return BadRequest("Failed to Update UserData");
        }
    }
}