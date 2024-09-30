using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace MovieAPI.Models;

public class User : IdentityUser
{
    [JsonIgnore]
    public virtual ICollection<Genre> Genres { get; set; }
}
