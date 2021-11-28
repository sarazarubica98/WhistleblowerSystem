﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhistleblowerSystem.Shared.DTOs;
using WhistleblowerSystem.Business.Services;
using WhistleblowerSystem.Server.Authentication;
using WhistleblowerSystem.Server.CustomAttributes;
using WhistleblowerSystem.Shared.Enums;

namespace WhistleblowerSystem.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FormController : UserBaseController
    {
        private readonly FormService _formService;
        public FormController(UserManager userManager,
            IHttpContextAccessor httpContextAccessor,
            FormService formService) : base(httpContextAccessor, userManager)
        {
            _formService = formService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<FormDto> Get()
        {
            return await _formService.CreateFormFromTemplateAsync();
        }

        [Authorize]
        [CheckRights(Roles.WhistleBlowerRole, Roles.CompanyUserRole)]
        [HttpGet("{id}")]
        public async Task<FormDto> GetById(string id)
        {
            CheckRight(id);
            return await _formService.GetAsync(id);
        }

        [AllowAnonymous]
        [HttpPost("save")]
        public async Task<FormDto> Save(FormDto formDto)
        {
            var form = await _formService.CreateFormAsync(formDto);
            if (form == null) throw new Exception("Save form failed");
            return form;
        }

        [CheckRights(Roles.CompanyUserRole)]
        [HttpGet("getAll")]
        public async Task<List<FormDto>> GetAll()
        {
            return await _formService.GetAllAsync();
        }

        [HttpPost("addMessage")]
        public async Task AddMessage(UserDto user, string message)
        {
            var timeStamp = DateTime.Now;
            FormMessageDto formMessageDto = new FormMessageDto(null, message, user, timeStamp);
            await _formService.AddMessage(user.Id!, formMessageDto);
        }

        [HttpPost("changeState")]
        public async Task ChangeState(UserDto user, ViolationState state)
        {
            if (user != null) {
                await _formService.ChangeState(user.Id!, state);
            }
        }
    }
}
