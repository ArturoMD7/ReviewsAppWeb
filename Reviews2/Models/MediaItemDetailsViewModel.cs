using System.Collections.Generic;

namespace Reviews2.Models
{
    public class MediaItemDetailsViewModel
    {
        public MediaItem MediaItem { get; set; }
        public Dictionary<string, UserInfoViewModel> UsuariosInfo { get; set; }
    }

    public class UserInfoViewModel
    {
        public string UserName { get; set; }
        public string NombreCompleto { get; set; }
        public string AvatarUrl { get; set; }
    }
}
