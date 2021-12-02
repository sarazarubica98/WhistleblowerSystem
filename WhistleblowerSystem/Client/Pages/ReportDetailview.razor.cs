using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using WhistleblowerSystem.Client.Services;
using WhistleblowerSystem.Shared.DTOs;
using WhistleblowerSystem.Shared.Enums;
using WhistleblowerSystem.Shared.Models;

namespace WhistleblowerSystem.Client.Pages

{
    public partial class ReportDetailview : IDisposable
    {
        private FormModel? _form;
        private FormMessageDto _formMessageDto;
        private bool _isCompany;
        private ViolationState _enumValue { get; set; }


        [Inject] private ICurrentAccountService CurrentAccountService { get; set; } = null!;
        [Inject] private IFormService FormService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        protected override void OnInitialized()
        {
            _formMessageDto = new FormMessageDto(null, "", CurrentAccountService.GetCurrentUser()!, DateTime.Now);
            _form = FormService.GetCurrentFormModel()!;
            _enumValue = _form.State;
            if (CurrentAccountService.GetCurrentUser() != null)
            {
                _isCompany = true;
            }
            else
            {
                _isCompany = false;
            }
        }

        private async Task SaveState()
        {
            if (_form != null) {
                _form.State = _enumValue;
                await FormService.UpdateState(_form.Id!, _enumValue);
            }
            _form!.State = _enumValue;
            await FormService.UpdateState(_form.Id!, _enumValue);
        }

        private void NavigateBack()
        {
            FormService.SetCurrentFormModel(null);
            NavigationManager.NavigateTo("/reportsList");
        }

        private void Close()
        {
            FormService.SetCurrentFormModel(null);
            NavigationManager.NavigateTo("");
        }

        private async Task SendMessage()
        {
            _form!.Messages!.Add(_formMessageDto);
            _formMessageDto = new FormMessageDto(null, "", CurrentAccountService.GetCurrentUser()!, DateTime.Now);
            StateHasChanged();
            //await FormService.AddMessage(_form.Id, _formMessageDto);
        }

        private string GetMessageStyle(FormMessageDto messageDto)
        {
            return (_isCompany && messageDto.User != null) ? "align-text: right;" : "";
        }

        void IDisposable.Dispose()
        {
            FormService.SetCurrentFormModel(null);
        }
    }
}