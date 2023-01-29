using System;
using System.Collections.Generic;

namespace OA.Data;

public partial class Userrole
{
    public short RoleId { get; set; }

    public string RoleName { get; set; }

    public sbyte IsDeleted { get; set; }

    public virtual ICollection<Loginuser> Loginusers { get; } = new List<Loginuser>();
}
