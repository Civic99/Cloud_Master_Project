using Common.DTO;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mappers
{
    public static class UserAuthMapper
    {
        public static UserAuthDto ToDto(User user)
        {
            var dto = new UserAuthDto();
            dto.Username = user.Username;
            dto.Password = user.Password;

            return dto;
        }

        public static User FromDto(UserAuthDto dto)
        {
            var user = new User();
            user.Username = dto.Username;
            user.Password = dto.Password;
            user.Id = Guid.NewGuid();

            return user;
        }
    }
}
