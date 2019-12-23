﻿using DatingApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
        [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;

        public UsersController(IDatingRepository repo)
        {
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers() 
        {
            var users = await _repo.GetUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            return Ok(user);
        }

    }
}
