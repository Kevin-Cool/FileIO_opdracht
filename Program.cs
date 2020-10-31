using System;
using System.Collections.Generic;
using System.IO;

namespace FileIO
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("I'm Alive!");
            DisplayImageFiles();
        }

        static void DisplayImageFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(@"C:\Users\kevin\source\repos\Opdracht_Delegates_en_Events\Opdracht_Delegates_en_Events");
            DirectoryInfo dir2 = new DirectoryInfo(@"C:\Users\kevin\source\repos\test");
            // Get all files with a .cs
            FileInfo[] csFiles = dir.GetFiles("*.cs", SearchOption.AllDirectories);

            Console.WriteLine("Found {0} .cs",csFiles.Length);
            //print info 
            foreach (FileInfo cs in csFiles)
            {
                long length = cs.Length;
                List<string> usingNames = new List<string>();
                List<string> methodNames = new List<string>();
                List<string> constructorNames = new List<string>();
                List<string> propertieNames = new List<string>();
                string namecla = "None";
                string namesp = "None";
                string father = "None";
                bool isInterface = false;
                bool isClass = false;
                bool dubbles = false;
                foreach (string line in File.ReadAllLines(cs.DirectoryName + "\\" + cs.Name))
                {
                    if (line.Contains("interface") || line.Contains("class"))
                    {
                        if (isInterface || isClass)
                        {
                            dubbles = true;
                        }
                        isInterface = true;
                        isInterface = line.Contains("interface") ? true : false;
                        isClass = line.Contains("class") ? true : false;
                        string[] temps = line.Split(' ');
                        namecla = temps[temps.Length - 1];
                    }
                    if (line.Contains("using"))
                    {
                        usingNames.Add(line);
                    }
                    if(string.IsNullOrWhiteSpace(line))
                    {
                        length--;
                    }
                    if (line.Contains("namespace"))
                    {
                        namesp = line.Replace("namespace ", "");
                    }
                    if (line.Contains("("))
                    {
                        if (!line.Contains(";")){
                            if (line.Contains("public") || line.Contains("private") || line.Contains("protected") || line.Contains("internal"))
                            {
                                string[] tempS = line.Split('(');
                                string[] tempS2 = tempS[0].Split(' ');
                                if(tempS2[^1] == namecla)
                                {
                                    constructorNames.Add(tempS2[^1] + "(" + tempS[1]);
                                }
                                else
                                {
                                    methodNames.Add(tempS2[^1] + "(" + tempS[1]);
                                }
                            }
                        }
                    }
                    if (line.Contains(";") && ((line.Contains("public") || line.Contains("private") || line.Contains("protected") || line.Contains("internal"))))
                    {
                        if (line.Contains("="))
                        {
                            string[] tempS = line.Trim().Split('=');
                            string[] tempS2 = tempS[0].Trim().Split(' ');
                            propertieNames.Add(tempS2[^1]);

                        }
                        else
                        {
                            if (line.Contains("{"))
                            {
                                string[] tempS = line.Trim().Split('{');
                                string[] tempS2 = tempS[0].Trim().Split(' ');
                                propertieNames.Add(tempS2[^1]);
                            }
                            else
                            {
                                string tempS = line.Trim().Replace(';', ' ');
                                string[] tempS2 = tempS.Trim().Split(' ');
                                propertieNames.Add(tempS2[^1]);
                            }
                        }
                    }
                    if(line.Contains(":") && line.Contains(namecla))
                    {
                        string[] tempS = line.Trim().Split(" ");
                        father = tempS[^1];
                    }
                }
                // Analyse schrijven 
                FileInfo f = new FileInfo(dir2 + "\\Analyse"+cs.Name.Replace(".cs", "") + ".txt");
                FileStream fs = f.Create();
                fs.Close();

                string[] text =
                {
                    "Dit bestand is een: ","","","",""
                };
                text[0] += isClass ? "class" : "interface";
                text[1] = "De naam is: " + namecla;
                text[2] = "De namespace is: " + namesp;
                text[3] = "Het aantal lijnen is: " + length;
                text[4] = dubbles ? "Meerdere classes of interfaces gevonden " : "Geen errors tegen gekomen.";


                File.WriteAllLines(dir2 + "\\Analyse" + cs.Name.Replace(".cs", "") + ".txt", text);

                // ClassInfo schrijven 
                f = new FileInfo(dir2 + "\\ClassInfo" + cs.Name.Replace(".cs", "") + ".txt");
                fs = f.Create();
                fs.Close();



                List<string> classText = new List<string>();
                classText.Add(namesp + " " + namecla);
                classText.Add("Lijst van constructors:");
                foreach (string item in constructorNames)
                {
                    classText.Add("   "+item);
                }
                classText.Add("Lijst van using statements:");
                foreach (string item in usingNames)
                {
                    classText.Add("   " + item);
                }
                classText.Add("Lijst van methodes:");
                foreach (string item in methodNames)
                {
                    classText.Add("   " + item);
                }
                classText.Add("Lijst van properties:");
                foreach (string item in propertieNames)
                {
                    classText.Add("   " + item);
                }
                if(father != "None")
                {
                    classText.Add("Deze klasse heeft geen andere klasse/interface waar hij van over erft");
                }
                else
                {
                    classText.Add("Deze klasse erft over van "+ father);
                }

                string[] classTextArray = classText.ToArray();
                foreach (string item in classTextArray)
                {
                    Console.WriteLine(item);
                }
                File.WriteAllLines(dir2 + "\\ClassInfo" + cs.Name.Replace(".cs", "") + ".txt", classTextArray);

            }
        }
    }
}
