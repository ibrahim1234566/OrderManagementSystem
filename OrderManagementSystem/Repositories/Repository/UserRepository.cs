﻿using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.Repository
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(OrderManagementDbContext context) : base(context) { }
        
    }
}
