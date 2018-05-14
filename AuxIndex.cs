using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDictionary
{
    public class AuxIndex
    {
        private bool keyType; //true - pk , false - fk
        private string[,] kArray;
        private int count;
        private Attribute currentAttribute;
        private string idxURL;
        public bool KeyType { get => keyType; set => keyType = value; }
        public string[,] KArray { get => kArray; set => kArray = value; }
        public int Count { get => count; set => count = value; }
        public Attribute CurrentAttribute { get => currentAttribute; set => currentAttribute = value; }
        public string IdxURL { get => idxURL; set => idxURL = value; }
        private FileStream fStream;
        private BinaryWriter bWritter;
        private BinaryReader bReader;

        /// <summary>
        /// set the indexHandler Ready to instert data.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="currAtt"></param>
        /// <param name="path"></param>
        public void SetIndexHandler(bool mode, Attribute currAtt, string path)
        {
            currentAttribute = currAtt;
            idxURL = path;
            keyType = mode;
            kArray = keyType ? new string[2, 20] : new string[20, 20];
            count = 0; //**//
            FillArr(); //**//
            UpdateIndexFile();
        }

        /// <summary>
        /// method that fill the array with -1.
        /// </summary>
        private void FillArr()
        {
            int N = keyType ? 2 : 20;
            for (int j = 0; j < 20; j++)
                for (int i = 0; i < N; i++)
                    kArray[i, j] = "-1";
        }

        /// <summary>
        /// Adds to the PK or FK strcture a new Register relevant data.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="RegAdrs"></param>
        public void Add(string Key, long RegAdrs)
        {
            if (count == 0)
            {
                kArray[0, count] = Key;
                kArray[1, count] = RegAdrs.ToString();
                count++;
                UpdateIndexFile();
            }
            else
            {
                if (keyType)
                {   //PK
                    int posToInsert = CheckIdxKeyPos(Key);
                    if (posToInsert == -1)
                    {
                        kArray[0, count] = Key;
                        kArray[1, count] = RegAdrs.ToString();
                        count++;
                        UpdateIndexFile();
                    }
                    else
                    {
                        for (int i = count; i >= posToInsert; i--)
                        {
                            if (i == posToInsert)
                            {
                                kArray[0, i] = Key;
                                kArray[1, i] = RegAdrs.ToString();
                            }
                            else
                            {
                                KArray[0, i] = KArray[0, i - 1];
                                KArray[1, i] = KArray[1, i - 1];
                            }
                        }
                        count++;
                        UpdateIndexFile();
                    }
                }
                else
                {   //FK
                    int pos = -1;
                    for (int i = 0; i < 20; i++)
                        if (kArray[0, i] == Key)
                        {
                            pos = i;
                            break;
                        }
                    if (pos != -1)
                    {   //Exists the index
                        string end = "";
                        int c = 1;
                        while (end != "-1")
                        {
                            end = kArray[c, pos];
                            if (end == "-1")
                                kArray[c, pos] = RegAdrs.ToString();
                            UpdateIndexFile();
                            c++;
                        }
                    }
                    else
                    {   //No Exist the index
                        int posToInsert = CheckIdxKeyPos(Key);
                        if (posToInsert == -1)
                        {   //final insert
                            kArray[0, count] = Key;
                            kArray[1, count] = RegAdrs.ToString();
                            count++;
                            UpdateIndexFile();
                        }
                        else
                        {   //no final insert
                            for (int i = count; i >= posToInsert; i--)
                                for (int h = 0; h < 20; h++)
                                {
                                    if (i == posToInsert)
                                    {
                                        if (h == 0)
                                        {
                                            kArray[0, i] = Key;
                                            kArray[1, i] = RegAdrs.ToString();
                                            h++;
                                        }
                                        else
                                            kArray[h, i] = "-1";
                                    }
                                    else
                                        KArray[h, i] = KArray[h, i - 1];
                                }
                            count++;
                            UpdateIndexFile();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes from the PK or FK Structure the selected Register to Delete
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="RegAdrs"></param>
        public void Remove(string Key, long RegAdrs)
        {
            int pos = -1;  //position to remove the stuff
            for (int i = 0; i < 20; i++)
                if (kArray[0, i] == Key)
                {
                    pos = i;
                    break;
                }
            if (KeyType)
            {   //PK
                for (int i = pos; i < count; i++)
                {
                    kArray[0, i] = kArray[0, i + 1];
                    kArray[1, i] = kArray[1, i + 1];
                }
                count--;
                UpdateIndexFile();
            }
            else
            {   //FK
                string end = "";
                int startsMinusOne = 1;
                int findIdx = -1;
                while (end != "-1")
                {
                    end = kArray[startsMinusOne, pos];
                    if (RegAdrs == long.Parse(end))
                        findIdx = startsMinusOne;
                    if (end != "-1")
                        startsMinusOne++;
                }
                if (startsMinusOne == 2)
                {   //last to delete
                    for (int i = pos; i < count; i++)
                        for (int h = 0; h < 20; h++)
                            kArray[h, i] = kArray[h, i + 1];
                    count--;
                    UpdateIndexFile();
                }
                else
                {   //remains stuff
                    for (int i = findIdx; i < startsMinusOne; i++)
                        kArray[i, pos] = kArray[i + 1, pos];
                    UpdateIndexFile();
                }
            }
        }

        /// <summary>
        /// method that check the Key Pos in the structure already sorted
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        private int CheckIdxKeyPos(string Key)
        {
            for (int i = 0; i < count; i++)
            {
                //if (currentAttribute.DataType == 0)
                {
                    if (kArray[0, i].CompareTo(Key) == 1)
                        return i;
                }
                //else
                {
                    if (int.Parse(kArray[0, i]).CompareTo(int.Parse(Key)) == 1)
                        return i;
                }
            }
            //last in the list
            return -1;
        }

        /// <summary>
        /// method that writtes the new changes on the memory to the file.
        /// </summary>
        private void UpdateIndexFile()
        {
            fStream = File.Open(idxURL, FileMode.Open);
            bWritter = new BinaryWriter(fStream);
            bWritter.Seek((int)currentAttribute.indexDir, SeekOrigin.Begin);
            int N = keyType ? 2 : 20;
            for (int j = 0; j < 20; j++)
            {
                for (int i = 0; i < N; i++)
                {
                    if (i == 0)
                    {   //Key string[n] or int
                        //if (currentAttribute.DataType == 0)
                            //bWritter.Write(StringToByteArray(kArray[0, j], currentAttribute.LengthDataType));
                        //else
                          //  bWritter.Write(int.Parse(kArray[0, j]));
                    }
                    else //Adrs long
                        bWritter.Write(long.Parse(kArray[i, j]));
                }
            }
            fStream.Close();
        }

        /// <summary>
        /// method that reads the data from the filo to the Data array.
        /// </summary>
        /*public void RestoreIndex()
        {
            fStream = File.Open(idxURL, FileMode.Open);
            bReader = new BinaryReader(fStream);
            bReader.BaseStream.Seek((int)currentAttribute.indexDir, SeekOrigin.Begin);
            int StartMinusOne = -1;
            bool Nofinded = true;
            int N = keyType ? 2 : 20;
            for (int j = 0; j < 20; j++)
            {
                for (int i = 0; i < N; i++)
                {
                    if (i == 0)
                    {   //Key string[n] or int
                        if (currentAttribute.DataType == 0)
                        {
                            byte[] Arrname = bReader.ReadBytes(currentAttribute.LengthDataType);
                            string item = ArraytoString(Arrname);
                            kArray[0, j] = item;
                        }
                        else
                        {
                            int item = bReader.ReadInt32();
                            kArray[0, j] = item.ToString();
                        }
                        if (kArray[0, j] == "-1" && Nofinded)
                        {
                            Nofinded = false;
                            StartMinusOne = j;
                        }
                    }
                    else
                    {   //Adrs long
                        long item = bReader.ReadInt64();
                        kArray[i, j] = item.ToString();
                    }
                }
            }
            fStream.Close();
            count = StartMinusOne;
        }*/

        /// <summary>
        /// Convert a string to byte[length].
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] StringToByteArray(string str, int length)
        {
            return Encoding.ASCII.GetBytes(str.PadRight(length, ' '));
        }

        /// <summary>
        /// Convert a byte[n] to string.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public string ArraytoString(byte[] byteArray)
        {
            return System.Text.Encoding.UTF8.GetString(byteArray).Trim();
        }
    }
}
