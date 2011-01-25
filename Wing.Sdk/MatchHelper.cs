using System;

namespace Wing
{
    public static class MatchHelper
    {
        public static bool IsValidCPF(String cpf)
        {
            int[] factor1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] factor2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digit;
            int sum;
            int rest;

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            tempCpf = cpf.Substring(0, 9);
            sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * factor1[i];

            rest = sum % 11;
            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            digit = rest.ToString();

            tempCpf = tempCpf + digit;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * factor2[i];

            rest = sum % 11;
            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            digit = digit + rest.ToString();

            return cpf.EndsWith(digit);
        }

        public static bool IsValidCNPJ(string cnpj)
        {
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");

            int[] digits = new int[14];
            int[] sum = new int[2];
            int[] result = new int[2];
            bool[] CNPJOk = new bool[2];
            string ftmt = "6543298765432";
            try
            {
                for (var currDig = 0; currDig < 14; currDig++)
                {
                    digits[currDig] = int.Parse(cnpj.Substring(currDig, 1));
                    if (currDig <= 11)
                        sum[0] += (digits[currDig] * int.Parse(ftmt.Substring(currDig + 1, 1)));
                    if (currDig <= 12)
                        sum[1] += (digits[currDig] * int.Parse(ftmt.Substring(currDig, 1)));
                }
                for (var currDig = 0; currDig < 2; currDig++)
                {
                    result[currDig] = (sum[currDig] % 11);
                    if ((result[currDig] == 0) || (result[currDig] == 1))
                        CNPJOk[currDig] = (digits[12 + currDig] == 0);
                    else
                        CNPJOk[currDig] = (digits[12 + currDig] == (11 - result[currDig]));
                }
                return (CNPJOk[0] && CNPJOk[1]);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidCEP(String cep)
        {
            return cep.FilterNumbers().Length == 8;
        }

        public static bool IsValidFone(String fone)
        {
            return fone.FilterNumbers().Length == 10;
        }
    }
}
