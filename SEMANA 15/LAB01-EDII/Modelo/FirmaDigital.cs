using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace LAB01_EDII.Modelo
{

    public class FirmaDigital
    {
        private RSAKeyGenerator keyGenerator;
        
        public FirmaDigital(BigInteger p, BigInteger q)
        {
            keyGenerator = new RSAKeyGenerator(p, q);
            keyGenerator.GenerateKeys();
        }

        public PublicKey GetPublicKey()
        {
            return keyGenerator.GetPublicKey();
        }

        public PrivateKey GetPrivateKey()
        {
            return keyGenerator.GetPrivateKey();
        }

        public byte[] GenerarFirma(string message)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));

            // Firma el hash con la clave privada usando el algoritmo RSA
            BigInteger hashInt = new BigInteger(hash);
            BigInteger signature = BigInteger.ModPow(hashInt, keyGenerator.J, keyGenerator.N);//llave privada
            return signature.ToByteArray();
        }

        public bool ValidarFirma(string message, byte[] signature)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));

            BigInteger hashInt = new BigInteger(hash);
            BigInteger signatureInt = new BigInteger(signature);

            BigInteger hashFromSignature = BigInteger.ModPow(signatureInt, keyGenerator.K, keyGenerator.N);//llave publica

            return hashInt == hashFromSignature;
        }
    }


    public class RSAKeyGenerator
    {
        public BigInteger P { get; private set; }
        public BigInteger Q { get; private set; }
        public BigInteger N { get; private set; }
        public BigInteger Z { get; private set; }
        public BigInteger K { get; private set; }
        public BigInteger J { get; private set; }

        public const int KEY_SIZE = 256;

        public RSAKeyGenerator(BigInteger p, BigInteger q)
        {
            P = p;
            Q = q;
            N = p * q;
            Z = (p - 1) * (q - 1);
        }

        public void GenerateKeys()
        {
            K = obtenerCoprimo(Z); //exponente publico e
            J = ModInverse(K, Z); // J es el inverso multiplicativo de K mod Z.  j exponente privado d
        }

        private static BigInteger GenerateRandomBigInteger(int bits)
        {
            int bytes = (bits + 7) / 8; // Convertir bits en bytes
            byte[] data = new byte[bytes];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }
            // Asegurarse de que el número generado es positivo y de que su bit más significativo está establecido
            data[bytes - 1] &= (byte)(0x7F);
            return new BigInteger(data);
        }

        private BigInteger calcularMCD(BigInteger a, BigInteger b)
        {
            if (b == BigInteger.Zero)
            {
                return a;
            }
            else
            {
                return calcularMCD(b, a % b);
            }
        }

        private BigInteger obtenerCoprimo(BigInteger z)
        {
            BigInteger e;
            do
            {
                e = GenerateRandomBigInteger(KEY_SIZE);
                // Asegurarse de que 'e' es menor que 'z'
                while (e >= z || e < 0)
                {
                    e = GenerateRandomBigInteger(KEY_SIZE);
                }
            } while (calcularMCD(e, z) != BigInteger.One);
            return e;
        }

        private BigInteger ModInverse(BigInteger a, BigInteger m) //algoritmo extendido de euclides para encontrar el maximo comun divisor de dos numeros para encontrar el inverso multiplicativo durante la generacion de la clave privada
        {
            BigInteger m0 = m;
            BigInteger y = 0, x = 1;

            if (m == 1)
                return 0;

            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;

                m = a % m;
                a = t;
                t = y;

                y = x - q * y;
                x = t;
            }

            if (x < 0)
                x += m0;

            return x;
        }

        public PublicKey GetPublicKey()
        {
            return new PublicKey(N.ToByteArray(), K.ToByteArray());
        }

        public PrivateKey GetPrivateKey()
        {
            return new PrivateKey(N.ToByteArray(), J.ToByteArray());
        }
    }

    public class PrivateKey//almacenan los componentes relevantes de las claves 
    {
        public BigInteger N { get; }
        public BigInteger J { get; }

        public PrivateKey(byte[] n, byte[] j)
        {
            N = new BigInteger(n);
            J = new BigInteger(j);
        }
    }

    public class PublicKey//almacenan los componentes relevantes de las claves 
    {
        public BigInteger N { get; }
        public BigInteger K { get; }

        public PublicKey(byte[] n, byte[] k)
        {
            N = new BigInteger(n);
            K = new BigInteger(k);
        }
    }


}

