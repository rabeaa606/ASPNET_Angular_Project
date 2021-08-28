using System.Security.Claims;
using System;
namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user) /// geting an username by user  :depends on token 
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static int GetUserId(this ClaimsPrincipal user)/// geting an id by user  :depends on token 
        {
            Console.WriteLine(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}