using System;
using System.ComponentModel.DataAnnotations;
namespace Play.Identity.Service.Dtos;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    Decimal Gil,
    DateTimeOffset CreatedDate
);