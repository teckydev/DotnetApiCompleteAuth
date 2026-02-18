using System;
using Microsoft.AspNetCore.Identity;

namespace DotnetApiComplet.Models;

public class User : IdentityUser
{
public string Name { get; set; }
}
