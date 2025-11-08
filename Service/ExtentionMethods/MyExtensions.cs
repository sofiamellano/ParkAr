using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.ExtentionMethods
{
    public static class MyExtensions
    {
        public static void TryAttach<TContext, TEntity>(this TContext context, TEntity entity)
            where TContext : DbContext
            where TEntity : class?
        {
            if (entity == null)
            {
                return;
            }
            if (!context.Set<TEntity>().Local.Any(e => e == entity))
            {
                context.Set<TEntity>().Attach(entity);
                return;
            }
            var entityAttached = context.Set<TEntity>().Local.FirstOrDefault(e => e == entity);
            entity = entityAttached;
            return;
        }

        public static string GetHashSha256(this string textoAEncriptar)
        {
            // Create a SHA256   
            using SHA256 sha256Hash = SHA256.Create();
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(textoAEncriptar));
            // Convert byte array to a string   
            StringBuilder hashObtenido = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hashObtenido.Append(bytes[i].ToString("x2"));
            }
            return hashObtenido.ToString();
        }
    }
}
