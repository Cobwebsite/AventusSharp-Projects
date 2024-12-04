using AventusSharp.Data;

namespace ${{projectName}}.Data;

public class User : Storable<User>
{
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
}
