using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

namespace ban_u2a
{
    public class BijoyToUni
    {
        Dictionary<string, string> ar; Dictionary<string, string> aM;
        Dictionary<string, string> aR; Dictionary<string, string> aS;

        public BijoyToUni()
        {
            ar = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.streamingAssetsPath + "/BijoyToUni/ur.json"));
            aR = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.streamingAssetsPath + "/BijoyToUni/uRR.json"));
            aM = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.streamingAssetsPath + "/BijoyToUni/uM.json"));
            aS = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Application.streamingAssetsPath + "/BijoyToUni/uS.json"));
        }

        string positionKars(string text)
        {
            string testAgainst = "w†‰‡";
            char[] array = text.ToCharArray();
            for(int i= 0; i<array.Length;i++)
            {
                if(testAgainst.Contains(array[i]) && i+1<array.Length)
                {
                    var temp = array[i];
                    array[i] = array[i+1];
                    array[i+1] = temp;
                    i++;
                }
                if(array[i]== '©' )
                {
                    if(i-2>-1 && testAgainst.Contains(array[i-1]))
                    {
                        var temp = array[i];
                        array[i] = array[i-2];
                        array[i-2] = temp;
                        i-=2;
                    }
                    else if(i-1>-1 && array[i-1] == '¨')
                    {
                        var temp = array[i];
                        array[i] = array[i-1];
                        array[i-1] = array[i-2];
                        array[i-2] = temp;
                    }
                    else //if(i-1>-1 && !testAgainst.Contains(array[i-1] ))
                    {
                        var temp = array[i];
                        array[i] = array[i-1];
                        array[i-1] = temp;
                        i--;
                    }
                    
                }
            }
            return new String(array);
        }
        
        string fb = "0123456789ABCDEF";

        string fa(char d)
        {
            var h = fb.substr(d & 15, 1);
            while (d > 15)
            {
                d >>= 4;
                h = fb.substr(d & 15, 1) + h;
            }
            while (h.Length < 4) h = "0" + h;
            return h;
        }

        string bF(string line, bool ef)
        {
            var text = "";
            for (int i = 0; i < line.Length; i++)
            {
                if (eu(line.ElementAt(i))) text += line.ElementAt(i);
                else text += "&#" + (ef ? fa(line.ElementAt(i)).ToString() : line.charCodeAt(i).ToString()) + ";";
            }
            return text;
        }
        public string Convert(string line)
        {
            // string az = "bangla";
            var G = ar;
            // if (az == "bijoy") G = ar;
            // else if (az == "somewherein") G = aM;
            // else if (az == "boisakhi") G = aR;
            // else if (az == "bangsee") G = aS;
            // else if (az == "bornosoft")
            // {
            //     return line;
            // }
            // else if (az == "phonetic")
            // {
            //     return line;
            // }
            // else if (az == "htmlsafehex")
            //     return bF(line, true);
            // else if (az == "htmlsafedec")
            //     return bF(line, false);

            // line = line.replace("ো", "ো");
            // line = line.replace("ৌ", "ৌ");
            var temp = line;
            line = positionKars(line);
            foreach (var eI in G)
            {
                line = line.replace(eI.Key, eI.Value);
            }
            line = line.replace("ো", "ো");
            line = line.replace("ৌ", "ৌ");
            return line;
        }

        bool bA(char e)
        {
            if (e == '০' || e == '১' || e == '২' || e == '৩' || e == '৪' || e == '৫' || e == '৬' || e == '৭' || e == '৮' || e == '৯') return true;
            return false;
        }

        bool ao(char e)
        {
            if (e == 'ি' || e == 'ৈ' || e == 'ে') return true;
            return false;
        }

        bool aJ(char e)
        {
            if (e == 'া' || e == 'ো' || e == 'ৌ' || e == 'ৗ' || e == 'ু' || e == 'ূ' || e == 'ী' || e == 'ৃ') return true;
            return false;
        }

        bool ah(char e)
        {
            if (ao(e) || aJ(e)) return true;
            return false;
        }

        bool v(char e)
        {
            if (e == 'ক' || e == 'খ' || e == 'গ' || e == 'ঘ' || e == 'ঙ' || e == 'চ' || e == 'ছ' || e == 'জ' || e == 'ঝ' || e == 'ঞ' || e == 'ট' || e == 'ঠ' || e == 'ড' || e == 'ঢ' || e == 'ণ' || e == 'ত' || e == 'থ' || e == 'দ' || e == 'ধ' || e == 'ন' || e == 'প' || e == 'ফ' || e == 'ব' || e == 'ভ' || e == 'ম' || e == 'শ' || e == 'ষ' || e == 'স' || e == 'হ' || e == 'য' || e == 'র' || e == 'ল' || e == 'য়' || e == 'ং' || e == 'ঃ' || e == 'ঁ' || e == 'ৎ') return true;
            return false;
        }

        bool Q(char e)
        {
            if (e == 'অ' || e == 'আ' || e == 'ই' || e == 'ঈ' || e == 'উ' || e == 'ঊ' || e == 'ঋ' || e == 'ঌ' || e == 'এ' || e == 'ঐ' || e == 'ও' || e == 'ঔ') return true;
            return false;
        }

        bool aF(char e)
        {
            if (e == 'ং' || e == 'ঃ' || e == 'ঁ') return true;
            return false;
        }

        bool bS(string e)
        {
            if (e == "্য" || e == "্র") return true;
            return false;
        }

        bool D(char e)
        {
            if (e == '্') return true;
            return false;
        }

        bool eT(char e)
        {
            if (bA(e) || ah(e) || v(e) || Q(e) || aF(e) || bS(e.ToString()) || D(e)) return true;
            return false;
        }

        bool eu(char e)
        {
            if (e >= 0 && e < 128) return true;
            return false;
        }

        bool cy(string e)
        {
            if (e == " " || e == "	" || e == "\n" || e == "\r") return true;
            return false;
        }

        string cJ(string e)
        {
            var t = "";
            if (e == "া") t = "আ";
            else if (e == "ি") t = "ই";
            else if (e == "ী") t = "ঈ";
            else if (e == "ু") t = "উ";
            else if (e == "ূ") t = "ঊ";
            else if (e == "ৃ") t = "ঋ";
            else if (e == "ে") t = "এ";
            else if (e == "ৈ") t = "ঐ";
            else if (e == "ো") t = "ও";
            else if (e == "ো") t = "ও";
            else if (e == "ৌ") t = "ঔ";
            else if (e == "ৌ") t = "ঔ";
            return t;
        }

        string bc(string e)
        {
            var t = "";
            if (e == "আ") t = "া";
            else if (e == "ই") t = "ি";
            else if (e == "ঈ") t = "ী";
            else if (e == "উ") t = "ু";
            else if (e == "ঊ") t = "ূ";
            else if (e == "ঋ") t = "ৃ";
            else if (e == "এ") t = "ে";
            else if (e == "ঐ") t = "ৈ";
            else if (e == "ও") t = "ো";
            else if (e == "ঔ") t = "ৌ";
            return t;
        }

    }
}