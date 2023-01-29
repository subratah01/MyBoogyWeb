using System;
using System.Collections.Generic;

namespace OA.Data;

public partial class Loginuser
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string DisplayName { get; set; }

    public short RoleId { get; set; }

    public sbyte IsProvider { get; set; }

    public sbyte IsActive { get; set; }

    public sbyte IsDeleted { get; set; }

    public virtual Userrole Role { get; set; }
}
