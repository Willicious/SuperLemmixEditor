﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NLEditor
{
    class FileLine
    {
        /// <summary>
        /// Stores content of one line in a text file.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <param name="num"></param>
        public FileLine(string key, string text = "", int num = 0)
        {
            fKey = key.ToUpper();
            fText = text;
            fValue = num;
        }

        string fKey;
        string fText;
        int fValue;

        public string Key { get { return fKey; } }
        public string Text { get { return fText; } }
        public int Value { get { return fValue;}}
    }
    
    
    class FileParser
    {
        /*---------------------------------------------------------
         *          This class parses NeoLemmix files
         * -------------------------------------------------------- */

        /*---------------------------------------------------------
         *  public methods:
         *    FileParser(string FilePath)
         *    
         *    GetNextLines()
         * -------------------------------------------------------- */

        /// <summary>
        /// Opens a text file to be parsed. 
        /// <para> You have to catch exceptions in the method creating the FileParser. </para>
        /// </summary>
        /// <param name="FilePath"></param>
        public FileParser(string FilePath)
        {
            fFileStream = new StreamReader(FilePath);
        }

        StreamReader fFileStream;
        FileLine fFileLatestLine;

        readonly string[] MultiLinePieces = { "OBJECT", "TERRAIN", "LEMMING" };
        
        /// <summary>
        /// Parses new block of lines, ignoring empty lines or ones starting with #.
        /// <para> Returns null at the file end. </para>
        /// </summary>
        /// <returns></returns>
        public List<FileLine> GetNextLines()
        {
            List<FileLine> FileLineList = new List<FileLine>();

            FileLine CurFileLine;
            do
            {
                if (fFileLatestLine != null)
                {
                    CurFileLine = fFileLatestLine;
                    fFileLatestLine = null;
                }
                else
                {
                    CurFileLine = GetNewLine();
                }

                // end of file reached
                if (CurFileLine == null) return null;
            } while (CurFileLine.Key == "");

            FileLineList.Add(CurFileLine);

            // Add more lines, if the piece requires multiple lines in the level file
            if (CurFileLine.Key.In(MultiLinePieces))
            {
                bool DoAddNextLine;
                do
                {
                    CurFileLine = GetNewLine();

                    if (CurFileLine == null || CurFileLine.Key == "")
                    {
                        DoAddNextLine = false;
                    }
                    else if (CurFileLine.Key.In(MultiLinePieces))
                    {
                        fFileLatestLine = CurFileLine;
                        DoAddNextLine = false;
                    }
                    else
                    {
                        FileLineList.Add(CurFileLine);
                        DoAddNextLine = true;
                    }
                } while (DoAddNextLine); 
            }

            return FileLineList;
        }

        /// <summary>
        /// Reads a single line. Returns null at the file end.
        /// </summary>
        /// <returns></returns>
        private FileLine GetNewLine()
        {
            string Line = fFileStream.ReadLine();

            // Check for end of file
            if (Line == null) return null;

            Line = Line.Trim();

            int SeperatorIndex = Line.IndexOf(' ');

            FileLine ThisFileLine;
            if (Line.Length > 0 && Line.StartsWith("#"))
            { 
                // comment line
                ThisFileLine = new FileLine("");
            }
            else if (SeperatorIndex == -1)
            {
                ThisFileLine = new FileLine(Line);
            }
            else
            {
                string Key = Line.Substring(0, SeperatorIndex).Trim();
                string Text = Line.Substring(SeperatorIndex).Trim();
                int Value;
                if (!Int32.TryParse(Text, out Value)) Value = 0;

                ThisFileLine = new FileLine(Key, Text, Value);
            }

            return ThisFileLine;
        }

    }
}
