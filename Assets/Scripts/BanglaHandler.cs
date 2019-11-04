using System;
using System.Collections.Generic;


public class BanglaHandler
{
    // public string BanglaWord {get; set;}
    static List<string> vowels = new List<string>(){"অ","আ","ই","ঈ","উ","ঊ","ঋ","এ","ঐ","ও","ঔ"};
    static List<string> consonants = new List<string>(){"ক","খ","গ","ঘ","ঙ",
                                                "চ","ছ","জ","ঝ","ঞ",
                                                "ট","ঠ","ড","ঢ","ণ",
                                                "ত","থ","দ","ধ","ন",
                                                "প","ফ","ব","ভ","ম",
                                                "য","র","ল",
                                                "শ","ষ","স","হ",
                                                "ড়","ঢ়","য়",
                                                "ৎ"}; //যদিও ক্ষ যুক্তবর্ণ তবুও যাচাই করার সুবিধার্থে এইখানে রাখা 
    static List<string> specialConsonants = new List<string>(){"ং","ঃ","ঁ"};
    static List<string> kars = new List<string>(){"া","ি","ী","ু","ূ","ৃ","ে","ৈ","ো","ৌ"};
    static string hasanta = "্";

    public static int Parts(string banglaWord)
    {
        int partsOfWord = 0;
        for (int i= 0; i< banglaWord.Length; i++)
        {
            
            //string test4 = String.Empty;
            if (vowels.Contains(banglaWord[i].ToString()))
            {
                // test4 += banglaWord[i];
                partsOfWord++;
            }
            else if(consonants.Contains(banglaWord[i].ToString()))
            {
                // test4 += banglaWord[i];
                if(i+1<banglaWord.Length && kars.Contains(banglaWord[i+1].ToString()))
                {
                    // test4 += banglaWord[i+1];
                    i=i+1;
                    if(i+1<banglaWord.Length && specialConsonants.Contains(banglaWord[i+1].ToString()))
                    {
                        // test4 += banglaWord[i+1];
                        i = i+1;
                    }
                }
                else if(i+1<banglaWord.Length && banglaWord[i+1].ToString() == hasanta)
                {
                    // test4 += banglaWord[i+1];
                    i=i+1;
                    if(i+1<banglaWord.Length && consonants.Contains(banglaWord[i+1].ToString()))
                    {
                        // test4 += banglaWord[i+1];
                        i=i+1;
                        if(i+1<banglaWord.Length && kars.Contains(banglaWord[i+1].ToString()))
                        {
                            // test4 += banglaWord[i+1];
                            i++;
                            if (i+1<banglaWord.Length && specialConsonants.Contains(banglaWord[i+1].ToString()))
                            {
                                // test4 += banglaWord[i+1];
                                i++;
                            }
                        }
                        else if (i+1<banglaWord.Length && specialConsonants.Contains(banglaWord[i+1].ToString()))
                        {
                            // test4 += banglaWord[i+1];
                            i++;
                        }
                    }
                }
                else if (i+1<banglaWord.Length && specialConsonants.Contains(banglaWord[i+1].ToString()))
                {
                    // test4 += banglaWord[i+1];
                    i++;
                }
                partsOfWord++;
            }
        }
        return partsOfWord;
    }

    public static List<string> DividedWords(string banglaword)
    {
        var dividedWord = new List<string>();

        for (int i = 0; i < banglaword.Length; i++)
        {
            string test4 = String.Empty;
            if (vowels.Contains(banglaword[i].ToString()))
            {
                test4 += banglaword[i];
            }
            else if (consonants.Contains(banglaword[i].ToString()))
            {
                test4 += banglaword[i];
                if (i + 1 < banglaword.Length && kars.Contains(banglaword[i + 1].ToString()))
                {
                    test4 += banglaword[i + 1];
                    i = i + 1;
                    if (i + 1 < banglaword.Length && specialConsonants.Contains(banglaword[i + 1].ToString()))
                    {
                        test4 += banglaword[i + 1];
                        i = i + 1;
                    }
                }
                else if (i + 1 < banglaword.Length && banglaword[i + 1].ToString() == hasanta)
                {
                    test4 += banglaword[i + 1];
                    i = i + 1;
                    if (i + 1 < banglaword.Length && consonants.Contains(banglaword[i + 1].ToString()))
                    {
                        test4 += banglaword[i + 1];
                        i = i + 1;
                        if (i + 1 < banglaword.Length && kars.Contains(banglaword[i + 1].ToString()))
                        {
                            test4 += banglaword[i + 1];
                            i++;
                            if (i + 1 < banglaword.Length && specialConsonants.Contains(banglaword[i + 1].ToString()))
                            {
                                test4 += banglaword[i + 1];
                                i++;
                            }
                        }
                        else if (i + 1 < banglaword.Length && specialConsonants.Contains(banglaword[i + 1].ToString()))
                        {
                            test4 += banglaword[i + 1];
                            i++;
                        }
                    }
                }
                else if (i + 1 < banglaword.Length && specialConsonants.Contains(banglaword[i + 1].ToString()))
                {
                    test4 += banglaword[i + 1];
                    i++;
                }
            }
            //Console.WriteLine(test4);
            dividedWord.Add(test4);
        }



        return dividedWord;
    }
}