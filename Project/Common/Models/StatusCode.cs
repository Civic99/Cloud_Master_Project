namespace Common.Models
{
    public enum StatusCode
    {
        Success = 200,
        BadRequest = 400,
        Forbidden = 403,
        NotFound = 404,
        Unoathorized = 401,
        Conflict = 409,
        InternalServerError = 500
    }
}
