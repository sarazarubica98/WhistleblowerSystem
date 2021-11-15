﻿using System.Collections.Generic;
using WhistleblowerSystem.Business.DTOs.Config;
using WhistleblowerSystem.Shared.Enums;


namespace WhistleblowerSystem.Business.DTOs
{
    public class FormFieldDto
    {
        public FormFieldDto(string? id, List<LanguageEntryDto> texts, ControlType type, string selectedValues, List<SelectionValueDto>? selectionValues)
        {
            Id = id;
            Texts = texts;
            Type = type;
            SelectedValues = selectedValues;
            SelectionValues = selectionValues;
        }

        public string? Id { get; set; }
        public List<LanguageEntryDto> Texts { get; set; }
        public ControlType Type { get; set; }
        public string SelectedValues { get; set; } // values which the user selected TODO: should be a list
        public List<SelectionValueDto>? SelectionValues { get; set; } // all values which can be selected
    }
}
