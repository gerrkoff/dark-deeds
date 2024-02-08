﻿using DD.ServiceAuth.Domain.Enums;

namespace DD.ServiceAuth.Domain.Dto;

public class SignUpResultDto
{
    public string Token { get; init; } = string.Empty;
    public SignUpResultEnum Result { get; init; }
}
