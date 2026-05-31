namespace MixAndMatch.Domain.ValueObjects;

public sealed class Calificacion
{
    public int Valor { get; private set; }

    private Calificacion()
    {
    }

    public Calificacion(int valor)
    {
        if (valor < 1 || valor > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(valor), "Calificacion debe estar entre 1 y 5.");
        }

        Valor = valor;
    }

    public static Calificacion From(int valor) => new(valor);
}
