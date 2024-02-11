﻿using MagicVilla_VillaAPI.Model;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IVillaRepository: IRepository<Villa>
    {
        Task<Villa> UpdateAsync(Villa entity);
    }
}
