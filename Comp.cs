using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Compile
{
    class Compiler
    {
        public  Tokens[] tokensArray = new Tokens[100];
        public  Tokens Token;
        public  int tokenCount = 0;
        public  Variables[] varTable = new Variables[100];
        public  int varCount = 0;
        public  string codigochilo = "";
        public  int codigoByteCount = 0;
        public  int LCount = 0;
        public  string JumpHelper = "";
        public  int directionHelper = 0;
        public  string lastDirection = "";
        public  string StringLength;
        public  int sc = 0;
        public  string directionSize = "0000";
        public  int[] JumpList = new int[50];
        public  string tipo = "";
        public  bool acceptNegative = true;
        public  int negativeCount = 0;
        public  bool wasNumber = false;
        public string rice = "";
        public string eLog = "";
        public int finalcount = 0;

        public  void otherMain(string codigo)
        {
            //string line = "{int a \n for(a=4 ; a<6 ; a=a+1){print(a)}  printl int b for(b=1; b<a;b=b+1){print(b)}}";
            Console.WriteLine("Evaluate: " + codigo);
            evaluate(codigo);
            Programa();
            CreateFile();
            RunVM();
            Console.ReadLine();
        }
        public  byte[] ConvertDoubleToByteArray(double d)
        {
            return BitConverter.GetBytes(d);
        }
        private  void CreateFile()
        {
            File.WriteAllBytes("vmcode.Chop", StringToByteArray(codigochilo));
        }
        private  void RunVM()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"Funcionaplox.exe";
            startInfo.Arguments = @"vmcode.Chop";
            Process.Start(startInfo);
        }
        private  byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
        }
        public  void evaluate(string expression)
        {
            //variables
            int count = 0;
            string word = "";
            bool breakFlag = false;
            char[] charArray = expression.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                wasNumber = false;
                Console.WriteLine("What im reading: " + charArray[i]);
                if (charArray[i] == ' ' || charArray[i] == '\n' || charArray[i] == '\r')
                    continue;
                if (Char.IsLetter(charArray[i]))
                {
                    //Words
                    tokensArray[count] = new Tokens(10, "");
                    while (i < charArray.Length && breakFlag == false)
                    {
                        if (charArray[i] == ' ' || charArray[i] == ';' || (Char.IsNumber(charArray[i]) == false && Char.IsLetter(charArray[i]) == false))
                        {
                            breakFlag = true;
                            i--;
                        }
                        else
                        {
                            word += charArray[i];
                        }
                        i++;
                    }
                    i--;
                    breakFlag = false;
                    tokensArray[count].index = checkWord(word);
                    if (tokensArray[count].index == 50)
                        tokensArray[count].valor = word;
                    count++;
                    //Console.WriteLine("Word:" + word);
                    word = "";
                    wasNumber = true; //wasn't really a number but a variable can represent a number
                }
                //Numbers
                
                else if (Char.IsNumber(charArray[i]))
                {
                    tokensArray[count] = new Tokens(37, "");
                    while (i < charArray.Length && breakFlag == false)
                    {
                        //if (charArray[i] == ' ' || charArray[i] == ';')
                        if (charArray[i] == ' ' || charArray[i] == ';' || (Char.IsNumber(charArray[i]) == false && charArray[i] != '.'))
                        {
                            breakFlag = true;
                            i--;
                        }
                        else
                        {
                            word += charArray[i];
                        }
                        i++;
                    }
                    i--;
                    breakFlag = false;
                    tokensArray[count].index = checkNumber(word);
                    //tokensArray[count].index = 37;
                    tokensArray[count].valor = word;
                    count++;
                    //Console.WriteLine("Number:" + word);
                    word = "";
                    wasNumber = true;
                    negativeCount = 0;
                }

                //Operadores
                else if (charArray[i] == '+')
                {
                    tokensArray[count] = new Tokens(13, "");
                    count++;
                }
                
                else if(charArray[i] == '-' && i < charArray.Length-1 && acceptNegative)
                {
                    if (char.IsNumber(charArray[i + 1]))
                    {
                        tokensArray[count] = new Tokens(37, "");
                        word += "-";
                        i++;
                        while (i < charArray.Length && breakFlag == false)
                        {
                            //if (charArray[i] == ' ' || charArray[i] == ';')
                            if (charArray[i] == ' ' || charArray[i] == ';' || (Char.IsNumber(charArray[i]) == false && charArray[i] != '.'))
                            {
                                breakFlag = true;
                                i--;
                            }
                            else
                            {
                                word += charArray[i];
                            }
                            i++;
                        }
                        i--;
                        breakFlag = false;
                        tokensArray[count].index = checkNumber(word);
                        //tokensArray[count].index = 37;
                        tokensArray[count].valor = word;
                        count++;
                        //Console.WriteLine("Number:" + word);
                        word = "";
                        wasNumber = true;
                        negativeCount = 0;
                    }
                    else
                    {
                        tokensArray[count] = new Tokens(14, "");
                        count++;
                    }
                }

                else if (charArray[i] == '-')
                {
                    tokensArray[count] = new Tokens(14, "");
                    count++;
                    negativeCount++;
                    if (negativeCount == 1)
                        acceptNegative = true;
                    else
                        acceptNegative = false;

                }

                else if (charArray[i] == '*')
                {
                    tokensArray[count] = new Tokens(15, "");
                    count++;
                }

                else if (charArray[i] == '/')
                {
                    tokensArray[count] = new Tokens(16, "");
                    count++;
                }

                //Simbolos
                else if (charArray[i] == '(')
                {
                    tokensArray[count] = new Tokens(11, "");
                    count++;
                }
                else if (charArray[i] == ')')
                {
                    tokensArray[count] = new Tokens(12, "");
                    count++;
                }
                else if (charArray[i] == '{')
                {
                    tokensArray[count] = new Tokens(23, "");
                    count++;
                }
                else if (charArray[i] == '}')
                {
                    tokensArray[count] = new Tokens(24, "");
                    count++;
                }
                else if (charArray[i] == '[')
                {
                    tokensArray[count] = new Tokens(25, "");
                    count++;
                }
                else if (charArray[i] == ']')
                {
                    tokensArray[count] = new Tokens(26, "");
                    count++;
                }
                else if (charArray[i] == ';')
                {
                    tokensArray[count] = new Tokens(28, "");
                    count++;
                }
                else if (charArray[i] == ',')
                {
                    tokensArray[count] = new Tokens(30, "");
                    count++;
                }
                else if (charArray[i] == '"') //guardar un string
                {
                    i++;
                    bool found = false;
                    tokensArray[count] = new Tokens(39, "");
                    while (found == false && i < charArray.Length)
                    {
                        if (charArray[i] == '"')
                        {
                            found = true;
                            tokensArray[count].valor = word;

                            count++;
                            //i++;
                        }
                        else
                        {
                            word += charArray[i] + "";
                            i++;
                        }
                    }
                    word = "";

                }
                else if (charArray[i] == '\'') //guardar un char
                {
                    if (i + 2 < charArray.Length)
                    {
                        if (charArray[i + 2] == '\'')
                        {
                            i++;
                            tokensArray[count] = new Tokens(40, "");
                            tokensArray[count].valor = charArray[i] + "";
                            i = i + 1;
                            count++;
                        }
                        else
                        {
                            Console.WriteLine(eLog+="Error. Char not properly written.");
                            eLog += "\r\n";
                            i++;
                        }
                    }
                    //tokensArray[count] = new Tokens(34, "");

                }
                else if (charArray[i] == '.')
                {
                    tokensArray[count] = new Tokens(36, "");
                    count++;
                }
                else if (charArray[i] == '&')
                {
                    if (i + 1 <= charArray.Length)
                    {

                        if (charArray[i + 1] == '&')
                        {
                            tokensArray[count] = new Tokens(31, "");
                            count++;
                            i++;
                        }
                        else
                        {
                            tokensArray[count] = new Tokens(51, "");
                            count++;
                        }

                    }
                    else
                    {
                        tokensArray[count] = new Tokens(51, "");
                        count++;
                    }
                }
                else if (charArray[i] == '|')
                {
                    if (i + 1 <= charArray.Length)
                    {

                        if (charArray[i + 1] == '|')
                        {
                            tokensArray[count] = new Tokens(32, "");
                            count++;
                            i++;
                        }
                        else
                        {
                            tokensArray[count] = new Tokens(51, "");
                            count++;
                        }
                    }
                    else
                    {
                        tokensArray[count] = new Tokens(51, "");
                        count++;
                    }
                }
                //Comparadores
                else if (charArray[i] == '<')
                {
                    if (charArray[i + 1] == '=')
                    {
                        tokensArray[count] = new Tokens(18, "");
                        count++;
                        i++;
                    }
                    else
                    {
                        tokensArray[count] = new Tokens(17, "");
                        count++;
                    }
                }

                else if (charArray[i] == '>')
                {
                    if (charArray[i + 1] == '=')
                    {
                        tokensArray[count] = new Tokens(20, "");
                        i++;
                    }
                    else
                        tokensArray[count] = new Tokens(19, "");
                    count++;
                }
                else if (charArray[i] == '!')
                {
                    if (i + 1 <= charArray.Length)
                    {

                        if (charArray[i + 1] == '=')
                        {
                            tokensArray[count] = new Tokens(22, "");
                            i++;
                        }
                        else
                            tokensArray[count] = new Tokens(51, "");
                    }
                    else
                        tokensArray[count] = new Tokens(51, "");
                    count++;
                }
                else if (charArray[i] == '=')
                {
                    //Console.WriteLine(" i + 1 <= charArray.Length " + (i + 1 <= charArray.Length));
                    if (i + 1 <= charArray.Length)
                    {
                        //Console.WriteLine("charArray[i + 1]" + (charArray[i + 1] == '='));
                        if (charArray[i + 1] == '=')
                        {
                            tokensArray[count] = new Tokens(21, "");
                            i++;
                        }
                        else
                            tokensArray[count] = new Tokens(27, "");
                    }
                    else
                        tokensArray[count] = new Tokens(27, "");
                    count++;
                }
                else
                {
                    tokensArray[count] = new Tokens(51, "");
                    count++;
                }
                
                if (wasNumber == true)
                    acceptNegative = false;

            }
            finalcount = count;
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(tokensArray[i].index + ", " + tokensArray[i].valor);
            }

        }
        public  int checkWord(string word)
        {
            bool errorFlag = false;
            int count = 0;
            foreach (char c in word)
            {
                if (count == 0)
                {
                    if (Char.IsLetter(c) == false || Char.IsNumber(c))
                    {
                        errorFlag = true;
                        Console.WriteLine("Aqui " + c);
                        Console.WriteLine("1: " + Char.IsLetter(c) == false + " 2: " + Char.IsNumber(c));
                    }
                    count++;
                    
                }
                else
                {
                    if (Char.IsLetter(c) == false && Char.IsNumber(c) == false)
                    {
                        errorFlag = true;
                        Console.WriteLine("Aqui " + c);
                        Console.WriteLine("1: " + Char.IsLetter(c) == false + " 2: " + Char.IsNumber(c)==false);
                    }
                    
                }
            }
            if (errorFlag == true)
            {
                Console.WriteLine("yep");
                return 51;
            }
            else if (word == "char")
                return 0;
            else if (word == "string")
                return 1;
            else if (word == "int")
                return 2;
            else if (word == "float")
                return 3;
            else if (word == "double")
                return 4;
            else if (word == "if")
                return 5;
            else if (word == "else")
                return 6;
            else if (word == "for")
                return 7;
            else if (word == "while")
                return 8;
            else if (word == "print")
                return 9;
            else if (word == "read")
                return 10;
            else if (word == "printl")
                return 41;
            else
                return 50;
        }
        public  int checkNumber(string word)
        {
            int count = 0;
            if (word[0] == '-')
                word = word.Substring(1);
            for (int i = 0; i < word.Length; i++)
                if (word[i] == '.')
                    count++;
            if (IsDigitsOnly(word))
                return 37;
            else if (count == 1)
                return 42;
            else
                return 51;
        }
        public  bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        public  void Programa()
        {

            //codigochilo += "\n"; //testing purposes, must die eventually
            codigoByteCount = 15;
            Token = tokensArray[tokenCount];
            Bloque();
            Console.WriteLine(rice +="HALT");
            rice += "\r\n";
            sc++;
            codigochilo += "00";

            Console.WriteLine("Last Direction: " + directionHelper + "hex: " + directionHelper.ToString("X4"));
            Console.WriteLine("SC: " + sc + "hex: " + sc.ToString("X4"));
            codigochilo = "2843294348554E4B554E" + directionSize + "" + sc.ToString("X4") + "" + codigochilo;

            for (int i = 0; i <= LCount; i++)
            {
                codigochilo = codigochilo.Replace("replace" + i + "replace", JumpList[i].ToString("X4"));
            }


            Console.WriteLine(codigochilo);
        }
        public  void Bloque()
        {
            Match("OB");
            while (isStatement())
            {
                Statement();
            }
            Match("CB");
        }
        public  void Statement()
        {
            if (isDeclaration())
            {
                DoDeclaration();
            }
            else if (Token.index == 50) // if token is nombre
            {
                DoAssignment();
            }
            else if (isInstruction()) // if token is print
            {
                DoInstruction();
            }
        }

        public  void DoDeclaration()
        {
            switch (Token.index)
            {
                case 0: //char
                    //char
                    nextToken();

                    if (Token.index == 50)
                    {
                        varTable[varCount] = new Variables();
                        Console.WriteLine(rice += "DEFC " + Token.valor);
                        rice += "\r\n";
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "char";
                        varTable[varCount].isArray = false;
                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        varTable[varCount] = new Variables();
                        Match("[");
                        if (Token.index != 37)
                        {
                            Console.WriteLine(eLog+="Error. Number expected in [ ].");
                            eLog += "\r\n";
                        }
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                        {
                            Console.WriteLine(eLog+="Error. Variable expected.");
                            eLog += "\r\n";
                        }
                        Console.WriteLine(rice += "DEFAC " + Token.valor + ", " + number);
                        rice += "\r\n";
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "charArray";
                        varTable[varCount].isArray = true;
                        varTable[varCount].size = Int32.Parse(number);
                        varCount++;
                    }
                    break;
                case 1://string
                    nextToken();
                    if (Token.index == 50)
                    {
                        varTable[varCount] = new Variables();
                        Console.Write(rice += "DEFS " + Token.valor + ",");
                        rice += "\r\n";
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "string";
                        varTable[varCount].isArray = false;
                        nextToken();
                        Match(",");
                        if (Token.index == 37)
                        {
                            varTable[varCount].length = Token.valor;
                        }
                        else
                        {
                            Console.WriteLine(eLog += "Error. Numero esperado.");
                            eLog += "\r\n";
                        }
                        Console.WriteLine(Token.valor);

                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        varTable[varCount] = new Variables();
                        nextToken();
                        if (Token.index != 37)
                        {
                            Console.WriteLine(eLog += "Error. Number expected in [ ].");
                            eLog += "\r\n";
                        }
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                        {
                            Console.WriteLine(eLog+="Error. Variable expected.");
                            eLog += "\r\n";
                        }
                        Console.Write(rice += "DEFAS " + Token.valor + ", " + number + ", " );
                        rice += "\r\n";
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "stringArray";
                        varTable[varCount].isArray = true;
                        varTable[varCount].size = Int32.Parse(number);

                        nextToken();
                        Match(",");
                        if (Token.index == 37)
                        {
                            varTable[varCount].length = Int32.Parse(Token.valor) +1 +"" ;
                        }
                        else
                        {
                            Console.WriteLine(eLog+="Error. Numero esperado.");
                            eLog += "\r\n";
                        }
                        Console.WriteLine(Token.valor);
                        varCount++;
                    }
                    break;
                case 2: //int
                    varTable[varCount] = new Variables();
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine(rice = "DEFI " + Token.valor);
                        rice += "\r\n";
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "int";
                        varTable[varCount].isArray = false;
                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        nextToken();
                        if (Token.index != 37)
                        {
                            Console.WriteLine(eLog+="Error. Number expected in [ ].");
                            eLog += "\r\n";
                        }
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                        {
                            Console.WriteLine(eLog += "Error. Variable expected.");
                            eLog += "\r\n";
                        }
                        Console.WriteLine(rice +="DEFAI " + Token.valor + ", " + number);
                        rice += "\r\n";
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "intArray";
                        varTable[varCount].isArray = true;
                        varTable[varCount].size = Int32.Parse(number);
                        varCount++;
                    }
                    break;
                case 3: //float
                    varTable[varCount] = new Variables();
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine(rice+="DEFF " + Token.valor);
                        rice += "\r\n";
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "float";
                        varTable[varCount].isArray = false;
                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        varTable[varCount] = new Variables();
                        nextToken();
                        if (Token.index != 37)
                        {
                            Console.WriteLine(eLog += "Error. Number expected in [ ].");
                            eLog += "\r\n";
                        }
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                        {
                            Console.WriteLine(eLog+="Error. Variable expected.");
                            eLog += "\r\n";
                        }
                        Console.WriteLine(rice+= "DEFAF " + Token.valor + ", " + number);
                        rice += "\r\n";
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "floatArray";
                        varTable[varCount].isArray = true;
                        varTable[varCount].size = Int32.Parse(number);
                        varCount++;
                    }
                    break;
                case 4: //double
                    varTable[varCount] = new Variables();
                    nextToken();
                    if (Token.index == 50)
                    {
                        Console.WriteLine(rice+= "DEFD " + Token.valor);
                        rice += "\r\n";
                        sc++;
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "double";
                        varTable[varCount].isArray = false;
                        varCount++;
                    }
                    else if (Token.index == 25)
                    {
                        varTable[varCount] = new Variables();
                        nextToken();
                        if (Token.index != 37)
                        {
                            Console.WriteLine(eLog+="Error. Number expected in [ ].");
                            eLog += "\r\n";
                        }
                        string number = Token.valor;
                        nextToken();
                        Match("]");
                        if (Token.index != 50)
                        {
                            Console.WriteLine(eLog+="Error. Variable expected.");
                            eLog += "\r\n";
                        }
                        Console.WriteLine(rice+= "DEFAD " + Token.valor + ", " + number);
                        rice += "\r\n";
                        varTable[varCount].name = Token.valor;
                        varTable[varCount].type = "doubleArray";
                        varTable[varCount].isArray = true;
                        varTable[varCount].size = Int32.Parse(number);
                        varCount++;
                    }
                    break;

                default:
                    //wat
                    Console.WriteLine(eLog+="Error. Tipo no reconocido.");
                    eLog += "\r\n";
                    break;
            }
            nextToken();
        }
        public  void DoAssignment()
        {
            if (Token.index != 50)
            {
                Console.WriteLine(eLog+="Error. No es una variable.");
                eLog += "\r\n";
            }
            string nombre = Token.valor;
            tipo = CheckVarTable(Token. valor, "type");
            nextToken();
            if (Token.index == 27) //igual
            {
                nextToken();
                if ((Token.index == 37 || Token.index == 50 || Token.index == 11) && tipo == "int")
                {
                    BoolExpretion();
                    Console.WriteLine(rice +="POPI " + nombre);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "1C" + lastDirection;
                    //codigochilo += "\n"; //testing purposes, must die eventually
                    //nextToken();
                }

                else if ((Token.index == 37 || Token.index == 42 || Token.index == 50 || Token.index == 11) && tipo == "float")
                {
                    BoolExpretion();
                    Console.WriteLine(rice +="POPF " + nombre);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "1D" + lastDirection;
                }
                else if ((Token.index == 37 || Token.index == 42 || Token.index == 50 || Token.index == 11) && tipo == "double")
                {
                    BoolExpretion();
                    Console.WriteLine(rice+="POPD " + nombre);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "1E" + lastDirection;
                }
                else if ((Token.index == 40 || Token.index == 50) && tipo == "char") //char
                {
                    BoolExpretion();
                    Console.WriteLine(rice+="POPC " + nombre);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "1B" + lastDirection;
                }

                else if ((Token.index == 39 || Token.index == 50 ) && tipo=="string") //string
                {
                    StringLength = CheckVarTable(nombre, "length");
                    BoolExpretion();
                    Console.WriteLine(rice+="POPS " + nombre);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "1F" + lastDirection;
                    // codigochilo += "\n"; //string testing purposes, must die eventually
                    
                }
                
            }
            else if(Token.index == 25)//array
            {
                if (tipo == "intArray")
                {
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "intArray";
                    Console.WriteLine(rice+="POPY");
                    rice += "\r\n";
                    codigochilo += "43";
                    sc++;
                    Match("]");
                    Match("Eq"); //=
                    BoolExpretion();
                    Console.WriteLine(rice+="MOVY");
                    rice += "\r\n";
                    codigochilo += "42";
                    sc++;
                    Console.WriteLine(rice+="POPAI " + nombre);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "22" + lastDirection;
                    //codigochilo += "\n"; //testing purposes, must die eventually
                }
                else if (tipo == "floatArray")
                {
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "floatArray";
                    Console.WriteLine(rice+="POPY");
                    rice += "\r\n";
                    codigochilo += "43";
                    sc++;
                    Match("]");
                    Match("Eq"); //=

                    BoolExpretion();
                    Console.WriteLine(rice+="MOVY");
                    rice += "\r\n";
                    codigochilo += "42";
                    sc++;
                    Console.WriteLine(rice+="POPAF " + nombre);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "23" + lastDirection;
                    //codigochilo += "\n"; //testing purposes, must die eventually
                }
                else if (tipo == "doubleArray")
                {
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "doubleArray";
                    Console.WriteLine(rice += "POPY");
                    rice += "\r\n";
                    codigochilo += "43";
                    sc++;
                    Match("]");
                    Match("Eq"); //=

                    BoolExpretion();
                    Console.WriteLine(rice+="MOVY");
                    rice += "\r\n";
                    codigochilo += "42";
                    sc++;
                    Console.WriteLine(rice+="POPAD " + nombre);
                    rice += "\r\n";
                    sc = sc + 5;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "24" + lastDirection;
                    //codigochilo += "\n"; //testing purposes, must die eventually
                }
                else if (tipo == "charArray")
                {
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "charArray";
                    Console.WriteLine(rice+="POPY");
                    rice += "\r\n";
                    codigochilo += "43";
                    sc++;
                    Match("]");
                    Match("Eq"); //=

                    BoolExpretion();
                    Console.WriteLine(rice+="MOVY");
                    rice += "\r\n";
                    codigochilo += "42";
                    sc++;
                    Console.WriteLine(rice+="POPAC " + nombre);
                    rice += "\r\n";
                    sc = sc + 5;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        
                        assignDirection(nombre);
                    }
                    codigochilo += "21" + lastDirection;

                }
                else if(tipo == "stringArray")
                {
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "stringArray";
                    Console.WriteLine(rice+="PUSHKI " + CheckVarTable(nombre,"length"));
                    rice += "\r\n";
                    sc += 5;
                    codigochilo += "17" + Int32.Parse(CheckVarTable(nombre, "length")).ToString("X4").PadLeft(8, '0'); ;
                    Console.WriteLine(rice+="MULT");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "3D";
                    Console.WriteLine(rice+="POPY");
                    rice += "\r\n";
                    codigochilo += "43";
                    sc++;
                    Match("]");
                    Match("Eq"); //=

                    StringLength = CheckVarTable(nombre, "length");
                    BoolExpretion();
                    Console.WriteLine(rice+="MOVY");
                    rice += "\r\n";
                    codigochilo += "42";
                    sc++;
                    Console.WriteLine(rice+="POPAS " + nombre);
                    rice += "\r\n";
                    sc = sc + 5;
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "25" + lastDirection;

                }
            }
        }
        private  void DoInstruction()
        {
            if (Token.index == 5) //ifp
            {
                nextToken();
                Match("OP");
                BoolExpretion();
                Match("CP");
                Console.WriteLine(rice+=JumpHelper + " L" + LCount); //JUMPNE L0
                rice += "\r\n";
                JumpWriteCode(JumpHelper);
                codigochilo += "replace" + LCount + "replace";
                sc = sc + 3;
                Bloque();
                Console.WriteLine(rice+="L" + LCount + ":");
                rice += "\r\n";
                JumpList[LCount] = sc;
            }
            else if (Token.index == 7) //for
            {
                nextToken();
                Match("OP");
                DoAssignment();
                Match(";");

                Console.WriteLine(rice+="L" + LCount + ":"); //L0
                rice += "\r\n";
                JumpList[LCount] = sc;
                //LCount++;
                BoolExpretion();
                Match(";");

                Console.WriteLine(rice+=JumpHelper + " L" + (LCount + 3)); //Jump L3
                rice += "\r\n";
                JumpWriteCode(JumpHelper);
                codigochilo += "replace" + (LCount + 3) + "replace";
                sc = sc + 3;
                Console.WriteLine(rice+="JMP " + "L" + (LCount +2)); //Jump L2
                rice += "\r\n";
                codigochilo += 30 + "replace" + (LCount + 2) + "replace";
                sc = sc + 3;

                Console.WriteLine(rice+="L" + LCount +1 + ":"); //L1
                rice += "\r\n";
                JumpList[LCount+1] = sc;
                //LCount++;
                //BoolExpretion();
                DoAssignment();
                Console.WriteLine(rice+="JMP " + "L" + (LCount)); //Jump L0
                rice += "\r\n";
                codigochilo += 30 + "replace" + (LCount) + "replace";
                sc = sc + 3;
                Match(")");

                Console.WriteLine(rice+="L" + LCount + 2 + ":"); //L2
                rice += "\r\n";
                JumpList[LCount + 2] = sc;
                Bloque();
                Console.WriteLine(rice+="JMP " + "L" + (LCount +1)); //Jump L1
                rice += "\r\n";
                codigochilo += 30 + "replace" + (LCount +1) + "replace";
                sc = sc + 3;

                Console.WriteLine(rice+="L" + LCount + 3 + ":"); //L1
                rice += "\r\n";
                JumpList[LCount + 3] = sc;
                LCount = LCount + 4;

            }
            else if (Token.index == 8) //while
            {
                nextToken();
                Console.WriteLine(rice+="L" + LCount + ":");
                rice += "\r\n";
                JumpList[LCount] = sc;
                Match("OP");
                BoolExpretion();
                Match("CP");
                Console.WriteLine(rice+=JumpHelper + " L" + (LCount + 1));
                rice += "\r\n";
                JumpWriteCode(JumpHelper);
                codigochilo += "replace" + (LCount + 1) + "replace";
                sc = sc + 3;
                Bloque();
                Console.WriteLine(rice+="JMP L" + LCount);
                rice += "\r\n";
                codigochilo += "30" + "replace" + LCount + "replace";
                sc = sc + 3;
                Console.WriteLine(rice+="L" + (LCount + 1) + ":");
                rice += "\r\n";
                JumpList[LCount + 1] = sc;
                LCount=LCount +2 ;

            }
            else if (Token.index == 9) //print
            {
                nextToken();
                Match("OP");
                PreparePrint();
                Match("CP");
            }
            else if (Token.index == 41) //printl
            {
                Console.WriteLine(rice+="PRTCR");
                rice += "\r\n";
                sc++;
                codigochilo += "01";
                //codigochilo += "\n"; //testing purposes, must die eventually
                nextToken();
            }
            else if(Token.index == 10) //read
            {
                nextToken();
                Match("OP");
                DoRead();
                Match("CP");
            }
        }
        private  void PreparePrint() 
        {
            string nombre;
            switch (CheckVarTable(Token.valor, "type"))
            {
                case "char": // char
                    Console.WriteLine(rice+="PRTC " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    codigochilo += "02" + GetDirection(Token.valor);
                    nextToken();
                    break;
                case "charArray":
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "charArray";
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="PRTAC " + nombre);
                    rice += "\r\n";
                    codigochilo += "07" + GetDirection(nombre);
                    sc = sc + 3;
                    break;
                case "string": //String
                    Console.WriteLine(rice+="PRTS " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    codigochilo += "06" + GetDirection(Token.valor);
                    //codigochilo += "\n"; //testing purposes, must die eventually
                    nextToken();
                    break;
                case "stringArray":
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "stringArray";
                    Console.WriteLine(rice+="PUSHKI " + CheckVarTable(nombre, "length"));
                    rice += "\r\n";
                    sc += 5;
                    codigochilo += "17" + Int32.Parse(CheckVarTable(nombre, "length")).ToString("X4").PadLeft(8, '0'); ;
                    Console.WriteLine(rice+="MULT");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "3D";
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="PRTAS " + nombre);
                    rice += "\r\n";
                    codigochilo += "0B" + GetDirection(nombre);
                    sc = sc + 3;
                    break;
                case "int": //int
                    Console.WriteLine(rice+="PRTI " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    codigochilo += "03" + GetDirection(Token.valor);
                    //codigochilo += "\n"; //testing purposes, must die eventually
                    nextToken();
                    break;
                case "intArray":
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    BoolExpretion();
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="PRTAI " + nombre);
                    rice += "\r\n";
                    codigochilo += "08" + GetDirection(nombre);
                    sc = sc + 3;
                    //nextToken();
                    break;
                case "float": //float
                    Console.WriteLine(rice+="PRTF " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    codigochilo += "04" + GetDirection(Token.valor);
                    nextToken();
                    break;
                case "floatArray":
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "floatArray";
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="PRTAF " + nombre);
                    rice += "\r\n";
                    codigochilo += "09" + GetDirection(nombre);
                    sc = sc + 3;
                    //nextToken();
                    break;
                case "double": //double
                    Console.WriteLine(rice+="PRTD " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    codigochilo += "05" + GetDirection(Token.valor);
                    nextToken();
                    break;
                case "doubleArray":
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "doubleArray";
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="PRTAD " + nombre);
                    rice += "\r\n";
                    codigochilo += "0A" + GetDirection(nombre);
                    sc = sc + 3;
                    //nextToken();
                    break;
                default:
                    Console.WriteLine(eLog+="Error. Variable no reconocida.");
                    eLog += "\r\n";
                    nextToken();
                    break;
            }
        }
        public  void DoRead()
        {
            tipo = CheckVarTable(Token.valor, "type");
            string nombre;
            switch (tipo)
            {
                case "int":
                    Console.WriteLine(rice+="READI " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(Token.valor);
                    if (lastDirection == "-1")
                    {
                        assignDirection(Token.valor);
                    }
                    codigochilo += "27" + lastDirection;
                    nextToken();
                    break;
                case "intArray": //read(a[0])
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "intArray";
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="READAI " + nombre);
                    rice += "\r\n";
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "2C" + lastDirection;
                    sc = sc + 3;
                    break;
                case "float":
                    Console.WriteLine(rice+="READF " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(Token.valor);
                    if (lastDirection == "-1")
                    {
                        assignDirection(Token.valor);
                    }
                    codigochilo += "28" + lastDirection;
                    nextToken();
                    break;
                case "floatArray":
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "floatArray";
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="READAF " + nombre);
                    rice += "\r\n";
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "2D" + lastDirection;
                    sc = sc + 3;
                    break;
                case "double":
                    Console.WriteLine(rice+="READD " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(Token.valor);
                    if (lastDirection == "-1")
                    {
                        assignDirection(Token.valor);
                    }
                    codigochilo += "29" + lastDirection;
                    nextToken();
                    break;
                case "doubleArray":
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "doubleArray";
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="READAD " + nombre);
                    rice += "\r\n";
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "2E" + lastDirection;
                    sc = sc + 3;
                    break;
                case "char":
                    Console.WriteLine(rice+="READC " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(Token.valor);
                    if (lastDirection == "-1")
                    {
                        assignDirection(Token.valor);
                    }
                    codigochilo += "26" + lastDirection;
                    nextToken();
                    break;
                case "charArray":
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "charArray";
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="READAC " + nombre);
                    rice += "\r\n";
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "2B" + lastDirection;
                    sc = sc + 3;
                    break;
                case "string":
                    Console.WriteLine(rice+="READS " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 3;
                    lastDirection = GetDirection(Token.valor);
                    if (lastDirection == "-1")
                    {
                        assignDirection(Token.valor);
                    }
                    codigochilo += "2A" + lastDirection;
                    nextToken();
                    break;
                case "stringArray":
                    nombre = Token.valor;
                    nextToken();
                    Match("[");
                    tipo = "int";
                    BoolExpretion();
                    tipo = "stringArray";
                    Console.WriteLine(rice+="PUSHKI " + CheckVarTable(nombre, "length"));
                    rice += "\r\n";
                    sc += 5;
                    codigochilo += "17" + Int32.Parse(CheckVarTable(nombre, "length")).ToString("X4").PadLeft(8, '0'); ;
                    Console.WriteLine(rice+="MULT");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "3D";
                    Match("]");
                    Console.WriteLine(rice+="POPX");
                    rice += "\r\n";
                    codigochilo += "20";
                    sc++;
                    Console.WriteLine(rice+="READAS " + nombre);
                    rice += "\r\n";
                    lastDirection = GetDirection(nombre);
                    if (lastDirection == "-1")
                    {
                        assignDirection(nombre);
                    }
                    codigochilo += "2F" + lastDirection;
                    sc = sc + 3;
                    break;

            }
            
        }
        public  void BoolExpretion()
        {
            Comparison();
            while (Token.index == 31 || Token.index == 32)
            {
                if (Token.index == 31)
                {
                    //pending, no idea what happens here
                }
                else if (Token.index == 32)
                {
                    //pending, no idea what happens here
                }
                else
                {
                    Console.WriteLine("Error. This shouldn't happen.");
                }
            }
        }
        public  void Comparison()
        {
            Expretion();
            while (Token.index == 17 || //<
                Token.index == 18 || //<=
                Token.index == 19 || //>
                Token.index == 20 || //>=
                Token.index == 21 || //==
                Token.index == 22) // !=
            {

                if (Token.index == 17)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine(rice+="CMP");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "40";
                    JumpHelper = "JMPGE"; //opuesto a la comparacion realizada
                }
                if (Token.index == 18)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine(rice+="CMP");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "40";
                    JumpHelper = "JMPGT";
                }
                if (Token.index == 19)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine(rice="CMP");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "40";
                    JumpHelper = "JMPLE";

                }
                if (Token.index == 20)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine(rice+="CMP");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "40";
                    JumpHelper = "JMMPLT";
                }
                if (Token.index == 21)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine(rice+="CMP");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "40";
                    JumpHelper = "JMPNE";
                }
                if (Token.index == 22)
                {
                    nextToken();
                    Expretion();
                    Console.WriteLine(rice+="CMP");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "40";
                    //Console.WriteLine("JMPNE "); 
                    JumpHelper = "JMPEQ";
                }

            }
        }
        public  void Expretion()
        {
            Factor();
            while (Token.index == 13 || Token.index == 14)
            {
                if (Token.index == 13)
                {
                    nextToken();
                    Factor();
                    Console.WriteLine(rice+="ADD");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "3B";
                    //codigochilo += "\n"; //testing purposes, must die eventually
                }
                else if (Token.index == 14)
                {
                    nextToken();
                    Factor();
                    Console.WriteLine(rice+="SUB");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "3C";
                }
            }
        }
        public  void Factor()
        {
            Terminal();
            while (Token.index == 15 || Token.index == 16)
            {
                if (Token.index == 15)
                {
                    nextToken();
                    Terminal();
                    Console.WriteLine(rice+="MULT");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "3D";
                }
                else if (Token.index == 16)
                {
                    nextToken();
                    Terminal();
                    Console.WriteLine(rice+="DIV");
                    rice += "\r\n";
                    sc++;
                    codigochilo += "3E";
                }
            }
        }
        public  void Terminal()
        {
            if (tipo == "int" || tipo=="intArray")
            {
                if (Token.index == 37) //numero
                {
                    Console.WriteLine(rice+="PUSHKI " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 5;
                    codigochilo += "17" + Int32.Parse(Token.valor).ToString("X4").PadLeft(8, '0');
                    //codigochilo += "\n"; //testing purposes, must die eventually
                    nextToken();
                }
                else if (Token.index == 50) // variable
                {
                    if (CheckVarTable(Token.valor, "type") =="int")
                    {
                        Console.WriteLine(rice+="PUSHI " + Token.valor);
                        rice += "\r\n";
                        sc = sc + 3;
                        codigochilo += "0D" + GetDirection(Token.valor);
                        //codigochilo += "\n"; //testing purposes, must die eventually
                        nextToken();
                    }
                    else if(CheckVarTable(Token.valor, "type") == "intArray")
                    {
                        string nombre = Token.valor;
                        nextToken();
                        Match("[");
                        tipo = "int";
                        BoolExpretion();
                        tipo = "intArray";
                        Console.WriteLine(rice+="POPX");
                        rice += "\r\n";
                        codigochilo += "20";
                        sc++;
                        
                        Console.WriteLine(rice+="PUSHAI " + nombre);
                        rice += "\r\n";
                        sc = sc + 3;
                        codigochilo += "12" + GetDirection(nombre);
                        //nextToken();
                        Match("]");
                    }
                }
            }
            else if(tipo =="float" || tipo=="floatArray")
            {
                if (Token.index == 37 || Token.index == 42) //numero
                {
                    Console.WriteLine(rice+="PUSHKF " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 5;
                    float f = float.Parse(Token.valor, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    byte[] array = BitConverter.GetBytes(f);
                    string fstring = array[3].ToString("X2") + array[2].ToString("X2") + array[1].ToString("X2") + array[0].ToString("X2");
                    codigochilo += "18" + fstring;
                    //codigochilo += "\n"; //testing purposes, must die eventually
                    nextToken();
                }
                else if (Token.index == 50) // variable
                {
                    if (CheckVarTable(Token.valor, "type") == "float")
                    {
                        Console.WriteLine(rice+="PUSHF " + Token.valor);
                        rice += "\r\n";
                        sc = sc + 3;
                        codigochilo += "0E" + GetDirection(Token.valor);
                        //codigochilo += "\n"; //testing purposes, must die eventually
                        nextToken();
                    }
                    else if (CheckVarTable(Token.valor, "type") == "floatArray")
                    {
                        string nombre = Token.valor;
                        nextToken();
                        Match("[");
                        tipo = "int";
                        BoolExpretion();
                        tipo = "floatArray";
                        Console.WriteLine(rice+="POPX");
                        rice += "\r\n";
                        codigochilo += "20";
                        sc++;

                        Console.WriteLine(rice+="PUSHAF " + nombre);
                        rice += "\r\n";
                        sc = sc + 3;
                        codigochilo += "13" + GetDirection(nombre);
                        Match("]");
                    }
                }
            }
            else if (tipo== "double" || tipo =="doubleArray")
            {
                if (Token.index == 37 || Token.index == 42) //numero
                {
                    Console.WriteLine(rice+="PUSHKD " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 9;
                    double d = double.Parse(Token.valor, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    byte[] array = BitConverter.GetBytes(d);
                    string dstring = array[7].ToString("X2") + array[6].ToString("X2") + array[5].ToString("X2") + array[4].ToString("X2") + array[3].ToString("X2") + array[2].ToString("X2") + array[1].ToString("X2") + array[0].ToString("X2");
                    codigochilo += "19" + dstring;
                    nextToken();
                }
                else if (Token.index == 50) // variable
                {
                    if (CheckVarTable(Token.valor, "type") == "double")
                    {
                        Console.WriteLine(rice+="PUSHD " + Token.valor);
                        rice += "\r\n";
                        sc = sc + 3;
                        codigochilo += "0F" + GetDirection(Token.valor);
                        nextToken();
                    }
                    if (CheckVarTable(Token.valor, "type") == "doubleArray")
                    {
                        string nombre = Token.valor;
                        nextToken();
                        Match("[");
                        tipo = "int";
                        BoolExpretion();
                        tipo = "doubleArray";
                        Console.WriteLine(rice+="POPX");
                        rice += "\r\n";
                        codigochilo += "20";
                        sc++;

                        Console.WriteLine(rice+="PUSHAD " + nombre);
                        rice += "\r\n";
                        sc = sc + 3;
                        codigochilo += "14" + GetDirection(nombre);
                        Match("]");
                    }
                }
            }
            else if(tipo=="char" || tipo=="charArray")
            {
                if (Token.index == 40) //char
                {
                    Console.WriteLine(rice+="PUSHKC " + Token.valor);
                    rice += "\r\n";
                    sc = sc + 2;
                    codigochilo += "16" + ConvertStringtoHexa(Token.valor);
                    nextToken();
                }
                else if (Token.index == 50)//var
                {
                    if (CheckVarTable(Token.valor, "type") == "char")
                    {
                        Console.WriteLine(rice+="PUSHC " + Token.valor);
                        rice += "\r\n";
                        sc = sc + 3;
                        codigochilo += "0C" + GetDirection(Token.valor);
                        nextToken();
                    }
                    else if (CheckVarTable(Token.valor, "type") == "charArray")
                    {
                        string nombre = Token.valor;
                        nextToken();
                        Match("[");
                        tipo = "int";
                        BoolExpretion();
                        tipo = "charArray";
                        Console.WriteLine(rice+="POPX");
                        rice += "\r\n";
                        codigochilo += "20";
                        sc++;

                        Console.WriteLine(rice+="PUSHAC " + nombre);
                        rice += "\r\n";
                        sc = sc + 3;
                        codigochilo += "11" + GetDirection(nombre);
                        //nextToken();
                        Match("]");

                    }

                }
            }
            else if(tipo=="string" || tipo=="stringArray")
            {
                if(Token.index == 39) //string
                {
                    Console.WriteLine(rice+="PUSHKS " + Token.valor);
                    rice += "\r\n";
                    if (Int32.Parse(StringLength)<Token.valor.Length)
                    {
                        Console.WriteLine(eLog+="Error. Valor de string mayor que longitud de variable");
                        eLog += "\r\n";
                    }
                    codigochilo += "1A" + (Token.valor.Length+"").PadLeft(2, '0') + ConvertStringtoHexa(Token.valor);
                    sc = sc + 2 + Int32.Parse(StringLength);
                    nextToken();
                }
                else if(Token.index == 50) //var
                {
                    if (CheckVarTable(Token.valor, "type") == "string")
                    {
                        Console.WriteLine(rice+="PUSHS " + Token.valor);
                        rice += "\r\n";
                        codigochilo += "10" + GetDirection(Token.valor);
                        sc = sc + 3;
                    }
                    else if (CheckVarTable(Token.valor, "type") == "stringArray")
                    {
                        string nombre = Token.valor;
                        nextToken();
                        Match("[");
                        tipo = "int";
                        BoolExpretion();
                        tipo = "stringArray";
                        Console.WriteLine(rice+="POPX");
                        rice += "\r\n";
                        codigochilo += "20";
                        sc++;
                        Console.WriteLine(rice+="PUSHAS " + nombre);
                        rice += "\r\n";
                        sc = sc + 3;
                        codigochilo += "15" + GetDirection(nombre);
                        //nextToken();
                        Match("]");
                    }

                }
            }
            else if (Token.index == 11) // OP  (
            {
                nextToken();
                Expretion();
                Match("CP");
            }
        }
        public  void Match(string comp)
        {
            switch (comp)
            {
                case "OB":
                    if (Token.index != 23)
                    {
                        Console.WriteLine(eLog+= "Error. { esperado. " + " Current token index: " + Token.index);
                        eLog += "\r\n";
                    }
                    break;
                case "CB":
                    if (Token.index != 24)
                    {
                        Console.WriteLine(eLog+= "Error. } esperado. " + " Current token index: "  + Token.index);
                        eLog += "\r\n";
                    }
                    break;
                case "Eq":
                    if (Token.index != 27)
                    {
                        Console.WriteLine(eLog+="Error. = esperado." + " Current token index: " + Token.index);
                        eLog += "\r\n";
                    }
                    break;
                case "OP":
                    if (Token.index != 11)
                    {
                        Console.WriteLine(eLog+="Error. ( esperado." + " Current token index: " + Token.index);
                        eLog += "\r\n";
                    }
                    break;
                case "CP":
                    if (Token.index != 12)
                    {
                        Console.WriteLine(eLog+="Error. ) esperado." + " Current token index: " + Token.index);
                        eLog += "\r\n";
                    }
                    break;
                case "[":
                    if (Token.index != 25)
                    {
                        Console.WriteLine(eLog+= "Error. [ esperado." + " Current token index: " + Token.index);
                        eLog += "\r\n";
                    }
                    break;
                case "]":
                    if (Token.index != 26)
                    {
                        Console.WriteLine(eLog+= "Error. ] esperado." + " Current token index: " + Token.index);
                        eLog += "\r\n";
                    }
                    break;
                case ",":
                    if(Token.index !=30)
                    {
                        Console.WriteLine(eLog+="Error. , esperado." + " Current token index: " + Token.index);
                        eLog += "\r\n";
                    }
                    break;
                case ";":
                    if(Token.index !=28)
                    {
                        Console.WriteLine(eLog+="Error. ; esperado." + " Current token index: " + Token.index);
                        eLog += "\r\n";
                    }
                    break;
            }
            nextToken();

        }
        public  bool isStatement()
        {
            try
            {
                return
                    Token.index == 0 || //char
                    Token.index == 1 || //string
                    Token.index == 2 || //int
                    Token.index == 3 || //float
                    Token.index == 4 || //double
                    Token.index == 5 || //if
                    Token.index == 7 || //for
                    Token.index == 8 || //while
                    Token.index == 9 || // print
                    Token.index == 10 || //read
                    Token.index == 41 || //printl
                    Token.index == 50;
            }
            catch(System.NullReferenceException)
            {
                return false;
            }

            //System.NullReferenceException
        }
        public  bool isDeclaration()
        {
            return Token.index == 0 || //char
                Token.index == 1 || //string
                Token.index == 2 || //int
                Token.index == 3 || //float
                Token.index == 4; //double
        }
        private  bool isInstruction()
        {
            return Token.index == 41 || //printl
                Token.index == 9 || // print
                Token.index == 8 || // while
                Token.index == 7 || // for
                Token.index == 6 || // else
                Token.index == 5 || // if
                Token.index == 10; //print


        }
        public  void nextToken()
        {
            tokenCount++;
            
            Token = tokensArray[tokenCount];
            if (tokenCount >= finalcount)
            {
                Token = new Tokens();
            }


        }
        public  string CheckVarTable(string token, string request)
        {
            bool found = false;
            for (int i = 0; i < varCount; i++)
            {
                if(token == varTable[i].name)
                {
                    found = true;
                    if (request == "type")
                        return varTable[i].type;
                    else if (request == "length")
                        return varTable[i].length;

                }
            }
            if(found== false)
            {
                Console.WriteLine(eLog+="Error. Variable no existe");
                eLog += "\r\n";
            }
            return "null";
        }
        public  void assignDirection(string name)
        {
            bool found = false;
            
            for (int i = 0; i < varCount && found==false; i++)
            {
                if (name == varTable[i].name)
                {
                    varTable[i].direction = "0000";

                    if(varTable[i].type == "char")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper++;
                    }
                    if (varTable[i].type == "charArray")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + varTable[i].size;
                    }
                    if (varTable[i].type == "string")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + Int32.Parse(StringLength);

                    }
                    if (varTable[i].type == "stringArray")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + varTable[i].size*Int32.Parse(StringLength);

                    }
                    if (varTable[i].type == "int")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + 4;
                    }
                    if (varTable[i].type == "intArray")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + varTable[i].size*4;
                    }
                    if (varTable[i].type == "float")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + 4;
                    }
                    if (varTable[i].type == "floatArray")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + varTable[i].size*4;
                    }
                    if (varTable[i].type == "double")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + 8;
                    }
                    if (varTable[i].type == "doubleArray")
                    {
                        varTable[i].direction = directionHelper.ToString("X4");
                        directionSize = varTable[i].direction;
                        directionHelper = directionHelper + varTable[i].size * 8;
                    }
                    lastDirection = varTable[i].direction;
                }
            }

            if (found == false)
            {
                //Console.WriteLine("Error. Variable no existe");
            }
        }
        public  string GetDirection(string n)
        {
            for(int i=0; i< varTable.Length;i++)
            {
                if(n == varTable[i].name)
                {
                    return varTable[i].direction;
                }
            }
            Console.WriteLine(eLog+="Error. Variable sin direccion.");
            eLog += "\r\n";
            return "";
        }

        public  string ConvertStringtoHexa(string input)
        {
            string result = "";
            char[] values = input.ToCharArray();
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                string hexOutput = String.Format("{0:X}", value);
                result += ""+hexOutput;
            }
            return result;
        }
        public  void JumpWriteCode(string JumpHelper)
        {
            if (JumpHelper == "JMPEQ")
                codigochilo += "31";
            else if (JumpHelper == "JMPNE")
                codigochilo += "32";
            else if (JumpHelper == "JMPGT")
                codigochilo += "33";
            else if (JumpHelper == "JMPGE")
                codigochilo += "34";
            else if (JumpHelper == "JMPLT")
                codigochilo += "35";
            else if (JumpHelper == "JMPLE")
                codigochilo += "36";
            
        }
        public  string getRice()
        {
            return rice;
        }
        public string getErrorLog()
        {
            return eLog;
        }
        public  string Float2Hex(float fNum)
        {
            MemoryStream ms = new MemoryStream(sizeof(float));
            StreamWriter sw = new StreamWriter(ms);

            // Write the float to the stream
            sw.Write(fNum);
            sw.Flush();

            // Re-read the stream
            ms.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[4];
            ms.Read(buffer, 0, 4);

            // Convert the buffer to Hex
            StringBuilder sb = new StringBuilder();
            foreach (byte b in buffer)
                sb.AppendFormat("{0:X2}", b);

            sw.Close();

            return sb.ToString();
        }
    }
    class Tokens
    {
        public int index { get; set; }
        public String valor { get; set; }
        public Tokens()
        {
            index = 51;
            valor = "";
        }
        public Tokens(int ind, String val)
        {
            index = ind;
            valor = val;
        }
    }
    class Variables
    {
        public string name { get; set; }
        public string type { get; set; }
        public string direction { get; set; }
        public bool isArray  { get; set; }
        public bool initialized { get; set; }
        public string length { get; set; }
        public int size { get; set; }
        public Variables()
        {
            name = "noname";
            type = "null";
            isArray = false;
            initialized = false;
            direction = "-1";
            length = "-1";
            size = -1;
        }

    }

}


//0 char
//1 string
//2 int
//3 float
//4 double
//5 if
//6 else 
//7 for
//8 while
//9 print
//10 read
//11 (
//12 )
//13 +
//14 -
//15 *
//16 /
//17 <
//18 <=
//19 >
//20 >=
//21 ==
//22 !=
//23 {
//24 }
//25 [
//26 ]
//27 =
//28 ;
//29 \n dead
//30 ,
//31 &&
//32 ||
//33 ! dead
//34 "
//35 '
//36 .
//37 number
//38 decimal
//39 un string
//40 un char
//41 printl
//50 "variable"
//51 unrecognized