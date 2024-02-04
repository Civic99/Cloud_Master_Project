namespace Common.Models
{
    public enum StatusCode
    {
        Success = 200,
        BadRequest = 400,
        NotFound = 404,
        Forbidden = 403,
        Unoathorized = 401,
        Conflict = 409,
        InternalServerError = 500
    }
}
