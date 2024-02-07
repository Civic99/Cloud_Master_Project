using Common.Entities;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mappers
{
    public static class UserEntityMapper
    {
        public static UserEntity ToEntity(User user)
        {
            var entity = new UserEntity();
            entity.PartitionKey = "Users";
            entity.RowKey = user.Id.ToString();
            entity.Id = user.Id;
            entity.Username = user.Username;
            entity.Password = user.Password;
            if(user.Orders != null)
            {
                entity.Orders = string.Join("|", user.Orders);
            }


            return entity;
        }

        public static User FromEntity(UserEntity entity)
        {
            var user = new User();
            user.Id = entity.Id;
            user.Username = entity.Username;
            user.Password = entity.Password;
            if(entity.Orders != null)
            {
                user.Orders = entity.Orders.Split('|').Select(Guid.Parse).ToList();
            }

            return user;
        }
    }
}
