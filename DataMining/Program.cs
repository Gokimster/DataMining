﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace DataMining
{
    class Program
    {
        public static List<String[]> fileContent = new List<string[]>();
        static void Main(string[] args)
        {
            initFile("..\\..\\spambase.data");
            //1-NN ACUURACY//
            //-------------//
            // spambase accuracy is 83.047163660073892
            // eyeMinMaxed accuracy is 85.682819383259911

            //Distribution//
            //------------//
            //EyeMinMaxed field 10, Class 0: 3, 72, 21, 1, 0 // Class 1: 2, 66, 27, 2, 0
            //EyeMinMaxed field 11, Class 0: 2, 27, 64, 4, 1 // Class 1: 0, 25, 65, 6, 2
            //EyeMinMaxed field 12, Class 0: 1, 52, 40, 3, 1 // Class 1: 0, 42, 51, 5, 1
            //EyeMinMaxed field 13, Class 0: 1, 30, 63, 3, 0 // Class 1: 0, 26, 66, 5, 0
            //EyeMinMaxed field 14, Class 0: 2, 44, 45, 5, 1 // Class 1: 0, 32, 57, 8, 0 
            //------------//
            //Spambase field 1, Class 0: 90, 3, 1, 1, 2 // Class 1: 76, 7, 8, 3, 4 
            //Spambase field 2, Class 0: 93, 2, 0, 0, 3 // Class 1: 73, 9, 8, 2, 5
            //Spambase field 3, Class 0: 77, 6, 4, 2, 8 // Class 1: 43, 14, 14, 11, 16
            //Spambase field 4, Class 0: 99, 0, 0, 0, 0 // Class 1: 98, 0, 0, 0, 1
            //Spambase field 5, Class 0: 82, 4, 3, 2, 6 // Class 1: 98, 0, 0, 0, 1

            //minMaxNormalize();
            do5BinDistribution(1, 4);
            //double x = getAccuracy();
            Console.WriteLine("done");
        }

        private static void initFile(string file)
        {
            using (FileStream reader = File.OpenRead(file)) // mind the encoding - UTF8
            using (TextFieldParser parser = new TextFieldParser(reader))
            {
                parser.TrimWhiteSpace = true; // if you want
                parser.Delimiters = new[] { "," };
                parser.HasFieldsEnclosedInQuotes = true;
                while (!parser.EndOfData)
                {
                    string[] line = parser.ReadFields();
                    fileContent.Add(line);
                }
            }
        }

        private static double getAccuracy()
        {
            double accuracy = 0;
            for(int i = 0; i < fileContent.Count; i++)
            {
                if (isClosestAccurate(i))
                {
                    accuracy++;
                }
                else
                {
                    Console.WriteLine(i + " is not accurate");
                }
            }
            return 100 * accuracy / fileContent.Count;
        }

        private static bool isClosestAccurate(int lineNo)
        {
            string[] current = fileContent[lineNo];
            double[] doubleCurrent = new double[current.Length - 1];
            for(int i = 0; i < current.Length - 1; i++)
            {
                doubleCurrent[i] = Convert.ToDouble(current[i]);
            }
            double? minDiff = null;
            int? closestLineNo = null;  
            for(int i = 0; i < fileContent.Count; i++)
            {
                if (i != lineNo)
                {
                    double diff = 0;
                    for (int j = 0; j < doubleCurrent.Length; j++)
                    {
                        double temp = Convert.ToDouble(fileContent[i][j]);
                        diff += (doubleCurrent[j] - temp) * (doubleCurrent[j] - temp);
                    }
                    if (minDiff == null)
                    {
                        minDiff = diff;
                        closestLineNo = i;
                    }
                    else
                    {
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            closestLineNo = i;
                        }
                    }
                }

            }
            if(Convert.ToDouble(fileContent[(int)closestLineNo][current.Length-1]) == Convert.ToDouble(current[current.Length-1]))
            {
                return true;
            }
            return false;

        }

        private static void minMaxNormalize()
        {
            StreamWriter writer = new StreamWriter(@"..\\..\\eyeMinMaxed.arff");
            int noColumns = fileContent[0].Length - 1;
            double[] mins = new double[noColumns];
            double[] maxes = new double[noColumns];
            for(int i = 0; i < noColumns; i++)
            {
                mins[i] = (double)findMinOfColumn(i);
                maxes[i] = (double)findMaxOfColumn(i);
            }
            foreach (var line in fileContent)
            {
                int i;
                for(i = 0; i < noColumns; i++)
                {
                    double value = (Convert.ToDouble(line[i]) - mins[i]) / (maxes[i] - mins[i]);
                    writer.Write(value + ",");
                }
                writer.WriteLine(line[i]);
            }
            writer.Close();
        }

        private static double? findMinOfColumn(int i)
        {
            double? min = null;
            foreach (var line in fileContent)
            {
                double temp = Convert.ToDouble(line[i]);
                if (min == null)
                {
                    min = temp;
                }else
                {
                    if(min > temp)
                    {
                        min = temp;
                    } 
                }
            }
            return min;
        }

        private static double? findMaxOfColumn(int i)
        {
            double? max = null;
            foreach (var line in fileContent)
            {
                double temp = Convert.ToDouble(line[i]);
                if (max == null)
                {
                    max = temp;
                }
                else
                {
                    if (max < temp)
                    {
                        max = temp;
                    }
                }
            }
            return max;
        }

        private static void writeToFileRandom()
        {
            StreamWriter writer = new StreamWriter(@"..\\..\\careDontOverride.arff");
            int i = 0;
            int r = 0;
            Random rand = new Random();
            r = rand.Next(1, 11);
            foreach (var line in fileContent)
            {
                i++;
                if (r == i)
                {
                    int j;
                    for(j = 0; j < line.Length - 1; j++)
                    {
                        writer.Write(line[j] + ",");
                    }
                    writer.WriteLine(line[j]);
                }
                if (i == 11)
                {
                    r = rand.Next(1, 11);
                    i = 0;
                }
            }
            writer.Close();
        }

        private static void do5BinDistribution(float classValue, int fieldNumber)
        {
            int[] noOfElements = new int[5];
            int noOfClassFields = 0;
            foreach(var line in fileContent)
            {
                if (Convert.ToDouble(line[line.Length - 1]) == classValue)
                {
                    noOfClassFields++;
                    double value = Convert.ToDouble(line[fieldNumber]);
                    if(value < 0.2)
                    {
                        noOfElements[0]++;
                    }else
                    {
                        if(value < 0.4)
                        {
                            noOfElements[1]++;
                        }else
                        {
                            if(value < 0.6)
                            {
                                noOfElements[2]++;
                            }else
                            {
                                if(value < 0.8)
                                {
                                    noOfElements[3]++;
                                }else
                                {
                                    noOfElements[4]++;
                                }
                            }
                        }
                    }
                }
            }
            double[] distribution = new double[5];
            for(int i = 0; i < 5; i++)
            {
                distribution[i] = 100 * noOfElements[i] / noOfClassFields;
            }
            Console.WriteLine("Percentage for 0.2: " + distribution[0]);
            Console.WriteLine("Percentage for 0.4: " + distribution[1]);
            Console.WriteLine("Percentage for 0.6: " + distribution[2]);
            Console.WriteLine("Percentage for 0.8: " + distribution[3]);
            Console.WriteLine("Percentage for 1: " + distribution[4]);
        }
    }
}