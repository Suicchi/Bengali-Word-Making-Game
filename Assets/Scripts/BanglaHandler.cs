using System;
using System.Collections.Generic;


public class BanglaHandler
{
    // public string BanglaWord {get; set;}
    static List<string> vowels = new List<string>() { "অ", "আ", "ই", "ঈ", "উ", "ঊ", "ঋ", "এ", "ঐ", "ও", "ঔ" };
    static List<string> consonants = new List<string>(){"ক","খ","গ","ঘ","ঙ",
                                                "চ","ছ","জ","ঝ","ঞ",
                                                "ট","ঠ","ড","ঢ","ণ",
                                                "ত","থ","দ","ধ","ন",
                                                "প","ফ","ব","ভ","ম",
                                                "য","র","ল",
                                                "শ","ষ","স","হ",
                                                "ড়","ঢ়","য়",
                                                "ৎ"}; //যদিও ক্ষ যুক্তবর্ণ তবুও যাচাই করার সুবিধার্থে এইখানে রাখা 
    static List<string> specialConsonants = new List<string>() { "ং", "ঃ", "ঁ" };
    static List<string> kars = new List<string>() { "া", "ি", "ী", "ু", "ূ", "ৃ", "ে", "ৈ", "ো", "ৌ" };
    static string hasanta = "্";

    public static int Parts(string banglaWord)
    {
        int partsOfWord = 0;

        for (int i = 0; i + 1 < banglaWord.Length; i++)
        {
            var test4 = String.Empty;
            if (vowels.Contains(banglaWord[i].ToString()))
            {
                if (banglaWord[i] == 'অ' && i < banglaWord.Length && banglaWord[i + 1].ToString() == hasanta)
                {
                    // test4 += "অ্যা";
                    i += 3;
                }
                else
                {
                    // test4 += banglaWord[i];
                    while (i + 1 < banglaWord.Length && !vowels.Contains(banglaWord[i + 1].ToString()) && !consonants.Contains(banglaWord[i + 1].ToString()))
                    {
                        i++;
                        // test4 += banglaWord[i];
                    }
                }
                partsOfWord++;
            }
            else if (consonants.Contains(banglaWord[i].ToString()))
            {
                test4 += banglaWord[i];
                while (i + 1 < banglaWord.Length && !vowels.Contains(banglaWord[i + 1].ToString()) && !consonants.Contains(banglaWord[i + 1].ToString()))
                {
                    if (banglaWord[i + 1].ToString() == hasanta)
                    {
                        // test4 += banglaWord[i + 1];
                        // test4 += banglaWord[i + 2];
                        i += 2;
                    }
                    else
                    {
                        i++;
                        // test4 += banglaWord[i];
                    }
                }
                partsOfWord++;
            }
            //Console.WriteLine(test4);
        }

        return partsOfWord;
    }


    public static List<string> DividedWords(string banglaword)
    {
        var dividedWord = new List<string>();

        for (int i = 0; i < banglaword.Length; i++)
        {
            var test4 = String.Empty;
            if (vowels.Contains(banglaword[i].ToString()))
            {
                if (banglaword[i] == 'অ' && i + 1 < banglaword.Length && banglaword[i + 1].ToString() == hasanta)
                {
                    test4 += "অ্যা";
                    i += 3;
                }
                else
                {
                    test4 += banglaword[i];
                    while (i + 1 < banglaword.Length && !vowels.Contains(banglaword[i + 1].ToString()) && !consonants.Contains(banglaword[i + 1].ToString()))
                    {
                        i++;
                        test4 += banglaword[i];
                    }
                }
            }
            else if (consonants.Contains(banglaword[i].ToString()))
            {
                test4 += banglaword[i];
                while (i + 1 < banglaword.Length && !vowels.Contains(banglaword[i + 1].ToString()) && !consonants.Contains(banglaword[i + 1].ToString()))
                {
                    if (banglaword[i + 1].ToString() == hasanta)
                    {
                        test4 += banglaword[i + 1];
                        test4 += banglaword[i + 2];
                        i += 2;
                    }
                    else
                    {
                        i++;
                        test4 += banglaword[i];
                    }
                }
            }
            //Console.WriteLine(test4);
            dividedWord.Add(test4);
        }

        return dividedWord;
    }


}