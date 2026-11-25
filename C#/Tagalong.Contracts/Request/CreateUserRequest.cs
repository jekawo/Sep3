using System;
using System.Collections.Generic;
using System.Text;

namespace Tagalong.Contracts.Request;

public class CreateUserRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}

public class UpdateUserRequest : CreateUserRequest;
