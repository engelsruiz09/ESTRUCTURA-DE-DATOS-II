using System;
using System.Numerics;
using System.Security.Cryptography;

public class PrimeGenerator
{
    private const int KEY_SIZE = 256;//establezco el tamaño de la clave en bits
    private RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();//creo una instancia de un generador de numeros aleatorios criptograficamente

    public BigInteger GeneratePrime()//metodo para generar un primo grande
    {
        BigInteger prime;
        do
        {
            prime = GenerateRandomBigInteger(KEY_SIZE);//genero el numero aleatorio grande
            //establece el bit mas significativo y el bit menos significativo a 1 para asegurar que es impar y tiene la longitud correcta.
            prime |= BigInteger.One;
            prime |= BigInteger.One << (KEY_SIZE - 1);

            //continua el ciclo hasta que se encuentre un probable num primo
        } while (!IsProbablePrime(prime, 10)); // 10 iteraciones para un nivel de confianza decente

        return prime;
    }
    //metodo para generar un biginteger aleatorio de un tamaño especifico en bits
    private BigInteger GenerateRandomBigInteger(int bits)
    {
        int bytes = bits / 8;
        //se crea un buffer para los bytes aleatorios, con un byte adicional para asegurar que el numero sea positivo
        byte[] buffer = new byte[bytes + 1]; // +1 para asegurar que el numero generado es positivo
        rng.GetBytes(buffer);
        //establece el byte mas significativo a 0 para asegurar que el numero sea positivo
        buffer[buffer.Length - 1] = 0; //asegura que el numero es positivo
        return new BigInteger(buffer);
    }

    public bool IsProbablePrime(BigInteger n, int k)//metodo para ver si el numero dado es un probablem primo
    {
        if (n < 2) return false;//retornara falso si n es menor que 2 o par, verdadero si n es 2 o 3
        if (n == 2 || n == 3) return true;
        if (n % 2 == 0) return false;

        // Escribe n-1 como 2^s * d con d impar (esto es factible porque n es impar)
        BigInteger d = n - 1;
        int s = 0;
        while (d % 2 == 0)
        {
            d /= 2;
            s += 1;
        }

        // Testea el numero k veces
        for (int i = 0; i < k; i++)
        {
            //selecciona un numero aleatorio a en el rango de 2, n-2
            BigInteger a = RandomInRange(2, n - 2);
            //calcula a^d mod n
            BigInteger x = BigInteger.ModPow(a, d, n);
            if (x == 1 || x == n - 1)
                continue;

            //se repite el cuadrado de x y se verifica si es -1 mod n
            for (int r = 1; r < s; r++)
            {
                x = BigInteger.ModPow(x, 2, n);
                if (x == 1) return false;
                if (x == n - 1) break;
            }
            //si x no es -1 mod n, n no es primo 
            if (x != n - 1) return false;
        }

        //Si no se encontraron testigos, entonces n es probablemente primo
        return true;
    }

    private BigInteger RandomInRange(BigInteger min, BigInteger max)//metodo para obtener un numero aleatorio dentro de un rango
    {
        byte[] bytes = max.ToByteArray();//se crea un array de bytes de la longitud del maximo valor
        BigInteger result;
        do
        {
            rng.GetBytes(bytes);//genera un numero aleatorio
            bytes[bytes.Length - 1] &= 0x7F; //127, ,asegura que el numero es positivo La operacion &= 0x7F; efectivamente enmascara el bit mas significativo del ultimo byte a 0
            result = new BigInteger(bytes);
        } while (result < min || result > max);//si el numero esta dentro del rango requerido

        return result;
    }
}

