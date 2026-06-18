namespace MixAndMatch.Application.Common;

public enum SuccessType
{
    Ok,        // 200 - éxito que devuelve datos (GET, update con retorno)
    Created,   // 201 - se creó un recurso nuevo
    NoContent  // 204 - éxito sin cuerpo (OJO: descarta el envelope ApiResponse)
}
